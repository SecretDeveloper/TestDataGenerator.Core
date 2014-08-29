
# Run mstest against all *Tests.dll within the Assemblies\TestOutput\ directory

# Get the PWD - should be the Development\ReleaseBranch folder
$basePath = Get-Location

# CLEAN
write-host "Cleaning" -foregroundcolor:blue
remove-item $basePath\src\BuildOutput\*.* -recurse
remove-item $basePath\src\TestOutput\* -recurse
remove-item $basePath\src\DataGenerator\TestResults\* -recurse


# BUILD
write-host "Building"  -foregroundcolor:blue
$msbuild = "c:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
$solutionPath = "$basePath\src\DataGenerator\DataGenerator.sln"
Invoke-expression "$msbuild $solutionPath /p:configuration=Release /t:Clean /t:Build /verbosity:q /nologo"
$lastResult = $?
if($lastResult -eq $False){
    Write-host "BUILD FAILED!"
    exit
}

# TESTING
write-host "Testing"  -foregroundcolor:blue
$trxPath = "$basePath\src\TestOutput\AllTest.trx"
$resultFile="/resultsfile:$trxPath"

$testDLLs = get-childitem -path "$basePath\src\TestOutput\*.*" -include "*Tests.dll"
 
$fs = New-Object -ComObject Scripting.FileSystemObject
$f = $fs.GetFile("C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\mstest.exe")
$mstestPath = $f.shortpath   
$arguments = " /testcontainer:" + $testDLLs + " /TestSettings:$basePath\src\DataGenerator\LocalTestRun.testrunconfig"

Invoke-Expression "$mstestPath $resultFile $arguments > LogTest.log"

$content = (Get-Content -Path "LogTest.log")
$failedContent = ($content -match "^Failed")
$failedCount = $failedContent.Count
if($failedCount -gt 0)
{    
    Write-host "TESTING FAILED!" -foregroundcolor:red
    $lastResult = $false
}
Foreach ($line in $failedContent) 
{
    write-host $line -foregroundcolor:red
}
if($lastResult -eq $False){    
    exit
}

# DOCUMENTING
Write-Host "Documenting" -foregroundcolor:blue
./src/buildoutput/tdg.exe -i:".\src\templates\README.template" -o:"./README.md"
Write-Host Finished -foregroundcolor:blue
