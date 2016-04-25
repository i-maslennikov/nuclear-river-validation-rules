param([string[]]$TaskList = @(), [hashtable]$Properties = @{})
#Requires –Version 3.0

if ($TaskList.Count -eq 0){
	$TaskList = @('Build-Packages')
}

if ($Properties.Count -eq 0){
 	$Properties.EnvironmentName = 'Test.20'
	$Properties.EntryPoints = @(
		'CustomerIntelligence.Querying.Host'
		'CustomerIntelligence.Replication.Host'
	)
	#$Properties.UseCaseRoute = 'ERM'
	#$Properties.UpdateSchemas = 'CustomerIntelligence'
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------
cls

$Properties.SolutionDir = Join-Path $PSScriptRoot '..'
$Properties.BuildFile = Join-Path $PSScriptRoot 'default.ps1'

# Restore-Packages
& {
	$NugetPath = Join-Path $Properties.SolutionDir '.nuget\NuGet_v3.3.0.exe'
	if (!(Test-Path $NugetPath)){
		$webClient = New-Object System.Net.WebClient
		$webClient.UseDefaultCredentials = $true
		$webClient.Proxy.Credentials = $webClient.Credentials
		$webClient.DownloadFile('https://dist.nuget.org/win-x86-commandline/v3.3.0/nuget.exe', $NugetPath)
	}
	$solution = Get-ChildItem $Properties.SolutionDir -Filter '*.sln'
	& $NugetPath @('restore', $solution.FullName, '-NonInteractive', '-Verbosity', 'quiet')
}

Import-Module "${env:UserProfile}\.nuget\packages\2GIS.NuClear.BuildTools\0.2.36\tools\buildtools.psm1" -DisableNameChecking -Force
Import-Module "$PSScriptRoot\metadata.psm1" -DisableNameChecking -Force

Add-Metadata (Parse-EnvironmentMetadata $Properties)
Run-Build $TaskList $Properties
