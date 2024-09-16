using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Pixsper.Cueordinator.Services;
using Pixsper.Cueordinator.Services.Connections;
using Pixsper.Cueordinator.ViewModels;
using Pixsper.Cueordinator.Views;

namespace Pixsper.Cueordinator;

public class App : Application, IAsyncDisposable
{
    public static new App? Current => (App?)Application.Current;

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



    public App()
    {
        var serviceCollection = new ServiceCollection();

        if (!Design.IsDesignMode)
        {
            serviceCollection.AddHttpClient();
            serviceCollection.AddSingleton<IConfigurationService, ConfigurationService>();
            serviceCollection.AddSingleton<IConnectionsService, ConnectionsService>();
            serviceCollection.AddSingleton<ISyncService, SyncService>();

            serviceCollection.AddSingleton<DisguiseConnection>();

            serviceCollection.AddHostedService<SchedulingService>();

            serviceCollection.AddTransient<AppTrayIconViewModel>();
            serviceCollection.AddTransient<ConfigurationWindowViewModel>();
            serviceCollection.AddTransient<AboutWindowViewModel>();
        }

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public async ValueTask DisposeAsync()
    {
        await _serviceProvider.DisposeAsync().ConfigureAwait(false);
    }


    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        if (!Design.IsDesignMode)
            DataContext = _serviceProvider.GetRequiredService<AppTrayIconViewModel>();
    }

    public void OpenMainWindow()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            if (desktopLifetime.MainWindow is AboutWindow || desktopLifetime.MainWindow?.IsEffectivelyVisible != true)
            {
                desktopLifetime.MainWindow?.Close();
                desktopLifetime.MainWindow = null;
            }

            if (desktopLifetime.MainWindow?.IsEffectivelyVisible != true)
            {
                var configWindow = new ConfigurationWindow
                {
                    DataContext = _serviceProvider.GetRequiredService<ConfigurationWindowViewModel>()
                };

                desktopLifetime.MainWindow = configWindow;
                desktopLifetime.MainWindow.Show();
            }
            else
            {
                activateMainWindow();
            }
        }
    }

    public void OpenAboutWindow()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            if (desktopLifetime.MainWindow?.IsEffectivelyVisible != true)
            {
                desktopLifetime.MainWindow?.Close();
                desktopLifetime.MainWindow = null;

                var aboutWindow = new AboutWindow
                {
                    DataContext = _serviceProvider.GetRequiredService<AboutWindowViewModel>()
                };
                desktopLifetime.MainWindow = aboutWindow;
                desktopLifetime.MainWindow.Show();
            }
            else if (desktopLifetime.MainWindow is AboutWindow)
            {
                activateMainWindow();
            }
            else if (desktopLifetime.MainWindow is ConfigurationWindow)
            {
                var aboutWindow = new AboutWindow
                {
                    DataContext = _serviceProvider.GetRequiredService<AboutWindowViewModel>()
                };

                aboutWindow.ShowDialog(desktopLifetime.MainWindow);
            }
        }
    }

    public void Exit()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            desktopLifetime.Shutdown();
    }

    private void activateMainWindow()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await Task.Delay(16);
                desktopLifetime.MainWindow?.Activate();
            });
        }
    }

    private void onOpenAboutWindowClicked(object? sender, EventArgs e)
    {
        OpenAboutWindow();
    }
}