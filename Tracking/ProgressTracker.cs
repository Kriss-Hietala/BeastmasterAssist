using BeastmasterAssist.Data;
using Dalamud.Plugin.Services;

namespace BeastmasterAssist.Tracking;

public sealed class ProgressTracker
{
    private readonly Configuration config;
    private DateTime nextRefresh = DateTime.MinValue;

    public ProgressTracker(Configuration config, GameData data, IDataManager dataManager, IClientState clientState, IPluginLog log)
    {
        this.config = config;
    }

    public void Tick()
    {
        if (DateTime.UtcNow < nextRefresh) return;
        nextRefresh = DateTime.UtcNow.AddSeconds(5);
        Refresh();
    }

    public void Refresh() { }

    public bool AchievementDone(AchievementDef def)
    {
        if (def.Key.StartsWith("hand"))
        {
            var need = def.Key[^1] switch { '1' => 10, '2' => 20, '3' => 30, '4' => 40, _ => 50 };
            return config.CapturedBeastIds.Count >= need;
        }
        return false;
    }

    public bool GearOwned(GearPiece piece) => config.OwnedGearKeys.Contains(piece.Key);

    public void ToggleGear(string key)
    {
        if (!config.OwnedGearKeys.Add(key)) config.OwnedGearKeys.Remove(key);
        config.Save();
    }

    public void ToggleQuest(string key)
    {
        if (!config.CompletedQuestKeys.Add(key)) config.CompletedQuestKeys.Remove(key);
        config.Save();
    }
}
