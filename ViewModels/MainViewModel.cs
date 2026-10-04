using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpLearningProject.AuthDemo;
using CSharpLearningProject.Lessons;
using CSharpLearningProject.Library;

namespace CSharpLearningProject.ViewModels;

/// <summary>
/// 主视图模型 —— 连接 UI 和学习模块
///
/// 职责:
///   1. 提供菜单命令 (RunModuleCommand, ClearCommand)
///   2. 重定向 Console 输出到 GUI
///   3. 异步执行模块, 不阻塞 UI
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    // ===== 绑定属性 (CommunityToolkit.Mvvm 自动生成公开属性) =====

    [ObservableProperty]
    private string _outputText = "";

    [ObservableProperty]
    private string _currentModuleName = "请选择模块";

    [ObservableProperty]
    private string _statusText = "就绪";

    // ===== 模块注册表 =====
    // 用数组替代 switch, 新增模块只需加一行
    private static readonly (string Id, string Name, Action Run)[] _modules =
    {
        ("1",  "模块 01: 基础语法",        () => L01_Basics.Run()),
        ("2",  "模块 02: 控制流",          () => L02_ControlFlow.Run()),
        ("3",  "模块 03: 方法与参数",      () => L03_Methods.Run()),
        ("4",  "模块 04: 数组",            () => L04_Arrays.Run()),
        ("5",  "模块 05: 字符串",          () => L05_Strings.Run()),
        ("6",  "模块 06: 集合",            () => L06_Collections.Run()),
        ("7",  "模块 07: 类与对象 (封装)", () => L07_Classes.Run()),
        ("8",  "模块 08: 继承",            () => L08_Inheritance.Run()),
        ("9",  "模块 09: 多态/抽象/接口",  () => L09_Polymorphism.Run()),
        ("10", "模块 10: 委托与事件",      () => L10_DelegatesEvents.Run()),
        ("11", "模块 11: 泛型",            () => L11_GenericClass.Run()),
        ("12", "模块 12: LINQ",            () => L12_LINQ.Run()),
        ("13", "模块 13: 异步编程",        () => L13_Async.RunAsync().Wait()),
        ("14", "模块 14: 异常处理",        () => L14_Exceptions.Run()),
        ("15", "模块 15: 文件 I/O",        () => L15_FileIO.Run()),
        ("16", "模块 16: 高级类型",        () => L16_AdvancedTypes.Run()),
        ("17", "模块 17: 特性与反射",      () => L17_Reflection.Run()),
        ("18", "模块 18: SQLite + 登录验证", () => AuthDemoEntry.Run()),
        ("0",  "综合应用: 图书馆管理系统", () => LibraryDemo.Run()),
    };

    // ===== 输出收集器 =====
    private readonly ObservableCollection<string> _outputLines = new();

    public MainViewModel()
    {
        _outputLines.CollectionChanged += (_, _) => UpdateOutput();
    }

    private void UpdateOutput()
    {
        Dispatcher.UIThread.Post(() =>
            OutputText = string.Join("", _outputLines));
    }

    private void AppendOutput(string text) => _outputLines.Add(text);

    private void ClearOutput()
    {
        _outputLines.Clear();
        OutputText = "";
    }

    // ===== 命令: 运行模块 =====
    [RelayCommand]
    private async Task RunModule(string moduleId)
    {
        ClearOutput();
        StatusText = "运行中...";

        // 查找模块
        var module = Array.Find(_modules, m => m.Id == moduleId);
        if (module.Name == null)
        {
            CurrentModuleName = "未知模块";
            StatusText = "出错";
            return;
        }

        CurrentModuleName = module.Name;

        // 重定向 Console 输出到 GUI
        var outputWriter = new OutputWriter(AppendOutput);
        var originalOut = Console.Out;
        Console.SetOut(outputWriter);

        try
        {
            await Task.Run(() =>
            {
                try { module.Run(); }
                catch (Exception ex) { AppendOutput($"\n[错误] {ex.Message}\n"); }
            });
            StatusText = "完成";
        }
        catch (Exception ex)
        {
            AppendOutput($"\n[错误] {ex.Message}\n");
            StatusText = "出错";
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    // ===== 命令: 清空输出 =====
    [RelayCommand]
    private void Clear()
    {
        ClearOutput();
        StatusText = "就绪";
    }
}
