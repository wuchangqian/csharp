namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 09: 多态、抽象类、接口
    /// 知识点: abstract、interface、is/as、向上/向下转型、接口默认实现、多态调度
    /// </summary>
    public static class L09_Polymorphism
    {
        public static void Run()
        {
            L01_Basics.PrintHeader("模块 09: 多态 / 抽象 / 接口");

            // ===== 1. 抽象类不能实例化, 只能实例化派生类 =====
            // LibraryItem item = new LibraryItem(); // 编译错误: 抽象类
            var book = new BookItem("978-001", "C#入门", "张三");
            var dvd = new DvdItem("DVD-001", "设计模式实战", 120);

            // ===== 2. 多态: 父类引用指向子类对象 =====
            // 编译时类型 = LibraryItem, 运行时类型 = BookItem
            // 调用虚方法时, 执行运行时类型的方法
            LibraryItem[] items = { book, dvd, new BookItem("978-002", "LINQ实战", "李四") };

            Console.WriteLine("=== 多态展示 ===");
            foreach (var item in items)
            {
                item.DisplayInfo();    // 每种类型执行自己的版本
                item.Checkout();       // 统一接口, 不同实现
                Console.WriteLine();
            }

            // ===== 3. 接口 =====
            // 接口 = 契约: "实现我的类必须提供这些方法"
            // 类比: USB 接口, 不管是什么设备, 插上就能用
            Console.WriteLine("=== 接口展示 ===");
            IBorrowable borrowable = book;
            borrowable.Borrow("张三");
            borrowable.Return("张三");

            ISearchable searchable = book;
            bool found = searchable.Match("C#");
            Console.WriteLine($"搜索 'C#': {found}");

            // ===== 4. 接口多实现 =====
            // 一个类可以实现多个接口 (C# 不允许多继承类, 但允许多实现接口)
            var mag = new Magazine("MAG-001", "程序员", 2024, 8);
            mag.Borrow("李四");       // IBorrowable
            bool match = mag.Match("程序"); // ISearchable
            mag.DisplayInfo();         // 继承自 LibraryItem
            Console.WriteLine($"匹配 '程序': {match}");

            // ===== 5. is 和 as 运算符 =====
            Console.WriteLine("\n=== 类型检查 ===");
            object obj = book;

            // is: 判断是否是某类型, 返回 bool
            if (obj is BookItem)
                Console.WriteLine("obj 是 BookItem");

            // is + 模式变量 (C# 7+): 判断 + 赋值
            if (obj is BookItem b)
                Console.WriteLine($"书名: {b.Title}");

            // is not (C# 9+)
            if (obj is not DvdItem)
                Console.WriteLine("obj 不是 DvdItem");

            // as: 尝试转换, 失败返回 null (不抛异常)
            BookItem? bookRef = obj as BookItem;
            if (bookRef != null)
                Console.WriteLine($"as 转换成功: {bookRef.Title}");

            DvdItem? dvdRef = obj as DvdItem;  // 转换失败
            Console.WriteLine($"as 转换为 DvdItem: {(dvdRef == null ? "null" : dvdRef.Title)}");

            // ===== 6. 接口引用 =====
            // 通过接口引用只能调用接口声明的方法
            Console.WriteLine("\n=== 接口引用 ===");
            IBorrowable[] borrowables = { book, dvd, mag };
            foreach (var ib in borrowables)
            {
                ib.Borrow("测试用户");
                ib.Return("测试用户");
            }

            // ===== 7. 接口默认实现 (C# 8+) =====
            ILogger logger = new ConsoleLogger();
            logger.Log("默认实现方法");
            logger.LogError("错误信息");  // 默认实现调用了 Log

            Console.WriteLine();
        }
    }

    // ===== 抽象类 =====
    // abstract class: 不能实例化, 作为基类
    // 可包含: 抽象方法(无实现) + 具体方法(有实现) + 字段 + 属性
    public abstract class LibraryItem
    {
        // 公共字段
        public string Id { get; }
        public string Title { get; set; }

        protected LibraryItem(string id, string title)
        {
            Id = id;
            Title = title;
        }

        // 抽象方法: 只有声明, 无实现, 子类必须 override
        // 类比: "模板/合同", 规定子类必须做什么, 但不关心怎么做
        public abstract string ItemType { get; }

        // 虚方法: 有默认实现, 子类可选重写
        public virtual void DisplayInfo() =>
            Console.WriteLine($"  [{ItemType}] {Id}: {Title}");

        // 具体方法: 子类直接继承
        public void Checkout() =>
            Console.WriteLine($"  → {Title} ({ItemType}) 已借出");
    }

    // ===== 接口 =====
    // 接口定义能力/行为, 不含实现 (C# 8+ 可有默认实现)
    // 接口 vs 抽象类:
    //   接口: 能做甚么 (can-do), 多实现, 无字段
    //   抽象类: 是什么 (is-a), 单继承, 可有字段
    public interface IBorrowable
    {
        // 接口成员默认 public abstract, 不需要写
        bool IsAvailable { get; }
        void Borrow(string borrower);
        void Return(string borrower);
    }

    public interface ISearchable
    {
        bool Match(string keyword);
    }

    // ===== 具体类: 继承抽象类 + 实现接口 =====
    public class BookItem : LibraryItem, IBorrowable, ISearchable
    {
        public string Author { get; }
        private bool _borrowed = false;

        public BookItem(string id, string title, string author)
            : base(id, title)
        {
            Author = author;
        }

        // 实现抽象属性
        public override string ItemType => "图书";

        // 重写虚方法
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"    作者: {Author}");
        }

        // 实现接口 IBorrowable
        public bool IsAvailable => !_borrowed;

        public void Borrow(string borrower)
        {
            if (!_borrowed)
            {
                _borrowed = true;
                Console.WriteLine($"  《{Title}》被 {borrower} 借走");
            }
        }

        public void Return(string borrower)
        {
            if (_borrowed)
            {
                _borrowed = false;
                Console.WriteLine($"  《{Title}》被 {borrower} 归还");
            }
        }

        // 实现接口 ISearchable
        public bool Match(string keyword) =>
            Title.Contains(keyword) || Author.Contains(keyword);
    }

    public class DvdItem : LibraryItem, IBorrowable
    {
        public int DurationMinutes { get; }
        private bool _borrowed = false;

        public DvdItem(string id, string title, int minutes)
            : base(id, title)
        {
            DurationMinutes = minutes;
        }

        public override string ItemType => "DVD";

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Console.WriteLine($"    时长: {DurationMinutes} 分钟");
        }

        public bool IsAvailable => !_borrowed;
        public void Borrow(string borrower) => _borrowed = true;
        public void Return(string borrower) => _borrowed = false;
    }

    // 多接口实现
    public class Magazine : LibraryItem, IBorrowable, ISearchable
    {
        public int Year { get; }
        public int Issue { get; }
        private bool _borrowed = false;

        public Magazine(string id, string title, int year, int issue)
            : base(id, title)
        {
            Year = year;
            Issue = issue;
        }

        public override string ItemType => "期刊";

        public bool IsAvailable => !_borrowed;
        public void Borrow(string borrower) => _borrowed = true;
        public void Return(string borrower) => _borrowed = false;
        public bool Match(string keyword) => Title.Contains(keyword);
    }

    // ===== 接口默认实现 (C# 8+) =====
    public interface ILogger
    {
        void Log(string message);
        void LogError(string error) => Log($"[ERROR] {error}");  // 默认实现
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message) => Console.WriteLine($"  [日志] {message}");
        // 不需要实现 LogError, 使用接口默认实现
    }
}
