namespace CSharpLearningProject.Library;

/// <summary>
/// 图书馆自定义异常基类
/// 所有图书馆相关异常都继承此类, 携带错误代码
/// </summary>
public class LibraryException : Exception
{
    public string Code { get; }
    public LibraryException(string message, string code = "LIB-ERR") : base(message) => Code = code;
}

/// <summary>馆藏不存在</summary>
public class ItemNotFoundException : LibraryException
{
    public ItemNotFoundException(string id) : base($"馆藏 {id} 不存在", "ITEM-404") { }
}

/// <summary>馆藏已被借出</summary>
public class ItemNotAvailableException : LibraryException
{
    public ItemNotAvailableException(string title) : base($"《{title}》已被借出", "ITEM-409") { }
}

/// <summary>馆藏编号重复</summary>
public class DuplicateItemException : LibraryException
{
    public DuplicateItemException(string id) : base($"馆藏 {id} 已存在", "ITEM-409") { }
}
