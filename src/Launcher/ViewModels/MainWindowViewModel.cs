using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Octokit;
using Velopack;
using Velopack.Sources;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace BrineAndCoin.Launcher.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private int _progressValue;

    [ObservableProperty]
    private string _changelog = string.Empty;

    public MainWindowViewModel()
    {
        SetChangelog().ConfigureAwait(false);
    }

    private async Task SetChangelog()
    {
        var client = new GitHubClient(new ProductHeaderValue("Brine-and-Coin-Launcher"));

        var repoOwner = "Portraits-in-Brick-and-Time";
        var repoName = "Brine-and-Coin";

        var sb = new StringBuilder();
        foreach (var release in await client.Repository.Release.GetAll(repoOwner, repoName))
        {
            if (release.Prerelease || release.Draft)
            {
                continue;
            }

            sb.AppendLine($"### {release.Name}");
            sb.AppendLine(release.Body);
            sb.AppendLine();
        }
        Changelog = sb.ToString();
    }

    [RelayCommand]
    async Task CheckForUpdatesAsync()
    {
#if DEBUG
        goto start;
#endif

        var mgr = new UpdateManager(new GithubSource("https://github.com/Portraits-in-Brick-and-Time/Brine-and-Coin", null, false));

        var newVersion = await mgr.CheckForUpdatesAsync();
        if (newVersion == null)
        {
            goto start;
        }

        await mgr.DownloadUpdatesAsync(newVersion, progress =>
        {
            ProgressValue = progress;
        });

        mgr.ApplyUpdatesAndRestart(newVersion);

start:
      StartGame();
      Environment.Exit(0);
    }

    private void StartGame()
    {
        const string assemblyName = "Portraits_in_Brick_and_Time_-_Brine_And_Coin-Game";

        var fileToRun = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? $"{assemblyName}.exe"
            : assemblyName;

        var psi = new ProcessStartInfo
        {
            FileName = fileToRun,
            UseShellExecute = false
        };

        Process.Start(psi);
    }
}