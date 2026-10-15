using System;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;

namespace MountTilt.Windows;

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
        using (ImRaii.Group())
        {
            DrawConfiguration();
        }

        KofiButton(ImGui.GetItemRectSize().X + ImGui.GetStyle().WindowPadding.X);
    }

    private void DrawConfiguration()
    {
        using var wrapWidth = ImRaii.TextWrapPos(ImGui.CalcItemWidth() + ImGui.CalcTextSize("Angle").X + ImGuiHelpers.GlobalScale * (ImGui.GetStyle().ItemInnerSpacing.X + ImGui.GetStyle().WindowPadding.X * 2));
        ImGui.TextWrapped("Adjust the sliders below to set your tilt multiplier, you can Ctrl+Click the sliders to manually enter a number, but the game caps the angle at about 80° so there's no point in entering extreme values.");
        bool edited = false;
        using (ImRaii.PushId("Ground"))
        {
            ImGui.Separator();
            ImGui.Text("Ground tilt multipliers");
            edited |= ImGui.SliderFloat("Angle", ref Plugin.Configuration.GroundTiltAngleMultiplier, vMin: 0f, vMax: 5f);
            edited |= ImGui.SliderFloat("Speed", ref Plugin.Configuration.GroundTiltSpeedMultiplier, vMin: 0f, vMax: 5f);
        }

        using (ImRaii.PushId("Flight"))
        {
            ImGui.Separator();
            ImGui.Text("Flight tilt multipliers");
            edited |= ImGui.SliderFloat("Angle", ref Plugin.Configuration.FlightTiltAngleMultiplier, vMin: 0f, vMax: 5f);
            edited |= ImGui.SliderFloat("Speed", ref Plugin.Configuration.FlightTiltSpeedMultiplier, vMin: 0f, vMax: 5f);
        }

        if (edited)
        {
            Plugin.TiltService.ForceUpdate();
            Plugin.Configuration.Save();
        }
    }

    private void KofiButton(float contentWidth)
    {
        var size = ImGui.CalcTextSize("Buy my cat a treat on Ko-fi") + ImGui.GetStyle().FramePadding * 2;
        if (SizeConstraints.HasValue)
        {
            var minimumSize = SizeConstraints.Value.MinimumSize * ImGui.GetIO().FontGlobalScale;
            contentWidth = Math.Max(contentWidth, minimumSize.X);
            if (ImGui.GetCursorPosY() + size.Y + ImGui.GetStyle().WindowPadding.Y <= minimumSize.Y)
            {
                ImGui.SetCursorPosY(minimumSize.Y - size.Y - ImGui.GetStyle().WindowPadding.Y);
            }
        }

        ImGui.Separator();
        ImGui.SetCursorPosX(contentWidth - size.X);
        using (ImRaii.PushColor(ImGuiCol.Text, 0xFFFFFFFF))
        using (ImRaii.PushColor(ImGuiCol.Button, 0xFFFF478E))
        using (ImRaii.PushColor(ImGuiCol.ButtonHovered, 0xFFFF2478))
        using (ImRaii.PushColor(ImGuiCol.ButtonActive, 0xFFBD0048))
            if (ImGui.Button("Buy my cat a treat on Ko-fi"))
            {
                Util.OpenLink("https://ko-fi.com/speedas");
            }
    }
}
