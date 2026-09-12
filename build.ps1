$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $scriptDir

Write-Host "=========================================================================" -ForegroundColor Cyan
Write-Host "         OpenTabletDriver Chinese Localization Build & Packaging" -ForegroundColor Green
Write-Host "=========================================================================" -ForegroundColor Cyan

# 1. Prepare dist directory
$distDir = Join-Path $scriptDir "dist"
if (Test-Path $distDir) {
    Remove-Item $distDir -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $distDir | Out-Null

$zipTempDir = Join-Path $distDir "ChineseLocalization"
New-Item -ItemType Directory -Force -Path $zipTempDir | Out-Null

# 2. Build Launcher
Write-Host "`n[1/4] Building WinExe Launcher (OTD-Launcher)..." -ForegroundColor Yellow
$launcherProj = Join-Path $scriptDir "src\OTD-Launcher\OTD-Launcher.csproj"
dotnet publish $launcherProj -c Release -r win-x64 --no-self-contained | Out-Null
$launcherOutput = Join-Path $scriptDir "src\OTD-Launcher\bin\Release\net8.0-windows\win-x64\publish\OTD-Launcher.exe"

# 3. Build UI Hook
Write-Host "[2/4] Building UI Hook (ChineseLocalizationHook)..." -ForegroundColor Yellow
$hookProj = Join-Path $scriptDir "src\ChineseLocalizationHook\ChineseLocalizationHook.csproj"
dotnet build $hookProj -c Release | Out-Null
$hookOutput = Join-Path $scriptDir "src\ChineseLocalizationHook\bin\Release\net8.0\ChineseLocalizationHook.dll"

# 4. Build Plugin
Write-Host "[3/4] Building Plugin (ChineseLocalizationPlugin)..." -ForegroundColor Yellow
$pluginProj = Join-Path $scriptDir "src\ChineseLocalizationPlugin\ChineseLocalizationPlugin.csproj"
dotnet build $pluginProj -c Release | Out-Null
$pluginOutput = Join-Path $scriptDir "src\ChineseLocalizationPlugin\bin\Release\net8.0\ChineseLocalizationPlugin.dll"

# 5. Assemble and Package
Write-Host "[4/4] Assembling and Packaging Release Files..." -ForegroundColor Yellow
$metaSrc = Join-Path $scriptDir "src\ChineseLocalizationPlugin\metadata.json"
$installBatSrc = Join-Path $scriptDir "install.bat"

# Copy to dist root
Copy-Item $pluginOutput (Join-Path $distDir "ChineseLocalization.dll") -Force
Copy-Item $hookOutput (Join-Path $distDir "ChineseLocalizationHook.dll") -Force
Copy-Item $launcherOutput (Join-Path $distDir "OpenTabletDriver.UX.Wpf.exe") -Force
Copy-Item $metaSrc (Join-Path $distDir "metadata.json") -Force
if (Test-Path $installBatSrc) {
    Copy-Item $installBatSrc (Join-Path $distDir "install.bat") -Force
}

# Copy to zip temp directory (pure standard OTD plugin package)
Copy-Item $pluginOutput (Join-Path $zipTempDir "ChineseLocalization.dll") -Force
Copy-Item $hookOutput (Join-Path $zipTempDir "ChineseLocalizationHook.dll") -Force
Copy-Item $metaSrc (Join-Path $zipTempDir "metadata.json") -Force

# Create ZIP archive
$zipFile = Join-Path $distDir "ChineseLocalization.zip"
if (Test-Path $zipFile) { Remove-Item $zipFile -Force }
Compress-Archive -Path "$zipTempDir\*" -DestinationPath $zipFile -Force
Remove-Item $zipTempDir -Recurse -Force

# Calculate SHA256 and generate ChineseLocalization.json for official OTD repository PR
$hash = (Get-FileHash $zipFile -Algorithm SHA256).Hash.ToLower()

$officialMeta = [ordered]@{
    Name = "ChineseLocalization"
    Owner = "LinHouYu"
    Description = "OpenTabletDriver 简体中文界面汉化增强插件 (Chinese Localization Plugin) by LinHouYu"
    PluginVersion = "1.0.0"
    SupportedDriverVersion = "0.6.0.0"
    RepositoryUrl = "https://github.com/LinHouYu/OTD_chineses"
    DownloadUrl = "https://github.com/LinHouYu/OTD_chineses/releases/download/v1.0.0/ChineseLocalization.zip"
    CompressionFormat = "zip"
    SHA256 = $hash
    WikiUrl = "https://github.com/LinHouYu/OTD_chineses/blob/main/README.md"
    LicenseIdentifier = "GPL-3.0-only"
}
$officialJsonContent = $officialMeta | ConvertTo-Json -Depth 4
$officialJsonPath = Join-Path $distDir "ChineseLocalization.json"
[System.IO.File]::WriteAllText($officialJsonPath, $officialJsonContent, [System.Text.Encoding]::UTF8)

# Create official repository directory layout
$repoDir = Join-Path $scriptDir "official_repository\Repository\0.6.0.0\LinHouYu\ChineseLocalization"
if (Test-Path $repoDir) { Remove-Item (Join-Path $scriptDir "official_repository") -Recurse -Force }
New-Item -ItemType Directory -Force -Path $repoDir | Out-Null
Copy-Item $officialJsonPath (Join-Path $repoDir "ChineseLocalization.json") -Force

Write-Host "`n=========================================================================" -ForegroundColor Green
Write-Host " Build and Packaging Completed Successfully!" -ForegroundColor Green
Write-Host " Output: $distDir"
Write-Host " 1. ChineseLocalization.zip     -- Standard OTD plugin (drag and drop to install)" -ForegroundColor Cyan
Write-Host " 2. ChineseLocalization.dll     -- Driver plugin"
Write-Host " 3. ChineseLocalizationHook.dll -- UI Localization Hook"
Write-Host " 4. metadata.json               -- Plugin metadata"
Write-Host " 5. ChineseLocalization.json    -- Official OTD Repo metadata (with SHA256)" -ForegroundColor Magenta
Write-Host " 6. OpenTabletDriver.UX.Wpf.exe -- Transparent launcher"
Write-Host "=========================================================================" -ForegroundColor Green