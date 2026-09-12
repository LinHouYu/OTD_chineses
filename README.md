# OpenTabletDriver 简体中文界面汉化增强插件 (Chinese Localization Plugin)

[![Release](https://img.shields.io/github/v/release/LinHouYu/OTD_chineses?style=flat-square)](https://github.com/LinHouYu/OTD_chineses/releases)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg?style=flat-square)](https://www.gnu.org/licenses/gpl-3.0)
[![Platform](https://img.shields.io/badge/Platform-Windows%20.NET%208-brightgreen?style=flat-square)](https://dotnet.microsoft.com/)

专门为 **[OpenTabletDriver](https://github.com/OpenTabletDriver/OpenTabletDriver)** 开发的简体中文汉化增强插件与注入框架。彻底解决官方驱动不支持中文界面的痛点，实现菜单、选项卡、模式参数、灵敏度、滤波与按键映射的全量中文显示。

---

## 🌟 核心特性 (Features)

- 🚀 **全量界面覆盖**：深度汉化顶部菜单（文件、数位板、插件、帮助）、7 大选项卡（输出模式、滤波器、数位笔、快捷键、鼠标、实用工具、设备信息）、相对/绝对模式参数、按键绑定与所有弹窗。
- ⚡ **动态视觉树劫持**：基于 .NET 8 原生 `DOTNET_STARTUP_HOOKS` 与视觉树递归遍历，无论切换 Tab、插拔数位板还是打开弹窗，汉化即刻生效。
- 🛡️ **0 内存驻留与 0 系统污染**：采用透明 WinExe 启动器，Core 进程拉起后启动器立即退出（0 MB 内存占用），不修改注册表与系统全局环境变量，绝不影响系统其他 .NET 应用。
- 🔌 **兼容 OTD 官方插件规范**：可直接打包为标准插件 zip，支持通过 OTD 自带的“插件管理器”拖拽安装，也可提交至 OpenTabletDriver 官方插件仓库。
- 🔁 **自适应热部署**：在 OTD Daemon 服务加载本插件时，若检测到原版环境，可全自动协助配置透明启动器与 Hook，用户开箱即用。

---

## 📥 安装使用方式 (Installation)

### 方式一：使用一键安装包 (推荐小白用户)
1. 前往 [Releases](https://github.com/LinHouYu/OTD_chineses/releases) 下载最新发布的 `ChineseLocalization.zip`。
2. 解压压缩包，将所有文件直接复制到 OpenTabletDriver 主程序所在目录（与 `OpenTabletDriver.UX.Wpf.exe` 同级）。
3. 双击运行 `install.bat`，根据提示按任意键完成自动部署。
4. 启动 OpenTabletDriver，享受全中文界面！

### 方式二：通过 OpenTabletDriver 插件管理器安装
1. 下载 `ChineseLocalization.zip`。
2. 打开 OpenTabletDriver，点击顶部菜单 `插件 (Plugins)` -> `打开插件管理器 (Open Plugin Manager...)`。
3. 将下载的 `ChineseLocalization.zip` 拖入插件管理器窗口完成安装。
4. 重启驱动服务与图形界面，汉化即刻生效。

---

## 🛠️ 项目架构与工作原理 (How It Works)

OpenTabletDriver 采用“无界面后台守护服务 (Daemon) + 独立前台客户端 (UX.Wpf)”的分层架构。传统驱动插件仅运行于 Daemon 内部，无法触及前台图形树。

本插件创造性地采用复合注入架构：
```
[用户双击快捷方式 / 直接打开 OpenTabletDriver.UX.Wpf.exe]
                     │
                     ▼
[透明 WinExe 启动器 (OTD-Launcher)]
  ├─ 检查当前目录的 ChineseLocalizationHook.dll
  └─ 携带 DOTNET_STARTUP_HOOKS 启动 OpenTabletDriver.UX.Wpf.Core.exe
                     │
                     ▼
[核心界面进程 OpenTabletDriver.UX.Wpf.Core.exe]
  ├─ CoreCLR 在 Main() 之前自动挂载 StartupHook.Initialize()
  ├─ 实时遍历 WPF 视觉树与 Eto.Forms 树，200+ 词条精准汉化
  └─ 后台守护线程自动汉化动态切换的面板与弹窗
```

---

## 💻 源码编译与打包 (Build From Source)

环境要求：
- Windows 10 / 11 (x64)
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) 或更高版本

### 一键构建打包
在 PowerShell 中运行根目录下的自动化构建脚本：
```powershell
./build.ps1
```
脚本执行完毕后，将在 `dist/` 目录下生成：
- `ChineseLocalization.zip`（官方规范插件压缩包）
- `ChineseLocalization.dll`（驱动插件）
- `ChineseLocalizationHook.dll`（UI 翻译挂钩）
- `OpenTabletDriver.UX.Wpf.exe`（透明启动器）
- `metadata.json`（插件元数据）

---

## 🤝 提交至 OpenTabletDriver 官方仓库指引

如需将本插件推送到官方 [OpenTabletDriver/Plugin-Repository](https://github.com/OpenTabletDriver/Plugin-Repository)：
1. Fork 官方插件仓库。
2. 在 `Repository/0.6.0.0/` 下创建 `ChineseLocalization` 文件夹。
3. 放入本项目的 `metadata.json` 与编译生成的 `ChineseLocalization.zip`。
4. 提交 Pull Request，官方合并后全球用户即可在驱动内一键联网搜索安装！

---

## 👨‍💻 作者信息 (Author)

- **作者 / Author**: LinHouYu
- **GitHub 个人主页**: [https://github.com/LinHouYu](https://github.com/LinHouYu)
- **项目仓库 / Repository**: [https://github.com/LinHouYu/OTD_chineses](https://github.com/LinHouYu/OTD_chineses)

---

## 📄 开源许可证 (License)

本项目遵循 [GNU General Public License v3.0 (GPL-3.0)](LICENSE) 开源协议。

