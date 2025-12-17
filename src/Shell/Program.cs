using Velopack;
using Velopack.Sources;

namespace BrineAndCoin;

public class Program
{
    public static void Main(string[] args)
    {
        VelopackApp
                   .Build()
#if WINDOWS
                       .OnAfterInstallFastCallback(v => new Shortcuts().CreateShortcutForThisExe(ShortcutLocation.Desktop))
#endif
                   .Run();

#if RELEASE
                   CheckForUpdatesAsync();
#endif
    }

    private static async void CheckForUpdatesAsync()
    {
        var mgr = new UpdateManager(new GithubSource("https://github.com/Portraits-in-Brick-and-Time/Brine-and-Coin", null, false));

        var newVersion = await mgr.CheckForUpdatesAsync();
        if (newVersion == null)
            return;

        //ToDo: add ui for downloading updates
        await mgr.DownloadUpdatesAsync(newVersion);

        mgr.ApplyUpdatesAndRestart(newVersion);
    }
}