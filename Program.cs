using CSharpLearningProject.Lessons;
using CSharpLearningProject.Library;
// 解决 Lessons 和 Library 命名空间同名类的冲突, Library 命名空间优先
using LibraryItem = CSharpLearningProject.Library.LibraryItem;
using LibraryException = CSharpLearningProject.Library.LibraryException;

// ============================================================
// C# 完整学习项目 —— 主程序入口
// 通过菜单选择各模块运行, 综合演示图书馆系统
// ============================================================

while (true)
{
    Console.Clear();
    Console.WriteLine("════════════════════════════════════════════════");
    Console.WriteLine("   C# 完整学习项目 —— 图书馆管理系统");
    Console.WriteLine("════════════════════════════════════════════════");
    Console.WriteLine();
    Console.WriteLine("  [基础语法]");
    Console.WriteLine("   1. 变量与数据类型        2. 控制流");
    Console.WriteLine("   3. 方法与参数            4. 数组");
    Console.WriteLine("   5. 字符串                6. 集合");
    Console.WriteLine();
    Console.WriteLine("  [面向对象]");
    Console.WriteLine("   7. 类与对象 (封装)       8. 继承");
    Console.WriteLine("   9. 多态/抽象/接口       10. 委托与事件");
    Console.WriteLine("  11. 泛型                 12. LINQ");
    Console.WriteLine();
    Console.WriteLine("  [进阶]");
    Console.WriteLine("  13. 异步编程 async/await  14. 异常处理");
    Console.WriteLine("  15. 文件 I/O 与 JSON      16. 高级类型 Record/Pattern");
    Console.WriteLine("  17. 特性与反射");
    Console.WriteLine();
    Console.WriteLine("  [综合应用]");
    Console.WriteLine("   0. 图书馆管理系统 (整合演示)");
    Console.WriteLine();
    Console.WriteLine("  Q. 退出");
    Console.WriteLine("════════════════════════════════════════════════");
    Console.Write("请选择: ");

    string? input = Console.ReadLine();
    Console.WriteLine();

    if (string.IsNullOrWhiteSpace(input) || input.Trim().ToUpper() == "Q")
        break;

    try
    {
        switch (input.Trim())
        {
            case "1": L01_Basics.Run(); break;
            case "2": L02_ControlFlow.Run(); break;
            case "3": L03_Methods.Run(); break;
            case "4": L04_Arrays.Run(); break;
            case "5": L05_Strings.Run(); break;
            case "6": L06_Collections.Run(); break;
            case "7": L07_Classes.Run(); break;
            case "8": L08_Inheritance.Run(); break;
            case "9": L09_Polymorphism.Run(); break;
            case "10": L10_DelegatesEvents.Run(); break;
            case "11": L11_GenericClass.Run(); break;
            case "12": L12_LINQ.Run(); break;
            case "13":
                L13_Async.RunAsync().Wait();
                break;
            case "14": L14_Exceptions.Run(); break;
            case "15": L15_FileIO.Run(); break;
            case "16": L16_AdvancedTypes.Run(); break;
            case "17": L17_Reflection.Run(); break;
            case "0": RunLibraryDemo(); break;
            default:
                Console.WriteLine("  无效选择");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  [错误] {ex.Message}");
    }

    Console.WriteLine("\n按回车返回菜单...");
    Console.ReadLine();
}

// ============================================================
// 综合应用: 图书馆管理系统演示
// ============================================================
void RunLibraryDemo()
{
    L01_Basics.PrintHeader("综合应用: 图书馆管理系统");

    var lib = Library.Instance;

    // 订阅全局借还事件
    Library.Instance.OnBorrowAction += (sender, e) =>
    {
        Console.WriteLine($"    [广播] {e.Reader} {e.Action} 《{e.Item.Title}》 at {e.Time:HH:mm:ss}");
    };

    // 1. 使用工厂模式添加馆藏
    Console.WriteLine("=== 添加馆藏 (工厂模式) ===");
    var items = new LibraryItem[]
    {
        Library.CreateItem("book", "BK-001", "C#入门经典", "张三", "978-001", 3),
        Library.CreateItem("book", "BK-002", "LINQ实战", "李四", "978-002", 2),
        Library.CreateItem("book", "BK-003", "设计模式", "王五", "978-003", 1),
        Library.CreateItem("magazine", "MG-001", "程序员", 2024, 8, 5),
        Library.CreateItem("dvd", "DVD-001", ".NET视频教程", 120, 2),
    };

    foreach (var item in items)
        lib.AddItem(item);

    Console.WriteLine($"  添加完成, 共 {lib.ItemCount} 件\n");

    // 2. 显示全部 (多态)
    Console.WriteLine("=== 全部馆藏 (多态 DisplayInfo) ===");
    lib.DisplayAll();

    // 3. 搜索 (策略模式)
    Console.WriteLine("\n=== 搜索: 按作者 '张' (策略模式) ===");
    var results = lib.Search("张", new AuthorSearchStrategy());
    foreach (var r in results)
        r.DisplayInfo();

    Console.WriteLine("\n=== 搜索: 按标题 'C#' ===");
    foreach (var r in lib.Search("C#", new TitleSearchStrategy()))
        r.DisplayInfo();

    // 4. 借阅 (事件触发)
    Console.WriteLine("\n=== 借阅操作 (事件驱动) ===");
    try
    {
        lib.BorrowItem("BK-001", "reader-001");
        lib.BorrowItem("BK-001", "reader-002");
        lib.BorrowItem("BK-002", "reader-001");
        lib.BorrowItem("BK-003", "reader-003");
        lib.BorrowItem("BK-003", "reader-004");  // 会抛异常 (只有1本)
    }
    catch (LibraryException ex)
    {
        Console.WriteLine($"  X {ex.Message} (代码: {ex.Code})");
    }

    // 5. 归还
    Console.WriteLine("\n=== 归还操作 ===");
    lib.ReturnItem("BK-001", "reader-001");

    // 6. 统计报告 (LINQ 聚合)
    Console.WriteLine("\n=== 统计报告 (LINQ) ===");
    lib.PrintStatistics();

    // 7. JSON 持久化
    Console.WriteLine("\n=== JSON 持久化 ===");
    string jsonPath = Path.Combine(Path.GetTempPath(), "CSharpLearning", "library_data.json");
    Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);
    lib.SaveToFile(jsonPath);

    Console.WriteLine("\n=== 综合演示结束 ===");
}
