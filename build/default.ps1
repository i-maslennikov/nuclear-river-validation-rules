Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\deploy.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\nuget.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\buildqueue.psm1" -DisableNameChecking

Include "$BuildToolsRoot\psake\nuget.ps1"
Include "$BuildToolsRoot\psake\unittests.ps1"
Include 'convertusecases.ps1'
Include 'updateschemas.ps1'
Include 'bulktool.ps1'
Include 'datatest.ps1'

Task QueueBuild-OData -Precondition { $Metadata['CustomerIntelligence.Querying.Host'] } {
	$projectFileName = Get-ProjectFileName 'CustomerIntelligence' 'CustomerIntelligence.Querying.Host'
	QueueBuild-WebPackage $projectFileName 'CustomerIntelligence.Querying.Host'
}
Task Deploy-OData -Depends Take-ODataOffline -Precondition { $Metadata['CustomerIntelligence.Querying.Host'] } {
	Deploy-WebPackage 'CustomerIntelligence.Querying.Host'
	Validate-WebSite 'CustomerIntelligence.Querying.Host' 'CustomerIntelligence/$metadata'
}

Task Take-ODataOffline -Precondition { $Metadata['CustomerIntelligence.Querying.Host'] } {
	Take-WebsiteOffline 'CustomerIntelligence.Querying.Host'
}

Task QueueBuild-TaskService -Precondition { $Metadata['Replication.EntryPoint'] } {
	$projectFileName = Get-ProjectFileName 'Replication' 'Replication.EntryPoint'
	QueueBuild-AppPackage $projectFileName 'Replication.EntryPoint'
}

Task Deploy-TaskService -Depends Import-WinServiceModule, Take-TaskServiceOffline -Precondition { $Metadata['Replication.EntryPoint'] } {
	Deploy-WinService 'Replication.EntryPoint'
}

Task Take-TaskServiceOffline -Depends Import-WinServiceModule -Precondition { $Metadata['Replication.EntryPoint'] } {
	Take-WinServiceOffline 'Replication.EntryPoint'
}

Task Import-WinServiceModule {
	Load-WinServiceModule 'Replication.EntryPoint'
}

Task Build-Queue { Invoke-MSBuildQueue }

Task Run-DataTests {
	$projects = Find-Projects '.' '*.StateInitialization.Tests*'
	Run-DataTests $projects 'UnitTests'
}

Task Validate-PullRequest -depends Run-UnitTests, Run-DataTests

Task Build-Packages -depends `
Build-ConvertUseCasesService, `
QueueBuild-OData, `
QueueBuild-TaskService, `
QueueBuild-BulkTool, `
Build-Queue

Task Deploy-Packages -depends `
Take-ODataOffline, `
Take-TaskServiceOffline, `
Update-Schemas, `
Run-BulkTool, `
Deploy-ConvertUseCasesService, `
Deploy-OData, `
Deploy-TaskService