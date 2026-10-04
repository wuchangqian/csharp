using CSharpLearningProject.Library;

namespace CSharpLearningProject.Library;

/// <summary>
/// 图书馆管理系统综合演示
/// 从 MainViewModel 中提取, 保持 ViewModel 精简
///
/// 演示流程:
///   工厂添加馆藏 → 多态展示 → 策略搜索 → 事件借还 → LINQ统计 → JSON持久化
/// </summary>
public static class LibraryDemo
{
    public static void Run()
    {
        var lib = Library.Instance;

        // 订阅全局借还事件
        lib.OnBorrowAction += (sender, e) =>
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
        foreach (var r in lib.Search("张", new AuthorSearchStrategy()))
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
}
