namespace CSharpLearningProject.Library;

/// <summary>
/// 借还事件参数
/// 当馆藏被借出或归还时, 携带相关信息触发事件
/// </summary>
public class BorrowEventArgs : EventArgs
{
    public LibraryItem Item { get; }
    public string Reader { get; }
    public DateTime Time { get; }
    public string Action { get; }  // "借出" | "归还"

    public BorrowEventArgs(LibraryItem item, string reader, string action)
    {
        Item = item;
        Reader = reader;
        Action = action;
        Time = DateTime.Now;
    }
}
