using System;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveMarbles.ObservableEvents;

namespace Pixsper.Cueordinator.Views;

public partial class AppTrayIcon : TrayIcon
{
    public AppTrayIcon()
    {
        AvaloniaXamlLoader.Load(this);

        this.Events().Clicked.SelectMany(
                e => this.Events().Clicked.Take(1).Timeout(
                    TimeSpan.FromMilliseconds(500), Observable.Empty<EventArgs>()))
            .Subscribe(e =>
            {
                App.Current?.OpenMainWindow();
            });
    }
}