namespace CSharpLearningProject.Library;

/// <summary>
/// 期刊 —— 继承 LibraryItem, sealed 不可再继承
/// 演示: sealed、构造函数参数传递
/// </summary>
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

/// <summary>
/// DVD —— 继承 LibraryItem, sealed 不可再继承
/// </summary>
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
