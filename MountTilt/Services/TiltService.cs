using System;
using System.Threading.Tasks;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace MountTilt.Services;

public class TiltService : IAsyncDisposable
{
    private Hook<EffectContainer.Delegates.LoadMountTiltData> loadTiltDataHook = null!;

    public TiltService()
    {
        Plugin.GameInteropProvider.InitializeFromAttributes(this);
    }
    
    public unsafe void Initialize()
    {
        loadTiltDataHook = Plugin.GameInteropProvider.HookFromAddress<EffectContainer.Delegates.LoadMountTiltData>(EffectContainer.Addresses.LoadMountTiltData.Value, LoadTiltDataDetour);
        loadTiltDataHook.Enable();
        ForceUpdate();
    }


    private unsafe void LoadTiltDataDetour(EffectContainer* thisPtr)
    {
        loadTiltDataHook.Original(thisPtr);
        // Apply only to mounts
        if (thisPtr->OwnerObject->ObjectKind != ObjectKind.Mount) return;
        thisPtr->MountFlightSwimTiltAngle *= Plugin.Configuration.FlightTiltAngleMultiplier;
        thisPtr->MountFlightSwimTiltSpeed *= Plugin.Configuration.FlightTiltSpeedMultiplier;
        thisPtr->MountGroundTiltAngle *= Plugin.Configuration.GroundTiltAngleMultiplier;
        thisPtr->MountGroundTiltSpeed *= Plugin.Configuration.GroundTiltSpeedMultiplier;
    }
    
    public unsafe void ForceUpdate()
    {
        foreach (var battleChara in Plugin.ObjectTable.PlayerObjects)
        {
            
            var csBattleChara = (BattleChara*)battleChara.Address;
            if (csBattleChara->Mount.MountObject != null)
            {
                csBattleChara->Mount.MountObject->Effects.LoadMountTiltData();
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