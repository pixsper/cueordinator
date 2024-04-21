using System;
using Pixsper.Cueordinator.Models;
using Pixsper.Cueordinator.Models.Disguise;

namespace Pixsper.Cueordinator.ViewModels.Connections;

public class DisguiseConnectionViewModel : ConnectionViewModel
{
    public DisguiseConnectionViewModel(IObservable<DisguiseConnectionConfiguration> configuration)
    {
    }

    public override void Dispose()
    {

    }

    public override ConnectionKind Kind => ConnectionKind.Disguise;
}