using System.Collections.Generic;
using System.IO;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities;
using Nuke.Components;
using ObjectModel.IO;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
    "deploy",
    GitHubActionsImage.WindowsLatest,
    OnPushBranches = new[] { "release" },
    AutoGenerate = false,
    FetchDepth = 0,
    PublishArtifacts = true,
    EnableGitHubToken = true,
    InvokedTargets = new[] { nameof(Deploy) })]
class BuildFile : NukeBuild, IHazGitVersion, IHazConfiguration
{
    public static int Main() => Execute<BuildFile>(x => x.Build);

    [Parameter("GitHub Token")] readonly string GitHubToken;
    [Parameter("AssetsFilename")] readonly string AssetsFilename = "core_assets.elf";

    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    AbsolutePath ReleaseDir => RootDirectory / "release";
    AbsolutePath PublishWinDir => RootDirectory / "publish-win";
    AbsolutePath PublishLinuxDir => RootDirectory / "publish-linux";

    const string UniqueIdentifier = "Brine_and_Coin";
    const string ExeName = "Portraits_in_Brick_and_Time_-_Brine_And_Coin";

    [NuGetPackage("vpk", "vpk.dll")]
    Tool Velopack;

    Target Clean => _ => _
        .Executes(() =>
        {
            PublishWinDir.DeleteDirectory();
            PublishLinuxDir.DeleteDirectory();

            Log.Information("Clean completed");
        });

    Target BuildAssets => _ => _
        .DependsOn(Clean)
        .Before(Build)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            AbsolutePath assetsDir = Solution.BrineAndCoin.Directory / "Assets";
            if (!Directory.Exists(assetsDir))
            {
                Directory.CreateDirectory(assetsDir);
            }

            AbsolutePath assetPath = assetsDir / AssetsFilename;
            if (!File.Exists(assetPath))
            {
                File.Create(assetPath).Close();
            }

            using var elf = File.Create(assetPath);
            var objectWriter = new GameAssetWriter(elf);
            var sources = Solution.BrineAndCoin.GetItems("AssetSources");
            foreach (var source in sources)
            {
                objectWriter.WriteObjects(Solution.BrineAndCoin.Directory / source);
            }
            objectWriter.Close();
            Log.Information($"Compiled assets to {AssetsFilename}");
        });

    Target Build => _ => _
        .DependsOn(Clean, BuildAssets)
        .Executes(() =>
        {
            var filename = Solution.Launcher.Path;
            DotNetBuild(s => s
                .SetProjectFile(filename)
                .SetConfiguration(((IHazConfiguration)this).Configuration));
        });

    Target Publish => _ => _
        .DependsOn(Build)
        .OnlyWhenStatic(() => IsServerBuild)
        .Executes(() =>
        {
            List<(string rid, string publishDir)> info = [
                ("win-x64", PublishWinDir),
                ("linux-x64", PublishLinuxDir)
            ];

            var filename = Solution.Launcher.Path;
            DotNetPublish(s => s
                .SetProject(filename)
                .SetConfiguration(((IHazConfiguration)this).Configuration)
                .SetSelfContained(false)
                .CombineWith(info, (settings, i) =>
                {
                    return settings
                        .SetRuntime(i.rid)
                        .SetOutput(i.publishDir);
                }));
        });

    string repository = GitHubActions.Instance?.Repository ?? "Portraits-in-Brick-and-Time/Brine-and-Coin";

    Target DownloadOldRelease => _ => _
        .DependsOn(Publish)
        .OnlyWhenStatic(() => IsServerBuild)
        .Executes(() =>
        {
            var channels = new List<string> { "win", "linux" };
            foreach (var channel in channels)
            {
                Velopack($"[{channel}] download github --repoUrl https://github.com/{repository} --token {GitHubToken} -o {ReleaseDir}");
            }
        });

    Target VelopackPack => _ => _
        .DependsOn(DownloadOldRelease)
        .OnlyWhenStatic(() => IsServerBuild)
        .Produces($"{PublishWinDir / ExeName}.exe", $"{PublishLinuxDir / ExeName}.appimage")
        .Executes(() =>
        {
            List<(string channel, string publishDir, string exeName)> info = [
                ("win", PublishWinDir, $"{ExeName}.exe"),
                ("linux", PublishLinuxDir, ExeName)
            ];

            foreach (var i in info)
            {
                string legacySemVer = ((IHazGitVersion)this).Versioning.MajorMinorPatch;
                var mainExePath = (AbsolutePath)i.publishDir / i.exeName;

                Velopack(
                    $"[{i.channel}] pack " +
                    $"--packVersion {legacySemVer} " +
                    $"-u {UniqueIdentifier} " +
                    $"--packTitle \"Brine and Coin\" " +
                    $"-p {i.publishDir} " +
                    $"--mainExe {mainExePath}");
            }
        });

    Target Deploy => _ => _
        .DependsOn(VelopackPack)
        .OnlyWhenStatic(() => IsServerBuild)
        .Executes(() =>
        {
            string version = ((IHazGitVersion)this).Versioning.MajorMinorPatch;

            var channels = new List<string> { "win", "linux" };
            foreach (var channel in channels)
            {
                Velopack($"upload github --channel {channel} --repoUrl https://github.com/{repository} --token {GitHubToken} --releaseName \"{version}\" --tag v{version} --publish --merge");
            }
        });
}

