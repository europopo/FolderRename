# FolderRename

一个基于 **WinUI 3** 的 Windows 文件夹定时更名工具。通过 Fluent 风格的分区表单配置目标文件夹、日期命名格式、一次性/周期计划及失败重试；成功、失败和重试都会保存到独立的日志页面。

## 架构

- `Models`：可序列化的配置与日志领域模型。
- `Services`：配置持久化、日志持久化、文件系统更名和调度职责分离。
- `ViewModels`：使用 CommunityToolkit.Mvvm 管理页面状态与命令。
- `Views`：仅处理导航、系统文件夹选择器和控件事件的展示层。

本机数据位于 `%LOCALAPPDATA%\FolderRename`。计划仅在应用正在运行时触发；关闭应用会暂停监控。

## Build

需要 Windows 10 1809+、.NET 8 SDK 和 Visual Studio 的 **.NET desktop development** 与 **Windows application development** 工作负载：

```powershell
dotnet build FolderRename/FolderRename.csproj
```

此项目是**自包含的非 MSIX 应用**。普通 x64/ARM64 构建会将 Windows App SDK 所需的运行时文件随输出部署，因此不会依赖机器上已注册的 Windows App Runtime；这可避免启动阶段的 `REGDB_E_CLASSNOTREG` / “没有注册类”。发布特定架构时可显式指定 RID，例如：

```powershell
dotnet publish FolderRename/FolderRename.csproj -c Release -r win-x64
```

请在 Visual Studio 的“配置管理器”中选择与电脑一致的 `x64` 或 `ARM64` 平台；`AnyCPU` 不应用于运行 WinUI 3 应用。若构建阶段仍报告 `GetLatestMSVCVersion` 并引用不存在的 `VC\Tools\MSVC` 路径，请使用 Visual Studio Installer 的“修复”功能或安装 **Desktop development with C++** 工作负载。这是机器的 Visual Studio 安装路径/组件问题。
