namespace CSharpLearningProject.Library;

/// <summary>
/// 可借阅接口
/// 定义借阅能力: 任何馆藏物品都可被借出和归还
/// </summary>
public interface IBorrowable
{
    bool IsAvailable { get; }
    bool Borrow(string reader);
    bool Return(string reader);
}

/// <summary>
/// 可搜索接口
/// 定义搜索能力: 可以按关键词匹配
/// </summary>
public interface ISearchable
{
    bool Match(string keyword);
}

/// <summary>
/// 搜索策略接口 (策略模式)
/// 不同的搜索策略实现不同的匹配逻辑
/// </summary>
public interface ISearchStrategy
{
    string Name { get; }
    bool Match(LibraryItem item, string keyword);
}
