///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target          = Argument<string>("target", "Default");
var configuration   = Argument<string>("configuration", "Debug");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var projectName = "Tamarind";

// Files
var solutionInfoCs = "SolutionInfo.cs";

// Directories
var packagingRoot = GetDirectories("./packaging/").First();
var solutions = GetFiles("./*.sln");
var solutionDirs = solutions.Select(solution => solution.GetDirectory());
var testResultsDir = GetDirectories("./testresults/").First();

var tamarindPackagingDir = packagingRoot.Combine("tamarind");

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
	foreach (var dir in solutionDirs)
	{
		Information("Cleaning {0}", dir);
		CleanDirectories(dir + "/packages");
		CleanDirectories(dir + "/**/bin/" + configuration);
		CleanDirectories(dir + "/**/obj/" + configuration);
	}

    foreach (var dir in new [] { packagingRoot, testResultsDir })
    {
        Information("Cleaning {0}", dir);
        CleanDirectories(dir.FullPath);
    }
});

Task("Restore")
	.Does(() =>
{
	foreach (var solution in solutions)
	{
		Information("Restoring {0}", solution);
		NuGetRestore(solution);
	}
});

Task("AssemblyInfo")
    .Does(() =>
{
    var releaseNotes = ParseReleaseNotes("ReleaseNotes.md");
    Information("Creating {0} - Version: {1}", solutionInfoCs, releaseNotes.Version);
    CreateAssemblyInfo(solutionInfoCs, new AssemblyInfoSettings {
        Product = projectName,
        Version = releaseNotes.Version.ToString(),
        FileVersion = releaseNotes.Version.ToString(),
        });
});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
    .IsDependentOn("AssemblyInfo")
	.Does(() =>
{
	foreach (var solution in solutions)
	{
		Information("Building {0}", solution);
		MSBuild(solution, settings =>
			settings.WithTarget("Build")
				.SetConfiguration(configuration)
			);
	}
});

Task("UnitTests")
    .IsDependentOn("Build")
    .Does(() =>
{
    // Clean Solution directories
    foreach (var dir in solutionDirs)
    {
        Information("Running Tests in {0}", dir);
        XUnit2(
            dir + "/**/bin/" + configuration + "/**/*.Tests*.dll",
            new XUnit2Settings {
                OutputDirectory = testResultsDir,
                HtmlReport = true
            }
        );
    }
});

Task("CreateTamarindPackage")
    .Does(() =>
{
    var net45Dir = tamarindPackagingDir.Combine("lib/net45/");
    var netcore45Dir = tamarindPackagingDir.Combine("lib/netcore45/");
    var portableDir = tamarindPackagingDir.Combine("lib/portable-net45+wp80+win+wpa81/");

    CleanDirectories(new [] { net45Dir, netcore45Dir, portableDir });

    // CopyFiles();

});

Task("Default")
	.IsDependentOn("Build")
    .IsDependentOn("UnitTests");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
