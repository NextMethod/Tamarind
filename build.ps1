Param(
    [string] $Script = "build.csx",
    [string] $Target = "Default",
    [string] $Configuration = "Debug",
    [string] $Verbosity = "Verbose"
)

$TOOLS_DIR = [io.path]::combine($PSScriptRoot, "tools")
$NUGET_EXE = [io.path]::combine($TOOLS_DIR, "nuget.exe")
$CAKE_EXE = [io.path]::combine($TOOLS_DIR, "Cake", "Cake.exe")

if (!(Test-Path $NUGET_EXE)) {
    Throw "Could not find " + $NUGET_EXE
}

Invoke-Expression "$NUGET_EXE install xunit.runners -OutputDirectory $TOOLS_DIR -ExcludeVersion -Prerelease"
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

Invoke-Expression "$NUGET_EXE install Cake -OutputDirectory $TOOLS_DIR -ExcludeVersion"
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

if (!(Test-Path $CAKE_EXE)) {
    Throw "Could not find " + $CAKE_EXE
}

Invoke-Expression "$CAKE_EXE `"$Script`" -target=`"$Target`" -configuration=`"$Configuration`" -verbosity=`"$Verbosity`""

exit $LASTEXITCODE
