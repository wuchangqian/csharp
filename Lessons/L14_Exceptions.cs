namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 14: 异常处理
    /// 知识点: try/catch/finally、异常过滤器、自定义异常、throw 表达式、using
    /// </summary>
    public static class L14_Exceptions
    {
        public static void Run()
        {
            L01_Basics.PrintHeader("模块 14: 异常处理");

            // ===== 1. 基本异常处理 =====
            try
            {
                int[] arr = { 1, 2, 3 };
                Console.WriteLine($"  arr[5] = {arr[5]}");  // 越界
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine($"  捕获越界异常: {ex.Message}");
            }

            // ===== 2. 多 catch =====
            // 顺序: 从子类到父类 (具体到宽泛)
            try
            {
                string? s = null;
                Console.WriteLine(s!.Length);  // null 引用异常
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine($"  捕获空引用: {ex.Message}");
            }
            catch (Exception ex)  // 最后兜底
            {
                Console.WriteLine($"  兜底: {ex.Message}");
            }

            // ===== 3. finally =====
            // 无论是否异常都会执行 (用于清理)
            FileStream? fs = null;
            try
            {
                fs = new FileStream("/tmp/test.txt", FileMode.OpenOrCreate);
                Console.WriteLine("  文件已打开");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  文件错误: {ex.Message}");
            }
            finally
            {
                fs?.Dispose();  // 确保关闭
                Console.WriteLine("  finally: 资源已释放");
            }

            // ===== 4. using 语句 (资源管理) =====
            // using = try/finally + Dispose 的语法糖
            using (var ms = new MemoryStream())
            {
                ms.WriteByte(65);
                Console.WriteLine($"  using: 写入 {ms.ToArray()[0]}");
            }  // 自动 Dispose

            // using 声明 (C# 8+): 不需要大括号, 离开作用域自动 Dispose
            using var ms2 = new MemoryStream();
            ms2.WriteByte(66);
            Console.WriteLine($"  using 声明: {ms2.ToArray()[0]}");

            // ===== 5. 异常过滤器 (when) =====
            // 在 catch 条件中加 when, 满足条件才捕获
            try
            {
                throw new HttpRequestException("404", null, System.Net.HttpStatusCode.NotFound);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"  过滤器: 404 Not Found");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                Console.WriteLine($"  过滤器: 500 Server Error");
            }

            // ===== 6. throw (重新抛出) =====
            try
            {
                try
                {
                    int d = 10 / int.Parse("0");
                }
                catch (DivideByZeroException ex)
                {
                    Console.WriteLine($"  内层捕获: {ex.Message}");
                    throw;  // 重新抛出, 保留原始堆栈
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  外层捕获: {ex.GetType().Name}");
            }

            // ===== 7. throw 表达式 (C# 7+) =====
            // 在表达式中 throw
            string? name = null;
            string displayName = name ?? throw new ArgumentNullException(nameof(name));
            // 上面会抛异常, 下面不会执行

            // ===== 8. 自定义异常 =====
            try
            {
                throw new LibraryException("图书已借出", "BK-001");
            }
            catch (LibraryException ex)
            {
                Console.WriteLine($"  自定义异常: {ex.Message} (代码: {ex.ErrorCode}, 图书: {ex.BookId})");
            }

            try
            {
                throw new OverdueBookException("BK-002", DateTime.Now.AddDays(-10));
            }
            catch (LibraryException ex)  // 子类异常被父类 catch 捕获
            {
                Console.WriteLine($"  多态异常: {ex.GetType().Name} - {ex.Message}");
            }

            // ===== 9. 异步异常 =====
            var task = L13_Async.RunAsync();  // 不 await, 用于保持 Run 为同步
            try
            {
                Task.Run(async () => await ThrowAsync()).Wait();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"  异步异常: {ex.InnerExceptions[0].Message}");
            }

            Console.WriteLine();
        }

        static async Task ThrowAsync()
        {
            await Task.Delay(10);
            throw new InvalidOperationException("异步操作失败");
        }
    }

    // ===== 自定义异常体系 =====
    // 继承 Exception, 建议实现序列化构造函数
    [Serializable]
    public class LibraryException : Exception
    {
        public string ErrorCode { get; }
        public string? BookId { get; }

        public LibraryException(string message, string? bookId = null, string? errorCode = "LIB-ERR")
            : base(message)
        {
            BookId = bookId;
            ErrorCode = errorCode ?? "LIB-ERR";
        }

        public LibraryException(string message, Exception inner, string? bookId = null)
            : base(message, inner)
        {
            BookId = bookId;
        }
    }

    // 异常继承: 子类异常
    public class OverdueBookException : LibraryException
    {
        public DateTime DueDate { get; }
        public int OverdueDays => (int)(DateTime.Now - DueDate).TotalDays;

        public OverdueBookException(string bookId, DateTime dueDate)
            : base($"图书 {bookId} 已逾期 {Math.Max(0, (int)(DateTime.Now - dueDate).TotalDays)} 天", bookId, "BK-OVERDUE")
        {
            DueDate = dueDate;
        }
    }

    public class BookNotFoundException : LibraryException
    {
        public BookNotFoundException(string bookId)
            : base($"图书 {bookId} 不存在", bookId, "BK-NOT-FOUND")
        {
        }
    }
}
