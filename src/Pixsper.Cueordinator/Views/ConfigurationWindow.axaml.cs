using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Pixsper.Cueordinator.ViewModels;

namespace Pixsper.Cueordinator.Views;

public partial class ConfigurationWindow : ReactiveWindow<ConfigurationWindowViewModel>
{
    public ConfigurationWindow()
    {
        InitializeComponent();
    }
}