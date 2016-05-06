Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking

function QueueBuild-BulkTool  {
	if ($Metadata['CustomerIntelligence.StateInitialization.Host']){
		$projectFileName = Get-ProjectFileName 'CustomerIntelligence' 'CustomerIntelligence.StateInitialization.Host'
		QueueBuild-AppPackage $projectFileName 'CustomerIntelligence.StateInitialization.Host'
	}
}

Task Run-BulkTool -Precondition { $Metadata['CustomerIntelligence.StateInitialization.Host'] } {
	$artifactName = Get-Artifacts 'CustomerIntelligence.StateInitialization.Host'

	$exePath = Join-Path $artifactName '2GIS.NuClear.CustomerIntelligence.StateInitialization.Host.exe'

	Write-Host 'Invoke bulktool with' $Metadata['CustomerIntelligence.StateInitialization.Host'].Arguments
	& $exePath $Metadata['CustomerIntelligence.StateInitialization.Host'].Arguments | Write-Host

	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}