param([string[]]$TaskList = @(), [hashtable]$Properties = @{})

if ($TaskList.Count -eq 0){
	$TaskList = @('Build-Packages', 'Deploy-Packages')
}

if ($Properties.Count -eq 0){
 	$Properties.EnvironmentName = 'Test.20'
	$Properties.EntryPoints = @(
		#'Web.OData'
		#'Replication.EntryPoint'
	)
	#$Properties.UseCaseRoute = 'ERM'
	#$Properties.UpdateSchemas = 'CustomerIntelligence'
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------
cls

$Properties.SemanticVersion = '0.1.5'
$Properties.SolutionDir = Join-Path $PSScriptRoot '..'
$Properties.BuildFile = Join-Path $PSScriptRoot 'default.ps1'

# Restore-Packages
& {
	$NugetPath = Join-Path $Properties.SolutionDir '.nuget\NuGet.exe'
	if (!(Test-Path $NugetPath)){
		$webClient = New-Object System.Net.WebClient
		$webClient.UseDefaultCredentials = $true
		$webClient.Proxy.Credentials = $webClient.Credentials
		$webClient.DownloadFile('https://www.nuget.org/nuget.exe', $NugetPath)
	}
	$solution = Get-ChildItem $Properties.SolutionDir -Filter '*.sln'
	& $NugetPath @('restore', $solution.FullName, '-NonInteractive', '-Verbosity', 'quiet')
}

Import-Module "$($Properties.SolutionDir)\packages\2GIS.NuClear.BuildTools.0.2.24\tools\buildtools.psm1" -DisableNameChecking -Force
Import-Module "$PSScriptRoot\metadata.psm1" -DisableNameChecking -Force

Add-Metadata (Parse-EnvironmentMetadata $Properties)
Run-Build $TaskList $Properties
