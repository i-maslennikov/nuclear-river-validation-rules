Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$PSScriptRoot\metadata.usecaseroute.psm1" -DisableNameChecking

$DBSuffixes = @{
	'Chile' = 'CL'
	'Cyprus' = 'CY'
	'Czech' = 'CZ'
	'Emirates' = 'AE'
	'Russia' = 'RU'
	'Ukraine' = 'UA'
	'Kazakhstan' = 'KZ'
	'Kyrgyzstan' = 'KG'
	'Italy' = 'IT'
}

function Get-DBHostMetadata($Context){
	switch($Context.EnvType){
		'Test' {
			switch($Context.Country){
				'Russia' { $dbHost = 'uk-sql01' }
				default { $dbHost = 'uk-erm-sql02' }
			}
		}
		'Edu' {
			$dbHost = 'uk-erm-edu01'
		}
		'Business' {
			$dbHost = 'uk-erm-edu02'
		}
		'Production' {
			$dbHost = 'uk-sql20\erm'
		}
	}

	return @{ 'DBHost' = $dbHost }
}

function Get-XdtMetadata($Context){
	$xdt = @()


	switch($Context.EntryPoint){
		'ConvertUseCasesService' {
			switch($Context.EnvType){
				'Test' {
					$xdt += @("Templates\ConvertUseCases.Test.config")
				}
				'Production' {
					$xdt += @("ConvertUseCases.Production.config")
				}
				default {
					$xdt += @("ConvertUseCases.config")
				}
			}
		}
		default {
			$xdt += @(
				'Common\log4net.Release.config'
				'Common\Erm.Release.config'
			)

			switch($Context.EnvType){
				'Test' {
					$xdt += @("Templates\Erm.Test.config")
				}
				'Production' {
					$xdt += @("Erm.Production.config")
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
		$regex += @{ '{DBSuffix}' = $DBSuffixes[$Context['Country']] }
	}
	if ($Context['EnvType']){
		$regex += @{ '{EnvType}' = $Context['EnvType'] }
	}

	$useCaseRouteMetadata = Get-UseCaseRouteMetadata $Context
	if ($useCaseRouteMetadata.Count -ne 0){
		foreach($keyValuePair in $useCaseRouteMetadata.UseCaseRoute.GetEnumerator()){
			$regex["{$($keyValuePair.Key)}"] = $keyValuePair.Value
		}
	}

	$dbHostMetadata = Get-DBHostMetadata $Context
	foreach($keyValuePair in $dbHostMetadata.GetEnumerator()){
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

Export-ModuleMember -Function Get-TransformMetadata