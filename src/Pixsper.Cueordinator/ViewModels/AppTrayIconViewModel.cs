using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Pixsper.Cueordinator.Services;
using ReactiveUI;

namespace Pixsper.Cueordinator.ViewModels;

public class AppTrayIconViewModel : ReactiveObject
{
    private readonly ISyncService _syncService;

    public AppTrayIconViewModel(ISyncService syncService)
    {
        _syncService = syncService;

        SyncNow = ReactiveCommand.CreateFromTask(onSyncNowAsync);
        ToggleIsPaused = ReactiveCommand.Create(() =>
        {
            IsPaused = !IsPaused;
        });

        OpenConfiguration = ReactiveCommand.Create(() => App.Current?.OpenMainWindow());
        OpenAbout = ReactiveCommand.Create(() => App.Current?.OpenAboutWindow());
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


    public ReactiveCommand<Unit, Unit> OpenAbout { get; }

    public ReactiveCommand<Unit, Unit> Exit { get; }



    private async Task onSyncNowAsync(CancellationToken cancellationToken)
    {
        //await _syncService.RunSyncAsync(cancellationToken).ConfigureAwait(false);
    }
}