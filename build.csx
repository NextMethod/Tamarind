#l "build/utilities.csx"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target          = Argument<string>("target", "Default");
var configuration   = Argument<string>("configuration", "Debug");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var projectName = "Tamarind";
var projectDescription = "C# port of Google's Guava library.";
var projectAuthors = new [] { "Tamarind Contributors" };
var projectOwners = new [] { "Jordan S. Jones", "Esteban Araya" };

// Directories
// WorkingDirectory is relative to this file. Make it relative to the Solution file.
var baseDir = GetContext().Environment.WorkingDirectory;
var packagingRoot = baseDir.Combine("Packaging");
var testResultsDir = baseDir.Combine("TestResults");
var tamarindPackagingDir = packagingRoot.Combine(projectName);

// Files
var solutionInfoCs = baseDir.Combine("build").GetFilePath("SolutionInfo.cs");
var nuspecFile = baseDir.Combine("build").GetFilePath(projectName + ".nuspec");
var solution = baseDir.GetFilePath(projectName + ".sln");
var solutionDir = solution.GetDirectory();

// Get whether or not this is a local build.
var local = IsLocalBuild();
var isPullRequest = IsPullRequest();

// Release notes
var releaseNotes = ParseReleaseNotes(baseDir.GetFilePath("ReleaseNotes.md"));

// Version
var buildNumber = GetBuildNumber();
var version = releaseNotes.Version.ToString();
var semVersion = local ? version : (version + string.Concat("-build-", buildNumber));

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(() =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");
});

Teardown(() =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
	.Does(() =>
{
	// Clean Solution directories
	Information("Cleaning {0}", solutionDir);
	CleanDirectories(solutionDir + "/packages");
	CleanDirectories(solutionDir + "/**/bin/" + configuration);
	CleanDirectories(solutionDir + "/**/obj/" + configuration);

    foreach (var dir in new [] { packagingRoot, testResultsDir })
    {
         Information("Cleaning {0}", dir);
         CleanDirectories(dir.FullPath);
    }
});

Task("Restore")
    .IsDependentOn("Clean")
	.Does(() =>
{
	Information("Restoring {0}", solution);
	NuGetRestore(solution);
});

Task("AssemblyInfo")
    .IsDependentOn("Restore")
    .WithCriteria(() => !local)
    .Does(() =>
{
    Information("Creating {0} - Version: {1}", solutionInfoCs, version);
    CreateAssemblyInfo(solutionInfoCs, new AssemblyInfoSettings {
        Product = projectName,
        Version = version,
        FileVersion = version,
        InformationalVersion = semVersion
    });
});

Task("Build")
	.IsDependentOn("AssemblyInfo")
	.Does(() =>
{
	Information("Building {0}", solution);
	MSBuild(solution, settings =>
        settings.SetConfiguration(configuration)
	);
});

Task("UnitTests")
    .IsDependentOn("Build")
    .Does(() =>
{
    Information("Running Tests in {0}", solution);
    XUnit2(
        solutionDir + "/**/bin/" + configuration + "/**/*.Tests*.dll",
        new XUnit2Settings {
            OutputDirectory = testResultsDir,
            HtmlReport = true
        }
    );
});

Task("CopyTamarindPackageFiles")
    .IsDependentOn("UnitTests")
    .Does(() =>
{
    var baseBuildDir = solutionDir.Combine(projectName).Combine("bin").Combine(configuration);

    var net45BuildDir = baseBuildDir.Combine("Net45");
    var net45PackageDir = tamarindPackagingDir.Combine("lib/net45/");

    var netcore45BuildDir = baseBuildDir.Combine("NetCore45");
    var netcore45PackageDir = tamarindPackagingDir.Combine("lib/netcore45/");

    var portableBuildDir = baseBuildDir.Combine("Portable-net45+win+wpa81+wp80");
    var portablePackageDir = tamarindPackagingDir.Combine("lib/portable-net45+wp80+win+wpa81/");

    var dirMap = new Dictionary<DirectoryPath, DirectoryPath> {
        { net45BuildDir, net45PackageDir },
        { netcore45BuildDir, netcore45PackageDir },
        { portableBuildDir, portablePackageDir }
    };

    CleanDirectories(dirMap.Values);

    foreach (var dirPair in dirMap)
    {
        var files = new DirectoryInfo(dirPair.Key.FullPath)
            .EnumerateFiles()
            .Select(x => new FilePath(x.FullName));
        CopyFiles(files, dirPair.Value);
    }

    var packageFiles = new FilePath[] {
        solutionDir.CombineWithFilePath("LICENSE.txt"),
        solutionDir.CombineWithFilePath("README.md"),
        solutionDir.CombineWithFilePath("ReleaseNotes.md")
    };

    CopyFiles(packageFiles, tamarindPackagingDir);
});

Task("CreateTamarindPackage")
    .IsDependentOn("CopyTamarindPackageFiles")
    .Does(() =>
{
    NuGetPack(
        nuspecFile,
        new NuGetPackSettings {
            Id = projectName,
            Title = projectName,
            Authors = projectAuthors.ToList(),
            Owners = projectOwners.ToList(),
            Summary = projectDescription,
            Description = projectDescription,
            Version = semVersion,
            ReleaseNotes = releaseNotes.Notes.ToArray(),
            BasePath = tamarindPackagingDir,
            OutputDirectory = packagingRoot,
            Symbols = false,
            NoPackageAnalysis = false
        }
    );
});

///////////////////////////////////////////////////////////////////////////////
// TASK TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Package")
    .IsDependentOn("CreateTamarindPackage");

Task("Default")
    .IsDependentOn("Package");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

Information("Building version {0} of {1} ({2}).", version, solution.GetFilename(), semVersion);

RunTarget(target);
