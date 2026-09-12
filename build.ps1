<#
.SYNOPSIS
    OTD_chineses 一键编译与打包脚本 (Build & Package Script)
#>
$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Set-Location $scriptDir

Write-Host "=========================================================================" -ForegroundColor Cyan
Write-Host "         OpenTabletDriver 简体中文汉化增强插件 一键编译打包工具" -ForegroundColor Green
Write-Host "=========================================================================" -ForegroundColor Cyan

# 1. 检查与准备输出目录
$distDir = Join-Path $scriptDir "dist"
if (Test-Path $distDir) {
    Remove-Item $distDir -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $distDir | Out-Null

$zipTempDir = Join-Path $distDir "ChineseLocalization"
New-Item -ItemType Directory -Force -Path $zipTempDir | Out-Null

# 2. 编译并单文件发布透明启动器 (OTD-Launcher)
Write-Host "`n[1/4] 编译透明 WinExe 启动器 (OTD-Launcher)..." -ForegroundColor Yellow
$launcherProj = Join-Path $scriptDir "src\OTD-Launcher\OTD-Launcher.csproj"
dotnet publish $launcherProj -c Release -r win-x64 --no-self-contained | Out-Null
$launcherOutput = Join-Path $scriptDir "src\OTD-Launcher\bin\Release\net8.0-windows\win-x64\publish\OTD-Launcher.exe"
if (!(Test-Path $launcherOutput)) {
    throw "Launcher build output not found at: $launcherOutput"
}

# 3. 编译核心 UI 挂钩 (ChineseLocalizationHook)
Write-Host "[2/4] 编译界面实时汉化挂钩 (ChineseLocalizationHook)..." -ForegroundColor Yellow
$hookProj = Join-Path $scriptDir "src\ChineseLocalizationHook\ChineseLocalizationHook.csproj"
dotnet build $hookProj -c Release | Out-Null
$hookOutput = Join-Path $scriptDir "src\ChineseLocalizationHook\bin\Release\net8.0\ChineseLocalizationHook.dll"
if (!(Test-Path $hookOutput)) {
    throw "Hook build output not found at: $hookOutput"
}

# 4. 编译 OTD 驱动插件 (ChineseLocalizationPlugin)
Write-Host "[3/4] 编译驱动插件主入口 (ChineseLocalizationPlugin)..." -ForegroundColor Yellow
$pluginProj = Join-Path $scriptDir "src\ChineseLocalizationPlugin\ChineseLocalizationPlugin.csproj"
dotnet build $pluginProj -c Release | Out-Null
$pluginOutput = Join-Path $scriptDir "src\ChineseLocalizationPlugin\bin\Release\net8.0\ChineseLocalizationPlugin.dll"
if (!(Test-Path $pluginOutput)) {
    throw "Plugin build output not found at: $pluginOutput"
}

# 5. 复制打包文件
Write-Host "[4/4] 正在组装并压缩 Release 插件包..." -ForegroundColor Yellow
$metaSrc = Join-Path $scriptDir "src\ChineseLocalizationPlugin\metadata.json"
$installBatSrc = Join-Path $scriptDir "install.bat"

# 复制到 dist 根目录
Copy-Item $pluginOutput (Join-Path $distDir "ChineseLocalization.dll") -Force
Copy-Item $hookOutput (Join-Path $distDir "ChineseLocalizationHook.dll") -Force
Copy-Item $launcherOutput (Join-Path $distDir "OpenTabletDriver.UX.Wpf.exe") -Force
Copy-Item $metaSrc (Join-Path $distDir "metadata.json") -Force

# 复制到 zip 临时目录
Copy-Item $pluginOutput (Join-Path $zipTempDir "ChineseLocalization.dll") -Force
Copy-Item $hookOutput (Join-Path $zipTempDir "ChineseLocalizationHook.dll") -Force
Copy-Item $launcherOutput (Join-Path $zipTempDir "OpenTabletDriver.UX.Wpf.exe") -Force
Copy-Item $metaSrc (Join-Path $zipTempDir "metadata.json") -Force
if (Test-Path $installBatSrc) {
    Copy-Item $installBatSrc (Join-Path $zipTempDir "install.bat") -Force
}

# 生成 ZIP 压缩包 (提供给 OTD 插件管理器与 GitHub Release)
$zipFile = Join-Path $distDir "ChineseLocalization.zip"
if (Test-Path $zipFile) { Remove-Item $zipFile -Force }
Compress-Archive -Path "$zipTempDir\*" -DestinationPath $zipFile -Force
Remove-Item $zipTempDir -Recurse -Force

Write-Host "`n=========================================================================" -ForegroundColor Green
Write-Host " 编译与打包顺利完成！" -ForegroundColor Green
Write-Host " 产物输出目录: $distDir"
Write-Host " 1. ChineseLocalization.zip     <- [核心] OTD 官方插件管理器格式压缩包" -ForegroundColor Cyan
Write-Host " 2. ChineseLocalization.dll     <- 驱动插件主程序"
Write-Host " 3. ChineseLocalizationHook.dll <- UI 汉化挂钩核心"
Write-Host " 4. OpenTabletDriver.UX.Wpf.exe <- 透明无窗口启动器"
Write-Host " 5. metadata.json               <- 插件元数据"
Write-Host "=========================================================================" -ForegroundColor Green
