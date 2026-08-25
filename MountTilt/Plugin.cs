using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using MountTilt.Services;
using MountTilt.Windows;

namespace MountTilt;

public sealed class Plugin : IAsyncDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IGameInteropProvider GameInteropProvider { get; private set; } = null!;
    [PluginService] internal static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;
    internal static TiltService TiltService { get; private set; } = null!;
    
    internal static Configuration Configuration { get; private set; } = null!;

    private const string CommandName = "/mounttilt";
    
    private readonly WindowSystem windowSystem = new("MountTilt");
    
    private ConfigWindow ConfigWindow { get; init; }

    public Plugin()
    {
        TiltService = new TiltService();
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        
        ConfigWindow = new ConfigWindow();
        windowSystem.AddWindow(ConfigWindow);
        
        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Open the configuration window"
        });

        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;
        PluginInterface.UiBuilder.Draw += windowSystem.Draw;

    }
    
    public Task LoadAsync(CancellationToken cancellationToken)
    {
        return Framework.Run(TiltService.Initialize, cancellationToken);
    }

    private void OnCommand(string command, string args)
    {
        ConfigWindow.Toggle();
    }
    
    public void ToggleConfigUi() => ConfigWindow.Toggle();

    public async ValueTask DisposeAsync()
    {
        PluginInterface.UiBuilder.Draw -= windowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        windowSystem.RemoveAllWindows();
        ConfigWindow.Dispose();
        CommandManager.RemoveHandler(CommandName);
        await TiltService.DisposeAsync();
    }
}
