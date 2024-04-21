using Avalonia;
using Avalonia.ReactiveUI;
using Projektanker.Icons.Avalonia.FontAwesome;
using Projektanker.Icons.Avalonia;
using System;
using System.Diagnostics;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Pixsper.Cueordinator;

public class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        var builder = BuildAvaloniaApp();
        var lifetime = new ClassicDesktopStyleApplicationLifetime()
        {
            Args = args,
            ShutdownMode = ShutdownMode.OnExplicitShutdown
        };
        builder.SetupWithLifetime(lifetime);

        var app = builder.Instance!;
        int returnCode = lifetime.Start(args);

        ((App)app).DisposeAsync().AsTask().Wait();

        return returnCode;
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        IconProvider.Current
            .Register<FontAwesomeIconProvider>();

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI()
            .With(new MacOSPlatformOptions
            {
                DisableSetProcessName = true,
                ShowInDock = false
            });
    }
}