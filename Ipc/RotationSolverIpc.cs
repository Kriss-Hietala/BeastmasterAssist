using System;
using System.Linq;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;

namespace BeastmasterAssist.Ipc;

// Integracja z Rotation Solver Reborn (InternalName pluginu: "RotationSolver").
// RSR publikuje przez ECommons EzIPC dwa zdarzenia w klasie ActionUpdater
// (prefiks "RotationSolverReborn.ActionUpdater"), ktore przekazuja surowe ID akcji
// z gry (0 = brak sugestii) za kazdym razem, gdy zmienia sie sugerowana akcja.
public sealed class RotationSolverIpc : IDisposable
{
    private const string InternalPluginName = "RotationSolver";

    private readonly IDalamudPluginInterface pi;
    private readonly ICallGateSubscriber<uint, object> nextActionChanged;
    private readonly ICallGateSubscriber<uint, object> nextGcdActionChanged;
    private bool subscribed;

    public uint NextActionId { get; private set; }
    public uint NextGcdActionId { get; private set; }

    public RotationSolverIpc(IDalamudPluginInterface pi)
    {
        this.pi = pi;
        nextActionChanged = pi.GetIpcSubscriber<uint, object>("RotationSolverReborn.ActionUpdater.NextActionChanged");
        nextGcdActionChanged = pi.GetIpcSubscriber<uint, object>("RotationSolverReborn.ActionUpdater.NextGCDActionChanged");
        TrySubscribe();
    }

    public bool Available => pi.InstalledPlugins.Any(p => p.InternalName == InternalPluginName && p.IsLoaded);

    // Wolamy co klatke z Plugin.OnUpdate - jesli RSR zostanie wlaczony po starcie
    // BeastmasterAssist, subskrypcja dolaczy sie automatycznie przy pierwszej okazji.
    public void Tick()
    {
        if (!subscribed && Available) TrySubscribe();
    }

    private void TrySubscribe()
    {
        if (subscribed) return;
        try
        {
            nextActionChanged.Subscribe(OnNextActionChanged);
            nextGcdActionChanged.Subscribe(OnNextGcdActionChanged);
            subscribed = true;
        }
        catch
        {
            // RotationSolver niezaladowany - sprobujemy ponownie przy kolejnym Tick().
        }
    }

    private void OnNextActionChanged(uint id) => NextActionId = id;
    private void OnNextGcdActionChanged(uint id) => NextGcdActionId = id;

    public void Dispose()
    {
        if (!subscribed) return;
        try { nextActionChanged.Unsubscribe(OnNextActionChanged); } catch { }
        try { nextGcdActionChanged.Unsubscribe(OnNextGcdActionChanged); } catch { }
    }
}
