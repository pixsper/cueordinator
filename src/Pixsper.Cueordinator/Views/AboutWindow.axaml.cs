using Avalonia.Input;
using Avalonia.ReactiveUI;
using Pixsper.Cueordinator.ViewModels;

namespace Pixsper.Cueordinator.Views;

public partial class AboutWindow : ReactiveWindow<AboutWindowViewModel>
{
    public AboutWindow()
    {
        InitializeComponent();
    }
}