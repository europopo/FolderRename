# FolderRename

一个基于 **WinUI 3** 的 Windows 文件夹定时更名工具。通过 Fluent 风格的分区表单配置目标文件夹、日期命名格式、一次性/周期计划及失败重试；成功、失败和重试都会保存到独立的日志页面。

## 架构

- `Models`：可序列化的配置与日志领域模型。
- `Services`：配置持久化、日志持久化、文件系统更名和调度职责分离。
- `ViewModels`：使用 CommunityToolkit.Mvvm 管理页面状态与命令。
- `Views`：仅处理导航、系统文件夹选择器和控件事件的展示层。

本机数据位于 `%LOCALAPPDATA%\FolderRename`。计划仅在应用正在运行时触发；关闭应用会暂停监控。

## Build

需要 Windows 10 1809+、.NET 8 SDK 和 Visual Studio 的 Windows App SDK/WinUI 工作负载：

```powershell
dotnet build FolderRename/FolderRename.csproj
```
