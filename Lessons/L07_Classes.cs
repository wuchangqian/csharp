namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 07: 类与对象 —— 封装
    /// 知识点: 类定义、字段vs属性、构造函数、静态成员、this、封装
    /// </summary>
    public static class L07_Classes
    {
        public static void Run()
        {
            L01_Basics.PrintHeader("模块 07: 类与对象 (封装)");

            // ===== 1. 创建对象 (实例化) =====
            // new 调用构造函数
            var book1 = new Book("978-7-123-45678-9", "C#入门经典", "张三");
            var book2 = new Book("978-7-987-65432-1", ".NET设计模式", "李四", 2024, 5);

            // ===== 2. 属性访问 =====
            // 属性 = 智能字段, 可加逻辑
            Console.WriteLine($"书名: {book1.Title}");
            Console.WriteLine($"作者: {book1.Author}");
            Console.WriteLine($"ISBN: {book1.ISBN}");

            // set 调用
            book1.Title = "C#入门经典(第2版)";
            Console.WriteLine($"改名后: {book1.Title}");

            // set 验证逻辑 (Title 不能为空)
            try
            {
                book1.Title = "";
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"空标题被拒绝: {ex.Message}");
            }

            // ===== 3. 只读属性 =====
            // ISBN 只能在构造函数中设置, 外部不能改
            // book1.ISBN = "xxx";  // 编译错误!

            // ===== 4. 静态成员 =====
            // 属于类本身, 不属于某个实例
            Console.WriteLine($"\n总图书数: {Book.TotalCount}");  // 通过类名访问
            Book.ShowBookCount();                                 // 静态方法

            // ===== 5. 静态构造函数 =====
            // 第一次访问类时自动执行, 只执行一次
            // 用于初始化静态成员 (见 Book 类的 static 构造函数)
            Console.WriteLine($"图书馆名称: {Book.LibraryName}");

            // ===== 6. 对象初始化器 =====
            // 不调用带参构造, 用无参构造 + 初始化器
            var book3 = new Book("978-000", "Python编程", "王五")
            {
                Year = 2023,
            };
            Console.WriteLine($"\n初始化器创建: {book3}");

            // ===== 7. this 关键字 =====
            // 演示: 构造函数链 (构造函数调用另一个构造函数)
            var book4 = new Book("自定义标题");
            Console.WriteLine($"链式构造: {book4}");

            // ===== 8. 计算属性 (表达式体属性) =====
            Console.WriteLine($"book2 可借? {book2.IsAvailable}");
            Console.WriteLine($"book2 借出率: {book2.BorrowRate:F1%}");

            // ===== 9. readonly 字段 vs const =====
            // 见 Book 类中的 readonly CreatedAt
            Console.WriteLine($"创建时间: {book1.CreatedAt:yyyy-MM-dd HH:mm:ss}");

            // ===== 10. ToString 重写 =====
            Console.WriteLine($"ToString: {book1}");
            Console.WriteLine($"book1 == book1? {book1 == book1}");

            Console.WriteLine();
        }
    }

    /// <summary>
    /// Book 类 —— 演示封装
    /// 封装 = 把数据藏起来(private), 通过属性(public)控制访问
    /// 类比: 银行保险柜, 钱存里面(private), 你只能通过窗口(属性)存取
    /// </summary>
    public class Book
    {
        // ===== 字段 (Field) =====
        // private: 只能在类内部访问 → 封装的核心
        private string _isbn;
        private string _title;
        private string _author;
        private int _copies;           // 总册数
        private int _borrowedCount;    // 已借出册数

        // readonly 字段: 只能在声明时或构造函数中赋值
        public readonly DateTime CreatedAt;

        // ===== 静态字段 =====
        // 属于类, 不属于实例, 所有实例共享
        private static int _totalCount = 0;
        public static int TotalCount => _totalCount;

        // 静态只读字段
        public static readonly string LibraryName;

        // ===== 静态构造函数 =====
        // 无参数, 无访问修饰符, 第一次访问类时自动调用
        static Book()
        {
            LibraryName = "银川数字图书馆";
            Console.WriteLine($"  [静态构造函数] 图书馆 \"{LibraryName}\" 已初始化");
        }

        // ===== 构造函数 (Constructor) =====
        // 重载: 多个构造函数, 参数不同

        // 主构造函数
        public Book(string isbn, string title, string author)
        {
            _isbn = isbn;
            Title = title;        // 通过属性 set, 触发验证
            _author = author;
            _copies = 1;
            _borrowedCount = 0;
            CreatedAt = DateTime.Now;
            _totalCount++;
            Console.WriteLine($"  [构造] 新书入库: {title}");
        }

        // 构造函数链: this(...) 调用另一个构造函数
        public Book(string isbn, string title, string author, int year, int copies)
            : this(isbn, title, author)  // 先调用上面的构造函数
        {
            Year = year;
            _copies = copies;
        }

        // 无参构造 + 默认值
        public Book() : this("未知", "未命名", "匿名")
        {
        }

        // 自定义标题的构造函数 (演示 this 消歧)
        public Book(string title) : this("未知ISBN", title, "匿名")
        {
            // 这里 this 指当前对象, 但在这个简单构造里没用到
        }

        // ===== 属性 (Property) =====
        // 属性 = 字段 + 方法, 提供受控访问

        // 自动属性 (Auto Property): 编译器自动生成私有字段
        // { get; set; } 可读可写
        public int Year { get; set; } = 2024;  // 带默认值的自动属性

        // 只读自动属性 (只在构造函数或初始化器中可设)
        // { get; } C# 6+
        // public string Id { get; } = Guid.NewGuid().ToString();

        // 带逻辑的属性
        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("书名不能为空", nameof(value));
                _title = value.Trim();
            }
        }

        public string Author
        {
            get => _author;
            set => _author = string.IsNullOrWhiteSpace(value) ? "匿名" : value;
        }

        // 只读属性 (只有 get)
        public string ISBN => _isbn;  // 表达式体属性, 等价于 { get { return _isbn; } }

        // 计算属性 (从其他字段算出来)
        public bool IsAvailable => _copies - _borrowedCount > 0;

        public double BorrowRate =>
            _copies == 0 ? 0 : (double)_borrowedCount / _copies;

        // init-only 属性 (C# 9+): 只能在构造或对象初始化器中赋值
        // 之后不可改, 用于不可变对象
        public string Category { get; init; } = "通用";

        // ===== 方法 =====
        public void Borrow()
        {
            if (!IsAvailable)
                throw new InvalidOperationException("没有可借副本");
            _borrowedCount++;
            Console.WriteLine($"  [借出] {Title} (已借 {_borrowedCount}/{_copies})");
        }

        public void Return()
        {
            if (_borrowedCount == 0)
                throw new InvalidOperationException("没有借出的副本");
            _borrowedCount--;
            Console.WriteLine($"  [归还] {Title} (已借 {_borrowedCount}/{_copies})");
        }

        // 静态方法
        public static void ShowBookCount() =>
            Console.WriteLine($"  当前馆藏图书总数: {_totalCount}");

        // ===== 重写 ToString =====
        // Object.ToString() 默认返回类名, 重写后返回有意义的描述
        public override string ToString() =>
            $"《{Title}》- {_author} ({Year}) [{_borrowedCount}/{_copies}借出]";
    }
}
