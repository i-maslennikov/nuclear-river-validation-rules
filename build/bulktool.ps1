Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking

Task QueueBuild-BulkTool -Precondition { $Metadata['ValidationRules.StateInitialization.EntryPoint'] } {
	$projectFileName = Get-ProjectFileName 'ValidationRules' 'ValidationRules.StateInitialization.EntryPoint'
	QueueBuild-AppPackage $projectFileName 'ValidationRules.StateInitialization.EntryPoint'
}

Task Run-BulkTool -Precondition { $Metadata['ValidationRules.StateInitialization.EntryPoint'] } {
	$artifactName = Get-Artifacts 'ValidationRules.StateInitialization.EntryPoint'

	$exePath = Join-Path $artifactName '2GIS.NuClear.ValidationRules.StateInitialization.EntryPoint.exe'

	Write-Host 'Invoke bulktool with' $Metadata['ValidationRules.StateInitialization.EntryPoint'].Arguments
	& $exePath $Metadata['ValidationRules.StateInitialization.EntryPoint'].Arguments | Write-Host

	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}