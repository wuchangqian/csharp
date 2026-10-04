# C# 完整学习项目 —— 图书馆管理系统

我和 wxy 学 C# 的项目。

本项目是一个基于 **Avalonia UI** 的跨平台 .NET 桌面 GUI 应用，采用 MVVM 模式，内置 17 个渐进式 C# 课程模块（基础 → 高级）外加一个综合"图书管理系统"演示。应用界面为深色主题：左侧菜单列出 18 个按钮（1-17 课 + 综合演示 0），右侧输出面板显示运行结果。点击菜单后，对应模块的 `Console.WriteLine` 输出会被拦截并重定向到 GUI 文本框。

## 技术栈

| 项 | 内容 |
|---|---|
| 语言 | C# 10.0+ |
| 框架 | Avalonia UI 12.1.3（跨平台 .NET GUI） |
| MVVM | CommunityToolkit.Mvvm 8.4.2（源生成器 `[ObservableProperty]` / `[RelayCommand]`） |
| 目标框架 | `net10.0`，`WinExe` 输出 |
| 主题 | FluentTheme，深色（`RequestedThemeVariant="Dark"`） |

### NuGet 依赖

- `Avalonia` 12.1.3
- `Avalonia.Desktop` 12.1.3
- `Avalonia.Themes.Fluent` 12.1.3
- `Avalonia.Fonts.Inter` 12.1.3
- `AvaloniaUI.DiagnosticsSupport` 2.2.3（仅 Debug）
- `CommunityToolkit.Mvvm` 8.4.2

无 `.sln` 文件，单项目仓库。

## 运行方式

### 前置要求

需安装 **.NET 10 SDK**（10.0.401+）。检查版本：

```bash
dotnet --version
```

### 方式一：命令行运行（最简单）

```bash
cd d:\git\csharp
dotnet run
```

首次运行会自动还原 NuGet 包，然后启动 GUI 窗口。

### 方式二：仅构建

```bash
dotnet build d:\git\csharp
```

构建产物位于 `bin/Debug/net10.0/CSharpLearningProject.dll`。

### 方式三：VSCode 调试

用 VSCode 打开项目文件夹，按 **F5**。`.vscode/launch.json` 启动 DLL，`tasks.json` 的 `build` 任务作为预启动任务。

## 使用方法

启动后弹出 1000×650 的深色窗口：

1. 左侧菜单点击任意编号按钮（1-17 为课程，0 为综合图书管理演示）
2. 右侧查看该模块的控制台输出
3. 顶部"清空输出"按钮重置面板

## 项目结构

```
CSharpLearningProject/
│
├── Program.cs                    # 入口: Avalonia 启动
├── App.axaml / .cs               # 应用配置 (暗色主题)
├── Styles.axaml                  # 菜单按钮样式
├── ViewLocator.cs                # MVVM View-ViewModel 映射
├── OutputWriter.cs               # Console 输出重定向到 GUI
├── CSharpLearningProject.csproj  # 项目文件 (net10.0, WinExe)
├── app.manifest                  # Windows 应用清单
│
├── Lessons/                      # 17 个学习模块
│   ├── L01_Basics.cs             # 01: 基础语法 (变量/类型/运算符)
│   ├── L02_ControlFlow.cs        # 02: 控制流 (if/switch/for/while)
│   ├── L03_Methods.cs            # 03: 方法 (ref/out/in/params)
│   ├── L04_Arrays.cs             # 04: 数组 (多维/交错)
│   ├── L05_Strings.cs            # 05: 字符串 (StringBuilder/正则)
│   ├── L06_Collections.cs        # 06: 集合 (List/Dict/HashSet)
│   ├── L07_Classes.cs            # 07: 类与对象 (封装)
│   ├── L08_Inheritance.cs        # 08: 继承 (virtual/override/base)
│   ├── L09_Polymorphism.cs       # 09: 多态/抽象/接口
│   ├── L10_DelegatesEvents.cs    # 10: 委托/事件/Lambda
│   ├── L11_GenericClass.cs        # 11: 泛型 (约束/协变逆变)
│   ├── L12_LINQ.cs               # 12: LINQ (查询/方法语法)
│   ├── L13_Async.cs              # 13: 异步 (async/await/Task)
│   ├── L14_Exceptions.cs         # 14: 异常处理 (自定义异常)
│   ├── L15_FileIO.cs             # 15: 文件 I/O + JSON 序列化
│   ├── L16_AdvancedTypes.cs      # 16: Record/Pattern/struct/enum
│   └── L17_Reflection.cs         # 17: 特性 + 反射
│
├── Library/                      # 综合应用: 图书馆管理系统
│   ├── Interfaces.cs             #   接口 (IBorrowable/ISearchable/ISearchStrategy)
│   ├── Exceptions.cs             #   自定义异常体系
│   ├── BorrowEventArgs.cs        #   借还事件参数
│   ├── LibraryItem.cs            #   抽象基类 (封装/事件/Object重写)
│   ├── Book.cs                   #   图书 (继承/override)
│   ├── Magazine.cs               #   期刊 + DVD (sealed)
│   ├── SearchStrategies.cs       #   搜索策略 (策略模式)
│   ├── Repository.cs             #   泛型仓储 (泛型 + LINQ)
│   ├── BorrowRecord.cs           #   借阅记录 (Record 类型)
│   ├── Library.cs                #   图书馆核心 (单例/工厂/事件/JSON)
│   └── LibraryDemo.cs            #   综合演示入口
│
├── ViewModels/                   # MVVM ViewModel
│   ├── MainViewModel.cs          #   菜单命令 → 模块执行 → 输出重定向
│   └── ViewModelBase.cs          #   ViewModel 基类
│
└── Views/                        # Avalonia 界面
    ├── MainWindow.axaml          #   左侧菜单 + 右侧输出区
    └── MainWindow.axaml.cs       #   代码后置
```

## 代码架构

```
┌─────────────── Views (界面) ──────────────┐
│         MainWindow.axaml                  │
│     左侧菜单    │    右侧输出文本框         │
└────────┬──────────────────────────────────┘
         │ 数据绑定 (MVVM)
┌────────▼──────────────────────────────────┐
│            ViewModels                      │
│         MainViewModel                       │
│  RunModuleCommand → Console 重定向          │
└────────┬──────────────────────────────────┘
         │ 调用
┌────────▼──────────────────────────────────┐
│             Lessons                        │
│  L01~L17 (每个模块独立的 Run() 方法)       │
└───────────────────────────────────────────┘

┌─────────────── Library ──────────────────┐
│  接口 → 异常 → 事件 → 抽象基类             │
│  → 具体类(Book/Magazine/Dvd)              │
│  → 策略 → 仓储 → 图书馆核心                │
│  → 综合演示(LibraryDemo)                   │
└───────────────────────────────────────────┘
```

## 启动流程

1. `Program.Main` → `AppBuilder.Configure<App>()` → 桌面生命周期
2. `App.OnFrameworkInitializationCompleted` 创建 `MainWindow`，`DataContext = new MainViewModel()`
3. `ViewLocator` 约定映射：`MainViewModel` → `MainWindow`（类型名中 "ViewModel" 替换为 "View"）

## 核心执行机制

- **模块注册表**：`MainViewModel` 持有 `(string Id, string Name, Action Run)[] _modules` 数组，共 18 项（L01-L17 + 综合演示 "0"）。新增模块只需加一行
- **执行流程**：`RunModuleCommand` 清空输出 → `Console.SetOut(new OutputWriter(...))` → `Task.Run` 跑模块（不阻塞 UI）→ `finally` 恢复 `Console.Out`
- **输出重定向**：`OutputWriter`（`StringWriter` 子类）将写入累积到缓冲，通过 `Action<string>` 回调 → `Dispatcher.UIThread.Post` → `OutputText` 绑定属性
- **Library 演示**：工厂创建条目 → 策略模式搜索 → 借还事件 → LINQ 统计 → JSON 持久化到 `%TEMP%/CSharpLearning/library_data.json`

## 设计原则

1. 每个类一个文件: Library/ 下 11 个文件, 每个文件只负责一个类型
2. 模块注册表替代 switch: MainViewModel 用数组注册模块, 新增模块只加一行
3. Console 输出重定向: OutputWriter 拦截 Console.WriteLine, 转发到 GUI
4. 异步执行: 模块在 Task.Run 中执行, 不阻塞 UI
5. 命名空间隔离: Lessons 和 Library 分开, 避免同名类型冲突

## 配置说明

- 无需 `appsettings.json` 或其他运行时配置文件，应用自包含
- 无环境变量、密钥或外部服务依赖
- `App.axaml` 硬编码 `RequestedThemeVariant="Dark"`
- `app.manifest` 声明 Windows 10 兼容性（其他平台无影响）
- `.gitignore` 排除 `bin/`、`obj/`、`*.txt`
- 唯一文件系统副作用：运行模块 0 时向临时目录写入 JSON 数据文件

## 知识点覆盖

| 阶段 | 模块 | 核心知识点 |
|------|------|-----------|
| 语言基础 | 01-06 | 值/引用类型、控制流、方法参数、数组、字符串、集合 |
| 面向对象 | 07-12 | 封装、继承、多态、接口、委托/事件、泛型、LINQ |
| 进阶特性 | 13-17 | async/await、异常体系、文件I/O+JSON、Record/Pattern、反射 |
| 综合应用 | Library | 单例/工厂/策略模式、事件驱动、泛型仓储、JSON持久化 |

## 学习路径

1. 先跑综合应用 (选 0) 看整体效果
2. 按模块 01→17 顺序学习, 每个模块读代码 + 运行
3. 阅读 Library/ 下的综合应用代码, 理解各知识点如何整合
4. 尝试修改代码: 比如添加新的馆藏类型、新的搜索策略

## 备注

`学习指南.md` 描述的是更早的 CLI 菜单版本（提到文本菜单和 `Program.cs` 作为"菜单入口"），当前代码已是 Avalonia GUI 版本。本文档为最新，反映 GUI 架构；学习指南中的"知识点对照表"仍准确，可作为每课知识点的参考。
