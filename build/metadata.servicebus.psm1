Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

$topicProperties = @{
	'Properties' = @{
		'EnableBatchedOperations' = $true
		'SupportOrdering' = $true
		'RequiresDuplicateDetection' = $true
	}
}

$subscriptionProperties = @{
	'Properties' = @{
		'EnableBatchedOperations' = $true
		'MaxDeliveryCount' = 0x7fffffff
		'LockDuration' = New-TimeSpan -Minutes 5
	}
}

function Get-ServiceBusMetadata ($Context) {

	$metadata = @{}
	$metadata += Get-TopicsMetadata $Context

	$metadata['UseCaseRoute'] = $Context.UseCaseRoute

	return @{ 'ServiceBus' = $metadata}
}

function Get-TopicsMetadata ($Context) {

	$metadata = @{}

	switch ($Context.EntryPoint) {

		'ValidationRules.Replication.Host' {

			switch ($Context.UseCaseRoute){
				{ $_ -eq 'ERM' -or $_ -eq $null } {
					$ermEventsFlowTopic = @{
						'ErmEventsFlowTopic' = @{
							'Name' = 'topic.performedoperations'
							'ConnectionStringName' = 'ServiceBus'
						} + $topicProperties
					}
					$ermEventsFlowSubscription = @{
						'ErmEventsFlowSubscription' = @{
							'TopicName' = 'topic.performedoperations'
							'Name' = '6A75B8B4-74A6-4523-9388-84E4DFFD5B06'
							'ConnectionStringName' = 'ServiceBus'
						} + $subscriptionProperties
					}
					$deleteTopic = @{}
					$deleteSubscription = @{}
				}

				'ERMProduction' {
					$ermEventsFlowTopic = @{
						'ErmEventsFlowTopic' = @{
							'Name' = "topic.performedoperations.production.$($Context.Country).import".ToLowerInvariant()
							'ConnectionStringName' = 'ServiceBus'
						} + $topicProperties
					}
					$ermEventsFlowSubscription = @{
						'ErmEventsFlowSubscription' = @{
							'TopicName' = "topic.performedoperations.production.$($Context.Country).import".ToLowerInvariant()
							'Name' = '6A75B8B4-74A6-4523-9388-84E4DFFD5B06'
							'ConnectionStringName' = 'ServiceBus'
						} + $subscriptionProperties
					}
					$deleteTopic = @{}
					$deleteSubscription = @{
						'DeleteErmEventsFlowSubscription' = @{
							'TopicName' = 'topic.performedoperations'
							'Name' = '6A75B8B4-74A6-4523-9388-84E4DFFD5B06'
							'ConnectionStringName' = 'ServiceBus'
						}
					}
				}
			}

			$metadata = @{
				'CreateTopics' = @{
					'CommonEventsFlowTopic' = @{
						'Name' = 'topic.river.validationrules.common'
						'ConnectionStringName' = 'ServiceBus'
					} + $topicProperties

					'MessageEventsFlowTopic' = @{
						'Name' = 'topic.river.validationrules.messages'
						'ConnectionStringName' = 'ServiceBus'
					} + $topicProperties

				} + $ermEventsFlowTopic

				'CreateSubscriptions' = @{
					'CommonEventsFlowSubscription' = @{
						'TopicName' = 'topic.river.validationrules.common'
						'Name' = 'CB1434CA-D575-4470-8616-4F08D074C8DA'
						'ConnectionStringName' = 'ServiceBus'
					} + $subscriptionProperties

					'MessageEventsFlowSubscription' = @{
						'TopicName' = 'topic.river.validationrules.messages'
						'Name' = '2B3D30F7-6E59-4510-B680-D7FDD9DEFE0F'
						'ConnectionStringName' = 'ServiceBus'
					} + $subscriptionProperties
				} + $ermEventsFlowSubscription
			} 
		}

		# нужен чтобы удалить subscription с production, т.к. только он знает нужный connectionString
		'ConvertUseCasesService-Production' {
			switch ($Context.UseCaseRoute){
				{ $_ -eq 'ERM' -or $_ -eq $null } {
					$metadata = @{
						'DeleteSubscriptions' = @{
							'DeleteConvertUseCasesSubscription-ERMProduction' = @{
								'TopicName' = 'topic.performedoperations.export'
								'Name' = $Context.EnvironmentName.ToLowerInvariant()
								'ConnectionStringName' = 'Source'
							}
						}
					}
				}
			}
		}

		'ConvertUseCasesService' {
			switch ($Context.UseCaseRoute){
				'ERMProduction' {
					$metadata = @{
						'CreateTopics' = @{
							'SourceTopic' = @{
								'Name' = 'topic.performedoperations.export'
								'ConnectionStringName' = 'Source'
							} + $topicProperties
							'DestTopic' = @{
								'Name' = "topic.performedoperations.production.$($Context.Country).import".ToLowerInvariant()
								'ConnectionStringName' = 'Dest'
							} + $topicProperties
						}
						'CreateSubscriptions' = @{
							'SourceSubscription' = @{
								'TopicName' = 'topic.performedoperations.export'
								'Name' = $Context.EnvironmentName.ToLowerInvariant()
								'ConnectionStringName' = 'Source'
							} + $subscriptionProperties

							'DestSubscription' = @{
								'TopicName'  = "topic.performedoperations.production.$($Context.Country).import".ToLowerInvariant()
								'Name' = '6A75B8B4-74A6-4523-9388-84E4DFFD5B06'
								'ConnectionStringName' = 'Dest'
							} + $subscriptionProperties
						}
					}
				}
			}
		}
	}

	return $metadata
}

Export-ModuleMember -Function Get-ServiceBusMetadata