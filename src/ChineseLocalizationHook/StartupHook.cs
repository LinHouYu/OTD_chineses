using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

internal class StartupHook
{
    public static readonly Dictionary<string, string> Dictionary = new(StringComparer.OrdinalIgnoreCase)
    {
        // ==================== 顶部主菜单 (Menu Headers & Items) ====================
        { "_File", "文件(_F)" },
        { "File", "文件" },
        { "&File", "文件(&F)" },
        { "Save settings", "保存设置" },
        { "Apply settings", "应用设置" },
        { "Save settings as...", "设置另存为..." },
        { "Save settings as", "设置另存为" },
        { "Load settings...", "加载设置..." },
        { "Load settings", "加载设置" },
        { "Reset to defaults", "恢复默认设置" },
        { "Presets", "预设" },
        { "Save as preset...", "另存为预设..." },
        { "Save preset...", "保存预设..." },
        { "Save preset", "保存预设" },
        { "Open presets directory", "打开预设目录" },
        { "Open presets directory...", "打开预设目录..." },
        { "Refresh presets", "刷新预设" },
        { "Quit", "退出" },
        { "Exit", "退出" },

        { "_Tablets", "数位板(_T)" },
        { "Tablets", "数位板" },
        { "&Tablets", "数位板(&T)" },
        { "Detect tablet", "检测数位板" },
        { "Detect Tablets", "检测数位板" },
        { "Tablet debugger...", "数位板调试器..." },
        { "Tablet debugger", "数位板调试器" },
        { "Device string reader...", "设备字符串读取器..." },
        { "Device string reader", "设备字符串读取器" },

        { "_Plugins", "插件(_P)" },
        { "Plugins", "插件" },
        { "&Plugins", "插件(&P)" },
        { "Open Plugin Manager...", "打开插件管理器..." },
        { "Open Plugin Manager", "打开插件管理器" },
        { "Plugin Manager", "插件管理器" },
        { "Open Plugins Directory", "打开插件目录" },
        { "Open Plugins Directory...", "打开插件目录..." },

        { "_Help", "帮助(_H)" },
        { "Help", "帮助" },
        { "&Help", "帮助(&H)" },
        { "About...", "关于..." },
        { "About", "关于" },
        { "About OpenTabletDriver", "关于 OpenTabletDriver (汉化: LinHouYu)" },
        { "Open Wiki...", "打开说明文档 Wiki..." },
        { "Open Wiki", "打开说明文档 Wiki" },
        { "Wiki", "说明文档 Wiki" },
        { "OpenTabletDriver Guide", "OpenTabletDriver 使用指南 (汉化: LinHouYu)" },
        { "Show guide...", "显示使用指南..." },
        { "Export diagnostics...", "导出诊断报告..." },
        { "Export diagnostics to Clipboard...", "复制诊断报告到剪贴板..." },
        { "Check for updates...", "检查驱动更新..." },
        { "Check for updates", "检查驱动更新" },
        { "OpenTabletDriver Updater", "OpenTabletDriver 更新检查器" },

        // ==================== 核心选项卡 (Tab Headers) ====================
        { "Output", "输出模式 (Output)" },
        { "Filters", "滤波器 (Filters)" },
        { "Pen Settings", "数位笔设置 (Pen)" },
        { "Auxiliary Settings", "快捷键设置 (Aux)" },
        { "Auxiliary", "数位板快捷键" },
        { "Mouse Settings", "鼠标设置 (Mouse)" },
        { "Tools", "实用工具 (Tools)" },
        { "Info", "设备信息 (Info)" },
        { "Information", "设备信息 (Info)" },

        // ==================== 常用操作按钮 (Buttons) ====================
        { "_Apply", "应用(_A)" },
        { "Apply", "应用 (Apply)" },
        { "_Save", "保存(_S)" },
        { "Save", "保存 (Save)" },
        { "Discard", "放弃修改" },
        { "Reset", "重置" },
        { "OK", "确定" },
        { "Cancel", "取消" },
        { "Close", "关闭" },
        { "Browse...", "浏览..." },
        { "Browse", "浏览" },
        { "Install", "安装" },
        { "Uninstall", "卸载" },
        { "Enable", "启用" },
        { "Disable", "禁用" },
        { "Open", "打开" },
        { "Yes", "是" },
        { "No", "否" },
        { "Refresh", "刷新" },
        { "Clear", "清空" },
        { "Copy", "复制" },
        { "Copy All", "复制全部" },
        { "Dump All", "导出全部" },
        { "Next", "下一步" },
        { "Previous", "上一步" },
        { "Resize", "调整大小" },
        { "Convert Area...", "转换区域..." },
        { "Converter", "区域转换器" },
        { "Send Request", "发送请求" },

        // ==================== 输出模式与区域参数 ====================
        { "Output Mode", "输出模式" },
        { "Display", "显示器设置" },
        { "Target Display", "目标显示器" },
        { "Area", "有效区域" },
        { "Width", "宽度" },
        { "Height", "高度" },
        { "X Offset", "X 偏移" },
        { "Y Offset", "Y 偏移" },
        { "Rotation", "旋转角度" },
        { "Lock Aspect Ratio", "锁定宽高比" },
        { "Lock to usable area", "限制在可用区域内" },
        { "Reset Area", "重置区域" },
        { "Full Area", "全幅有效区域" },
        { "Quarter area", "1/4 区域" },
        { "Mode", "工作模式" },
        { "Absolute Mode", "绝对模式 (常规/绘图)" },
        { "Relative Mode", "相对模式 (FPS/游戏)" },
        { "Relative", "相对模式参数" },
        { "Sensitivity", "灵敏度" },
        { "X Sensitivity", "X 轴灵敏度" },
        { "Y Sensitivity", "Y 轴灵敏度" },
        { "XSensitivity", "X 轴灵敏度" },
        { "YSensitivity", "Y 轴灵敏度" },
        { "Relative Rotation", "相对旋转角" },
        { "Relative Reset Delay", "相对模式复位延迟" },
        { "Reset Time", "相对模式复位延迟" },
        { "Clamp input outside area", "限制超出区域的输入" },
        { "Ignore input outside area", "忽略超出区域的输入" },
        { "Flip", "翻转" },
        { "Horizontal", "水平" },
        { "Vertical", "垂直" },
        { "Align", "对齐" },
        { "Top", "顶端对齐" },
        { "Bottom", "底端对齐" },
        { "Center", "居中对齐" },
        { "Left", "左对齐" },
        { "Right", "右对齐" },
        { "Handedness", "习惯用手" },

        // ==================== 按键设置与映射 ====================
        { "Tip Button", "笔尖映射" },
        { "Tip Binding", "笔尖按键映射" },
        { "Tip Settings", "笔尖设置" },
        { "Tip Threshold", "笔尖激活阈值" },
        { "Tip Activation Threshold", "笔尖激活阈值" },
        { "Eraser Button", "橡皮擦映射" },
        { "Eraser Binding", "橡皮擦映射" },
        { "Eraser Settings", "橡皮擦设置" },
        { "Eraser Threshold", "橡皮擦激活阈值" },
        { "Eraser Activation Threshold", "橡皮擦激活阈值" },
        { "Eraser", "橡皮擦" },
        { "Pen Buttons", "笔身按键" },
        { "Binding Editor", "按键绑定编辑器" },
        { "Advanced Binding Editor", "高级按键绑定编辑器" },
        { "Button 1", "按键 1" },
        { "Button 2", "按键 2" },
        { "Button 3", "按键 3" },
        { "Button 4", "按键 4" },
        { "Button 5", "按键 5" },
        { "Button 6", "按键 6" },
        { "Button 7", "按键 7" },
        { "Button 8", "按键 8" },
        { "Button 9", "按键 9" },
        { "Button 10", "按键 10" },
        { "Button 11", "按键 11" },
        { "Button 12", "按键 12" },
        { "Express Keys", "数位板快捷键" },
        { "Mouse Buttons", "鼠标键映射" },
        { "Mouse Scrollwheel", "鼠标滚轮" },
        { "Left Click", "鼠标左键" },
        { "Right Click", "鼠标右键" },
        { "Middle Click", "鼠标中键" },
        { "Scroll Up", "向上滚动" },
        { "Scroll Down", "向下滚动" },

        // ==================== 滤波与算法参数 ====================
        { "Noise Reduction", "降噪滤波 (Noise Reduction)" },
        { "Smoothing", "平滑滤波 (Hawku Smoothing)" },
        { "Frequency", "采样频率 (Hz)" },
        { "Latency", "平滑延迟 (ms)" },
        { "Samples", "采样点数 (Samples)" },
        { "Distance Threshold", "距离过滤阈值" },
        { "Interpolation", "插值算法" },

        // ==================== 插件管理器与通用 ====================
        { "Drag and drop plugins here to install.", "将插件拖放到此处即可自动安装。" },
        { "Install plugin...", "安装插件..." },
        { "Choose a plugin to install...", "选择要安装的插件..." },
        { "No plugin selected.", "未选择任何插件。" },
        { "No plugins containing this type are installed.", "未安装此类型的任何插件。" },
        { "Plugin Version", "插件版本" },
        { "Driver Version", "驱动支持版本" },
        { "Show plugin wiki", "查看插件说明文档" },
        { "Show source code", "查看插件源代码" },
        { "Source Code Repository", "源代码仓库" },
        { "Owner", "作者" },
        { "License", "许可证" },
        { "Description", "插件描述" },
        { "Name", "名称" },
        { "Type", "类型" },
        { "Options", "选项" },
        { "Level", "级别" },
        { "Message", "消息" },
        { "Time", "时间" },
        { "Device", "设备" },
        { "Device String", "设备字符串" },
        { "Raw Tablet Data", "原始数位板数据" },
        { "Tablet Report", "数位板报告" },
        { "Report Rate", "回报率" },
        { "Reports Recorded", "已记录报告数" },
        { "Enable Data Recording", "启用数据记录" },
        { "Visualizer", "可视化显示" },

        // ==================== 提示与状态文本 ====================
        { "Connecting to OpenTabletDriver Daemon...", "正在连接 OpenTabletDriver 守护服务..." },
        { "No tablets detected", "未检测到数位板设备" },
        { "No tablets are detected.", "未检测到数位板设备。" },
        { "No presets loaded", "未加载任何预设" },
        { "Checking for updates...", "正在检查驱动更新..." },
        { "No updates are available.", "当前已是最新版本，无需更新。" },
    };

    private static bool _monitorStarted = false;
    private static System.Threading.Timer? _periodicTimer;
    private static string? _logPath;

    public static void Initialize()
    {
        try
        {
            string proc = Process.GetCurrentProcess().ProcessName;
            if (proc.IndexOf("UX", StringComparison.OrdinalIgnoreCase) < 0 ||
                proc.IndexOf("Daemon", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return;
            }

            if (_monitorStarted) return;
            _monitorStarted = true;

            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string logsDir = Path.Combine(baseDir, "userdata", "Logs");
                Directory.CreateDirectory(logsDir);
                _logPath = Path.Combine(logsDir, "ChineseLocalization.log");
                File.AppendAllText(_logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 简体中文汉化挂载成功 (作者: LinHouYu https://github.com/LinHouYu): 进程 {proc} (PID: {Environment.ProcessId})\n");
            }
            catch { }

            _periodicTimer = new System.Threading.Timer(_ =>
            {
                try
                {
                    TryTranslateWpf();
                    TryTranslateEto();
                }
                catch { }
            }, null, 150, 350);
        }
        catch { }
    }

    private static void TryTranslateEto()
    {
        try
        {
            var etoAppType = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Eto")?
                .GetType("Eto.Forms.Application");

            if (etoAppType == null) return;

            var instanceProp = etoAppType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            var instance = instanceProp?.GetValue(null);
            if (instance == null) return;

            var asyncInvokeMethod = etoAppType.GetMethod("AsyncInvoke", new[] { typeof(Action) });
            asyncInvokeMethod?.Invoke(instance, new object[] { new Action(() =>
            {
                try
                {
                    var mainFormProp = etoAppType.GetProperty("MainForm", BindingFlags.Public | BindingFlags.Instance);
                    var mainForm = mainFormProp?.GetValue(instance);
                    if (mainForm != null)
                    {
                        TranslateEtoForm(mainForm);
                    }
                }
                catch { }
            }) });
        }
        catch { }
    }

    private static void TranslateEtoForm(object form)
    {
        if (form == null) return;
        try
        {
            var t = form.GetType();

            var titleProp = t.GetProperty("Title");
            if (titleProp != null && titleProp.CanWrite && titleProp.PropertyType == typeof(string))
            {
                string? title = titleProp.GetValue(form) as string;
                if (!string.IsNullOrEmpty(title) && !title.Contains("【中文增强"))
                {
                    titleProp.SetValue(form, $"{title} 【中文增强版 by LinHouYu】");
                }
            }

            var menuProp = t.GetProperty("Menu");
            var menu = menuProp?.GetValue(form);
            if (menu != null)
            {
                var itemsProp = menu.GetType().GetProperty("Items");
                if (itemsProp?.GetValue(menu) is IEnumerable items)
                {
                    foreach (var item in items)
                    {
                        TranslateEtoMenuItem(item);
                    }
                }
            }
        }
        catch { }
    }

    private static void TranslateEtoMenuItem(object? item)
    {
        if (item == null) return;
        try
        {
            var textProp = item.GetType().GetProperty("Text");
            if (textProp != null && textProp.CanWrite && textProp.PropertyType == typeof(string))
            {
                string? text = textProp.GetValue(item) as string;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    string raw = text.Trim();
                    if (Dictionary.TryGetValue(raw, out var trans))
                    {
                        textProp.SetValue(item, trans);
                    }
                }
            }

            var itemsProp = item.GetType().GetProperty("Items");
            if (itemsProp?.GetValue(item) is IEnumerable subItems)
            {
                foreach (var sub in subItems)
                {
                    TranslateEtoMenuItem(sub);
                }
            }
        }
        catch { }
    }

    private static void TryTranslateWpf()
    {
        try
        {
            var pfAsm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "PresentationFramework");
            var appType = pfAsm?.GetType("System.Windows.Application");
            if (appType == null) return;

            var curApp = appType.GetProperty("Current", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
            if (curApp == null) return;

            var dispProp = appType.GetProperty("Dispatcher", BindingFlags.Public | BindingFlags.Instance);
            object? disp = dispProp?.GetValue(curApp);
            if (disp == null) return;

            var invokeAsync = disp.GetType().GetMethod("InvokeAsync", new[] { typeof(Action) });
            invokeAsync?.Invoke(disp, new object[] { new Action(() =>
            {
                try
                {
                    var winProp = appType.GetProperty("Windows", BindingFlags.Public | BindingFlags.Instance);
                    if (winProp?.GetValue(curApp) is IEnumerable wins)
                    {
                        foreach (var w in wins)
                        {
                            if (w == null) continue;
                            TranslateWpfVisualElement(w);
                        }
                    }
                }
                catch { }
            }) });
        }
        catch { }
    }

    private static void TranslateWpfVisualElement(object? element)
    {
        if (element == null) return;

        try
        {
            var t = element.GetType();

            // 1. 窗口标题 (Window.Title)
            var titleProp = t.GetProperty("Title");
            if (titleProp != null && titleProp.CanWrite && titleProp.PropertyType == typeof(string))
            {
                string? title = titleProp.GetValue(element) as string;
                if (!string.IsNullOrEmpty(title) && !title.Contains("【中文增强"))
                {
                    string newTitle = $"{title} 【中文增强版 by LinHouYu】";
                    titleProp.SetValue(element, newTitle);
                    LogTranslation("Title", title, newTitle);
                }
            }

            // 2. AccessText / TextBlock (Text 属性)
            var textProp = t.GetProperty("Text");
            if (textProp != null && textProp.CanWrite && textProp.PropertyType == typeof(string))
            {
                string? text = textProp.GetValue(element) as string;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    string raw = text.Trim();
                    if (Dictionary.TryGetValue(raw, out var trans))
                    {
                        textProp.SetValue(element, trans);
                        LogTranslation("Text", raw, trans);
                    }
                }
            }

            // 3. HeaderedContentControl (MenuItem, TabItem, GroupBox)
            var headerProp = t.GetProperty("Header");
            if (headerProp != null && headerProp.CanWrite)
            {
                object? headerVal = headerProp.GetValue(element);
                if (headerVal is string headerStr && !string.IsNullOrWhiteSpace(headerStr))
                {
                    string raw = headerStr.Trim();
                    if (Dictionary.TryGetValue(raw, out var trans))
                    {
                        headerProp.SetValue(element, trans);
                        LogTranslation("Header", raw, trans);
                    }
                }
            }

            // 4. ContentControl (Button, CheckBox, Label, RadioButton)
            var contentProp = t.GetProperty("Content");
            if (contentProp != null && contentProp.CanWrite)
            {
                object? contentVal = contentProp.GetValue(element);
                if (contentVal is string contentStr && !string.IsNullOrWhiteSpace(contentStr))
                {
                    string raw = contentStr.Trim();
                    if (Dictionary.TryGetValue(raw, out var trans))
                    {
                        contentProp.SetValue(element, trans);
                        LogTranslation("Content", raw, trans);
                    }
                }
            }

            // 5. 递归遍历 Visual 子树 (VisualTreeHelper)
            var pcAsm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "PresentationCore");
            var vthType = pcAsm?.GetType("System.Windows.Media.VisualTreeHelper");
            if (vthType != null)
            {
                var getCount = vthType.GetMethod("GetChildrenCount", BindingFlags.Public | BindingFlags.Static);
                var getChild = vthType.GetMethod("GetChild", BindingFlags.Public | BindingFlags.Static);
                if (getCount != null && getChild != null)
                {
                    int childCount = (int)(getCount.Invoke(null, new[] { element }) ?? 0);
                    for (int i = 0; i < childCount; i++)
                    {
                        var child = getChild.Invoke(null, new[] { element, i });
                        TranslateWpfVisualElement(child);
                    }
                }
            }
        }
        catch { }
    }

    private static void LogTranslation(string category, string raw, string trans)
    {
        if (_logPath != null)
        {
            try
            {
                File.AppendAllText(_logPath, $"[{category}] {raw} -> {trans}\n");
            }
            catch { }
        }
    }
}
