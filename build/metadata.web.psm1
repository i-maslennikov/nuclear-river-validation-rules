﻿Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$PSScriptRoot\metadata.transform.psm1" -DisableNameChecking

function Get-TargetHostsMetadata ($Context) {

	switch ($Context.EnvType) {
		'Test' {
			switch ($Context.Country) {
				'Russia' {
					return @{ 'TargetHosts' = @('uk-erm-test01') }
				}
				default {
					return @{ 'TargetHosts' = @('uk-erm-test02') }
				}
			}
		}
		'Edu' {
			return @{ 'TargetHosts' = @('uk-erm-edu03') }
		}
		'Business' {
			return @{ 'TargetHosts' = @('uk-erm-edu03') }
		}
		'Production' {
			return @{ 'TargetHosts' = @('uk-erm-iis03', 'uk-erm-iis01', 'uk-erm-iis02', 'uk-erm-iis04') }
		}
		'Int' {
			switch ($Context.Country) {
				'Russia' {
					return @{ 'TargetHosts' = @('uk-test-int02') }
				}
				default {
					return @{ 'TargetHosts' = @('uk-test-int01') }
				}
			}
		}
		'Load' {
			return @{ 'TargetHosts' = @('uk-erm-iis12', 'uk-erm-iis11', 'uk-erm-iis10') }
		}
		'Appveyor' {
			return @{ 'TargetHosts' = @() }
		}
		default {
			throw "Unknown environment type '$($Context.EnvType)'"
		}
	}
}

function Get-ValidateWebsiteMetadata ($Context) {

	if ( @('Production', 'Load') -contains $Context.EnvType ){
		return @{}
	}

	switch($Context.EntryPoint){
		'ValidationRules.Querying.Host' {
			$uriPath = 'api/version'
		}
		default {
			$uriPath = '/'
		}
	}

	return @{ 'ValidateUriPath' = $uriPath }
}

function Get-IisAppPathMetadata ($Context) {

	switch ($Context.EntryPoint) {
		'ValidationRules.Querying.Host' { $prefix = "validation$($Context['Index']).api" }
		default {
			return @{}
		}
	}

	$envTypeLower = $Context.EnvType.ToLowerInvariant()
	$domain = $DomainNames[$Context.Country]

	switch ($Context.EnvType) {
		'Production' {
			return @{ 'IisAppPath' = "$prefix.prod.erm.2gis.$domain" }
		}
		default {
			return @{ 'IisAppPath' = "$prefix.$envTypeLower.erm.2gis.$domain" }
		}
	}
}

function Get-IisAppPoolMetadata ($Context) {
	
	switch ($Context.EnvType) {
		{ @('Production', 'Load') -contains $_ } {
			switch($Context.Country){
				'Russia' {
					$appPoolName = "$($Context.EntryPoint) ($($Context.EnvironmentName))"
				}
				default {
					$appPoolName = "ERM ($($Context.EnvironmentName))"
				}
			}
		}
		default {
			$appPoolName = "ERM ($($Context.EnvironmentName))"
		}
	}
	
	return @{ 'AppPoolName' = $appPoolName }
}

function Get-IisAppOfflineMetadata ($Context) {

	# should always use app_offline.htm to unload librdkafka.dll before deploy
	return @{ 'TakeOffline' = $true }
}

function Get-IisMetadata ($Context) {
	$metadata = @{}
	$metadata += Get-IisAppPathMetadata $Context
	$metadata += Get-IisAppPoolMetadata $Context
	$metadata += Get-IisAppOfflineMetadata $Context
	
	return $metadata
}

function Get-WebMetadata ($Context) {

	$metadata = @{}
	$metadata += Get-ValidateWebsiteMetadata $Context
	$metadata += Get-TargetHostsMetadata $Context
	$metadata += Get-IisMetadata $Context

	$metadata += Get-TransformMetadata $Context

	$metadata += @{
		'EntrypointType' = 'Web'
	}
	
	return @{ "$($Context.EntryPoint)" = $metadata }
}

Export-ModuleMember -Function Get-WebMetadata