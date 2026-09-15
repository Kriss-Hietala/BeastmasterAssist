using System;
using System.Linq;
using BeastmasterAssist.Data;
using Dalamud.Plugin.Services;

namespace BeastmasterAssist.Ipc;

public sealed class NavigationController
{
    private enum State { Idle, Teleporting, WaitNavReady, Pathing }

    // Znane nazwy stref ARR uzywane w BestiaryCatalog. Location w katalogu czesto
    // nie ma zadnego separatora miedzy nazwa strefy a podlokacja (np. "Western Thanalan
    // The Footfalls"), wiec dopasowujemy prefiks do tej listy zamiast liczyc na przecinek/nawias.
    // Posortowane od najdluzszych, zeby dluzsze nazwy nie byly przycinane przez krotsze prefiksy.
    private static readonly string[] KnownZones = new[]
    {
        "Limsa Lominsa Upper Decks", "Limsa Lominsa Lower Decks", "Wolves' Den Pier",
        "Ul'dah - Steps of Nald", "Ul'dah - Steps of Thal",
        "Western Thanalan", "Central Thanalan", "Eastern Thanalan", "Southern Thanalan", "Northern Thanalan",
        "Middle La Noscea", "Lower La Noscea", "Eastern La Noscea", "Western La Noscea", "Upper La Noscea", "Outer La Noscea",
        "New Gridania", "Old Gridania", "Central Shroud", "East Shroud", "South Shroud", "North Shroud",
        "Mor Dhona",
    }.OrderByDescending(z => z.Length).ToArray();

    private readonly NavigationIpc ipc;
    private readonly IChatGui chat;
    private readonly IPluginLog log;

    private State state = State.Idle;
    private DateTime stateEnteredAt;
    private System.Numerics.Vector3? targetPos;
    private string? targetLabel;

    public NavigationController(NavigationIpc ipc, IChatGui chat, IPluginLog log)
    {
        this.ipc = ipc;
        this.chat = chat;
        this.log = log;
    }

    public bool IsBusy => state != State.Idle;

    public bool CanTravel(BeastEntry beast) =>
        ipc.LifestreamAvailable && !string.IsNullOrWhiteSpace(ResolveZone(beast));

    public void TravelTo(BeastEntry beast)
    {
        if (state != State.Idle) return;

        if (!ipc.LifestreamAvailable)
        {
            chat.PrintError("[BST Assist] Lifestream nie jest zainstalowany lub nieaktywny.");
            return;
        }

        var zone = ResolveZone(beast);
        if (string.IsNullOrWhiteSpace(zone))
        {
            chat.PrintError("[BST Assist] Nie mozna wyznaczyc celu teleportu dla tej bestii (duty/quest lub nieznana strefa).");
            return;
        }

        if (!ipc.TeleportTo(zone))
        {
            chat.PrintError($"[BST Assist] Nie udalo sie wyslac polecenia teleportu do Lifestream ({zone}).");
            return;
        }

        targetPos = beast.TargetPosition;
        targetLabel = beast.Name;

        chat.Print($"[BST Assist] Teleportuje w rejon: {zone}...");
        state = State.Teleporting;
        stateEnteredAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (state == State.Pathing) ipc.StopPathing();
        state = State.Idle;
        targetPos = null;
        targetLabel = null;
    }

    public void Tick()
    {
        if (state == State.Idle) return;

        if (DateTime.UtcNow - stateEnteredAt > TimeSpan.FromSeconds(30))
        {
            chat.PrintError("[BST Assist] Przekroczono czas oczekiwania na nawigacje.");
            Cancel();
            return;
        }

        switch (state)
        {
            case State.Teleporting:
                if (!ipc.IsLifestreamBusy())
                {
                    if (targetPos is null || !ipc.VnavmeshAvailable)
                    {
                        if (targetPos is not null && !ipc.VnavmeshAvailable)
                            chat.Print("[BST Assist] vnavmesh niedostepny - dojdz do bestii recznie.");
                        state = State.Idle;
                        targetPos = null;
                        targetLabel = null;
                        return;
                    }
                    state = State.WaitNavReady;
                    stateEnteredAt = DateTime.UtcNow;
                }
                break;

            case State.WaitNavReady:
                if (ipc.IsNavReady())
                {
                    if (ipc.MoveTo(targetPos!.Value))
                    {
                        state = State.Pathing;
                        stateEnteredAt = DateTime.UtcNow;
                    }
                    else
                    {
                        chat.PrintError("[BST Assist] vnavmesh odrzucil pathfinding.");
                        state = State.Idle;
                        targetPos = null;
                        targetLabel = null;
                    }
                }
                break;

            case State.Pathing:
                if (!ipc.IsPathing())
                {
                    chat.Print($"[BST Assist] Na miejscu: {targetLabel}.");
                    state = State.Idle;
                    targetPos = null;
                    targetLabel = null;
                }
                break;
        }
    }

    private static string? ResolveZone(BeastEntry beast)
    {
        if (!string.IsNullOrWhiteSpace(beast.Zone)) return beast.Zone;

        // Duty (dungeon/trial/raid/alliance) i questowe familiary nie maja sensownego
        // celu teleportu przez Lifestream.
        if (beast.Duty) return null;
        if (string.Equals(beast.Habitat, "Quest", StringComparison.OrdinalIgnoreCase)) return null;

        var location = beast.Location;
        if (string.IsNullOrWhiteSpace(location)) return null;

        foreach (var zone in KnownZones)
        {
            if (location.StartsWith(zone, StringComparison.OrdinalIgnoreCase))
                return zone;
        }

        // Fallback: stara metoda przez separator, gdyby strefa byla spoza znanej listy
        // (np. przyszle rozszerzenie bestiariusza o inne expansiony).
        var idx = location.IndexOfAny(new[] { '(', ',' });
        var fallback = idx > 0 ? location[..idx] : location;
        fallback = fallback.Trim();
        return fallback.Length == 0 ? null : fallback;
    }
}
