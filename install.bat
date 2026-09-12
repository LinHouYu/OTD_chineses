@echo off
chcp 65001 > nul
title OpenTabletDriver 简体中文汉化插件一键安装器

echo ========================================================================
echo        OpenTabletDriver 简体中文汉化增强插件 一键安装助手
echo ========================================================================
echo.

set "SCRIPT_DIR=%~dp0"
set "OTD_DIR="

:: 1. 检测是否在 OTD 根目录
if exist "%SCRIPT_DIR%OpenTabletDriver.Daemon.exe" (
    set "OTD_DIR=%SCRIPT_DIR%"
    goto :INSTALL
)

:: 2. 检测是否在 userdata\Plugins\ChineseLocalization 目录
if exist "%SCRIPT_DIR%..\..\..\OpenTabletDriver.Daemon.exe" (
    set "OTD_DIR=%SCRIPT_DIR%..\..\..\"
    goto :INSTALL
)

:: 3. 常见默认路径检测
if exist "%LOCALAPPDATA%\OpenTabletDriver\OpenTabletDriver.Daemon.exe" (
    set "OTD_DIR=%LOCALAPPDATA%\OpenTabletDriver\"
    goto :INSTALL
)

if exist "C:\Program Files\OpenTabletDriver\OpenTabletDriver.Daemon.exe" (
    set "OTD_DIR=C:\Program Files\OpenTabletDriver\"
    goto :INSTALL
)

echo [提示] 未能自动定位 OpenTabletDriver 安装目录。
echo 请将本脚本及其同级文件直接解压复制到 OpenTabletDriver 主程序目录下运行！
echo.
pause
exit /b 1

:INSTALL
echo -> 识别到 OpenTabletDriver 安装目录: %OTD_DIR%
echo.

:: 结束旧运行进程
taskkill /F /IM OpenTabletDriver.UX.Wpf.exe >nul 2>&1
taskkill /F /IM OpenTabletDriver.UX.Wpf.Core.exe >nul 2>&1
taskkill /F /IM OpenTabletDriver.Daemon.exe >nul 2>&1

:: 1. 备份原版 UX.Wpf.exe 为 UX.Wpf.Core.exe
if not exist "%OTD_DIR%OpenTabletDriver.UX.Wpf.Core.exe" (
    if exist "%OTD_DIR%OpenTabletDriver.UX.Wpf.exe" (
        echo -> 正在备份原版图形界面核心...
        ren "%OTD_DIR%OpenTabletDriver.UX.Wpf.exe" "OpenTabletDriver.UX.Wpf.Core.exe"
    )
)

:: 2. 部署透明启动器
if exist "%SCRIPT_DIR%OpenTabletDriver.UX.Wpf.exe" (
    echo -> 正在部署中文透明启动器...
    copy /Y "%SCRIPT_DIR%OpenTabletDriver.UX.Wpf.exe" "%OTD_DIR%OpenTabletDriver.UX.Wpf.exe" >nul
)

:: 3. 部署 Hook DLL
if exist "%SCRIPT_DIR%ChineseLocalizationHook.dll" (
    echo -> 正在部署界面翻译挂钩核心 (ChineseLocalizationHook.dll)...
    copy /Y "%SCRIPT_DIR%ChineseLocalizationHook.dll" "%OTD_DIR%ChineseLocalizationHook.dll" >nul
)

:: 4. 部署驱动插件至 userdata\Plugins
if not exist "%OTD_DIR%userdata\Plugins\ChineseLocalization" (
    mkdir "%OTD_DIR%userdata\Plugins\ChineseLocalization" >nul 2>&1
)

if exist "%SCRIPT_DIR%ChineseLocalization.dll" (
    copy /Y "%SCRIPT_DIR%ChineseLocalization.dll" "%OTD_DIR%userdata\Plugins\ChineseLocalization\ChineseLocalization.dll" >nul
)
if exist "%SCRIPT_DIR%metadata.json" (
    copy /Y "%SCRIPT_DIR%metadata.json" "%OTD_DIR%userdata\Plugins\ChineseLocalization\metadata.json" >nul
)

echo.
echo ========================================================================
echo   恭喜！简体中文汉化增强插件已成功安装就绪！
echo   现在您可以启动 OpenTabletDriver，尽享全中文驱动界面体验！
echo ========================================================================
echo.
pause
