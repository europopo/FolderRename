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

应用的普通构建不再固定运行时标识（RID），因此不应要求 Visual Studio 的 C++/MSVC 工具集。若要创建特定架构的发布包，请在发布时显式指定 RID，例如：

```powershell
dotnet publish FolderRename/FolderRename.csproj -c Release -r win-x64
```

如果现有 Visual Studio 仍在构建时报告 `GetLatestMSVCVersion` 并引用不存在的 `VC\Tools\MSVC` 路径，请使用 Visual Studio Installer 的“修复”功能或安装 **Desktop development with C++** 工作负载；这是该机器的 Visual Studio 安装路径/组件问题，而不是应用运行时所需组件。
