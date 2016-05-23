Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$BuildToolsRoot\modules\entrypoint.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\transform.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\sql.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking

Task Update-Schemas -Precondition { $Metadata['ValidationRules.StateInitialization.Host'] -and $Metadata['UpdateSchemas'] } {

	$projectFileName = Get-ProjectFileName 'ValidationRules' 'ValidationRules.StateInitialization.Host'
	$projectDir = Split-Path $projectFileName
	$configFileName = Join-Path $projectDir 'app.config'
	[xml]$config = Get-TransformedConfig $configFileName 'ValidationRules.StateInitialization.Host'

	Update-Schemas $config
}

function Update-Schemas ($config) {
	$updateSchemasMetadata = $Metadata['UpdateSchemas']

	foreach ($schema in $updateSchemasMetadata.Values) {

		$connectionString = Get-ConnectionString $config $schema.ConnectionStringKey
		Write-Host $connectionString
		$connection = Create-SqlConnection $connectionString

		$sqlFilePath = Join-Path $Metadata.Common.Dir.Solution $schema.SqlFile
		$sql = Get-Content $sqlFilePath -Raw

		Write-Host ((Split-Path $sqlFilePath -Leaf) + '...')
		Execute-Sql $sql $connection
	}
}