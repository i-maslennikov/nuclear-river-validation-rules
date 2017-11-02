Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$BuildToolsRoot\modules\unittests.psm1" -DisableNameChecking

Task Run-DatabaseComparisonTests {
	$project = Find-Projects '.' -Filter 'ValidationRules.Replication.DatabaseComparison.Tests.csproj'
	Run-UnitTests $project 'UnitTests'
}