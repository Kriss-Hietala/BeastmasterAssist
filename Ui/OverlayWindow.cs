using System.Numerics;
using BeastmasterAssist.Combat;
using BeastmasterAssist.Data;
using BeastmasterAssist.Tracking;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
namespace BeastmasterAssist.Ui;
public sealed class OverlayWindow : Window
{
    private readonly Configuration config; private readonly CombatState state; private readonly RotationAdvisor rotation; private readonly ReactionAdvisor reactions; private readonly MitigationAdvisor mitigation; private readonly CaptureTracker capture;
    public OverlayWindow(Configuration config, CombatState state, RotationAdvisor rotation, ReactionAdvisor reactions, MitigationAdvisor mitigation, CaptureTracker capture) : base("Beastmaster Assist##BeastmasterAssistOverlay", ImGuiWindowFlags.NoCollapse) { this.config=config; this.state=state; this.rotation=rotation; this.reactions=reactions; this.mitigation=mitigation; this.capture=capture; RespectCloseHotkey=false; Size=new Vector2(420,0); SizeCondition=ImGuiCond.FirstUseEver; Position=new Vector2(360,650); PositionCondition=ImGuiCond.FirstUseEver; }
    public override void PreDraw() { Flags=ImGuiWindowFlags.NoCollapse; if(config.LockOverlay) Flags|=ImGuiWindowFlags.NoMove; ImGui.SetNextWindowBgAlpha(0.94f); }
    public override void Draw() { Loc.Polish=config.PreferPolishUi; ImGui.SetWindowFontScale(Math.Clamp(config.OverlayScale*1.28f,1f,2f)); ImGui.TextColored(new Vector4(1f,.78f,.25f,1f),"BEASTMASTER ASSIST"); ImGui.SameLine(); ImGui.TextDisabled(state.Player is not null ? $"Lv{state.Level}" : "No player"); ImGui.SameLine(); if(ImGui.Button(Loc.T("UKRYJ OVERLAY","HIDE OVERLAY"))) UiNavigator.ToggleOverlay?.Invoke(); ImGui.TextDisabled($"TP {state.PlayerTp}/250    Familiar {state.FamiliarTp}/250    {state.ActiveKinship}"); ImGui.Separator(); DrawAdviceSection(Loc.T("ROTACJA","ROTATION"),rotation.Advise(state),4,new Vector4(.95f,.95f,.95f,1f)); if(config.ShowReactions) DrawAdviceSection(Loc.T("REAKCJE","REACTIONS"),reactions.Advise(state),3,new Vector4(1f,.58f,.34f,1f)); if(config.ShowMitigation) DrawAdviceSection(Loc.T("MITYGACJA","MITIGATION"),mitigation.Advise(state),3,new Vector4(.4f,.83f,1f,1f)); if(config.ShowCaptureHud&&!string.IsNullOrEmpty(capture.CapturePrompt)){ImGui.Separator();ImGui.TextColored(new Vector4(.6f,1f,.48f,1f),Loc.T("LAPANIE","CAPTURE"));ImGui.TextWrapped(capture.CapturePrompt);} ImGui.SetWindowFontScale(1f); }
    private static void DrawAdviceSection(string title,IEnumerable<Advice> advice,int maximum,Vector4 colour){var shown=advice.Take(maximum).ToList();if(shown.Count==0)return;ImGui.TextColored(colour,title);foreach(var item in shown){ImGui.TextColored(colour,$"▶ {item.Action}");ImGui.SameLine();ImGui.TextDisabled(item.Reason);}}
}
