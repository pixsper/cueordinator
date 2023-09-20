using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Pixsper.Cueordinator.Services;
using Pixsper.Cueordinator.ViewModels;
using Pixsper.Cueordinator.Views;

namespace Pixsper.Cueordinator;

public partial class App : Application, IAsyncDisposable
{

    public new static App? Current => (App?)Application.Current;

    public static string Version
    {
        get
        {
#if DEBUG
            return $"{GitVersionInformation.FullSemVer} DEBUG";
#else
            return GitVersionInformation.FullSemVer;
#endif
        }
    }

    private readonly ServiceProvider _serviceProvider;
    private readonly SyncService _syncService;



    public App()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddHttpClient();
        serviceCollection.AddSingleton<SyncService>();
        serviceCollection.AddSingleton<DisguiseHttpClient>();

        serviceCollection.AddTransient<AppTrayIconViewModel>();
        serviceCollection.AddTransient<ConfigWindowViewModel>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
        _syncService = _serviceProvider.GetRequiredService<SyncService>();
    }

    public async ValueTask DisposeAsync()
    {
        await _serviceProvider.DisposeAsync().ConfigureAwait(false);
    }


    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = _serviceProvider.GetRequiredService<AppTrayIconViewModel>();
    }

    public void OpenMainWindow()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            if (desktopLifetime.MainWindow?.IsEffectivelyVisible != true)
            {
                var configWindow = new ConfigWindow
                {
                    DataContext = _serviceProvider.GetRequiredService<ConfigWindowViewModel>()
                };

                desktopLifetime.MainWindow = configWindow;
                desktopLifetime.MainWindow.Show();
            }
            else
            {
                Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await Task.Delay(16);
                    desktopLifetime.MainWindow?.Activate();
                });
            }
        }
    }

    public void Exit()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            desktopLifetime.Shutdown();
    }
}