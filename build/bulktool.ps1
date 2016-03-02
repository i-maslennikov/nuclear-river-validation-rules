Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking

Task QueueBuild-BulkTool -Precondition { $Metadata['CustomerIntelligence.StateInitialization.EntryPoint'] } {
	$projectFileName = Get-ProjectFileName 'CustomerIntelligence' 'CustomerIntelligence.StateInitialization.EntryPoint'
	QueueBuild-AppPackage $projectFileName 'CustomerIntelligence.StateInitialization.EntryPoint'
}

Task Run-BulkTool -Precondition { $Metadata['CustomerIntelligence.StateInitialization.EntryPoint'] } {
	$artifactName = Get-Artifacts 'CustomerIntelligence.StateInitialization.EntryPoint'

	$exePath = Join-Path $artifactName '2GIS.NuClear.CustomerIntelligence.StateInitialization.EntryPoint.exe'

	Write-Host 'Invoke bulktool with' $Metadata['CustomerIntelligence.StateInitialization.EntryPoint'].Arguments
	& $exePath $Metadata['CustomerIntelligence.StateInitialization.EntryPoint'].Arguments | Write-Host

	if ($LastExitCode -ne 0) {
		throw "Command failed with exit code $LastExitCode"
	}
}