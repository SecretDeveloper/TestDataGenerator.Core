param(
    $buildType = "Release"
)


function clean{
    # CLEAN
    write-host "Cleaning" -foregroundcolor:blue
    if(!(Test-Path "$basePath\src\BuildOutput\"))
    {
        mkdir "$basePath\src\BuildOutput\"
    }
    if(!(Test-Path "$logPath"))
    {
        mkdir "$logPath"
    }
    if(!(Test-Path "$basePath\src\TestOutput\"))
    {
        mkdir "$basePath\src\TestOutput\"
    }    
    remove-item $basePath\src\BuildOutput\*.* -recurse
    remove-item $basePath\src\TestOutput\* -recurse
    if((Test-Path "$basePath\src\DataGenerator\TestResults\"))
    {
        remove-item $basePath\src\DataGenerator\TestResults -recurse
    }
    remove-item $logPath\* -recurse
    $lastResult = $true
}

function build{
    # BUILD
    write-host "Building"  -foregroundcolor:blue
    $msbuild = "c:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
    $solutionPath = "$basePath\src\DataGenerator\DataGenerator.sln"
    Invoke-expression "$msbuild $solutionPath /p:configuration=$buildType /t:Clean /t:Build /verbosity:q /nologo > $logPath\LogBuild.log"

    if($? -eq $False){
        Write-host "BUILD FAILED!"
        exit
    }
    
    $content = (Get-Content -Path "$logPath\LogBuild.log")
    $failedContent = ($content -match "error")
    $failedCount = $failedContent.Count
    if($failedCount -gt 0)
    {    
        Write-host "BUILDING FAILED!" -foregroundcolor:red
        $lastResult = $false
        
        Foreach ($line in $content) 
        {
            write-host $line -foregroundcolor:red
        }
    }

    if($lastResult -eq $False){    
        exit
    } 
}

function test{
    # TESTING
    write-host "Testing"  -foregroundcolor:blue

    $trxPath = "$basePath\src\TestOutput\AllTest.trx"
    $resultFile="/resultsfile:$trxPath"

    $testDLLs = get-childitem -path "$basePath\src\TestOutput\*.*" -include "*Tests.dll"
     
    $arguments = " /testcontainer:" + $testDLLs + " /TestSettings:$basePath\src\DataGenerator\LocalTestRun.testrunconfig"

    Invoke-Expression "mstest $resultFile $arguments > $logPath\LogTest.log"

    $content = (Get-Content -Path "$logPath\LogTest.log")
    $passedContent = ($content -match "Passed")
    if($passedContent.Count -eq 0)
    {    
        Write-host "TESTING FAILED!" -foregroundcolor:red
        $lastResult = $false
    }
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
}

function document{
    # DOCUMENTING
    Write-Host "Documenting" -foregroundcolor:blue
    Invoke-expression "./src/buildoutput/tdg.exe -i '.\src\templates\README.template.md' -o './README.md'"
    if($? -eq $False){
        Write-host "DOCUMENT FAILED!"  -foregroundcolor:red
        exit
    }
    Invoke-expression "./src/buildoutput/tdg.exe -i '.\src\templates\README.template.txt' -o '.\src\buildoutput\README.txt'"    
    if($? -eq $False){
        Write-host "DOCUMENT FAILED!"  -foregroundcolor:red
        exit
    }
}

function pack{
    # Packing
    write-host "Packing" -foregroundcolor:blue
    nuget pack .\src\DataGenerator\gk.DataGenerator.CommandLine\gk.DataGenerator.tdg.csproj -OutputDirectory .\releases > $logPath\LogPacking.log     
    if($? -eq $False){
        Write-host "PACK FAILED!"  -foregroundcolor:red
        exit
    }
}

function deploy{
    # DEPLOYING
    write-host "Deploying" -foregroundcolor:blue
    $outputName = $projectName+"_V"+$buildVersion+"_BUILD.zip"
    zip a -tzip .\src\buildoutput\$outputName .\src\BuildOutput\*.* > $logPath\LogDeploy.log

}

$basePath = Get-Location
$logPath = "$basePath\src\logs"
$buildVersion = Get-Content .\VERSION
$projectName = "TestDataGenerator"

if($buildType -eq "package"){
    
    $buildType="Release"

    clean
    build
    test
    document
    pack
    deploy

    exit
}
if($buildType -eq "clean"){
    
    clean  
    exit
}
if($buildType -eq "document"){
    
    document
    exit
}
else {
    clean
    build
    test    
    document
}
Write-Host Finished -foregroundcolor:blue

