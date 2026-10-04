using System.Text.Json;
using System.Text.Json.Serialization;

namespace CSharpLearningProject.Library
{
    // ============================================================
    // 综合应用: 图书馆管理系统
    // 整合所有知识点: OOP、泛型、LINQ、事件、异常、文件I/O、反射
    // ============================================================

    #region 接口层 (抽象)

    /// <summary>可借阅接口</summary>
    public interface IBorrowable
    {
        bool IsAvailable { get; }
        bool Borrow(string reader);
        bool Return(string reader);
    }

    /// <summary>可搜索接口</summary>
    public interface ISearchable
    {
        bool Match(string keyword);
    }

    /// <summary>搜索策略接口 (策略模式)</summary>
    public interface ISearchStrategy
    {
        string Name { get; }
        bool Match(LibraryItem item, string keyword);
    }

    #endregion

    #region 自定义异常

    public class LibraryException : Exception
    {
        public string Code { get; }
        public LibraryException(string message, string code = "LIB-ERR") : base(message) => Code = code;
    }

    public class ItemNotFoundException : LibraryException
    {
        public ItemNotFoundException(string id) : base($"馆藏 {id} 不存在", "ITEM-404") { }
    }

    public class ItemNotAvailableException : LibraryException
    {
        public ItemNotAvailableException(string title) : base($"《{title}》已被借出", "ITEM-409") { }
    }

    public class DuplicateItemException : LibraryException
    {
        public DuplicateItemException(string id) : base($"馆藏 {id} 已存在", "ITEM-409") { }
    }

    #endregion

    #region 事件

    public class BorrowEventArgs : EventArgs
    {
        public LibraryItem Item { get; }
        public string Reader { get; }
        public DateTime Time { get; }
        public string Action { get; }  // "借出" | "归还"

        public BorrowEventArgs(LibraryItem item, string reader, string action)
        {
            Item = item; Reader = reader; Action = action; Time = DateTime.Now;
        }
    }

    #endregion

    #region 抽象基类

    /// <summary>
    /// 馆藏物品抽象基类
    /// 演示: 抽象类、抽象属性/方法、虚方法、封装
    /// </summary>
    public abstract class LibraryItem : IBorrowable, ISearchable
    {
        // 封装: 私有字段 + 公开属性
        public string Id { get; init; }          // init-only (C# 9+): 构造后不可改
        public string Title { get; set; }
        public DateTime AddedDate { get; init; }
        public string Category { get; set; } = "通用";

        protected int _totalCopies;
        protected int _borrowedCount;

        // 抽象属性: 子类必须实现
        public abstract string ItemType { get; }

        // 构造函数
        protected LibraryItem(string id, string title, int copies = 1)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("ID 不能为空", nameof(id));
            Id = id;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            _totalCopies = copies;
            _borrowedCount = 0;
            AddedDate = DateTime.Now;
        }

        // 计算属性 (表达式体)
        public bool IsAvailable => _borrowedCount < _totalCopies;
        public int AvailableCopies => _totalCopies - _borrowedCount;
        public double BorrowRate => _totalCopies == 0 ? 0 : (double)_borrowedCount / _totalCopies;

        // 虚方法: 子类可重写
        public virtual void DisplayInfo()
        {
            Console.WriteLine($"  [{ItemType}] {Id} 《{Title}》" +
                $" (可借 {AvailableCopies}/{_totalCopies}) [{Category}]");
        }

        // IBorrowable 实现
        public bool Borrow(string reader)
        {
            if (!IsAvailable) return false;
            _borrowedCount++;
            OnBorrowEvent(reader, "借出");
            return true;
        }

        public bool Return(string reader)
        {
            if (_borrowedCount == 0) return false;
            _borrowedCount--;
            OnBorrowEvent(reader, "归还");
            return true;
        }

        // ISearchable 实现 (虚方法, 子类可扩展)
        public virtual bool Match(string keyword) =>
            Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            Id.Contains(keyword, StringComparison.OrdinalIgnoreCase);

        // 事件
        public event EventHandler<BorrowEventArgs>? BorrowEvent;
        protected virtual void OnBorrowEvent(string reader, string action)
        {
            BorrowEvent?.Invoke(this, new BorrowEventArgs(this, reader, action));
        }

        // 重写 Object 方法
        public override string ToString() => $"[{ItemType}]《{Title}》({Id})";
        public override bool Equals(object? obj) => obj is LibraryItem other && Id == other.Id;
        public override int GetHashCode() => Id.GetHashCode();
    }

    #endregion

    #region 具体类 (继承 + 多态)

    public class Book : LibraryItem
    {
        public string Author { get; set; }
        public string Isbn { get; init; }
        public int? Year { get; set; }

        public Book(string id, string title, string author, string isbn, int copies = 1)
            : base(id, title, copies)
        {
            Author = author;
            Isbn = isbn;
            Category = "图书";
        }

        public override string ItemType => "图书";

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"    作者: {Author}, ISBN: {Isbn}, 年份: {Year?.ToString() ?? "未知"}");
        }

        public override bool Match(string keyword) =>
            base.Match(keyword) ||
            Author.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            (Isbn?.Contains(keyword) ?? false);
    }

    public sealed class Magazine : LibraryItem
    {
        public int IssueYear { get; }
        public int IssueNumber { get; }

        public Magazine(string id, string title, int year, int issue, int copies = 1)
            : base(id, title, copies)
        {
            IssueYear = year;
            IssueNumber = issue;
            Category = "期刊";
        }

        public override string ItemType => "期刊";

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"    {IssueYear}年第{IssueNumber}期");
        }
    }

    public sealed class Dvd : LibraryItem
    {
        public int DurationMinutes { get; }

        public Dvd(string id, string title, int duration, int copies = 1)
            : base(id, title, copies)
        {
            DurationMinutes = duration;
            Category = "音像";
        }

        public override string ItemType => "DVD";

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"    时长: {DurationMinutes} 分钟");
        }
    }

    #endregion

    #region 搜索策略 (策略模式)

    public class TitleSearchStrategy : ISearchStrategy
    {
        public string Name => "按标题";
        public bool Match(LibraryItem item, string keyword) =>
            item.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

    public class AuthorSearchStrategy : ISearchStrategy
    {
        public string Name => "按作者";
        public bool Match(LibraryItem item, string keyword) =>
            item is Book b && b.Author.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

    public class IdSearchStrategy : ISearchStrategy
    {
        public string Name => "按编号";
        public bool Match(LibraryItem item, string keyword) =>
            item.Id.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region 泛型仓储 (泛型 + LINQ)

    /// <summary>泛型仓储: 管理馆藏的 CRUD</summary>
    public class Repository<T> where T : LibraryItem
    {
        protected List<T> _items = new();

        public int Count => _items.Count;

        public virtual void Add(T item)
        {
            if (_items.Any(i => i.Id == item.Id))
                throw new DuplicateItemException(item.Id);
            _items.Add(item);
        }

        public bool Remove(string id) =>
            _items.RemoveAll(i => i.Id == id) > 0;

        public T? GetById(string id) =>
            _items.FirstOrDefault(i => i.Id == id);

        public IEnumerable<T> GetAll() => _items.AsReadOnly();

        // LINQ 查询
        public IEnumerable<T> Find(Func<T, bool> predicate) =>
            _items.Where(predicate);

        public IEnumerable<T> GetAvailable() =>
            _items.Where(i => i.IsAvailable);

        public IEnumerable<IGrouping<string, T>> GroupByCategory() =>
            _items.GroupBy(i => i.Category);

        public Dictionary<string, int> CountByCategory() =>
            _items.GroupBy(i => i.Category)
                  .ToDictionary(g => g.Key, g => g.Count());

        // 排序
        public IEnumerable<T> GetSorted<TKey>(Func<T, TKey> keySelector, bool descending = false) =>
            descending ? _items.OrderByDescending(keySelector) : _items.OrderBy(keySelector);

        // 分页
        public IEnumerable<T> GetPage(int page, int pageSize) =>
            _items.Skip((page - 1) * pageSize).Take(pageSize);
    }

    #endregion

    #region 图书馆核心 (单例 + 工厂 + 事件 + JSON 持久化)

    /// <summary>
    /// 图书馆: 单例模式, 整合所有功能
    /// </summary>
    public class Library
    {
        // ===== 单例 (懒汉式, 线程安全) =====
        private static readonly Lazy<Library> _instance = new(() => new Library());
        public static Library Instance => _instance.Value;

        // ===== 组合: Library 拥有 Repository =====
        private readonly Repository<LibraryItem> _repo;
        private readonly Dictionary<string, int> _borrowRecords = new();  // readerId → 借书数

        // ===== 事件 =====
        public event EventHandler<BorrowEventArgs>? OnBorrowAction;

        // 私有构造 (单例)
        private Library()
        {
            _repo = new Repository<LibraryItem>();
        }

        // ===== 工厂方法 (简单工厂模式) =====
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

        // ===== 添加 =====
        public void AddItem(LibraryItem item)
        {
            _repo.Add(item);
            // 订阅借还事件
            item.BorrowEvent += (sender, e) => OnBorrowAction?.Invoke(sender, e);
        }

        // ===== 借阅 =====
        public void BorrowItem(string itemId, string readerId)
        {
            var item = _repo.GetById(itemId) ?? throw new ItemNotFoundException(itemId);
            if (!item.Borrow(readerId))
                throw new ItemNotAvailableException(item.Title);

            // 记录借阅数
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
            var topBorrowed = all.OrderByDescending(i => i.BorrowRate).Take(3);
            foreach (var item in topBorrowed)
                Console.WriteLine($"    {item.Title}: {item.BorrowRate:P0}");

            if (_borrowRecords.Count > 0)
            {
                Console.WriteLine("  读者借阅排行:");
                foreach (var kv in _borrowRecords.OrderByDescending(kv => kv.Value).Take(5))
                    Console.WriteLine($"    {kv.Key}: {kv.Value} 本");
            }
        }

        // ===== 显示全部 (多态) =====
        public void DisplayAll()
        {
            foreach (var item in _repo.GetSorted(i => i.AddedDate))
                item.DisplayInfo();  // 多态: 不同类型不同显示
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
                TotalCopies = i.AvailableCopies,  // 简化
            });
            string json = JsonSerializer.Serialize(data, JsonOpts);
            File.WriteAllText(path, json);
            Console.WriteLine($"  ✓ 已保存到 {path}");
        }

        public int ItemCount => _repo.Count;
    }

    #endregion

    #region 借阅记录 (Record + 不可变)

    public record BorrowRecord(string ItemId, string ReaderId, DateTime BorrowDate, DateTime? ReturnDate = null)
    {
        public bool IsActive => ReturnDate == null;
        public int DaysBorrowed => (int)((ReturnDate ?? DateTime.Now) - BorrowDate).TotalDays;
    }

    #endregion
}
