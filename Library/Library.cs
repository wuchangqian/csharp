using System.Text.Json;
using System.Text.Json.Serialization;

namespace CSharpLearningProject.Library;

/// <summary>
/// 图书馆核心 —— 单例模式
///
/// 整合所有知识点:
///   - 单例模式 (Lazy<T> 线程安全)
///   - 简单工厂模式 (CreateItem)
///   - 组合 (Library 拥有 Repository)
///   - 事件 (OnBorrowAction)
///   - 策略模式 (Search 接受 ISearchStrategy)
///   - LINQ 聚合 (PrintStatistics)
///   - JSON 持久化 (SaveToFile)
/// </summary>
public class Library
{
    // ===== 单例 (懒汉式, 线程安全) =====
    private static readonly Lazy<Library> _instance = new(() => new Library());
    public static Library Instance => _instance.Value;

    // ===== 组合: Library 拥有 Repository =====
    private readonly Repository<LibraryItem> _repo;
    private readonly Dictionary<string, int> _borrowRecords = new();

    // ===== 事件 =====
    public event EventHandler<BorrowEventArgs>? OnBorrowAction;

    // 私有构造 (单例)
    private Library()
    {
        _repo = new Repository<LibraryItem>();
    }

    // ===== 简单工厂模式 =====
    public static LibraryItem CreateItem(string type, params object[] args) =>
        type.ToLower() switch
        {
            "book" => new Book((string)args[0], (string)args[1], (string)args[2], (string)args[3],
                args.Length > 4 ? (int)args[4] : 1),
            "magazine" => new Magazine((string)args[0], (string)args[1], (int)args[2], (int)args[3],
                args.Length > 4 ? (int)args[4] : 1),
            "dvd" => new Dvd((string)args[0], (string)args[1], (int)args[2],
                args.Length > 3 ? (int)args[3] : 1),
            _ => throw new ArgumentException($"未知类型: {type}")
        };

    // ===== 添加馆藏 =====
    public void AddItem(LibraryItem item)
    {
        _repo.Add(item);
        // 订阅该物品的借还事件, 转发到图书馆全局事件
        item.BorrowEvent += (sender, e) => OnBorrowAction?.Invoke(sender, e);
    }

    // ===== 借阅 =====
    public void BorrowItem(string itemId, string readerId)
    {
        var item = _repo.GetById(itemId) ?? throw new ItemNotFoundException(itemId);
        if (!item.Borrow(readerId))
            throw new ItemNotAvailableException(item.Title);

        _borrowRecords.TryGetValue(readerId, out int count);
        _borrowRecords[readerId] = count + 1;

        Console.WriteLine($"  ✓ {readerId} 借阅了 {item.Title}");
    }

    // ===== 归还 =====
    public void ReturnItem(string itemId, string readerId)
    {
        var item = _repo.GetById(itemId) ?? throw new ItemNotFoundException(itemId);
        if (item.Return(readerId))
        {
            Console.WriteLine($"  ✓ {readerId} 归还了 {item.Title}");
        }
    }

    // ===== 搜索 (策略模式) =====
    public IEnumerable<LibraryItem> Search(string keyword, ISearchStrategy? strategy = null)
    {
        strategy ??= new TitleSearchStrategy();
        return _repo.GetAll().Where(i => strategy.Match(i, keyword));
    }

    // ===== 显示全部 (多态) =====
    public void DisplayAll()
    {
        foreach (var item in _repo.GetSorted(i => i.AddedDate))
            item.DisplayInfo();
    }

    // ===== 统计报告 (LINQ 聚合) =====
    public void PrintStatistics()
    {
        var all = _repo.GetAll().ToList();
        if (all.Count == 0) { Console.WriteLine("  (空馆)"); return; }

        Console.WriteLine($"  总馆藏: {all.Count} 件");
        Console.WriteLine($"  可借: {all.Count(i => i.IsAvailable)} 件");

        Console.WriteLine("  按类别:");
        foreach (var g in _repo.GroupByCategory().OrderByDescending(g => g.Count()))
            Console.WriteLine($"    {g.Key}: {g.Count()} 件");

        Console.WriteLine("  借阅最多的:");
        foreach (var item in all.OrderByDescending(i => i.BorrowRate).Take(3))
            Console.WriteLine($"    {item.Title}: {item.BorrowRate:P0}");

        if (_borrowRecords.Count > 0)
        {
            Console.WriteLine("  读者借阅排行:");
            foreach (var kv in _borrowRecords.OrderByDescending(kv => kv.Value).Take(5))
                Console.WriteLine($"    {kv.Key}: {kv.Value} 本");
        }
    }

    // ===== JSON 持久化 =====
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public void SaveToFile(string path)
    {
        var data = _repo.GetAll().Select(i => new
        {
            Type = i.ItemType,
            Id = i.Id,
            Title = i.Title,
            Category = i.Category,
            Available = i.AvailableCopies,
        });
        string json = JsonSerializer.Serialize(data, JsonOpts);
        File.WriteAllText(path, json);
        Console.WriteLine($"  ✓ 已保存到 {path}");
    }

    public int ItemCount => _repo.Count;
}
