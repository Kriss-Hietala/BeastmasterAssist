# Beastmaster Assist

Dalamud overlay and tracker for Final Fantasy XIV Beastmaster (Patch 7.56).
Suggests rotation, reactions and mitigation, tracks all 50 Master's Bestiary pacts, and follows exclusive gear / achievement progress.
**Does not press abilities for you.** Square Enix bans combat automation.

## Commands

| Command | Effect |
|---|---|
| `/bstassist` or `/bestia` | Bestiary window |
| `/bstassist overlay` | Toggle overlay |
| `/bstassist progress` | Achievements, exclusive gear, job quests |
| `/bstassist config` | Settings |
| `/bstassist debug` | Dump resolved BST action IDs to `/xllog` |

## Build (Linux / Bazzite)

Requires .NET 10 and XIVLauncher + Dalamud.

```bash
dotnet build -c Release
```

If Dalamud after 7.56 wants API 16, bump `DalamudApiLevel` in `BeastmasterAssist.json` and the SDK version in the `.csproj`.

Dev plugin in XIVLauncher.Core:

1. Build the DLL.
2. In-game `/xlplugins` → gear icon → Experimental / Dev Plugins.
3. Add the path to `bin/Release/BeastmasterAssist.json` or the folder that contains the DLL.

Typical xlcore paths:

- Dalamud: `~/.xlcore/dalamud/`
- Windows: `%AppData%\\XIVLauncher\\`

## After load

1. Switch to Beastmaster and run `/bstassist debug` — `/xllog` lists the resolved ClassJob and action names from your client Excel (EN/PL).
2. Tick captured beasts; chat pacts auto-check if the message contains the beast name.
3. Quests and gear are a manual checklist; remnant counts are typed in.
4. Job-gauge TP after 7.56 does not yet have a stable ClientStructs layout. Overlay TP is inferred from statuses and combo until you wire `CombatState.ReadGaugeFallback` to the real gauge.

## Level 50 rotation (short)

1. Battlehorn (One with Nature, reset TR/Borrow).
2. Borrow **or** Tempered Release — both consume One with Nature.
3. Smash Axe → Axeblade Bite → Shieldsplitter until TP ≥ 100.
4. Trick, then the clockwise instinct axe in the 7s Heart window (Volant → Rampant → Durant → Eldritch).
5. At 250 TP the axe upgrades to Sunstrider/Moonstalker; the opposite state completes Universality.
6. Parting Blow **only** with Lingering Vantage, then swap (90s CD from withdraw).

Duty beasts (17, 18, 37, 38, 43–50): Unrestricted Party does **not** hurt catch rate. Alternative: Kornago gourd for remnants, Central Shroud (21.9, 22.6).
