using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using ReactiveUI;

namespace Pixsper.Cueordinator.ViewModels;

public class AboutWindowViewModel : ReactiveObject, IActivatableViewModel
{
    public AboutWindowViewModel()
    {
        Activator = new ViewModelActivator();
        OpenUrl = ReactiveCommand.Create<string>(openUrl);

        this.WhenActivated((CompositeDisposable disposables) =>
        {

        });
    }

    public ViewModelActivator Activator { get; }

    public string AppVersion => App.Version;

    public ReactiveCommand<string, Unit> OpenUrl { get; }


    private void openUrl(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo("cmd",
                        $"/c start {uri.ToString().Replace("&", "^&")}")
                    {
                        CreateNoWindow = true
                    });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", uri.ToString());
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", uri.ToString());
                }
            }
            catch (Exception)
            {
                Debug.Assert(false);
            }
        }
    }
}