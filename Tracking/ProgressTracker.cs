using BeastmasterAssist.Data;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace BeastmasterAssist.Tracking;

public sealed class ProgressTracker
{
    private readonly Configuration config;
    private readonly GameData gameData;
    private DateTime nextRefresh = DateTime.MinValue;

    // Kontenery skanowane pod katem posiadanego gearu Beastmastera. Siodlarnie
    // pomijamy - gear BST tam nie trafia, a skanowanie ich jest niepotrzebne.
    private static readonly InventoryType[] ScannedContainers =
    [
        InventoryType.EquippedItems,
        InventoryType.Inventory1, InventoryType.Inventory2, InventoryType.Inventory3, InventoryType.Inventory4,
        InventoryType.ArmoryMainHand, InventoryType.ArmoryOffHand, InventoryType.ArmoryHead,
        InventoryType.ArmoryBody, InventoryType.ArmoryHands, InventoryType.ArmoryFeets, InventoryType.ArmoryEar,
    ];

    public ProgressTracker(Configuration config, GameData data, IDataManager dataManager, IClientState clientState, IPluginLog log)
    {
        this.config = config;
        this.gameData = data;
    }

    public void Tick()
    {
        if (DateTime.UtcNow < nextRefresh) return;
        nextRefresh = DateTime.UtcNow.AddSeconds(5);
        Refresh();
    }

    // Automatycznie zaznacza w config.OwnedGearKeys itemy gearu BST znalezione
    // w ekwipunku, sakiewkach lub zbrojowni. Nigdy nie odznacza automatycznie -
    // jesli item zostanie sprzedany albo przeniesiony do retainera, checkbox
    // zostaje jako zapis postepu (ten sam wzorzec co CapturedBeastIds).
    public unsafe void Refresh()
    {
        var im = InventoryManager.Instance();
        if (im is null) return;

        var owned = new HashSet<uint>();
        foreach (var type in ScannedContainers)
        {
            var container = im->GetInventoryContainer(type);
            if (container is null) continue;
            for (var i = 0; i < container->Size; i++)
            {
                var slot = container->GetInventorySlot(i);
                if (slot is null || slot->ItemId == 0) continue;
                owned.Add(slot->ItemId);
            }
        }

        if (owned.Count == 0) return;

        var changed = false;
        foreach (var piece in ProgressCatalog.ExclusiveGear)
        {
            if (piece.Slot is "Set" or "Mount") continue;
            if (!gameData.Items.TryGetValue(piece.Name, out var itemId) || itemId == 0) continue;
            if (!owned.Contains(itemId)) continue;
            if (config.OwnedGearKeys.Add(piece.Key)) changed = true;
        }

        if (changed) config.Save();
    }

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
