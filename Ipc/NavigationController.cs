using System;
using BeastmasterAssist.Data;
using Dalamud.Plugin.Services;

namespace BeastmasterAssist.Ipc;

public sealed class NavigationController
{
    private enum State { Idle, Teleporting, WaitNavReady, Pathing }

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
            chat.PrintError("[BST Assist] Brak zdefiniowanej strefy dla tej bestii.");
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
        var location = beast.Location;
        if (string.IsNullOrWhiteSpace(location)) return null;
        var idx = location.IndexOfAny(new[] { '(', ',' });
        var zone = idx > 0 ? location[..idx] : location;
        zone = zone.Trim();
        return zone.Length == 0 ? null : zone;
    }
}
