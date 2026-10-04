namespace CSharpLearningProject.Library;

/// <summary>
/// 泛型仓储 —— 管理馆藏的增删改查
///
/// 演示知识点:
///   - 泛型类 (Generic Class): Repository&lt;T&gt;
///   - 泛型约束 (where T : LibraryItem)
///   - LINQ 查询: Where, GroupBy, OrderBy, Skip/Take
///   - IEnumerable&lt;T&gt; 返回值: 延迟执行
/// </summary>
public class Repository<T> where T : LibraryItem
{
    protected List<T> _items = new();

    public int Count => _items.Count;

    /// <summary>添加 (重复 ID 抛异常)</summary>
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

    // ===== LINQ 查询方法 =====

    public IEnumerable<T> Find(Func<T, bool> predicate) =>
        _items.Where(predicate);

    public IEnumerable<T> GetAvailable() =>
        _items.Where(i => i.IsAvailable);

    public IEnumerable<IGrouping<string, T>> GroupByCategory() =>
        _items.GroupBy(i => i.Category);

    public Dictionary<string, int> CountByCategory() =>
        _items.GroupBy(i => i.Category)
              .ToDictionary(g => g.Key, g => g.Count());

    // ===== 排序与分页 =====

    public IEnumerable<T> GetSorted<TKey>(Func<T, TKey> keySelector, bool descending = false) =>
        descending ? _items.OrderByDescending(keySelector) : _items.OrderBy(keySelector);

    public IEnumerable<T> GetPage(int page, int pageSize) =>
        _items.Skip((page - 1) * pageSize).Take(pageSize);
}
