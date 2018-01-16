Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$PSScriptRoot\metadata.web.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\metadata.transform.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\metadata.servicebus.psm1" -DisableNameChecking

function Get-QuartzConfigMetadata ($Context){

	$quartzConfigs = @('quartz.ams_config')
	$alterQuartzConfigs = @()

	switch ($Context.EnvType){
		'Test' {
			switch ($Context.Country){
				default {
					$quartzConfigs += @('Templates\quartz.Test.config')
				}
			}
		}
		'Production' {
			switch ($Context.Country){
				default {
					$quartzConfigs += @('quartz.Production.config')
				}
			}
		}
		default {
			$quartzConfigs += @("quartz.$($Context.EnvType).config")
			$alterQuartzConfigs += @('Templates\quartz.Test.config')
		}
	}

	return @{
		'QuartzConfigs' =  $quartzConfigs
		'AlterQuartzConfigs' = $alterQuartzConfigs
	}
}

function Get-TargetHostsMetadata ($Context){

	switch ($Context.EnvType) {
		'Production' {
			switch ($Context.EntryPoint){
				'ValidationRules.Replication.Host'{
					return @{ 'TargetHosts' = @('uk-erm-bus01', 'uk-erm-bus02', 'uk-erm-bus03', 'uk-erm-bus04') }
				}
				'ConvertUseCasesService' {
					return @{ 'TargetHosts' = @('uk-erm-bus01') }
				}
				'ConvertUseCasesService-Production' { return @{} }
				default {
					throw "Unknown entrypoint $_"
				}
			}
		}
		'Load' {
			return @{ 'TargetHosts' = @('uk-erm-iis10', 'uk-erm-iis11', 'uk-erm-iis12') }
		}
		'Test' {
			switch ($Context.Country) {
				'Russia' {
					return @{ 'TargetHosts' = @('uk-erm-test03') }
				}
				default {
					return @{ 'TargetHosts' = @('uk-erm-test02') }
				}
			}
		}
		default {
			$webMetadata = Get-WebMetadata $Context
			if ($webMetadata -eq $null){
				throw "Can't find web metadata for entrypoint $($Context.EntryPoint)"
			}
			
			return @{'TargetHosts' = $webMetadata[$Context.EntryPoint].TargetHosts}
		}
	}
}

function Get-ServiceNameMetadata ($Context) {
	switch ($Context.EntryPoint) {
		'ValidationRules.Replication.Host' {
			return @{
				'ServiceName' = 'ValidationRules.Replication.Host'
				'ServiceDisplayName' = '2GIS NuClear River ValidationRules Replication Host Service'
			}
		}
		'ConvertUseCasesService' {
			return @{
				'ServiceName' = 'ConvertUseCases'
				'ServiceDisplayName' = '2GIS NuClear River Convert UseCases Service'
			}
		}
		default {
			return @{}
		}
	}
}

function Get-WinServiceMetadata ($Context) {

	$metadata = @{}
	$metadata += Get-TargetHostsMetadata $Context
	$metadata += Get-QuartzConfigMetadata $Context
	$metadata += Get-ServiceNameMetadata $Context
	$metadata += Get-TransformMetadata $Context
	$metadata += Get-ServiceBusMetadata $Context
	
	$metadata += @{
		'EntrypointType' = 'Desktop'
	}
	
	return @{ "$($Context.EntryPoint)" = $metadata }
}

Export-ModuleMember -Function Get-WinServiceMetadata