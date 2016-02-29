Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#------------------------------

Import-Module "$PSScriptRoot\metadata.usecaseroute.psm1" -DisableNameChecking

function Get-XdtMetadata($Context){
	$xdt = @()


	switch($Context.EntryPoint){
		'ConvertUseCasesService' {
			switch($Context.EnvType){
				'Test' {
					$xdt += @("Templates\ConvertUseCases.$($Context.EnvType).MultiCulture.config")
				}
				default {
					$xdt += @("ConvertUseCases.$($Context.EnvType).MultiCulture.config")
				}
			}
		}
		'ConvertUseCasesServiceProduction' {
			$xdt += @("ConvertUseCases.Production.MultiCulture.config")
		}
		default {
			$xdt += @(
				'Common\log4net.Release.config'
				'Common\Erm.Release.config'
			)

			switch($Context.EnvType){
				'Test' {
					$xdt += @("Templates\Erm.$($Context.EnvType).$($Context.Country).config")
				}
				default {
					$xdt += @("Erm.$($Context.EnvType).$($Context.Country).config")
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
	}

	$useCaseRouteMetadata = Get-UseCaseRouteMetadata $Context
	if ($useCaseRouteMetadata.Count -ne 0){
		foreach($keyValuePair in $useCaseRouteMetadata.UseCaseRoute.GetEnumerator()){
			$regex["{$($keyValuePair.Key)}"] = $keyValuePair.Value
		}
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