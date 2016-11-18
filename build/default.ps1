Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\buildqueue.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\deployqueue.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\deploy.winservice.psm1" -DisableNameChecking

Include "$BuildToolsRoot\psake\nuget.ps1"
Include "$BuildToolsRoot\psake\unittests.ps1"
Include 'servicebus.ps1'
Include 'convertusecases.ps1'
Include 'updateschemas.ps1'
Include 'bulktool.ps1'
Include 'datatest.ps1'

# Querying.Host
function QueueBuild-QueryingHost {
	if ($Metadata['ValidationRules.Querying.Host']){
		$projectFileName = Get-ProjectFileName '.' 'ValidationRules.Querying.Host'
		QueueBuild-WebPackage $projectFileName 'ValidationRules.Querying.Host'
	}
}
function QueueDeploy-QueryingHost {
	if ($Metadata['ValidationRules.Querying.Host']){
		QueueDeploy-WebPackage 'ValidationRules.Querying.Host'
	}
}

# Replication.Host
function QueueBuild-ReplicationHost {
	if ($Metadata['ValidationRules.Replication.Host']){
		$projectFileName = Get-ProjectFileName '.' 'ValidationRules.Replication.Host'
		QueueBuild-AppPackage $projectFileName 'ValidationRules.Replication.Host'
	}
}
function QueueDeploy-ReplicationHost {
	if ($Metadata['ValidationRules.Replication.Host']){
		QueueDeploy-WinService 'ValidationRules.Replication.Host'
	}
}
Task Stop-ReplicationHost -Precondition { $Metadata['ValidationRules.Replication.Host'] } {
	Load-WinServiceModule 'ValidationRules.Replication.Host'
	Take-WinServiceOffline 'ValidationRules.Replication.Host'
}

Task QueueBuild-Packages {

	QueueBuild-BulkTool
	QueueBuild-QueryingHost
	QueueBuild-ReplicationHost

	Invoke-MSBuildQueue
}

Task QueueDeploy-Packages {

	QueueDeploy-ConvertUseCasesService
	QueueDeploy-QueryingHost
	QueueDeploy-ReplicationHost

	Invoke-DeployQueue
}

Task Run-DataTests {
	$projects = Find-Projects '.' '*.StateInitialization.Tests*'
	Run-DataTests $projects 'UnitTests'
}

Task Validate-PullRequest -depends Run-UnitTests, Run-DataTests

Task Build-Packages -depends `
Build-ConvertUseCasesService, `
QueueBuild-Packages

Task Deploy-Packages -depends `
Deploy-ServiceBus, `
Update-Schemas, `
Run-BulkTool, `
QueueDeploy-Packages