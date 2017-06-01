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
		Write-Host "Skip Deploy-ServiceBus"
		return
	}

	if ($serviceBusMetadata['CreateTopics']){
		foreach($createTopicsMetadata in $serviceBusMetadata.CreateTopics.Values){
			$connectionString = Get-EntryPointConnectionString $entryPointName $createTopicsMetadata.ConnectionStringName
			Create-Topic $connectionString $createTopicsMetadata.Name $createTopicsMetadata.Properties
		}
	}
	if ($serviceBusMetadata['DeleteTopics']){
		foreach($deleteTopicsMetadata in $serviceBusMetadata.DeleteTopics.Values){
			$connectionString = Get-EntryPointConnectionString $entryPointName $deleteTopicsMetadata.ConnectionStringName
			Delete-Topic $connectionString $deleteTopicsMetadata.Name
		}
	}

	if ($serviceBusMetadata['CreateSubscriptions']){
		foreach($createSubscriptionsMetadata in $serviceBusMetadata.CreateSubscriptions.Values){
			$connectionString = Get-EntryPointConnectionString $entryPointName $createSubscriptionsMetadata.ConnectionStringName
			if ($Metadata['UpdateSchemas']){
				Delete-Subscription $connectionString $createSubscriptionsMetadata.TopicName $createSubscriptionsMetadata.Name
				Write-Host "Delete-Subscription:", $createSubscriptionsMetadata.TopicName, " ", $createSubscriptionsMetadata.Name
			}
			Create-Subscription $connectionString $createSubscriptionsMetadata.TopicName $createSubscriptionsMetadata.Name $createSubscriptionsMetadata.Properties
			Write-Host "Create-Subscription:", $createSubscriptionsMetadata.TopicName, " ", $createSubscriptionsMetadata.Name
		}
	}
	if ($serviceBusMetadata['DeleteSubscriptions']){
		foreach($deleteSubscriptionsMetadata in $serviceBusMetadata.DeleteSubscriptions.Values){
			$connectionString = Get-EntryPointConnectionString $entryPointName $deleteSubscriptionsMetadata.ConnectionStringName
			Delete-Subscription $connectionString $deleteSubscriptionsMetadata.TopicName $deleteSubscriptionsMetadata.Name
			Write-Host "Delete-Subscription:", $deleteSubscriptionsMetadata.TopicName, " ", $deleteSubscriptionsMetadata.Name
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
