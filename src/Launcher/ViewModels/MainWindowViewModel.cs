using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Velopack;
using Velopack.Sources;

namespace BrineAndCoin.Launcher.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private int _progressValue;

    [ObservableProperty]
    private string _changelog = string.Empty;

    [RelayCommand]
    async Task CheckForUpdatesAsync()
    {
#if DEBUG
        return;
#endif

        var mgr = new UpdateManager(new GithubSource("https://github.com/Portraits-in-Brick-and-Time/Brine-and-Coin", null, false));

        var newVersion = await mgr.CheckForUpdatesAsync();
        if (newVersion == null)
        {
            goto start;
        }

        Changelog = newVersion.TargetFullRelease.NotesMarkdown;
                await mgr.DownloadUpdatesAsync(newVersion, progress =>
        {
            ProgressValue = progress;
        });

        mgr.ApplyUpdatesAndRestart(newVersion);

start:
        Process.Start("Portraits_in_Brick_and_Time_-_Brine_And_Coin-Game.exe");
        Environment.Exit(0);
    }
}