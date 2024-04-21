using System;
using Pixsper.Cueordinator.Models;
using Pixsper.Cueordinator.Models.Eos;

namespace Pixsper.Cueordinator.ViewModels.Connections;

public class EosConnectionViewModel : ConnectionViewModel
{
    public EosConnectionViewModel(IObservable<EosConnectionConfiguration> configuration)
    {
    }

    public override void Dispose()
    {

    }

    public override ConnectionKind Kind => ConnectionKind.Eos;
}