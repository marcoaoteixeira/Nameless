// Cake Build Automation System.
// Learn more: https://cakebuild.net

//////////////////////////////////////////////////////////////////////
// INITIALIZATION
//////////////////////////////////////////////////////////////////////

// Set the working directory to the current directory
if (HasArgument("working-dir")) {
    var argument = Argument<DirectoryPath>("working-dir");
    var workingDir = !System.IO.Path.IsPathRooted(argument.FullPath)
        ? Context.Environment.WorkingDirectory.Combine(argument)
        : argument;

    Information($"Set working path to: {workingDir}");
    Context.Environment.WorkingDirectory = workingDir;
}

//////////////////////////////////////////////////////////////////////
// GLOBAL ARGUMENTS
//////////////////////////////////////////////////////////////////////

var solutionPath = Argument("solution", string.Empty);
if (string.IsNullOrEmpty(solutionPath)) {
    Information("Solution was not provided. Searching for the first valid match: *.sln...");
    var solutionFile = Context
        .FileSystem.GetDirectory(Context.Environment.WorkingDirectory)
        .GetFiles("*.sln", SearchScope.Current)
        .FirstOrDefault();
    if (solutionFile is null) {
        Error("No solution file found. Please provide a valid solution path.");
        Environment.Exit(1);
    }

    Information($"Found solution file: {solutionFile.Path.FullPath}");
    solutionPath = solutionFile.Path.FullPath;
}

var configuration = Argument("configuration", "Release");
var verbosity = Argument("verbosity", DotNetVerbosity.Normal);
var codeCoverageDir = Argument<DirectoryPath>("code-coverage-dir", "code_coverage");
var nupkgOutputDir = Argument<DirectoryPath>("nupkg-output-dir", "nupkgs");

var releaseVersion = Argument("release-version", "1.0.0");
if (!SemVersion.TryParse(releaseVersion, out var _)) {
    Information("Release version is not a valid SemVersion. Using default version: 1.0.0");
    releaseVersion = "1.0.0";
}

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

// Cleans the solution
Task("Clean")
    .Does(() =>
    {
        DotNetClean(solutionPath, new DotNetCleanSettings
        {
            Configuration = configuration,
            NoLogo = true,
            Verbosity = verbosity
        });
    });

// Restores the solution
Task("Restore")
    .Does(() =>
    {
        DotNetRestore(solutionPath, new DotNetRestoreSettings
        {
            Verbosity = verbosity
        });
    });

// Executes the vanilla build process
Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetBuild(solutionPath, new DotNetBuildSettings
        {
            Configuration = configuration,
            MSBuildSettings = new DotNetMSBuildSettings
            {
                AssemblyVersion = releaseVersion,
                FileVersion = releaseVersion,
                Version = releaseVersion,
            },
            NoLogo = true,
            NoRestore = true,
            Verbosity = verbosity
        });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var filter = Argument("test-filter", string.Empty);
        //var logger = Argument("test-logger", $"Html;LogFileName={codeCoverageDir.Combine("code_coverage_log.html")}");
        var logger = Argument("test-logger", "console;verbosity=normal");
        var collector = Argument("test-collector", "XPlat Code Coverage");

        DotNetTest(solutionPath, new DotNetTestSettings
        {
            Configuration = configuration,
            NoLogo = true,
            NoBuild = true,
            NoRestore = true,
            Filter = filter,
            Collectors = [collector],
            Loggers = [logger],
            ResultsDirectory = codeCoverageDir.Combine("test_results"),
            Verbosity = verbosity
        });
    });

Task("CodeCoverage")
    .IsDependentOn("Test")
    .Does(() =>
    {
        // the '**' in the path is a wildcard that matches any number of directories
        FilePath coberturaFilePath = $"{codeCoverageDir}/**/coverage.cobertura.xml";
        DirectoryPath reportDirectoryPath = codeCoverageDir.Combine("coverage_report");

        ReportGenerator(coberturaFilePath, reportDirectoryPath, new ReportGeneratorSettings
        {
            ReportTypes = [ReportGeneratorReportType.Html],
            Verbosity = ReportGeneratorVerbosity.Verbose
        });

        if (!HasArgument("open-code-coverage")) { return; }

        FilePath reportFile = reportDirectoryPath.GetFilePath("index.html");
        if (!Context.FileSystem.Exist(reportFile)) {
            Information($"Code coverage report not found: {reportFile}.");

            return;
        }

        Information($"Opening code coverage report: {reportFile}.");
        switch (Context.Environment.Platform.Family)
        {
            case PlatformFamily.Windows:
                StartProcess("cmd.exe", $"/C start {reportFile}");
                break;
            case PlatformFamily.Linux:
                StartProcess("xdg-open", reportFile.FullPath);
                break;
            case PlatformFamily.OSX:
                StartProcess("open", reportFile.FullPath);
                break;
        }
    });

Task("NuGetPack")
    .IsDependentOn("Test")
    .Does(() =>
    {
        DotNetPack(solutionPath, new DotNetPackSettings
        {
            Configuration = configuration,
            IncludeSymbols = true,
            OutputDirectory = nupkgOutputDir,
            MSBuildSettings = new DotNetMSBuildSettings
            {
                PackageVersion = releaseVersion,
            },
            Verbosity = verbosity,
            NoLogo = true,
            NoBuild = true,
            NoRestore = true,
        });
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("CodeCoverage");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(Argument("target", "Default"));