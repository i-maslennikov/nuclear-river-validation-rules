Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$PSScriptRoot\metadata.servicebus.psm1" -DisableNameChecking

$DomainNames = @{
	'Chile' = 'cl'
	'Cyprus' = 'com.cy'
	'Czech' = 'cz'
	'Emirates' = 'ae'
	'Russia' = 'ru'
	'Ukraine' = 'ua'
	'Kazakhstan' = 'kz'
	'Kyrgyzstan' = 'kg'
}

$DBSuffixes = @{
	'Chile' = 'CL'
	'Cyprus' = 'CY'
	'Czech' = 'CZ'
	'Emirates' = 'AE'
	'Russia' = 'RU'
	'Ukraine' = 'UA'
	'Kazakhstan' = 'KZ'
	'Kyrgyzstan' = 'KG'
}

function Get-DBSuffix($Context){

	$countrySuffix = $DBSuffixes[$Context['Country']];

	switch($Context.EnvType){
		'Business' {
			$envTypeSuffix = $Context.EnvType
		}
		'Edu' {
			$envTypeSuffix = $Context.EnvType
		}
		default {
			$envTypeSuffix = $null
		}
	}

	return $envTypeSuffix + $countrySuffix + $Context['Index']
}

function Get-DBHostMetadata($Context){
	switch($Context.EnvType){
		'Test' {
			switch($Context.Country){
				'Russia' { $dbHost = 'uk-sql01' }
				default { $dbHost = 'uk-erm-sql02' }
			}
		}
		'Business' {
			$dbHost = 'uk-erm-edu03'
		}
		'Edu' {
			$dbHost = 'uk-erm-edu03'
		}
		'Production' {
			$dbHost = 'uk-sql20\erm'
		}
		'Load' {
			$dbHost = 'uk-test-sql01\MSSQL2016'
		}
		'Appveyor' {
			$dbHost = '(local)\SQL2016'
		}
	}

	return @{ 'DBHost' = $dbHost }
}

function Get-AmsFactsTopicMetadata($Context){
	switch($Context.EnvType){
		'Test' {
			return @{
				'AmsFactsTopic' = 'ams_okapi_prod.am.validity'
			}
		 }
		'Business' {
			return @{ 'AmsFactsTopic' = "ams_okapi_business$($Context['Index']).am.validity" }
		}
		'Edu' {
			return @{ 'AmsFactsTopic' = "ams_okapi_edu$($Context['Index']).am.validity" }
		}
		 'Production' {
			 return @{
				 'AmsFactsTopic' = 'ams_okapi_prod.am.validity'
			 }
		}
		default {
			return @{}
		}
	}
}

function Get-RulesetsFactsTopicsMetadata($Context){
	switch($Context.EnvType){
		'Test' {
			if (($Context.RulesetsFactsTopic -ne $null) -And ($Context.RulesetsFactsTopic -ne "")){
				return @{
					'RulesetsFactsTopic' = $Context.RulesetsFactsTopic
				}
			}
			return @{
				'RulesetsFactsTopic' = 'casino_staging_flowRulesets_compacted'
			}
		 }
		'Business' {
			if ($Context['Index'] -eq '1'){
				return @{'RulesetsFactsTopic' = 'erm_business01_flowRulesets'}
			}
			return @{ 'RulesetsFactsTopic' = 'casino_staging_flowRulesets_compacted' }
		}
		'Edu' {
			return @{ 'RulesetsFactsTopic' = 'casino_staging_flowRulesets_compacted' }
		}
		 'Production' {
			 return @{
				 'RulesetsFactsTopic' = 'casino_staging_flowRulesets_compacted'
			 }
		}
		default {
			return @{}
		}
	}
}

function Get-ValidationUrlMetadata($Context){

	$domain = $DomainNames[$Context.Country]

	switch($Context.EnvType){
		'Production'{
			switch($Context.Country){
				'Russia' {
					$ermValidationUrl = "https://order-validation22.api.test.erm.2gis.ru/Validate.svc/Soap"
					$riverValidationUrl = "https://validation.api.prod.erm.2gis.ru"
				}
				'Kazakhstan'{
					$ermValidationUrl = "https://order-validation21.api.test.erm.2gis.ru/Validate.svc/Soap"
					$riverValidationUrl = "https://validation.api.prod.erm.2gis.kz"
				}
				default {
					$ermValidationUrl = $null
					$riverValidationUrl = $null
				}
			}
		}
		'Test'{
			$ermValidationUrl = "https://order-validation$($Context['Index']).api.test.erm.2gis.$domain/Validate.svc/Soap"
			$riverValidationUrl = "https://validation$($Context['Index']).api.test.erm.2gis.$domain"
		}
		default {
			$ermValidationUrl = $null
			$riverValidationUrl = $null
		}
	}

	return @{
		'ErmValidationUrl' = $ermValidationUrl
		'RiverValidationUrl' = $riverValidationUrl
	}
}

function Get-XdtMetadata($Context){
	$xdt = @()


	switch($Context.EntryPoint){
		'ConvertUseCasesService-Production' {
			$xdt += @("ConvertUseCases.Production.config")
		}
		'ConvertUseCasesService' {
			switch($Context.EnvType){
				'Test' {
					$xdt += @("Templates\ConvertUseCases.Test.config")
				}
				'Load' {
					$xdt += @("ConvertUseCases.Load.config")
				}
				default {
					$xdt += @("ConvertUseCases.config")
				}
			}
		}
		default {
			$xdt += @(
				'Common\Erm.Release.config'
			)

			switch($Context.EnvType){
				'Test' {
					$xdt += @("Templates\Erm.Test.config")
				}
				'Production' {
					$xdt += @("Erm.Production.config")
				}
				'Load' {
					$xdt += @("Erm.Load.config")
				}
				default {
					$xdt += @("Erm.config")
				}
			}
		}
	}

	return $xdt
}

function Get-RegexMetadata($Context){

	$regex = @{}

	if ($Context['Index']){
		$regex += @{ '{EnvNum}' = $Context['Index'] }
	}
	if ($Context['Country']){
		$regex += @{ '{Country}' = $Context['Country'] }
		$regex += @{ '{DBSuffix}' = (Get-DBSuffix $Context) }
	}
	if ($Context['EnvType']){
		$regex += @{ '{EnvType}' = $Context['EnvType'] }
	}

	$serviceBusMetadata = (Get-ServiceBusMetadata $Context)['ServiceBus']
	if ($serviceBusMetadata.Count -ne 0){
		if ($serviceBusMetadata['CreateTopics']){
			foreach($metadata in $serviceBusMetadata.CreateTopics.GetEnumerator()){
				$regex["{$($metadata.Key)}"] = $metadata.Value.Name
			}
		}

		if ($serviceBusMetadata['CreateSubscriptions']){
			foreach($metadata in $serviceBusMetadata.CreateSubscriptions.GetEnumerator()){
				$regex["{$($metadata.Key)}"] = $metadata.Value.Name
			}
		}
	}

	$keyValuePairs = @{}
	$keyValuePairs += Get-DBHostMetadata $Context
	$keyValuePairs += Get-ValidationUrlMetadata $Context
	$keyValuePairs += Get-AmsFactsTopicMetadata $Context
	$keyValuePairs += Get-RulesetsFactsTopicsMetadata $Context

	foreach($keyValuePair in $keyValuePairs.GetEnumerator()){
		$regex["{$($keyValuePair.Key)}"] = $keyValuePair.Value
	}

	return $regex
}

function Get-TransformMetadata ($Context) {

	return @{
		'Transform' = @{
			'Xdt' = Get-XdtMetadata $Context
			'Regex' = Get-RegexMetadata $Context
		}
	}
}

Export-ModuleMember -Function Get-TransformMetadata -Variable DomainNames