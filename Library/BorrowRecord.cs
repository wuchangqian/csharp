using System.Text.Json;
using System.Text.Json.Serialization;

namespace CSharpLearningProject.Library;

/// <summary>
/// 借阅记录 —— Record 类型 (C# 9+)
/// 不可变, 自带值相等, with 表达式, 解构
/// </summary>
public record BorrowRecord(string ItemId, string ReaderId, DateTime BorrowDate, DateTime? ReturnDate = null)
{
    public bool IsActive => ReturnDate == null;
    public int DaysBorrowed => (int)((ReturnDate ?? DateTime.Now) - BorrowDate).TotalDays;
}
