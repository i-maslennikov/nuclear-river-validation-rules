Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$PSScriptRoot\metadata.web.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\metadata.transform.psm1" -DisableNameChecking

function Get-QuartzConfigMetadata ($Context){

	switch ($Context.EnvType){
		'Test' {
			switch ($Context.Country){
				'Russia' {
					$quartzConfigs = @('Templates\quartz.Test.Russia.config')
					
					$alterQuartzConfigs = @()
				}
				'Emirates' {
					$quartzConfigs = @('Templates\quartz.Test.Emirates.config')
					$alterQuartzConfigs = @()
				}
				default {
					$quartzConfigs = @('Templates\quartz.Test.MultiCulture.config')
					$alterQuartzConfigs = @()
				}
			}
		}
		'Production' {
			switch ($Context.Country){
				'Russia' {
					$quartzConfigs = @('quartz.Production.Russia.config')
					$alterQuartzConfigs = @()
				}
				'Emirates' {
					$quartzConfigs = @('quartz.Production.Emirates.config')
					$alterQuartzConfigs = @()
				}
				default {
					$quartzConfigs = @('quartz.Production.MultiCulture.config')
					$alterQuartzConfigs = @()
				}
			}
		}
		default {
			switch ($Context.Country){
				'Russia' {
					$quartzConfigs = @("quartz.$($Context.EnvType).Russia.config")
					$alterQuartzConfigs = @('Templates\quartz.Test.Russia.config')
				}
				'Emirates' {
					$quartzConfigs = @("quartz.$($Context.EnvType).Emirates.config")
					$alterQuartzConfigs = @('Templates\quartz.Test.Emirates.config')
				}
				default {
					$quartzConfigs = @("quartz.$($Context.EnvType).MultiCulture.config")
					$alterQuartzConfigs = @('Templates\quartz.Test.MultiCulture.config')
				}
			}
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
				'Replication.EntryPoint'{
					return @{ 'TargetHosts' = @('uk-erm-sb01', 'uk-erm-sb03', 'uk-erm-sb04') }
				}
				{ @('ConvertUseCasesService', 'ConvertUseCasesServiceProduction') -contains $_ } {
					return @{ 'TargetHosts' = @('uk-erm-sb01') }
				}
				default {
					throw "Unknown entrypoint $_"
				}
			}
		}
		'Load' {
			return @{ 'TargetHosts' = @('uk-erm-iis10', 'uk-erm-iis11', 'uk-erm-iis12') }
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
		'Replication.EntryPoint' {
			return @{
				'ServiceName' = 'AdvSearch'
				'ServiceDisplayName' = '2GIS ERM AdvancedSearch Replication Service'
			}
		}
		{ @('ConvertUseCasesService', 'ConvertUseCasesServiceProduction') -contains $_ } {
			return @{
				'ServiceName' = 'ConvertUseCases'
				'ServiceDisplayName' = '2GIS ERM AdvancedSearch Convert UseCases Service'
			}
		}
	}
}

function Get-TaskServiceMetadata ($Context) {

	$metadata = @{}
	$metadata += Get-TargetHostsMetadata $Context
	$metadata += Get-QuartzConfigMetadata $Context
	$metadata += Get-ServiceNameMetadata $Context
	$metadata += Get-TransformMetadata $Context
	
	$metadata += @{
		'EntrypointType' = 'Desktop'
	}
	
	return @{ "$($Context.EntryPoint)" = $metadata }
}

Export-ModuleMember -Function Get-TaskServiceMetadata