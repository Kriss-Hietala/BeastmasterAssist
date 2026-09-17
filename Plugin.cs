using BeastmasterAssist.Combat;
using BeastmasterAssist.Data;
using BeastmasterAssist.Ipc;
using BeastmasterAssist.Tracking;
using BeastmasterAssist.Ui;
using Dalamud.Game.Command;
using Dalamud.Game.Gui.NamePlate;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace BeastmasterAssist;

public sealed class Plugin : IDalamudPlugin
{
    public const string Command = "/bstassist";
    public const string CommandAlias = "/bestia";

    private readonly IDalamudPluginInterface pluginInterface;
    private readonly ICommandManager commands;
    private readonly IFramework framework;
    private readonly IChatGui chat;
    private readonly IObjectTable objects;
    private readonly IPluginLog log;
    private readonly WindowSystem windows = new("BeastmasterAssist");

    private readonly Configuration config;
    private readonly GameData gameData;
    private readonly CombatState combatState;
    private readonly LevelingAdvisor leveling;
    private readonly CaptureTracker capture;
    private readonly ProgressTracker progress;
    private readonly NameplateMarker nameplateMarker;
    private readonly NavigationIpc navigationIpc;
    private readonly NavigationController navigation;

    private readonly OverlayWindow overlay;
    private readonly BestiaryWindow bestiary;
    private readonly ProgressWindow progressWindow;
    private readonly ConfigWindow configWindow;

    public Plugin(
        IDalamudPluginInterface pluginInterface,
        ICommandManager commands,
        IFramework framework,
        IChatGui chat,
        IClientState clientState,
        IDataManager data,
        ITargetManager targets,
        IObjectTable objects,
        ICondition condition,
        INamePlateGui namePlateGui,
        ITextureProvider textures,
        IPluginLog log)
    {
        this.pluginInterface = pluginInterface;
        this.commands = commands;
        this.framework = framework;
        this.chat = chat;
        this.objects = objects;
        this.log = log;

        config = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        config.Initialize(pluginInterface);

        UiText.Sync(config);

        gameData = new GameData(data, log);
        gameData.Resolve();

        combatState = new CombatState(objects, targets, condition, gameData, log);
        leveling = new LevelingAdvisor();

        capture = new CaptureTracker(config, gameData, chat, clientState, condition, objects, log);
        progress = new ProgressTracker(config, gameData, data, clientState, log);
        nameplateMarker = new NameplateMarker(config, namePlateGui, capture, gameData, combatState);
        navigationIpc = new NavigationIpc(pluginInterface);
        navigation = new NavigationController(navigationIpc, chat, log);

        overlay = new OverlayWindow(config, combatState, capture, leveling);
        bestiary = new BestiaryWindow(config, capture, gameData, navigation);
        progressWindow = new ProgressWindow(config, progress, gameData);
        configWindow = new ConfigWindow(config);

        windows.AddWindow(overlay);
        windows.AddWindow(bestiary);
        windows.AddWindow(progressWindow);
        windows.AddWindow(configWindow);

        UiNavigator.OpenBestiary = () => bestiary.IsOpen = !bestiary.IsOpen;
        UiNavigator.OpenProgress = () => progressWindow.IsOpen = !progressWindow.IsOpen;
        UiNavigator.OpenSettings = () => configWindow.IsOpen = !configWindow.IsOpen;
        UiNavigator.ToggleOverlay = () =>
        {
            config.OverlayEnabled = !config.OverlayEnabled;
            config.Save();
        };

        commands.AddHandler(Command, new CommandInfo(OnCommand)
        {
            HelpMessage = "Beastmaster Assist: overlay|bestiary|progress|config|debug"
        });
        commands.AddHandler(CommandAlias, new CommandInfo(OnCommand)
        {
            HelpMessage = "Alias Beastmaster Assist."
        });

        pluginInterface.UiBuilder.Draw += windows.Draw;
        pluginInterface.UiBuilder.OpenConfigUi += OpenConfig;
        pluginInterface.UiBuilder.OpenMainUi += OpenBestiary;
        framework.Update += OnUpdate;

        capture.Attach();
        nameplateMarker.Attach();
        progress.Refresh();
    }

    public void Dispose()
    {
        framework.Update -= OnUpdate;
        pluginInterface.UiBuilder.Draw -= windows.Draw;
        pluginInterface.UiBuilder.OpenConfigUi -= OpenConfig;
        pluginInterface.UiBuilder.OpenMainUi -= OpenBestiary;

        capture.Detach();
        nameplateMarker.Detach();

        commands.RemoveHandler(Command);
        commands.RemoveHandler(CommandAlias);
        windows.RemoveAllWindows();

        config.Save();

        UiNavigator.OpenBestiary = null;
        UiNavigator.OpenProgress = null;
        UiNavigator.OpenSettings = null;
        UiNavigator.ToggleOverlay = null;
    }

    private void OnUpdate(IFramework _)
    {
        combatState.Tick();
        capture.Tick(combatState);
        progress.Tick();
        nameplateMarker.Tick();
        navigation.Tick();

        var shouldShowOverlay = config.OverlayEnabled &&
            (!config.ShowOnlyOnBeastmaster || gameData.IsBeastmaster(combatState.Player));

        overlay.IsOpen = shouldShowOverlay;
    }

    private void OnCommand(string command, string args)
    {
        switch ((args ?? string.Empty).Trim().ToLowerInvariant())
        {
            case "bestiary":
            case "bestiariusz":
                bestiary.IsOpen = true;
                break;
            case "progress":
            case "progres":
                progressWindow.IsOpen = true;
                break;
            case "config":
            case "cfg":
                configWindow.IsOpen = true;
                break;
            case "overlay":
                UiNavigator.ToggleOverlay?.Invoke();
                break;
            case "debug":
                gameData.Dump(log);
                chat.Print("[BST Assist] Zrzut akcji Beastmastera wyslany do /xllog.");
                break;
            default:
                bestiary.IsOpen = true;
                break;
        }
    }

    private void OpenConfig() => configWindow.IsOpen = true;
    private void OpenBestiary() => bestiary.IsOpen = true;
}
