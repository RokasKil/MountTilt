using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;

namespace MountTiltPlugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    public ConfigWindow() : base("Mount Tilt Configuration")
    {
        Flags = ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;

        SizeCondition = ImGuiCond.Always;
    }

    public void Dispose() { }


    public override void Draw()
    {

        using var wrapWidth = ImRaii.TextWrapPos(ImGui.CalcItemWidth() + ImGui.CalcTextSize("Angle").X + ImGuiHelpers.GlobalScale * (ImGui.GetStyle().ItemInnerSpacing.X + ImGui.GetStyle().WindowPadding.X * 2));
        ImGui.TextWrapped("Adjust the sliders below to set your tilt multiplier, you can Ctrl+Click the sliders to manually enter a number, but the game caps the angle at about 80° so there's no point in entering extreme values.");
        bool edited = false;
        ImGui.Separator();
        ImGui.Text("Ground tilt multipliers");
        edited |= ImGui.SliderFloat("Angle", ref Plugin.Configuration.GroundTiltAngleMultiplier, vMin: 0f, vMax: 5f);
        edited |= ImGui.SliderFloat("Speed", ref Plugin.Configuration.GroundTiltSpeedMultiplier, vMin: 0f, vMax: 5f);
        ImGui.Separator();
        ImGui.Text("Flight tilt multipliers");
        edited |= ImGui.SliderFloat("Angle", ref Plugin.Configuration.FlightTiltAngleMultiplier, vMin: 0f, vMax: 5f);
        edited |= ImGui.SliderFloat("Speed", ref Plugin.Configuration.FlightTiltSpeedMultiplier, vMin: 0f, vMax: 5f);
        if (edited)
        {
            Plugin.TiltService.ForceUpdate();
            Plugin.Configuration.Save();
        }
    }
}
