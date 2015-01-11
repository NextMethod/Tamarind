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
var buildDir = GetDirectories("./").FirstOrDefault();
var solutions = GetFiles("./*.sln");
var solutionDirs = solutions.Select(solution => solution.GetDirectory());

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

	Information("Cleaning {0}", buildDir);
	CleanDirectory(buildDir);
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
        Version = releaseNotes.Version,
        FileVersion = releaseNotes.Version,
        });
});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.Does(() =>
{
	// foreach (var solution in solutions)
	// {
	// 	Information("Building {0}", solution);
	// 	MSBuild(solution, settings =>
	// 		settings.WithTarget("Build")
	// 			.SetConfiguration(configuration)
	// 		);
	// }
});

Task("Default")
	.IsDependentOn("Build");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
