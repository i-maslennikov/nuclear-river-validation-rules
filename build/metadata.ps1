param ([hashtable]$Properties)

Import-Module "$PSScriptRoot\metadata.web.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\metadata.winservice.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\metadata.transform.psm1" -DisableNameChecking
Import-Module "$PSScriptRoot\metadata.nunit.psm1" -DisableNameChecking

function Get-EntryPointsMetadata ($EntryPoints, $Context) {

	$entryPointsMetadata = @{}

	# конвертер нужен всегда (очистка ресурсов)
	$Context.EntryPoint = 'ConvertUseCasesService'
	$entryPointsMetadata += Get-WinServiceMetadata $Context

	# production копия конвертера нужна всегда (очистка ресурсов)
	$productionContext = $Context.Clone()
	$productionContext.EnvType = 'Production' 
	$productionContext.EntryPoint = 'ConvertUseCasesService-Production'
	$entryPointsMetadata += Get-WinServiceMetadata $productionContext 

	switch ($EntryPoints){
		'ValidationRules.Querying.Host' {
			$Context.EntryPoint = $_
			$entryPointsMetadata += Get-WebMetadata $Context
		}

		'ValidationRules.Replication.Host' {
			$Context.EntryPoint = $_
			$entryPointsMetadata += Get-WinServiceMetadata $Context
		}

		'ValidationRules.Replication.Comparison.Tests' {
			$Context.EntryPoint = $_
			$entryPointsMetadata += Get-AssemblyMetadata $Context
		}

        'ValidationRules.Replication.StateInitialization.Tests' {
			$Context.EntryPoint = $_
			$entryPointsMetadata += Get-AssemblyMetadata $Context
		}

		default {
			throw "Can't find entrypoint $_"
		}
	}

	return $entryPointsMetadata
}

function Get-BulkToolMetadata ($updateSchemas, $Context){
	$metadata = @{}

	$arguments = @()
    if($updateSchemas -contains 'Facts') {
        $arguments += @('-facts', '-aggregates', '-messages')
    }
    if($updateSchemas -contains 'Aggregates') {
        $arguments += @('-aggregates', '-messages')
    }
    if($updateSchemas -contains 'Messages') {
        $arguments += @('-messages')
    }

	$metadata += @{ 'Arguments' = ($arguments | select -Unique) }

	$Context.EntryPoint = 'ValidationRules.StateInitialization.Host'
	$metadata += Get-TransformMetadata $Context

	return @{ 'ValidationRules.StateInitialization.Host' = $metadata }
}

function Get-NuGetMetadata {
	return @{
		'NuGet' = @{
			'Publish' = @{
				'Source' = 'https://www.nuget.org/api/v2/package'
				'PrereleaseSource' = 'http://nuget.2gis.local/api/v2/package'

				'SymbolSource'= 'https://nuget.smbsrc.net/api/v2/package'
				'PrereleaseSymbolSource' = 'http://nuget.2gis.local/SymbolServer/NuGet'
			}
		}
	}
}

function Parse-EnvironmentMetadata ($Properties) {

	$environmentMetadata = @{}

    if($Properties['BuildSystem']) {
		$buildSystem = $Properties.BuildSystem
    } else {
		$buildSystem = 'Local'
	}

	$environmentMetadata += @{ 'BuildSystem' = $buildSystem } 
	$environmentMetadata += Get-NuGetMetadata

	$environmentName = $Properties['EnvironmentName']
	if (!$environmentName){
		return $environmentMetadata
	}

    $environmentMetadata += @{ 'Common' = @{ 'EnvironmentName' = $environmentName } }

	$context = $AllEnvironments[$environmentName]
	if ($context -eq $null){
		throw "Can't find environment for name '$environmentName'"
	}
	$context.EnvironmentName = $environmentName

	$context.UseCaseRoute = $Properties['UseCaseRoute']
	$context.RulesetsFactsTopic = $Properties['RulesetsFactsTopic']

	if ($Properties.ContainsKey('EntryPoints')){
		$entryPoints = $Properties['EntryPoints']
	
		if ($entryPoints -and $entryPoints -isnot [array]){
			$entryPoints = $entryPoints.Split(@(','), 'RemoveEmptyEntries')
		}

	} else {
		$entryPoints = $AllEntryPoints
	}

	$environmentMetadata += Get-EntryPointsMetadata $entryPoints $context

	$updateSchemas = $Properties['UpdateSchemas']
	if ($updateSchemas){
		if ($updateSchemas -isnot [array]){
			$updateSchemas = $updateSchemas.Split(@(','), 'RemoveEmptyEntries')
		}

		$environmentMetadata += Get-BulkToolMetadata $updateSchemas $context
		$environmentMetadata += @{ 'UpdateSchemas' = $true }
	}

	return $environmentMetadata
}

$AllEntryPoints = @(
	'ValidationRules.Querying.Host'
	'ValidationRules.Replication.Host'
	'ConvertUseCasesService'
)

$AllEnvironments = @{

	'Edu.Chile' = @{ EnvType = 'Edu'; Country = 'Chile' }
	'Edu.Cyprus' = @{ EnvType = 'Edu'; Country = 'Cyprus' }
	'Edu.Czech' = @{ EnvType = 'Edu'; Country = 'Czech' }
	'Edu.Emirates' = @{ EnvType = 'Edu'; Country = 'Emirates' }
	'Edu.Kazakhstan' = @{ EnvType = 'Edu'; Country ='Kazakhstan' }
	'Edu.Kyrgyzstan' = @{ EnvType = 'Edu'; Country ='Kyrgyzstan' }
	'Edu.Russia' = @{ EnvType = 'Edu'; Country = 'Russia' }
	'Edu.Ukraine' = @{ EnvType = 'Edu'; Country = 'Ukraine' }

	'Business.Russia' = @{ EnvType = 'Business'; Country = 'Russia' }
	'Business.Russia.01' = @{ EnvType = 'Business'; Country = 'Russia'; Index = '01'}
	
	'Int.Chile' = @{ EnvType = 'Int'; Country = 'Chile' }
	'Int.Cyprus' = @{ EnvType = 'Int'; Country = 'Cyprus' }
	'Int.Czech' = @{ EnvType = 'Int'; Country = 'Czech' }
	'Int.Emirates' = @{ EnvType = 'Int'; Country = 'Emirates' }
	'Int.Kazakhstan' = @{ EnvType = 'Int'; Country = 'Kazakhstan' }
	'Int.Kyrgyzstan' = @{ EnvType = 'Int'; Country = 'Kyrgyzstan' }
	'Int.Russia' = @{ EnvType = 'Int'; Country = 'Russia' }
	'Int.Ukraine' = @{ EnvType = 'Int'; Country = 'Ukraine' }
	
	'Load.Russia' = @{ EnvType = 'Load'; Country = 'Russia' }
	'Load.Cyprus' = @{ EnvType = 'Load'; Country = 'Cyprus' }
	'Load.Czech' = @{ EnvType = 'Load'; Country = 'Czech' }
	'Load.Chile' = @{ EnvType = 'Load'; Country = 'Chile' }
	'Load.Emirates' = @{ EnvType = 'Load'; Country = 'Emirates' }
	'Load.Kazakhstan' = @{ EnvType = 'Load'; Country = 'Kazakhstan' }
	'Load.Kyrgyzstan' = @{ EnvType = 'Load'; Country = 'Kyrgyzstan' }
	'Load.Ukraine' = @{ EnvType = 'Load'; Country = 'Ukraine' }

	'Production.Chile' = @{ EnvType = 'Production'; Country = 'Chile' }
	'Production.Cyprus' = @{ EnvType = 'Production'; Country = 'Cyprus' }
	'Production.Czech' = @{ EnvType = 'Production'; Country = 'Czech' }
	'Production.Emirates' = @{ EnvType = 'Production'; Country = 'Emirates' }
	'Production.Kazakhstan' = @{ EnvType = 'Production'; Country = 'Kazakhstan' }
	'Production.Russia' = @{ EnvType = 'Production'; Country = 'Russia' }
	'Production.Ukraine' = @{ EnvType = 'Production'; Country = 'Ukraine' }
	'Production.Kyrgyzstan' = @{ EnvType = 'Production'; Country = 'Kyrgyzstan' }
	
	'Test.01' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '01' }
	'Test.02' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '02' }
	'Test.03' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '03' }
	'Test.04' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '04' }
	'Test.05' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '05' }
	'Test.06' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '06' }

	'Test.07' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '07' }
	'Test.08' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '08' }
	'Test.09' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '09' }
	'Test.10' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '10' }
	'Test.11' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '11' }
	'Test.12' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '12' }
	'Test.13' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '13' }
	'Test.14' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '14' }
	'Test.15' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '15' }
	'Test.16' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '16' }
	'Test.17' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '17' }
	'Test.18' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '18' }
	'Test.19' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '19' }
	'Test.20' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '20' }
	'Test.21' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '21' }
	'Test.22' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '22' }
	'Test.23' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '23' }
	'Test.24' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '24' }
	'Test.25' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '25' }
	'Test.26' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '26' }
	'Test.27' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '27' }
	'Test.28' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '28' }
	'Test.29' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '29' }
	'Test.30' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '30' }
	'Test.31' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '31' }
	'Test.32' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '32' }
	'Test.33' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '33' }
	'Test.34' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '34' }
	'Test.35' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '35' }
	'Test.88' = @{ EnvType = 'Test'; Country = 'Russia'; Index = '88' }

	'Test.101' = @{ EnvType = 'Test'; Country = 'Cyprus'; Index = '101' }
	'Test.102' = @{ EnvType = 'Test'; Country = 'Cyprus'; Index = '102' }
	'Test.103' = @{ EnvType = 'Test'; Country = 'Cyprus'; Index = '103' }
	'Test.104' = @{ EnvType = 'Test'; Country = 'Cyprus'; Index = '104' }
	'Test.105' = @{ EnvType = 'Test'; Country = 'Cyprus'; Index = '105' }
	'Test.106' = @{ EnvType = 'Test'; Country = 'Cyprus'; Index = '106' }
	'Test.107' = @{ EnvType = 'Test'; Country = 'Cyprus'; Index = '107' }
	'Test.108' = @{ EnvType = 'Test'; Country = 'Cyprus'; Index = '108' }
	
	'Test.201' = @{ EnvType = 'Test'; Country = 'Czech'; Index = '201' }
	'Test.202' = @{ EnvType = 'Test'; Country = 'Czech'; Index = '202' }
	'Test.203' = @{ EnvType = 'Test'; Country = 'Czech'; Index = '203' }
	'Test.204' = @{ EnvType = 'Test'; Country = 'Czech'; Index = '204' }
	'Test.205' = @{ EnvType = 'Test'; Country = 'Czech'; Index = '205' }
	'Test.206' = @{ EnvType = 'Test'; Country = 'Czech'; Index = '206' }
	'Test.207' = @{ EnvType = 'Test'; Country = 'Czech'; Index = '207' }
	'Test.208' = @{ EnvType = 'Test'; Country = 'Czech'; Index = '208' }
	
	'Test.301' = @{ EnvType = 'Test'; Country = 'Chile'; Index = '301' }
	'Test.302' = @{ EnvType = 'Test'; Country = 'Chile'; Index = '302' }
	'Test.303' = @{ EnvType = 'Test'; Country = 'Chile'; Index = '303' }
	'Test.304' = @{ EnvType = 'Test'; Country = 'Chile'; Index = '304' }
	'Test.305' = @{ EnvType = 'Test'; Country = 'Chile'; Index = '305' }
	'Test.306' = @{ EnvType = 'Test'; Country = 'Chile'; Index = '306' }
	'Test.307' = @{ EnvType = 'Test'; Country = 'Chile'; Index = '307' }
	'Test.308' = @{ EnvType = 'Test'; Country = 'Chile'; Index = '308' }
	'Test.320' = @{ EnvType = 'Test'; Country = 'Chile'; Index = '320' }
	
	'Test.401' = @{ EnvType = 'Test'; Country = 'Ukraine'; Index = '401' }
	'Test.402' = @{ EnvType = 'Test'; Country = 'Ukraine'; Index = '402' }
	'Test.403' = @{ EnvType = 'Test'; Country = 'Ukraine'; Index = '403' }
	'Test.404' = @{ EnvType = 'Test'; Country = 'Ukraine'; Index = '404' }
	'Test.405' = @{ EnvType = 'Test'; Country = 'Ukraine'; Index = '405' }
	'Test.406' = @{ EnvType = 'Test'; Country = 'Ukraine'; Index = '406' }
	'Test.407' = @{ EnvType = 'Test'; Country = 'Ukraine'; Index = '407' }
	'Test.408' = @{ EnvType = 'Test'; Country = 'Ukraine'; Index = '408' }
	
	'Test.501' = @{ EnvType = 'Test'; Country = 'Emirates'; Index = '501' }
	'Test.502' = @{ EnvType = 'Test'; Country = 'Emirates'; Index = '502' }
	'Test.503' = @{ EnvType = 'Test'; Country = 'Emirates'; Index = '503' }
	'Test.504' = @{ EnvType = 'Test'; Country = 'Emirates'; Index = '504' }
	'Test.505' = @{ EnvType = 'Test'; Country = 'Emirates'; Index = '505' }
	'Test.506' = @{ EnvType = 'Test'; Country = 'Emirates'; Index = '506' }
	'Test.507' = @{ EnvType = 'Test'; Country = 'Emirates'; Index = '507' }
	'Test.508' = @{ EnvType = 'Test'; Country = 'Emirates'; Index = '508' }
	
	'Test.601' = @{ EnvType = 'Test'; Country = 'Kazakhstan'; Index = '601' }
	'Test.602' = @{ EnvType = 'Test'; Country = 'Kazakhstan'; Index = '602' }
	'Test.603' = @{ EnvType = 'Test'; Country = 'Kazakhstan'; Index = '603' }
	'Test.604' = @{ EnvType = 'Test'; Country = 'Kazakhstan'; Index = '604' }
	'Test.605' = @{ EnvType = 'Test'; Country = 'Kazakhstan'; Index = '605' }
	'Test.606' = @{ EnvType = 'Test'; Country = 'Kazakhstan'; Index = '606' }
	'Test.607' = @{ EnvType = 'Test'; Country = 'Kazakhstan'; Index = '607' }
	'Test.608' = @{ EnvType = 'Test'; Country = 'Kazakhstan'; Index = '608' }

	'Test.701' = @{ EnvType = 'Test'; Country = 'Kyrgyzstan'; Index = '701' }
	'Test.702' = @{ EnvType = 'Test'; Country = 'Kyrgyzstan'; Index = '702' }
	'Test.703' = @{ EnvType = 'Test'; Country = 'Kyrgyzstan'; Index = '703' }
	'Test.704' = @{ EnvType = 'Test'; Country = 'Kyrgyzstan'; Index = '704' }
	'Test.705' = @{ EnvType = 'Test'; Country = 'Kyrgyzstan'; Index = '705' }
	'Test.706' = @{ EnvType = 'Test'; Country = 'Kyrgyzstan'; Index = '706' }
	'Test.707' = @{ EnvType = 'Test'; Country = 'Kyrgyzstan'; Index = '707' }
	'Test.708' = @{ EnvType = 'Test'; Country = 'Kyrgyzstan'; Index = '708' }
		
	'Appveyor' = @{ EnvType = 'Appveyor'; Country = 'Russia' }
}

return Parse-EnvironmentMetadata $Properties