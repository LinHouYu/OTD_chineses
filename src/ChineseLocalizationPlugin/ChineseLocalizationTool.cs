using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;

namespace OpenTabletDriver.Localization.Chinese
{
    [PluginName("简体中文汉化增强 (Chinese Localization) - By LinHouYu")]
    public sealed class ChineseLocalizationTool : ITool
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int MessageBoxW(IntPtr hWnd, string lpText, string lpCaption, uint uType);

        public ChineseLocalizationTool()
        {
        }

        public bool Initialize()
        {
            try
            {
                // 在后台异步触发配置与检测，避免阻塞 OTD Daemon 初始化
                Task.Run(() => SetupAndPromptLocalization(true));
            }
            catch { }

            return true;
        }

        public void Dispose()
        {
            try
            {
                Task.Run(() => SetupAndPromptLocalization(false));
            }
            catch { }
        }

        private static void SetupAndPromptLocalization(bool enable)
        {
            try
            {
                string pluginDir = Path.GetDirectoryName(typeof(ChineseLocalizationTool).Assembly.Location) ?? AppDomain.CurrentDomain.BaseDirectory;
                string hookDll = Path.Combine(pluginDir, "ChineseLocalizationHook.dll");
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string baseHook = Path.Combine(baseDir, "ChineseLocalizationHook.dll");

                if (enable)
                {
                    // 1. 同步 Hook DLL 到程序基目录（若权限允许）
                    if (File.Exists(hookDll))
                    {
                        try { File.Copy(hookDll, baseHook, true); } catch { }
                    }

                    string targetHook = File.Exists(baseHook) ? baseHook : hookDll;

                    // 2. 检查用户级环境变量 DOTNET_STARTUP_HOOKS
                    string? currentHook = Environment.GetEnvironmentVariable("DOTNET_STARTUP_HOOKS", EnvironmentVariableTarget.User);
                    bool isNewlyConfigured = !string.Equals(currentHook, targetHook, StringComparison.OrdinalIgnoreCase);

                    if (isNewlyConfigured)
                    {
                        Environment.SetEnvironmentVariable("DOTNET_STARTUP_HOOKS", targetHook, EnvironmentVariableTarget.User);
                    }

                    // 3. 检查是否有前台界面正在运行
                    var uxProcesses = Process.GetProcessesByName("OpenTabletDriver.UX.Wpf")
                        .Concat(Process.GetProcessesByName("OpenTabletDriver.UX.Wpf.Core"))
                        .ToList();

                    // 如果是新配置且当前界面正在运行，弹窗提示用户一键重启界面立即生效
                    if (isNewlyConfigured && uxProcesses.Count > 0)
                    {
                        int res = MessageBoxW(IntPtr.Zero,
                            "OpenTabletDriver 简体中文汉化增强已成功就绪！\n\n是否立即重启驱动界面以使全量中文生效？\n\n【确定】立即重启界面生效\n【取消】稍后手动重启界面",
                            "OpenTabletDriver 汉化插件 - By LinHouYu",
                            0x00040001 | 0x00000040); // MB_TOPMOST | MB_OKCANCEL | MB_ICONINFORMATION

                        if (res == 1) // IDOK
                        {
                            RestartUx(uxProcesses, targetHook, baseDir);
                        }
                    }
                }
                else
                {
                    // 用户在 Tools 中取消勾选：清理用户级环境变量
                    string? currentHook = Environment.GetEnvironmentVariable("DOTNET_STARTUP_HOOKS", EnvironmentVariableTarget.User);
                    if (!string.IsNullOrEmpty(currentHook))
                    {
                        Environment.SetEnvironmentVariable("DOTNET_STARTUP_HOOKS", null, EnvironmentVariableTarget.User);

                        var uxProcesses = Process.GetProcessesByName("OpenTabletDriver.UX.Wpf")
                            .Concat(Process.GetProcessesByName("OpenTabletDriver.UX.Wpf.Core"))
                            .ToList();

                        if (uxProcesses.Count > 0)
                        {
                            int res = MessageBoxW(IntPtr.Zero,
                                "已停用简体中文汉化增强。\n\n是否立即重启图形界面恢复英文原生界面？\n\n【确定】立即重启恢复英文\n【取消】稍后手动重启",
                                "OpenTabletDriver 汉化插件 - By LinHouYu",
                                0x00040001 | 0x00000040);

                            if (res == 1)
                            {
                                RestartUx(uxProcesses, null, baseDir);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private static void RestartUx(List<Process> uxProcesses, string? hookPath, string baseDir)
        {
            try
            {
                string? exePath = null;
                try { exePath = uxProcesses[0].MainModule?.FileName; } catch { }

                foreach (var p in uxProcesses)
                {
                    try { p.Kill(); p.WaitForExit(2000); } catch { }
                }

                // 自动修复：若之前版本将原版改名为 Core.exe，自动还原官方原版
                string coreUx = Path.Combine(baseDir, "OpenTabletDriver.UX.Wpf.Core.exe");
                string origUx = Path.Combine(baseDir, "OpenTabletDriver.UX.Wpf.exe");
                if (File.Exists(coreUx) && File.Exists(origUx))
                {
                    try
                    {
                        var infoOrig = new FileInfo(origUx);
                        var infoCore = new FileInfo(coreUx);
                        if (infoOrig.Length < 1000000 && infoCore.Length > 1000000)
                        {
                            File.Delete(origUx);
                            File.Move(coreUx, origUx);
                            exePath = origUx;
                        }
                    }
                    catch { }
                }

                if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
                {
                    exePath = origUx;
                    if (!File.Exists(exePath))
                    {
                        exePath = coreUx;
                    }
                }

                if (File.Exists(exePath))
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = exePath,
                        WorkingDirectory = Path.GetDirectoryName(exePath) ?? baseDir,
                        UseShellExecute = false
                    };

                    if (!string.IsNullOrEmpty(hookPath))
                    {
                        psi.Environment["DOTNET_STARTUP_HOOKS"] = hookPath;
                    }
                    else
                    {
                        psi.Environment.Remove("DOTNET_STARTUP_HOOKS");
                    }

                    Process.Start(psi);
                }
            }
            catch { }
        }
    }
}
