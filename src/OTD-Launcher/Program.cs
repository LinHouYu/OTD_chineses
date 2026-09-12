using System;
using System.Diagnostics;
using System.IO;

namespace OTD_Launcher;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string coreExe = Path.Combine(baseDir, "OpenTabletDriver.UX.Wpf.Core.exe");

        if (!File.Exists(coreExe))
        {
            string fallbackExe = Path.Combine(baseDir, "OpenTabletDriver.UX.Wpf.Original.exe");
            if (File.Exists(fallbackExe))
            {
                coreExe = fallbackExe;
            }
            else
            {
                return;
            }
        }

        string hookDll = Path.Combine(baseDir, "ChineseLocalizationHook.dll");
        if (!File.Exists(hookDll))
        {
            hookDll = Path.Combine(baseDir, "userdata", "Plugins", "ChineseLocalization", "ChineseLocalizationHook.dll");
        }
        if (!File.Exists(hookDll))
        {
            hookDll = Path.Combine(baseDir, "userdata", "Plugins", "ChineseLocalization", "ChineseLocalization.dll");
        }

        var psi = new ProcessStartInfo
        {
            FileName = coreExe,
            WorkingDirectory = baseDir,
            UseShellExecute = false
        };

        foreach (var arg in args)
        {
            psi.ArgumentList.Add(arg);
        }

        // 仅在存在汉化 Hook DLL 时注入 DOTNET_STARTUP_HOOKS
        if (File.Exists(hookDll))
        {
            psi.Environment["DOTNET_STARTUP_HOOKS"] = hookDll;
        }

        try
        {
            Process.Start(psi);
        }
        catch { }
    }
}
