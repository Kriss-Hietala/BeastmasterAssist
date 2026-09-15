using System;
using System.Linq;
using System.Numerics;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;

namespace BeastmasterAssist.Ipc;

public sealed class NavigationIpc
{
    private readonly IDalamudPluginInterface pi;

    private readonly ICallGateSubscriber<string, object> lifestreamExecuteCommand;
    private readonly ICallGateSubscriber<bool> lifestreamIsBusy;

    private readonly ICallGateSubscriber<bool> navIsReady;
    private readonly ICallGateSubscriber<Vector3, bool, bool> navPathfindAndMoveTo;
    private readonly ICallGateSubscriber<bool> navPathfindInProgress;
    private readonly ICallGateSubscriber<object> navPathStop;

    public NavigationIpc(IDalamudPluginInterface pi)
    {
        this.pi = pi;

        lifestreamExecuteCommand = pi.GetIpcSubscriber<string, object>("Lifestream.ExecuteCommand");
        lifestreamIsBusy = pi.GetIpcSubscriber<bool>("Lifestream.IsBusy");

        navIsReady = pi.GetIpcSubscriber<bool>("vnavmesh.Nav.IsReady");
        navPathfindAndMoveTo = pi.GetIpcSubscriber<Vector3, bool, bool>("vnavmesh.SimpleMove.PathfindAndMoveTo");
        navPathfindInProgress = pi.GetIpcSubscriber<bool>("vnavmesh.SimpleMove.PathfindInProgress");
        navPathStop = pi.GetIpcSubscriber<object>("vnavmesh.Path.Stop");
    }

    public bool LifestreamAvailable => pi.InstalledPlugins.Any(p => p.InternalName == "Lifestream" && p.IsLoaded);
    public bool VnavmeshAvailable => pi.InstalledPlugins.Any(p => p.InternalName == "vnavmesh" && p.IsLoaded);

    public bool TeleportTo(string destination)
    {
        if (!LifestreamAvailable) return false;
        try { lifestreamExecuteCommand.InvokeAction(destination); return true; }
        catch { return false; }
    }

    public bool IsLifestreamBusy()
    {
        if (!LifestreamAvailable) return false;
        try { return lifestreamIsBusy.InvokeFunc(); }
        catch { return false; }
    }

    public bool IsNavReady()
    {
        if (!VnavmeshAvailable) return false;
        try { return navIsReady.InvokeFunc(); }
        catch { return false; }
    }

    public bool MoveTo(Vector3 pos, bool fly = false)
    {
        if (!VnavmeshAvailable) return false;
        try { return navPathfindAndMoveTo.InvokeFunc(pos, fly); }
        catch { return false; }
    }

    public bool IsPathing()
    {
        if (!VnavmeshAvailable) return false;
        try { return navPathfindInProgress.InvokeFunc(); }
        catch { return false; }
    }

    public void StopPathing()
    {
        if (!VnavmeshAvailable) return;
        try { navPathStop.InvokeAction(); }
        catch { }
    }
}
