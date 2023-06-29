#Requires -Version 2.0
<#
    .SYNOPSIS
    Unlist packages from NuPkg provider.

    .DESCRIPTION
    Use this script if you want to unlist specific packages from your NuPkg provider.

    .LINK
	  Project home: https://github.com/marcoaoteixeira/PowerShell-Scripts/blob/master/Unlist_Packages.ps1

	.NOTES
	Author: Marco Antonio Orestes Teixeira
	Version: 1.0.0
	
	This script is designed to be called from PowerShell.
#>
[CmdletBinding()]
Param (
  [Parameter(Mandatory = $true, HelpMessage = "The NuPkg source URL.")]
  [string]$SourceUrl = $null,

  [Parameter(Mandatory = $true, HelpMessage = "The NuPkg source API key.")]
  [string]$ApiKey = $null,

  [Parameter(Mandatory = $true, HelpMessage = "The NuPkg version.")]
  [string]$Version = $null,

  [Parameter(Mandatory = $false, HelpMessage = "The folder where all C# projects reside.")]
  [string]$CSProjFolder = $null,

  [Parameter(Mandatory = $false, HelpMessage = "A list of packages to unlist.")]
  [string[]]$Packages = @(),

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
    Write-Host "An error occurred while running script $($THIS_SCRIPT_NAME):`n$errorMessage`n" -Foreground Red
    
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

# Display the time when this script run started.
$scriptStartTime = Get-Date
Write-Verbose "[$($THIS_SCRIPT_NAME)] Started at $($scriptStartTime.TimeOfDay.ToString())"

# Display the version of PowerShell being used to run the script, as this can help solve some problems that are hard to reproduce on other machines.
Write-Verbose "Using PowerShell Version: $($PSVersionTable.PSVersion.ToString())."

Try {
  $nupkgs = @()
  
  If (!(Test-StringIsNullOrWhitespace $CSProjFolder)) {
    Get-ChildItem -Recurse -Path $CSProjFolder -Filter *.csproj -Exclude *.UnitTests.csproj,*.Tests.csproj | ForEach-Object -Process {
      $nupkgs += $_.BaseName
    }
  }

  If ($Packages.Count -gt 0) {
    $nupkgs += $Packages
  }

  ForEach ($nupkg In $nupkgs) {
    Invoke-Expression "dotnet nuget delete $nupkg $Version -k $ApiKey -s $SourceUrl --non-interactive"
  }
} Finally {
    Write-Verbose "[$($THIS_SCRIPT_NAME)] Performing cleanup..."
}

# Display the time that this script finished running, and how long it took to run.
$scriptFinishTime = Get-Date
$scriptElapsedTimeInSeconds = ($scriptFinishTime - $scriptStartTime).TotalSeconds.ToString()
Write-Verbose "[$($THIS_SCRIPT_NAME)] Finished at $($scriptFinishTime.TimeOfDay.ToString())"
Write-Verbose "[$($THIS_SCRIPT_NAME)] Completed in $scriptElapsedTimeInSeconds seconds"