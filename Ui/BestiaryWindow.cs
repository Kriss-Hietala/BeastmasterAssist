using BeastmasterAssist.Data;
using BeastmasterAssist.Tracking;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
namespace BeastmasterAssist.Ui;
public sealed class BestiaryWindow:Window
{
 private readonly CaptureTracker capture; private string filter="";
 public BestiaryWindow(Configuration config,CaptureTracker capture,GameData data):base("Master's Bestiary##BeastmasterAssistBestiary"){this.capture=capture;}
 public override void Draw(){Nav();ImGui.TextUnformatted($"Pakty {capture.CapturedCount}/50");ImGui.InputText("Filtr",ref filter,64);ImGui.Separator();foreach(var b in BestiaryCatalog.All){if(!string.IsNullOrWhiteSpace(filter)&&b.Name.IndexOf(filter,StringComparison.OrdinalIgnoreCase)<0&&b.Location.IndexOf(filter,StringComparison.OrdinalIgnoreCase)<0)continue;var have=capture.Has(b.Id);if(ImGui.Checkbox($"##have{b.Id}",ref have))capture.Toggle(b.Id);ImGui.SameLine();ImGui.TextWrapped($"#{b.Id} {b.Name} | {b.Class}/{b.Affinity} | Lv{b.Level} | {b.Location} | {b.Trick}/{b.TemperedRelease}");}ImGui.Separator();ImGui.TextDisabled("Komendy: /bstassist lub /bestia | /bstassist overlay | /bstassist progress | /bstassist config");}
 private void Nav(){if(ImGui.Button("BESTIARIUSZ"))IsOpen=true;ImGui.SameLine();if(ImGui.Button("PROGRES"))UiNavigator.OpenProgress?.Invoke();ImGui.SameLine();if(ImGui.Button("USTAWIENIA"))UiNavigator.OpenSettings?.Invoke();ImGui.SameLine();if(ImGui.Button("ZAMKNIJ"))IsOpen=false;}
}
