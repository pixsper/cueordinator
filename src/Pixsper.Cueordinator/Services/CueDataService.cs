using System;
using System.Reactive.Disposables;
using DynamicData;
using Microsoft.Extensions.Logging;
using Pixsper.Cueordinator.Models;

namespace Pixsper.Cueordinator.Services;

internal class CueDataService : ICueDataService, IDisposable
{
    private readonly ILogger _log;

    private readonly CompositeDisposable _disposable = new();

    private readonly SourceCache<Cue, CueNumber> _cues;

    public CueDataService(ILogger<CueDataService> log)
    {
        _log = log;

        _cues = new SourceCache<Cue, CueNumber>(c => c.Number)
            .DisposeWith(_disposable);
    }

    public IObservableCache<Cue, CueNumber> Cues => _cues.AsObservableCache();

    public void Dispose()
    {
        _disposable.Dispose();
    }
}