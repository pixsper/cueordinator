using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace Pixsper.Cueordinator.ViewModels;

public class AppTrayIconViewModel : ReactiveObject
{
    public AppTrayIconViewModel()
    {
        SyncNow = ReactiveCommand.CreateFromTask(onSyncNowAsync);
        ToggleIsPaused = ReactiveCommand.Create(() =>
        {
            IsPaused = !IsPaused;
        });

        OpenConfiguration = ReactiveCommand.Create(() => App.Current?.OpenMainWindow());
        Exit = ReactiveCommand.Create(() => App.Current?.Exit());
    }

    private bool _isPaused;
    public bool IsPaused
    {
        get => _isPaused;
        set => this.RaiseAndSetIfChanged(ref _isPaused, value);
    }

    public ReactiveCommand<Unit, Unit> SyncNow { get; }

    public ReactiveCommand<Unit, Unit> ToggleIsPaused { get; }

    public ReactiveCommand<Unit, Unit> OpenConfiguration { get; }

    public ReactiveCommand<Unit, Unit> Exit { get; }



    private async Task onSyncNowAsync(CancellationToken cancellationToken)
    {
        
    }
}