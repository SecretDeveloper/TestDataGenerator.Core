# Run mstest against all *Tests.dll within the Assemblies\TestOutput\ directory

# Get the PWD - should be the Development\ReleaseBranch folder
$basePath = Get-Location
$trxPath = "$basePath\src\TestOutput\AllTest.trx"

if(test-path $trxPath){
    Remove-Item $trxPath
}

#Configure result file
$resultFile="/resultsfile:$trxPath"

# grab the dlls
$testDLLs = get-childitem -path "$basePath\src\TestOutput\*.*" -include "*Tests.dll"
write-host $testDLLs

$fs = New-Object -ComObject Scripting.FileSystemObject
$f = $fs.GetFile("C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\mstest.exe")
$mstestPath = $f.shortpath   

for ($i=0; $i -lt $testDLLs.Count; $i++) {
    $f = $testDLLs[$i]
    write-host "Adding $f tests"
    #$content = Get-Content $f
    $arguments = $arguments + " /testcontainer:" + $f
}

write-host "Running tests"
Invoke-Expression "$mstestPath $resultFile $arguments > LogTest.log"

Foreach ($line in (Get-Content -Path "LogTest.log" | Where {$_ -match '^Failed'})) 
{ 
    write-host $line
}
write-host "Finished tests"




Write-Host Generating README.md from template.
./src/buildoutput/tdg.exe -i:".\src\templates\README.template" -o:"./README.md"
Write-Host Finished
