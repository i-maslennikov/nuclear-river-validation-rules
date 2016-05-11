Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\buildqueue.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\deployqueue.psm1" -DisableNameChecking

Include "$BuildToolsRoot\psake\nuget.ps1"
Include "$BuildToolsRoot\psake\unittests.ps1"
Include 'convertusecases.ps1'
Include 'updateschemas.ps1'
Include 'bulktool.ps1'
Include 'datatest.ps1'

# OData
function QueueBuild-OData {
	if ($Metadata['CustomerIntelligence.Querying.Host']){
		$projectFileName = Get-ProjectFileName 'CustomerIntelligence' 'CustomerIntelligence.Querying.Host'
		QueueBuild-WebPackage $projectFileName 'CustomerIntelligence.Querying.Host'
	}
}
function QueueDeploy-OData {
	if ($Metadata['CustomerIntelligence.Querying.Host']){
		QueueDeploy-WebPackage 'CustomerIntelligence.Querying.Host'
	}
}

# task service
function QueueBuild-TaskService {
	if ($Metadata['CustomerIntelligence.Replication.Host']){
		$projectFileName = Get-ProjectFileName 'CustomerIntelligence' 'CustomerIntelligence.Replication.Host'
		QueueBuild-AppPackage $projectFileName 'CustomerIntelligence.Replication.Host'
	}
}
function QueueDeploy-TaskService {
	if ($Metadata['CustomerIntelligence.Replication.Host']){
		QueueDeploy-WinService 'CustomerIntelligence.Replication.Host'
	}
}

Task QueueBuild-Packages {

	QueueBuild-BulkTool
	QueueBuild-OData
	QueueBuild-TaskService

	Invoke-MSBuildQueue
}

Task QueueDeploy-Packages {

	QueueDeploy-ConvertUseCasesService
	QueueDeploy-OData
	QueueDeploy-TaskService

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
Update-Schemas, `
Run-BulkTool, `
Create-Topics, `
QueueDeploy-Packages