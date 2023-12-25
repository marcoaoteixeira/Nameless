#Requires -Version 2.0
<#
    .SYNOPSIS
    Executes code coverage tool

    .DESCRIPTION
    Executes code coverage tool

    .LINK
	Project home: https://github.com/marcoaoteixeira/nameless

	.NOTES
	Author:
	Version: 1.0.0
	
	This script is designed to be called from PowerShell.
#>
[CmdletBinding()]
Param (
    [Parameter(Mandatory = $false, HelpMessage = "Whether should show prompt for errors")]
    [Switch]$PromptOnError = $false
)

# Turn on Strict Mode to help catch syntax-related errors.
#   This must come after a script's/function's param section.
#   Forces a Function to be the first non-comment code to appear in a PowerShell Module.
Set-StrictMode -Version Latest

#==========================================================
# Define any necessary global variables, such as file paths.
#==========================================================

# Gets the script file name, without extension.
$THIS_SCRIPT_NAME = [System.IO.Path]::GetFileNameWithoutExtension($MyInvocation.MyCommand.Definition)

# Get the directory that this script is in.
$THIS_SCRIPT_DIRECTORY_PATH = Split-Path $script:MyInvocation.MyCommand.Path

#==========================================================
# Define functions used by the script.
#==========================================================

# Catch any exceptions Thrown, display the error message, wait for input if appropriate, and then stop the script.
Trap [Exception] {
    $errorMessage = $_
    Write-Host "An error occurred while executing the script:`n$errorMessage`n" -Foreground Red
    
    If ($PromptOnError) {
        Write-Host "Press any key to continue ..."
        $userInput = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyUp")
    }

    Break;
}

# PowerShell v2.0 compatible version of [String]::IsNullOrWhitespace.
Function Test-StringIsNullOrWhitespace([String]$string) {
    Return [String]::IsNullOrWhiteSpace($string)
}

#==========================================================
# Perform the script tasks.
#==========================================================

# Display the time that this script started running.
$scriptStartTime = Get-Date
Write-Verbose "[$($THIS_SCRIPT_NAME)] Starting at $($scriptStartTime.TimeOfDay.ToString())."

# Display the version of PowerShell being used to run the script, as this can help solve some problems that are hard to reproduce on other machines.
Write-Verbose "Using PowerShell Version: $($PSVersionTable.PSVersion.ToString())."

Try {
	$OutputDir = "$(pwd)/CodeCoverage"
	$TestResultsDir = "$OutputDir/TestResults"
	$CoverageReportDir = "$OutputDir/CoverageReport"
	$CoverageHistoryDir = "$OutputDir/CoverageHistory"
	
	# Delete previous test run results (there's a bunch of subfolders named with guids)
	If (Test-Path -Path $TestResultsDir) {
		Remove-Item -Recurse -Force $TestResultsDir
	}
	
	# Restore solution
	dotnet restore
	
	# Build solution
	dotnet build `
		--no-restore `
		--verbosity normal
	
	# Run tests and log results
	dotnet test `
		--no-restore `
		--no-build `
		--logger:"Html;LogFileName=$OutputDir/CodeCoverageLog.html" `
		--collect:"XPlat Code Coverage" `
		--results-directory $TestResultsDir `
		--verbosity normal
	
	# Install Coverlet Report Generator
	dotnet tool install -g dotnet-reportgenerator-globaltool
	
	# Delete previous test run reports - note if you're getting wrong results
	# do a Solution Clean and Rebuild to remove stale DLLs in the bin folder
	If (Test-Path -Path $CoverageReportDir) {
		Remove-Item -Recurse -Force $CoverageReportDir
	}
	
	# To keep a history of the Code Coverage we need to use the argument: -historydir:SOME_DIRECTORY 
	If (!(Test-Path -Path $CoverageHistoryDir)) {
		New-Item -ItemType Directory -Path $CoverageHistoryDir
	}
	
	# Generate the Code Coverage HTML Report
	reportgenerator `
		-reports:$OutputDir/**/coverage.cobertura.xml `
		-targetdir:$CoverageReportDir `
		-reporttypes:Html `
		-historydir:$CoverageHistoryDir
	
	# Open the Code Coverage HTML Report (if running on a WorkStation)
	$OS = Get-CimInstance -ClassName Win32_OperatingSystem
	If ($OS.ProductType -eq 1) {
		Invoke-Item "$CoverageReportDir/index.html"
	}
} Finally {
    Write-Verbose "[$($THIS_SCRIPT_NAME)] Performing cleanup..."
}

# Display the time that this script finished running, and how long it took to run.
$scriptFinishTime = Get-Date
$scriptElapsedTimeInSeconds = ($scriptFinishTime - $scriptStartTime).TotalSeconds.ToString()
Write-Verbose "[$($THIS_SCRIPT_NAME)] Finished at $($scriptFinishTime.TimeOfDay.ToString())."
Write-Verbose "Completed in $scriptElapsedTimeInSeconds seconds."