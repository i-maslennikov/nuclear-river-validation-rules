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
}

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
	switch($Context.EnvType){
		{ @('Production', 'Load') -contains $_ } {
			return @{ 'ValidateWebsite' = $false }
		}
		default {
			return @{ 'ValidateWebsite' = $true }
		}
	}
}

function Get-IisAppPathMetadata ($Context) {

	switch ($Context.EntryPoint) {
		'2Gis.Erm.UI.Web.Mvc' { $prefix = "web-app$($Context['Index'])" }
		'2Gis.Erm.API.WCF.Operations' { $prefix = "basic-operations$($Context['Index']).api" }
		'2Gis.Erm.API.WCF.MoDi' { $prefix = "money-distribution$($Context['Index']).api" }
		'2Gis.Erm.API.WCF.Metadata' { $prefix = "metadata$($Context['Index']).api" }
		'2Gis.Erm.API.WCF.OrderValidation' { $prefix = "order-validation$($Context['Index']).api" }
		'2Gis.Erm.API.WCF.Operations.Special' { $prefix = "financial-operations$($Context['Index']).api" }
		'2Gis.Erm.API.WCF.Releasing' { $prefix = "releasing$($Context['Index']).api" }
		'2Gis.Erm.UI.Desktop.WPF' { $prefix = "wpf-app$($Context['Index'])" }
		# line added
		'Web.OData' { $prefix = "search$($Context['Index']).api" }
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

function Get-TakeOfflineMetadata ($Context) {
	switch($Context.EnvType){
		'Production' {
			return @{ 'TakeOffline' = $false }
		}
		default {
			return @{ 'TakeOffline' = $true }
		}
	}
}

function Get-OptionsMetadata ($Context) {

	switch ($Context.Country){
		'Russia' {
			return @{
				'OptionModi' = $true
			}
		}
		default {
			return @{
				'OptionModi' = $false
			}
		}
	}
}

function Get-WebMetadata ($Context) {

	$metadata = @{}
	$metadata += Get-ValidateWebsiteMetadata $Context
	$metadata += Get-TargetHostsMetadata $Context
	$metadata += Get-IisAppPathMetadata $Context
	$metadata += Get-TakeOfflineMetadata $Context
	$metadata += Get-OptionsMetadata $Context
	$metadata += Get-TransformMetadata $Context
	
	$metadata += @{
		'EntrypointType' = 'Web'
	}
	
	return @{ "$($Context.EntryPoint)" = $metadata }
}

Export-ModuleMember -Function Get-WebMetadata