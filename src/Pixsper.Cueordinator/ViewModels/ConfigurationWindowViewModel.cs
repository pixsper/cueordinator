using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Pixsper.Cueordinator.Models;
using Pixsper.Cueordinator.Models.Disguise;
using Pixsper.Cueordinator.Models.Eos;
using Pixsper.Cueordinator.Services;
using Pixsper.Cueordinator.ViewModels.Connections;
using ReactiveUI;

namespace Pixsper.Cueordinator.ViewModels;

public class ConfigurationWindowViewModel : ReactiveObject, IActivatableViewModel
{
    private readonly IConfigurationService _configurationService;
    private readonly ObservableCollection<ConnectionViewModel> _connections = [];

    public ConfigurationWindowViewModel(IConfigurationService configurationService)
    {
        _configurationService = configurationService;

        Connections = new ReadOnlyObservableCollection<ConnectionViewModel>(_connections);

        CreateConnection = ReactiveCommand.Create<ConnectionKind>(createConnection);
        RemoveConnection = ReactiveCommand.Create<ConnectionViewModel>(removeConnection);

        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            _configurationService.Connections.Connect()
                .OnItemAdded(onConfigurationItemAdded)
                .OnItemRemoved(onConfigurationItemRemoved)
                .Subscribe()
                .DisposeWith(disposables);

            Disposable.Create(() =>
            {
                foreach (var vm in Connections)
                    vm.Dispose();
            }).DisposeWith(disposables);
        });
    }

    public ViewModelActivator Activator { get; }
    public ReadOnlyObservableCollection<ConnectionViewModel> Connections { get; }


    public ReactiveCommand<ConnectionKind, Unit> CreateConnection { get; }
    public ReactiveCommand<ConnectionViewModel, Unit> RemoveConnection { get; }

    private void onConfigurationItemAdded(IConnectionConfiguration configuration)
    {
        var configurationObservable = _configurationService.Connections.WatchValue(configuration.Id);

        ConnectionViewModel vm = configuration.Kind switch
        {
            ConnectionKind.Disguise => new DisguiseConnectionViewModel(configurationObservable.Cast<DisguiseConnectionConfiguration>()),
            ConnectionKind.Eos => new EosConnectionViewModel(configurationObservable.Cast<EosConnectionConfiguration>()),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        _connections.Add(vm);
    }

    private void onConfigurationItemRemoved(IConnectionConfiguration configuration)
    {
        var vm = _connections.FirstOrDefault(vm => vm.Id == configuration.Id);
        if (vm is not null)
        {
            _connections.Remove(vm);
            vm.Dispose();
        }
    }


    private void createConnection(ConnectionKind connectionKind)
    {
        IConnectionConfiguration configuration = connectionKind switch
        {
            ConnectionKind.Disguise => new DisguiseConnectionConfiguration(),
            ConnectionKind.Eos => new EosConnectionConfiguration(),
            _ => throw new ArgumentOutOfRangeException(nameof(connectionKind), connectionKind, null)
        };

        _configurationService.CreateOrUpdateConnection(configuration);
    }

    private void removeConnection(ConnectionViewModel connectionViewModel)
    {
        _configurationService.DeleteConnection(connectionViewModel.Id);
    }
}