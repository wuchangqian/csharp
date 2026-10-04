namespace CSharpLearningProject.Library;

/// <summary>
/// 图书 —— 继承 LibraryItem
/// 演示: 继承、override、base 调用、扩展属性
/// </summary>
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

    // 扩展搜索: 在基类基础上增加按作者和ISBN匹配
    public override bool Match(string keyword) =>
        base.Match(keyword) ||
        Author.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
        (Isbn?.Contains(keyword) ?? false);
}
