using System.Reactive.Disposables;
using ReactiveUI;

namespace Pixsper.Cueordinator.ViewModels;

public class AboutWindowViewModel : ReactiveObject, IActivatableViewModel
{
    public AboutWindowViewModel()
    {
        Activator = new ViewModelActivator();

        this.WhenActivated((CompositeDisposable disposables) =>
        {

        });
    }

    public ViewModelActivator Activator { get; }

    public string AppVersion => App.Version;
}