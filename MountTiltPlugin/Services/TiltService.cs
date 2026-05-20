using System;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace MountTiltPlugin.Services;

public class TiltService : IAsyncDisposable
{
    private Hook<EffectContainer.Delegates.LoadTiltData> loadTiltDataHook = null!;

    public TiltService()
    {
        Plugin.GameInteropProvider.InitializeFromAttributes(this);
    }
    
    public unsafe void Initialize()
    {
        loadTiltDataHook = Plugin.GameInteropProvider.HookFromAddress<EffectContainer.Delegates.LoadTiltData>(EffectContainer.Addresses.LoadTiltData.Value, LoadTiltDataDetour);
        loadTiltDataHook.Enable();
        ForceUpdate();
    }


    private unsafe void LoadTiltDataDetour(EffectContainer* thisPtr)
    {
        loadTiltDataHook.Original(thisPtr);
        // Apply only to mounts
        if (thisPtr->OwnerObject->ObjectKind != ObjectKind.Mount) return;
        thisPtr->FlightTiltAngle *= Plugin.Configuration.FlightTiltAngleMultiplier;
        thisPtr->FlightTiltSpeed *= Plugin.Configuration.FlightTiltSpeedMultiplier;
        thisPtr->GroundTiltAngle *= Plugin.Configuration.GroundTiltAngleMultiplier;
        thisPtr->GroundTiltSpeed *= Plugin.Configuration.GroundTiltSpeedMultiplier;
    }
    
    public unsafe void ForceUpdate()
    {
        foreach (var battleChara in Plugin.ObjectTable.PlayerObjects)
        {
            
            var csBattleChara = (BattleChara*)battleChara.Address;
            if (csBattleChara->Mount.MountObject != null)
            {
                csBattleChara->Mount.MountObject->Effects.LoadTiltData();
            }
        }
    }
    
    private void Dispose()
    {
        loadTiltDataHook?.Dispose();
        ForceUpdate();
    }

    public async ValueTask DisposeAsync()
    {
        await Plugin.Framework.RunOnFrameworkThread(Dispose);
    }
}