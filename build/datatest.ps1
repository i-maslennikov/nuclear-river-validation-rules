Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
#Requires –Version 3.0
#------------------------------

Import-Module "$BuildToolsRoot\modules\nuget.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\msbuild.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\buildqueue.psm1" -DisableNameChecking
Import-Module "$BuildToolsRoot\modules\metadata.psm1" -DisableNameChecking

$RunnerPackageInfo = Get-PackageInfo '2Gis.NuClear.DataTest'
$RunnerPath = Join-Path $RunnerPackageInfo.VersionedDir "tools\2Gis.NuClear.DataTest.Runner.exe"

function Run-DataTests ($Projects, $entryPointMetadataKey){

    if ($Projects -eq $null){
        return
    }

    $testLocations = @{}

    foreach($project in $Projects){

        $projectName = [System.IO.Path]::GetFileNameWithoutExtension($Project.Name)
        $outDir = Join-Path $Metadata.Common.Dir.TempPersist "$entryPointMetadataKey\$projectName"

        $buildFileName = Create-BuildFile $project.FullName -Properties @{
            'OutDir' = $outDir
        }
        Add-MSBuildQueue $buildFileName $outDir $entryPointMetadataKey

        $testLocations[$projectName] = $outDir
    }

    Invoke-MSBuildQueue -PublishArtifacts $false

    $assemblies = @()
    foreach($testLocation in $testLocations.GetEnumerator()){
        $testAssemblies = Get-ChildItem $testLocation.Value -Filter "*$($testLocation.Key)*.dll"
        foreach($testAssembly in $testAssemblies){
            $assemblies += $testAssembly.FullName
        }
    }
    
    $buildSystem = $Metadata['BuildSystem']
    switch($buildSystem) {
        'TeamCity' {
            $argumentList = ($assemblies, '--teamcity=true')
        }
        'Jenkins' {
            $ouputFile = Join-Path $Metadata.Common.Dir.TempPersist "DataTest.xml"
            $argumentList = ($assemblies, "--nunit25-output=$ouputFile")
            Write-Host "Results (nunit2.5) will be saved as $ouputFile"
        }
        default {
            if($buildSystem) {
                Write-Host "WARNING: unknown build system '$buildSystem' running datatests as local"
            }
            $argumentList = ($assemblies)
        }
    }

    & $RunnerPath $argumentList

    if ($lastExitCode -ne 0) {
        throw "Command failed with exit code $lastExitCode"
    }
}