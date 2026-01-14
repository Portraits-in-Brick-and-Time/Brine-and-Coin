using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using NJsonSchema.Annotations;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
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
    AutoGenerate = true,
    PublishArtifacts = false,
    EnableGitHubToken = true,
    InvokedTargets = new[] { nameof(Deploy) })]
class BuildFile : NukeBuild, IHazGitVersion, IHazConfiguration
{
    public static int Main() => Execute<BuildFile>(x => x.Deploy);

    [Parameter("GitHub Token")] readonly string GitHubToken;
    [Parameter("AssetsFilename")] readonly string AssetsFilename = "Assets/core_assets.elf";

    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    AbsolutePath ReleaseDir => RootDirectory / "release";
    AbsolutePath PublishWinDir => RootDirectory / "publish-win";
    AbsolutePath PublishLinuxDir => RootDirectory / "publish-linux";

    const string UniqueIdentifier = "Brine_and_Coin";
    const string ExeName = "Portraits_in_Brick_and_Time_-_Brine_And_Coin";

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
            if (!Directory.Exists("Assets"))
            {
                Directory.CreateDirectory("Assets");
            }

            if (!File.Exists(AssetsFilename))
            {
                File.Create(AssetsFilename).Close();
            }

            using var elf = File.Create(AssetsFilename);
            var objectWriter = new GameAssetWriter(elf);
            var sources = Solution.Shell.BrineAndCoin.GetItems("AssetSources");
            foreach (var source in sources)
            {
                objectWriter.WriteObjects(Solution.Shell.BrineAndCoin.Directory / source);
            }
            objectWriter.Close();
            //ToDo: copy core_assets.elf to output dir
            Log.Information($"Compiled assets to {AssetsFilename}");
        });

    Target Build => _ => _
        .DependsOn(Clean, BuildAssets)
        .Executes(() =>
        {
            var filename = Solution.Shell.BrineAndCoin.Path;
            DotNetBuild(s => s
                .SetProjectFile(filename)
                .SetConfiguration(((IHazConfiguration)this).Configuration));
        });

    Target PublishWindows => _ => _
        .DependsOn(Build)
        //.OnlyWhenStatic(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            var filename = Solution.Shell.BrineAndCoin.Path;
            DotNetPublish(s => s
                .SetProject(filename)
                .SetConfiguration(((IHazConfiguration)this).Configuration)
                .SetRuntime("win-x64")
                .SetSelfContained(true)
                .SetOutput(PublishWinDir));
        });

    Target PublishLinux => _ => _
        .DependsOn(Build)
        .ProceedAfterFailure()
        //.OnlyWhenStatic(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            var filename = Solution.Shell.BrineAndCoin.Path;
            DotNetPublish(s => s
                .SetProject(filename)
                .SetConfiguration(((IHazConfiguration)this).Configuration)
                .SetRuntime("linux-x64")
                .SetSelfContained(true)
                .SetOutput(PublishLinuxDir));
        });

    Target DownloadOldRelease => _ => _
        .DependsOn(PublishLinux, PublishWindows)
        //.OnlyWhenStatic(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            string repository = GitHubActions.Instance?.Repository ?? "Portraits-in-Brick-and-Time/Brine-and-Coin";
            VelopackTasks.VelopackDownloadGithub(_ => _
                .SetRepoUrl($"https://github.com/{repository}")
                .SetToken(GitHubToken)
                .SetOutputDir(ReleaseDir)
                .CombineWith(["windows", "linux"], (settings, channel) =>
                {
                    settings.SetChannel(channel);
                    return settings;
                }), 2, true
            );
        });

    Target VelopackPackWindows => _ => _
        .DependsOn(DownloadOldRelease)
        .Executes(() =>
        {
            ProcessTasks.StartProcess("vpk",
                $"pack -u \"{UniqueIdentifier}\" -v {((IHazGitVersion)this).Versioning.LegacySemVer} -p {PublishWinDir} --mainExe {ExeName}.exe --packTitle \"Brine and Coin\"",
                logOutput: true).AssertZeroExitCode();
        });

    Target VelopackPackLinux => _ => _
        .DependsOn(DownloadOldRelease)
        .Executes(() =>
        {
            ProcessTasks.StartProcess("vpk",
                $"[linux] pack -u \"{UniqueIdentifier}\" -v {((IHazGitVersion)this).Versioning.LegacySemVer} -p {PublishLinuxDir} --mainExe {ExeName} --packTitle \"Brine and Coin\"",
                logOutput: true).AssertZeroExitCode();
        });

    Target Deploy => _ => _
        .DependsOn(VelopackPackWindows, VelopackPackLinux)
        .Executes(() =>
        {
            // Windows channel
            string version = ((IHazGitVersion)this).Versioning.LegacySemVer;
            ProcessTasks.StartProcess("vpk",
                $"upload github --channel win --repoUrl https://github.com/{GitHubActions.Instance.Repository} " +
                $"--releaseName \"{version}\" --tag v{version} --publish --token {GitHubToken}",
                logOutput: true).AssertZeroExitCode();

            // Linux channel
            ProcessTasks.StartProcess("vpk",
                $"[linux] upload github --merge --channel linux --repoUrl https://github.com/{GitHubActions.Instance.Repository} " +
                $"--releaseName \"{version}\" --tag v{version} --publish --token {GitHubToken}",
                logOutput: true).AssertZeroExitCode();
        });

}

