namespace CSharpLearningProject.Library;

/// <summary>
/// 馆藏物品抽象基类
///
/// 演示知识点:
///   - 抽象类 (abstract class): 不能实例化, 作为基类
///   - 抽象属性 (abstract property): 子类必须实现
///   - 虚方法 (virtual method): 子类可选重写
///   - 封装: protected 字段 + public 属性
///   - 接口实现: IBorrowable, ISearchable
///   - 事件 (event): 借还时触发
///   - init-only 属性 (C# 9+): 构造后不可改
///   - 重写 Object.Equals/GetHashCode/ToString
/// </summary>
public abstract class LibraryItem : IBorrowable, ISearchable
{
    // ===== 封装: 私有/受保护字段 + 公开属性 =====

    public string Id { get; init; }          // init-only: 构造后不可改
    public string Title { get; set; }
    public DateTime AddedDate { get; init; }
    public string Category { get; set; } = "通用";

    // protected: 子类可访问, 外部不可
    protected int _totalCopies;
    protected int _borrowedCount;

    // ===== 抽象属性: 子类必须实现 =====
    public abstract string ItemType { get; }

    // ===== 构造函数 =====
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

    // ===== 计算属性 (表达式体属性) =====

    public bool IsAvailable => _borrowedCount < _totalCopies;
    public int AvailableCopies => _totalCopies - _borrowedCount;
    public double BorrowRate => _totalCopies == 0 ? 0 : (double)_borrowedCount / _totalCopies;

    // ===== 虚方法: 子类可重写 =====

    public virtual void DisplayInfo()
    {
        Console.WriteLine($"  [{ItemType}] {Id} 《{Title}》" +
            $" (可借 {AvailableCopies}/{_totalCopies}) [{Category}]");
    }

    // ===== IBorrowable 实现 =====

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

    // ===== ISearchable 实现 (虚方法, 子类可扩展) =====

    public virtual bool Match(string keyword) =>
        Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
        Id.Contains(keyword, StringComparison.OrdinalIgnoreCase);

    // ===== 事件 =====

    public event EventHandler<BorrowEventArgs>? BorrowEvent;

    protected virtual void OnBorrowEvent(string reader, string action)
    {
        BorrowEvent?.Invoke(this, new BorrowEventArgs(this, reader, action));
    }

    // ===== 重写 Object 方法 =====

    public override string ToString() => $"[{ItemType}]《{Title}》({Id})";
    public override bool Equals(object? obj) => obj is LibraryItem other && Id == other.Id;
    public override int GetHashCode() => Id.GetHashCode();
}
