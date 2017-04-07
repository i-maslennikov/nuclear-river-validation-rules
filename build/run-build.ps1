param([string[]]$TaskList = @(), [hashtable]$Properties = @{})
#Requires –Version 3.0

if ($TaskList.Count -eq 0){
	$TaskList = @('Build-Packages')
}

if ($Properties.Count -eq 0){
 	$Properties.EnvironmentName = 'Test.20'
	$Properties.EntryPoints = @(
		'ValidationRules.Querying.Host'
		'ValidationRules.Replication.Host'
	)
	#$Properties.UseCaseRoute = 'ERM'
	#$Properties.UpdateSchemas = 'PriceAggregate'
}

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

$Properties.SolutionDir = Join-Path $PSScriptRoot '..'
$Properties.BuildFile = Join-Path $PSScriptRoot 'default.ps1'

# Restore-Packages
& {
	$NugetPath = Join-Path $Properties.SolutionDir '.nuget\NuGet_v3.5.0.exe'
	if (!(Test-Path $NugetPath)){
		$webClient = New-Object System.Net.WebClient
		$webClient.UseDefaultCredentials = $true
		$webClient.Proxy.Credentials = $webClient.Credentials
		$webClient.DownloadFile('https://dist.nuget.org/win-x86-commandline/v3.5.0/nuget.exe', $NugetPath)
	}
	$solution = Get-ChildItem $Properties.SolutionDir -Filter '*.sln'
	& $NugetPath @('restore', $solution.FullName, '-NonInteractive', '-Verbosity', 'quiet')
}

$packageName = "2GIS.NuClear.BuildTools"
$packageVersion = (ConvertFrom-Json (Get-Content "$PSScriptRoot\project.json" -Raw)).dependencies.PSObject.Properties[$packageName].Value
Import-Module "${env:UserProfile}\.nuget\packages\$packageName\$packageVersion\tools\buildtools.psm1" -DisableNameChecking -Force

$metadata = & "$PSScriptRoot\metadata.ps1" $Properties
Add-Metadata $metadata

Run-Build $TaskList $Properties
