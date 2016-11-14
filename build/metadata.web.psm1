Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$PSScriptRoot\metadata.transform.psm1" -DisableNameChecking

$DomainNames = @{
	'Chile' = 'cl'
	'Cyprus' = 'com.cy'
	'Czech' = 'cz'
	'Emirates' = 'ae'
	'Russia' = 'ru'
	'Ukraine' = 'ua'
	'Kazakhstan' = 'kz'
	'Kyrgyzstan' = 'kg'
	'Italy' = 'it'}

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
			return @{ 'TargetHosts' = @('uk-erm-edu01') }
		}
		'Business' {
			return @{ 'TargetHosts' = @('uk-erm-edu02') }
		}
		'Production' {
			return @{ 'TargetHosts' = @('uk-erm-iis03') }
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
			return @{ 'TargetHosts' = @('uk-erm-iis12') }
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
	
	return @{ 'AppPoolName' = 'ErmAppPool' }
}

function Get-WebMetadata ($Context) {

	$metadata = @{}
	$metadata += Get-ValidateWebsiteMetadata $Context
	$metadata += Get-TargetHostsMetadata $Context
	$metadata += Get-IisAppPathMetadata $Context
	$metadata += Get-IisAppPoolMetadata $Context
	$metadata += Get-TransformMetadata $Context
	
	$metadata += @{
		'EntrypointType' = 'Web'
	}
	
	return @{ "$($Context.EntryPoint)" = $metadata }
}

Export-ModuleMember -Function Get-WebMetadata