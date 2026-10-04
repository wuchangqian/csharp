# C# 完整学习项目 —— 图书馆管理系统

我和 wxy 学 C# 的项目。

## 运行方式

```bash
cd CSharpLearningProject
dotnet run
```

或用 VSCode 打开项目文件夹, 按 F5 调试运行。

## 项目结构

```
CSharpLearningProject/
│
├── Program.cs                    # 入口: Avalonia 启动
├── App.axaml / .cs               # 应用配置 (暗色主题)
├── Styles.axaml                  # 菜单按钮样式
├── ViewLocator.cs                # MVVM View-ViewModel 映射
├── OutputWriter.cs               # Console 输出重定向到 GUI
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

## 设计原则

1. 每个类一个文件: Library/ 下 11 个文件, 每个文件只负责一个类型
2. 模块注册表替代 switch: MainViewModel 用数组注册模块, 新增模块只加一行
3. Console 输出重定向: OutputWriter 拦截 Console.WriteLine, 转发到 GUI
4. 异步执行: 模块在 Task.Run 中执行, 不阻塞 UI
5. 命名空间隔离: Lessons 和 Library 分开, 避免同名类型冲突

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
