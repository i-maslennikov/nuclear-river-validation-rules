Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$BuildToolsRoot\modules\artifacts.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\servicebus.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking

# TODO: QueueDeploy-ServiceBus
Task Deploy-ServiceBus {

	if ($Metadata['ValidationRules.Replication.Host']){
		Deploy-ServiceBus 'ValidationRules.Replication.Host'	
	}

	if ($Metadata['ConvertUseCasesService-Production']){
		Deploy-ServiceBus 'ConvertUseCasesService-Production'	
	}

	if ($Metadata['ConvertUseCasesService']){
		Deploy-ServiceBus 'ConvertUseCasesService'	
	}
}

function Deploy-ServiceBus ($entryPointName){

	$serviceBusMetadata = $Metadata[$entryPointName]['ServiceBus']
	if (!$serviceBusMetadata){
		return
	}

	if ($serviceBusMetadata['CreateTopics']){
		foreach($metadata in $serviceBusMetadata.CreateTopics.Values){
			$connectionString = Get-EntryPointConnectionString $entryPointName $metadata.ConnectionStringName
			Create-Topic $connectionString $metadata.Name $metadata.Properties
		}
	}
	if ($serviceBusMetadata['DeleteTopics']){
		foreach($metadata in $serviceBusMetadata.DeleteTopics.Values){
			$connectionString = Get-EntryPointConnectionString $entryPointName $metadata.ConnectionStringName
			Delete-Topic $connectionString $metadata.Name
		}
	}

	if ($serviceBusMetadata['CreateSubscriptions']){
		foreach($metadata in $serviceBusMetadata.CreateSubscriptions.Values){
			$connectionString = Get-EntryPointConnectionString $entryPointName $metadata.ConnectionStringName
			Create-Subscription $connectionString $metadata.TopicName $metadata.Name $metadata.Properties
		}
	}
	if ($serviceBusMetadata['DeleteSubscriptions']){
		foreach($metadata in $serviceBusMetadata.DeleteSubscriptions.Values){
			$connectionString = Get-EntryPointConnectionString $entryPointName $metadata.ConnectionStringName
			Delete-Subscription $connectionString $metadata.TopicName $metadata.Name
		}
	}
}

function Get-EntryPointConnectionString ($entryPointName, $connectionStringName) {
	$artifactDir = Get-Artifacts $entryPointName

	$configItem = @(Get-ChildItem $artifactDir -Filter '*.exe.config')
	if ($configItem.Length -eq 0){
		throw "Can't find *.exe.config file in $artifactDir"
	}
	if ($configItem.Length -gt 1){
		throw "Find more than one *.exe.config file in $artifactDir"
	}

	[xml]$config = Get-Content $configItem.FullName
	$connectionString = Get-ConnectionString $config $connectionStringName
	return $connectionString
}
