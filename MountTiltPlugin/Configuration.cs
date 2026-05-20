using Dalamud.Configuration;
using System;

namespace MountTiltPlugin;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public float GroundTiltAngleMultiplier = 2f;
    public float GroundTiltSpeedMultiplier = 1.5f;
    public float FlightTiltAngleMultiplier = 2f;
    public float FlightTiltSpeedMultiplier = 1.5f;

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
