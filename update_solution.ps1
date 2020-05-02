#Requires -Version 2.0
<#
    .SYNOPSIS
    Update SLN files

    .DESCRIPTION
    Update SLN files

    .LINK
	Project home: https://www.me.com.br/development/index.html

	.NOTES
	Author: Mercado Eletrônico Development Team
	Version: 1.0.0
	
	This script is designed to be called from PowerShell.
#>
[CmdletBinding()]
Param (
    [Parameter(Mandatory = $false, HelpMessage = "Whether should show prompt for errors")]
    [Switch]$PromptOnError = $false,
	
    [Alias("s")]
	[Parameter(Mandatory = $true, ParameterSetName = "Default", HelpMessage = "The solution file path.")]
    [String]$SolutionFilePath = $null,

	[Alias("pfp")]
	[Parameter(Mandatory = $true, ParameterSetName = "Default", HelpMessage = "The C# projects folder path.")]
    [String]$ProjectsFolderPath = $null
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
    Write-Host "An error occurred while running $($THIS_SCRIPT_NAME) script:`n$errorMessage`n" -Foreground Red
    
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

# Display the version of PowerShell being used to run the script, as this can help solve some problems that are hard to reproduce on other machines.
Write-Verbose "Using PowerShell Version: $($PSVersionTable.PSVersion.ToString())."

# Display the time that this script started running.
$scriptStartTime = Get-Date
Write-Verbose "$($THIS_SCRIPT_NAME) script started running at $($scriptStartTime.TimeOfDay.ToString())."

Try {

    # Execute the command.
    Get-ChildItem $ProjectsFolderPath -Recurse *.csproj | ForEach-Object { dotnet sln $SolutionFilePath add $_.FullName }

} Finally {
    Write-Verbose "Performing any required $($THIS_SCRIPT_NAME) script cleanup..."
}

# Display the time that this script finished running, and how long it took to run.
$scriptFinishTime = Get-Date
$scriptElapsedTimeInSeconds = ($scriptFinishTime - $scriptStartTime).TotalSeconds.ToString()
Write-Verbose "$($THIS_SCRIPT_NAME) script finished running at $($scriptFinishTime.TimeOfDay.ToString()). Completed in $scriptElapsedTimeInSeconds seconds."