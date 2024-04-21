using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Pixsper.Cueordinator.Models;

namespace Pixsper.Cueordinator.Services;

internal class ConfigurationService : IConfigurationService, IAsyncDisposable
{
    private const string ConfigurationFileName = "configuration.json";
    private const int SaveThrottleTimeMs = 2000;

    private readonly ILogger _log;

    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly CompositeDisposable _disposable = new();

    private readonly AsyncLock _asyncLock = new();

    private readonly SourceCache<IConnectionConfiguration, Guid> _connections;
    private readonly SourceValue<Guid?> _sourceConnectionId;
    private readonly SourceList<Guid> _targetConnectionIds;

    public ConfigurationService(ILogger<ConfigurationService> log)
    {
        _log = log;

        _connections = new SourceCache<IConnectionConfiguration, Guid>(v => v.Id)
            .DisposeWith(_disposable);

        _sourceConnectionId = new SourceValue<Guid?>(null)
            .DisposeWith(_disposable);

        _targetConnectionIds = new SourceList<Guid>()
            .DisposeWith(_disposable);

        var saveExecutionsObservable = _connections.Connect()
            .Select(_ => Unit.Default)
            .Merge(_sourceConnectionId.Select(_ => Unit.Default))
            .Merge(_targetConnectionIds.Connect().Select(_ => Unit.Default))
            .Throttle(TimeSpan.FromMilliseconds(SaveThrottleTimeMs))
            .Select(_ => Observable.FromAsync(ct => saveAsync(collectConfiguration(), ct)))
            .Concat();

        Observable.StartAsync(loadAsync)
            .Subscribe(c =>
            {
                if (c is not null)
                {
                    _connections.AddOrUpdate(c.Connections);
                    _sourceConnectionId.Value = c.SourceConnectionId;
                    _targetConnectionIds.AddRange(c.TargetConnectionIds);

                    Observable.FromAsync(ct => saveAsync(c, ct))
                        .Concat(saveExecutionsObservable)
                        .Subscribe()
                        .DisposeWith(_disposable);
                }
            })
            .DisposeWith(_disposable);
    }

    public void Dispose()
    {

    }

    public async ValueTask DisposeAsync()
    {
        using var _ = await _asyncLock.LockAsync().ConfigureAwait(false);
        _disposable.Dispose();
    }


    public IObservableCache<IConnectionConfiguration, Guid> Connections => _connections.AsObservableCache();
    public IObservableValue<Guid?> SourceConnectionId => _sourceConnectionId.AsObservableValue();
    public IObservableList<Guid> TargetConnectionIds => _targetConnectionIds.AsObservableList();


    public void CreateOrUpdateConnection(IConnectionConfiguration connection) => _connections.AddOrUpdate(connection);
    public void DeleteConnection(IConnectionConfiguration connection)
    {
        _targetConnectionIds.Remove(connection.Id);
        _connections.Remove(connection);
    }

    public void DeleteConnection(Guid id)
    {
        _targetConnectionIds.Remove(id);
        _connections.RemoveKey(id);
    }

    public void SetSourceConnectionId(Guid? id)
    {
        _sourceConnectionId.Value = id;
    }

    public void CreateTargetConnection(Guid id) => _targetConnectionIds.Add(id);
    public void DeleteTargetConnection(Guid id) => _targetConnectionIds.Edit(u => u.Remove(id));

    private async Task<Configuration?> loadAsync(CancellationToken cancellationToken)
    {
        using var _ = await _asyncLock.LockAsync(cancellationToken);

        Configuration? configuration = null;
        try
        {
            var configurationFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData,
                    Environment.SpecialFolderOption.Create), App.ApplicationName, ConfigurationFileName);

            if (!File.Exists(configurationFileName))
            {
                configuration = new Configuration();
                return configuration;
            }

            await using var fs = File.OpenRead(configurationFileName);
            configuration = await JsonSerializer.DeserializeAsync<Configuration>(fs, _serializerOptions, cancellationToken).ConfigureAwait(false);
            if (configuration is null)
                _log.LogError("Failed to read configuration");
        }
        catch (JsonException ex)
        {
            _log.LogError(ex, "Failed to read configuration due to JSON error");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or PathTooLongException or DirectoryNotFoundException)
        {
            _log.LogError(ex, "Failed to read configuration due to filesystem error");
        }

        return configuration;
    }

    private Configuration collectConfiguration()
    {
        return new Configuration
        {
            Connections = _connections.Items.ToList(),
            SourceConnectionId = _sourceConnectionId.Value,
            TargetConnectionIds = _targetConnectionIds.Items.ToList()
        };
    }

    private async Task saveAsync(Configuration configuration, CancellationToken cancellationToken)
    {
        using var _ = await _asyncLock.LockAsync(cancellationToken);

        var configurationDirectoryName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            App.ApplicationName);

        if (!Directory.Exists(configurationDirectoryName))
        {
            try
            {
                Directory.CreateDirectory(configurationDirectoryName);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or DirectoryNotFoundException)
            {

                return;
            }
        }

        var configurationFileName = Path.Combine(configurationDirectoryName, ConfigurationFileName);

        var tempFileName = Path.GetTempFileName();
        try
        {
            {
                await using var fs = File.OpenWrite(tempFileName);
                await JsonSerializer.SerializeAsync(fs, configuration, _serializerOptions, cancellationToken).ConfigureAwait(false);
            }

            if (File.Exists(configurationFileName))
                File.Replace(tempFileName, configurationFileName, null);
            else
                File.Copy(tempFileName, configurationFileName);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or PathTooLongException or DirectoryNotFoundException)
        {
            _log.LogError(ex, "Failed to save configuration");
        }
        finally
        {
            File.Delete(tempFileName);
        }
    }
}