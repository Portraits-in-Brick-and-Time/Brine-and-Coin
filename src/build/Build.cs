using System;
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
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
    "deploy",
    GitHubActionsImage.WindowsLatest,
    OnPushBranches = new[] { "release" },
    AutoGenerate = true,
    PublishArtifacts = false,
    EnableGitHubToken = true,
    InvokedTargets = new[] { nameof(DeployToGitHubReleases) })]
class BuildFile : NukeBuild
{
    public static int Main() => Execute<BuildFile>(x => x.DeployToGitHubReleases);

    [Parameter("GitHub Token")] readonly string GitHubToken;

    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    [GitVersion]
    GitVersion Version;

    AbsolutePath PublishWinDir => RootDirectory / "publish-win";
    AbsolutePath PublishLinuxDir => RootDirectory / "publish-linux";

    const string UniqueIdentifier = "Brine_and_Coin";
    const string ExeName = "Portraits_in_Brick_and_Time_-_Brine_And_Coin";

    [Parameter]
    readonly Configuration Configuration = IsServerBuild ? Configuration.Release : Configuration.Debug;

    Target Clean => _ => _
        .Executes(() =>
        {
            PublishWinDir.DeleteDirectory();
            PublishLinuxDir.DeleteDirectory();
        });

    Target InstallVelopack => _ => _
        .Executes(() =>
        {
            DotNet("tool install -g vpk");
        });

    Target BuildAssetCompiler => _ => _
        .DependsOn(Clean)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            try
            {
                var filename = Solution.AssetCompiler.Path;
                DotNetBuild(s => s
                    .SetProjectFile(filename)
                    .SetConfiguration(Configuration));
            } catch (Exception ex)
            {
                Log.Error(ex, "");
            }
        });

    Target Build => _ => _
        .DependsOn(Clean, BuildAssetCompiler)
        .Executes(() =>
        {
            var filename = Solution.Shell.BrineAndCoin.Path;
            DotNetBuild(s => s
                .SetProjectFile(filename)
                .SetConfiguration(Configuration));
        });

    Target PublishWindows => _ => _
        .DependsOn(Build)
        //.OnlyWhenStatic(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            var filename = Solution.Shell.BrineAndCoin.Path;
            DotNetPublish(s => s
                .SetProject(filename)
                .SetConfiguration(Configuration)
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
                .SetConfiguration(Configuration)
                .SetRuntime("linux-x64")
                .SetSelfContained(true)
                .SetOutput(PublishLinuxDir));
        });

    Target VelopackDownloadOldRelease => _ => _
        .DependsOn(PublishLinux, PublishWindows, InstallVelopack)
        //.OnlyWhenStatic(() => Configuration == Configuration.Release)
        .Executes(() =>
        {
            // Windows
            ProcessTasks.StartProcess("vpk",
                $"download github --repoUrl https://github.com/{GitHubActions.Instance.Repository} --token {GitHubToken}",
                logOutput: true).AssertZeroExitCode();

            // Linux
            ProcessTasks.StartProcess("vpk",
                $"[linux] download github --repoUrl https://github.com/{GitHubActions.Instance.Repository} --token {GitHubToken}",
                logOutput: true).AssertZeroExitCode();
        });

    Target VelopackPackWindows => _ => _
        .DependsOn(VelopackDownloadOldRelease)
        .Executes(() =>
        {
            ProcessTasks.StartProcess("vpk",
                $"pack -u \"{UniqueIdentifier}\" -v {Version.LegacySemVer} -p {PublishWinDir} --mainExe {ExeName}.exe --packTitle \"Brine and Coin\"",
                logOutput: true).AssertZeroExitCode();
        });

    Target VelopackPackLinux => _ => _
        .DependsOn(VelopackDownloadOldRelease)
        .Executes(() =>
        {
            ProcessTasks.StartProcess("vpk",
                $"[linux] pack -u \"{UniqueIdentifier}\" -v {Version} -p {PublishLinuxDir} --mainExe {ExeName} --packTitle \"Brine and Coin\"",
                logOutput: true).AssertZeroExitCode();
        });

    Target DeployToGitHubReleases => _ => _
        .DependsOn(VelopackPackWindows, VelopackPackLinux)
        .Executes(() =>
        {
            // Windows channel
            ProcessTasks.StartProcess("vpk",
                $"upload github --channel win --repoUrl https://github.com/{GitHubActions.Instance.Repository} " +
                $"--releaseName \"{Version}\" --tag v{Version} --publish --token {GitHubToken}",
                logOutput: true).AssertZeroExitCode();

            // Linux channel
            ProcessTasks.StartProcess("vpk",
                $"[linux] upload github --merge --channel linux --repoUrl https://github.com/{GitHubActions.Instance.Repository} " +
                $"--releaseName \"{Version}\" --tag v{Version} --publish --token {GitHubToken}",
                logOutput: true).AssertZeroExitCode();
        });

}

