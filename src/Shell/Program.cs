using System.Reflection;
using LibObjectFile.Elf;
using NetAF.Assets;
using NetAF.Assets.Characters;
using NetAF.Assets.Locations;
using NetAF.Logic;
using NetAF.Rendering.FrameBuilders;
using NetAF.Targets.Console;
using NetAF.Utilities;
using ObjectModel;
using ObjectModel.IO;
using Shell.Core;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Structs;
using Spectre.Console;
using Splat;
using Velopack;
using Velopack.Sources;

namespace BrineAndCoin;

public class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length == 1 && args[0] == "--version")
        {
            var gitVersionInformationType = Assembly.GetEntryAssembly()!.GetType("GitVersionInformation");
            var field = gitVersionInformationType!.GetField("MajorMinorPatch");

            Console.WriteLine(field!.GetValue(null));
            return;
        }

        VelopackApp
                   .Build()
#if WINDOWS
                       .OnAfterInstallFastCallback(v => new Shortcuts().CreateShortcutForThisExe(ShortcutLocation.Desktop))
#endif
                   .Run();

#if RELEASE
        await CheckForUpdatesAsync();
#endif

        new MainMenuPage().Display();
    }

    static async Task CheckForUpdatesAsync()
    {
        var mgr = new UpdateManager(new GithubSource("https://github.com/Portraits-in-Brick-and-Time/Brine-and-Coin", null, false));

        Console.WriteLine("Checking for updates...");
        var newVersion = await mgr.CheckForUpdatesAsync();
        if (newVersion == null)
        {
            Console.Clear();
            return;
        }

        await mgr.DownloadUpdatesAsync(newVersion);

        mgr.ApplyUpdatesAndRestart(newVersion);
    }
}