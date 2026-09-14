using BeastmasterAssist.Combat;
using BeastmasterAssist.Data;
using BeastmasterAssist.Tracking;
using BeastmasterAssist.Ui;
using Dalamud.Game.Command;
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
    private readonly IClientState clientState;
    private readonly IObjectTable objects;
    private readonly IPluginLog log;
    private readonly WindowSystem windows = new("BeastmasterAssist");

    private readonly Configuration config;
    private readonly GameData gameData;
    private readonly CombatState combatState;
    private readonly RotationAdvisor rotation;
    private readonly ReactionAdvisor reactions;
    private readonly MitigationAdvisor mitigation;
    private readonly CaptureTracker capture;
    private readonly ProgressTracker progress;
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
        IPluginLog log)
    {
        this.pluginInterface = pluginInterface;
        this.commands = commands;
        this.framework = framework;
        this.chat = chat;
        this.clientState = clientState;
        this.objects = objects;
        this.log = log;

        config = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        config.Initialize(pluginInterface);

        gameData = new GameData(data, log);
        gameData.Resolve();

        combatState = new CombatState(objects, targets, condition, gameData, log);
        rotation = new RotationAdvisor(gameData, config);
        reactions = new ReactionAdvisor(gameData);
        mitigation = new MitigationAdvisor(gameData);
        capture = new CaptureTracker(config, gameData, chat, clientState, objects, log);
        progress = new ProgressTracker(config, gameData, data, clientState, log);

        overlay = new OverlayWindow(config, combatState, rotation, reactions, mitigation, capture);
        bestiary = new BestiaryWindow(config, capture, gameData);
        progressWindow = new ProgressWindow(config, progress, gameData);
        configWindow = new ConfigWindow(config);

        windows.AddWindow(overlay);
        windows.AddWindow(bestiary);
        windows.AddWindow(progressWindow);
        windows.AddWindow(configWindow);

        commands.AddHandler(Command, new CommandInfo(OnCommand)
        {
            HelpMessage = "Beastmaster Assist. /bstassist overlay|bestiary|progress|config"
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
        progress.Refresh();

        log.Info("Beastmaster Assist loaded. JobId={0} actions={1}", gameData.JobId, gameData.ResolvedActionCount);
    }

    public void Dispose()
    {
        framework.Update -= OnUpdate;
        pluginInterface.UiBuilder.Draw -= windows.Draw;
        pluginInterface.UiBuilder.OpenConfigUi -= OpenConfig;
        pluginInterface.UiBuilder.OpenMainUi -= OpenBestiary;
        capture.Detach();
        commands.RemoveHandler(Command);
        commands.RemoveHandler(CommandAlias);
        windows.RemoveAllWindows();
        config.Save();
    }

    private void OnUpdate(IFramework _)
    {
        var onJob = gameData.IsBeastmaster(objects.LocalPlayer);
        overlay.IsOpen = config.OverlayEnabled && (!config.ShowOnlyOnBeastmaster || onJob);
        if (!onJob && config.ShowOnlyOnBeastmaster)
            return;
        combatState.Tick();
        capture.Tick(combatState);
        progress.Tick();
    }

    private void OnCommand(string command, string args)
    {
        switch ((args ?? string.Empty).Trim().ToLowerInvariant())
        {
            case "bestiary":
            case "bestiariusz":
                bestiary.Toggle();
                break;
            case "progress":
            case "progres":
                progressWindow.Toggle();
                break;
            case "config":
            case "cfg":
                configWindow.Toggle();
                break;
            case "overlay":
                config.OverlayEnabled = !config.OverlayEnabled;
                config.Save();
                break;
            case "debug":
                gameData.Dump(log);
                chat.Print("[BST Assist] Zrzut akcji Beastmastera poszedl do /xllog.");
                break;
            default:
                bestiary.Toggle();
                break;
        }
    }

    private void OpenConfig() => configWindow.Toggle();
    private void OpenBestiary() => bestiary.Toggle();
}
