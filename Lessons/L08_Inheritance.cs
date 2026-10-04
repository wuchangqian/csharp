namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 08: 继承
    /// 知识点: 继承、virtual/override、base、sealed、构造函数调用顺序、Object 方法重写
    /// </summary>
    public static class L08_Inheritance
    {
        public static void Run()
        {
            L01_Basics.PrintHeader("模块 08: 继承");

            // ===== 1. 基类对象 =====
            var animal = new Animal("动物");
            animal.Eat();
            animal.Describe();
            Console.WriteLine();

            // ===== 2. 派生类 =====
            // Dog 继承 Animal, 拥有 Animal 的所有成员 + 自己的新成员
            var dog = new Dog("旺财", "金毛");
            dog.Eat();         // 继承自 Animal
            dog.Bark();        // Dog 特有
            dog.Describe();    // 重写后的版本

            // ===== 3. virtual 和 override =====
            // Animal.Describe 是 virtual, Dog 重写了它
            // 运行时根据实际类型调用
            Animal polyDog = new Dog("大黄", "中华田园犬");  // 向上转型
            polyDog.Describe();  // 调用 Dog.Describe, 不是 Animal.Describe

            // ===== 4. base 关键字 =====
            // 在派生类中调用基类成员
            // 见 Dog 构造函数: base(name) 调用 Animal 构造函数
            // 见 Dog.Describe: base.Describe() 调用基类版本

            // ===== 5. 构造函数调用顺序 =====
            Console.WriteLine("\n--- 构造顺序演示 ---");
            var cat = new Cat("咪咪", "白色");
            // 输出: 先基类构造 → 再派生类构造

            Console.WriteLine("\n--- 析构/Dispose 演示 ---");
            // using 语句自动调用 Dispose
            using (var ctx = new DataContext("图书馆DB"))
            {
                Console.WriteLine("  使用数据库连接...");
            }  // 自动调用 Dispose

            // ===== 6. sealed 类 =====
            // sealed: 不能被继承 (如 string, DateTime 都是 sealed)
            var manager = new LibraryManager("管理员张三");
            manager.Describe();
            // class HeadLibrarian : LibraryManager {} // 编译错误: sealed 不能继承

            // ===== 7. 重写 Equals 和 GetHashCode =====
            var b1 = new Book2("C#入门", "张三");
            var b2 = new Book2("C#入门", "张三");
            var b3 = b1;

            Console.WriteLine($"\n引用相等: b1 == b3? {b1 == b3}");  // 同一对象
            Console.WriteLine($"值相等(重写后): b1 == b2? {b1 == b2}");  // 内容相同

            Console.WriteLine();
        }
    }

    // ===== 基类 =====
    public class Animal
    {
        // protected: 子类可访问, 外部不可
        // 类比: 家传秘方, 家人(子类)能看, 外人不行
        protected string Name;

        // 构造函数
        public Animal(string name)
        {
            Name = name;
            Console.WriteLine($"  [Animal构造] 名字: {name}");
        }

        // 普通方法: 派生类直接继承
        public void Eat() =>
            Console.WriteLine($"  {Name} 正在吃东西");

        // virtual 方法: 允许派生类重写
        // virtual = "这个方法可以被子类改写"
        public virtual void Describe() =>
            Console.WriteLine($"  这是一个动物, 名叫 {Name}");
    }

    // ===== 派生类 =====
    // 语法: class 子类 : 父类
    // C# 只支持单继承 (一个类只能有一个基类)
    public class Dog : Animal
    {
        private string Breed;

        // base(name): 调用基类构造函数
        // 如果不写, 编译器会尝试调用基类的无参构造函数
        public Dog(string name, string breed) : base(name)
        {
            Breed = breed;
            Console.WriteLine($"  [Dog构造] 品种: {breed}");
        }

        // 子类特有的方法
        public void Bark() =>
            Console.WriteLine($"  {Name}({Breed}) 汪汪汪!");

        // override: 重写基类的 virtual 方法
        public override void Describe()
        {
            // base.Describe(): 调用基类版本 (可选)
            base.Describe();
            Console.WriteLine($"  → 它是一只 {Breed} 狗");
        }
    }

    public class Cat : Animal
    {
        private string Color;
        public Cat(string name, string color) : base(name)
        {
            Color = color;
            Console.WriteLine($"  [Cat构造] 颜色: {color}");
        }

        public override void Describe() =>
            Console.WriteLine($"  {Name} 是一只 {Color} 的猫");

        ~Cat()
        {
            // 析构函数 (终结器): 对象被 GC 回收前调用
            // 不推荐手动管理资源, 用 IDisposable 代替
            Console.WriteLine($"  [Cat析构] {Name} 被回收");
        }
    }

    // ===== sealed: 不能被继承 =====
    public sealed class LibraryManager : Animal
    {
        public LibraryManager(string name) : base(name) { }

        public override void Describe() =>
            Console.WriteLine($"  {Name} 是图书馆管理员");
    }

    // ===== IDisposable + 构造/析构模式 =====
    public class DataContext : IDisposable
    {
        private string _connectionName;
        private bool _disposed = false;

        public DataContext(string name)
        {
            _connectionName = name;
            Console.WriteLine($"  [DataContext构造] 连接 {_connectionName}");
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Console.WriteLine($"  [DataContext.Dispose] 关闭连接 {_connectionName}");
                _disposed = true;
            }
        }
    }

    // ===== 重写 Equals/GetHashCode =====
    public class Book2
    {
        public string Title { get; }
        public string Author { get; }

        public Book2(string title, string author)
        {
            Title = title;
            Author = author;
        }

        // 重写 Equals: 自定义"相等"的含义
        public override bool Equals(object? obj)
        {
            if (obj is not Book2 other) return false;
            return Title == other.Title && Author == other.Author;
        }

        // 重写 GetHashCode: 相等的对象必须有相同的哈希码
        // (否则 Dictionary/HashSet 会出错)
        public override int GetHashCode() =>
            HashCode.Combine(Title, Author);

        // 重写 == 运算符
        public static bool operator ==(Book2? a, Book2? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(Book2? a, Book2? b) => !(a == b);
    }
}
