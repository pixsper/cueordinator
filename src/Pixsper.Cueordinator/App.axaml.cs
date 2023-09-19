using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Pixsper.Cueordinator.ViewModels;
using Pixsper.Cueordinator.Views;

namespace Pixsper.Cueordinator;

public partial class App : Application
{
    public new static App? Current => (App?)Application.Current;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = new AppTrayIconViewModel();
    }

    public void OpenMainWindow()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            if (desktopLifetime.MainWindow?.IsEffectivelyVisible != true)
            {
                desktopLifetime.MainWindow = new MainWindow();
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

    private void onTrayIconClicked(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}