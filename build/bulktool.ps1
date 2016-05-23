Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking

function QueueBuild-BulkTool  {
	if ($Metadata['ValidationRules.StateInitialization.Host']){
		$projectFileName = Get-ProjectFileName 'ValidationRules' 'ValidationRules.StateInitialization.Host'
		QueueBuild-AppPackage $projectFileName 'ValidationRules.StateInitialization.Host'
	}
}

Task Run-BulkTool -Precondition { $Metadata['ValidationRules.StateInitialization.Host'] } {
	$artifactName = Get-Artifacts 'ValidationRules.StateInitialization.Host'

	$exePath = Join-Path $artifactName '2GIS.NuClear.ValidationRules.StateInitialization.Host.exe'

	Write-Host 'Invoke bulktool with' $Metadata['ValidationRules.StateInitialization.Host'].Arguments
	& $exePath $Metadata['ValidationRules.StateInitialization.Host'].Arguments | Write-Host

	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}