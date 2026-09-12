using System;
using System.IO;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;

namespace OpenTabletDriver.Localization.Chinese
{
    [PluginName("简体中文汉化增强 (Chinese Localization) - By LinHouYu")]
    public sealed class ChineseLocalizationTool : ITool
    {
        public ChineseLocalizationTool()
        {
        }

        public bool Initialize()
        {
            try
            {
                // 当在 OTD Daemon 中作为驱动插件加载时，自动协助前台界面就绪汉化环境
                SetupUiHookEnvironment();
            }
            catch { }

            return true;
        }

        private static void SetupUiHookEnvironment()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string pluginDir = Path.GetDirectoryName(typeof(ChineseLocalizationTool).Assembly.Location) ?? baseDir;

                string rootHook = Path.Combine(baseDir, "ChineseLocalizationHook.dll");
                string pluginHook = Path.Combine(pluginDir, "ChineseLocalizationHook.dll");

                // 1. 同步 Hook DLL 至根目录
                if (File.Exists(pluginHook) && (!File.Exists(rootHook) || File.GetLastWriteTimeUtc(pluginHook) > File.GetLastWriteTimeUtc(rootHook)))
                {
                    try { File.Copy(pluginHook, rootHook, true); } catch { }
                }

                // 2. 如果插件目录下携带了透明启动器，协助配置
                string pluginLauncher = Path.Combine(pluginDir, "OpenTabletDriver.UX.Wpf.exe");
                string rootUx = Path.Combine(baseDir, "OpenTabletDriver.UX.Wpf.exe");
                string rootCore = Path.Combine(baseDir, "OpenTabletDriver.UX.Wpf.Core.exe");

                if (File.Exists(pluginLauncher) && File.Exists(rootUx) && !File.Exists(rootCore))
                {
                    var uxInfo = new FileInfo(rootUx);
                    if (uxInfo.Length > 1000000) // 原版程序通常大于 10MB
                    {
                        File.Move(rootUx, rootCore, true);
                        File.Copy(pluginLauncher, rootUx, true);
                    }
                }
            }
            catch { }
        }

        public void Dispose()
        {
        }
    }
}
