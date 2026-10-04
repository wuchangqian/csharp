namespace CSharpLearningProject.Library;

// ===== 搜索策略 (策略模式) =====
// 不同策略实现不同的搜索逻辑, 通过接口多态切换

/// <summary>按标题搜索</summary>
public class TitleSearchStrategy : ISearchStrategy
{
    public string Name => "按标题";
    public bool Match(LibraryItem item, string keyword) =>
        item.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase);
}

/// <summary>按作者搜索 (仅对图书有效)</summary>
public class AuthorSearchStrategy : ISearchStrategy
{
    public string Name => "按作者";
    public bool Match(LibraryItem item, string keyword) =>
        item is Book b && b.Author.Contains(keyword, StringComparison.OrdinalIgnoreCase);
}

/// <summary>按编号搜索</summary>
public class IdSearchStrategy : ISearchStrategy
{
    public string Name => "按编号";
    public bool Match(LibraryItem item, string keyword) =>
        item.Id.Contains(keyword, StringComparison.OrdinalIgnoreCase);
}
