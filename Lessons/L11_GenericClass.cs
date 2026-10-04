namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 11: 泛型
    /// 知识点: 泛型类、泛型方法、泛型约束、协变/逆变、泛型缓存
    /// </summary>
    public static class L11_GenericClass
    {
        public static void Run()
        {
            L01_Basics.PrintHeader("模块 11: 泛型");

            // ===== 1. 泛型类的基本使用 =====
            // 泛型 = 类型参数化, 写一份代码, 适用于多种类型
            // 类比: 模板/泛型 = 万能容器, 装什么类型由你指定

            var intRepo = new Repository<int>();
            intRepo.Add(1);
            intRepo.Add(2);
            intRepo.Add(3);
            Console.WriteLine($"intRepo: [{string.Join(", ", intRepo.GetAll())}]");
            Console.WriteLine($"Count={intRepo.Count}");

            var strRepo = new Repository<string>();
            strRepo.Add("苹果");
            strRepo.Add("香蕉");
            Console.WriteLine($"strRepo: [{string.Join(", ", strRepo.GetAll())}]");

            // ===== 2. 泛型方法 =====
            // 不需要整个类泛型化, 只需方法泛型
            int maxInt = MaxOf(3, 7);
            double maxDouble = MaxOf(3.14, 2.71);
            string maxStr = MaxOf("abc", "xyz");  // 字符串按字典序比较
            Console.WriteLine($"\nMaxOf(3,7)={maxInt}, MaxOf(3.14,2.71)={maxDouble}, MaxOf(\"abc\",\"xyz\")=\"{maxStr}\"");

            // 类型推断: 编译器从参数推断 T, 可以省略 <int>
            Console.WriteLine($"推断: MaxOf(10, 20) = {MaxOf(10, 20)}");

            // Swap
            int a = 5, b = 10;
            Console.WriteLine($"Swap 前: a={a}, b={b}");
            Swap(ref a, ref b);
            Console.WriteLine($"Swap 后: a={a}, b={b}");

            // ===== 3. 泛型约束 (where) =====
            // 约束 = 对泛型参数的限制
            // 常见约束:
            //   where T : class       (引用类型)
            //   where T : struct      (值类型)
            //   where T : new()       (有无参构造)
            //   where T : BaseClass   (继承某类)
            //   where T : IInterface  (实现某接口)
            //   where T : unmanaged   (非托管类型)
            //   where T : notnull     (不可空)

            // 约束为 IComparable<T>: T 必须实现可比较接口
            var sorter = new Sorter<int>();
            int[] arr = { 5, 2, 8, 1, 9 };
            var sorted = sorter.Sort(arr);
            Console.WriteLine($"\n排序: [{string.Join(", ", sorted)}]");

            // 约束为 class + new(): T 必须是引用类型且有无参构造
            var factory = new Factory<BookEntity>();
            var book = factory.Create();
            book.Title = "泛型工厂创建的图书";
            Console.WriteLine($"工厂创建: {book.Title}");

            // ===== 4. 泛型字典 (泛型缓存) =====
            // 静态泛型字段: 每个不同的 T 类型会有一份独立的副本
            Console.WriteLine($"\n泛型缓存演示:");
            GenericCache<int>.Value = "整数缓存";
            GenericCache<string>.Value = "字符串缓存";
            Console.WriteLine($"Cache<int>: {GenericCache<int>.Value}");
            Console.WriteLine($"Cache<string>: {GenericCache<string>.Value}");
            // int 和 string 有各自的静态字段

            // ===== 5. 协变 (Covariance) 和逆变 (Contravariance) =====
            // out T (协变): 可以返回 T, 不能接受 T 作参数
            // in T (逆变): 可以接受 T 作参数, 不能返回 T
            Console.WriteLine("\n协变/逆变:");
            ICovariant<string> strProducer = new Producer<string>();
            ICovariant<object> objProducer = strProducer; // 协变: string → object (子→父)
            Console.WriteLine($"协变: {objProducer.Get()}");

            IContravariant<object> objConsumer = new Consumer<object>();
            IContravariant<string> strConsumer = objConsumer; // 逆变: object → string (父→子)
            strConsumer.Accept("逆变参数");

            // ===== 6. 元组 =====
            var pair = new Pair<int, string>(1, "一");
            Console.WriteLine($"\nPair: {pair.First} - {pair.Second}");

            Console.WriteLine();
        }

        // ===== 泛型方法 =====

        // 泛型方法: <T> 放在方法名后
        // 约束 where T : IComparable<T>: T 必须可比较
        static T MaxOf<T>(T a, T b) where T : IComparable<T>
            => a.CompareTo(b) >= 0 ? a : b;

        static void Swap<T>(ref T a, ref T b)
        {
            (b, a) = (a, b);  // 元组交换
        }
    }

    // ===== 泛型类 =====
    public class Repository<T>
    {
        private List<T> _items = new();

        public int Count => _items.Count;

        public void Add(T item) => _items.Add(item);
        public bool Remove(T item) => _items.Remove(item);
        public T Get(int index) => _items[index];
        public List<T> GetAll() => new(_items);
        public void Clear() => _items.Clear();
    }

    // ===== 泛型约束 =====
    public class Sorter<T> where T : IComparable<T>
    {
        public T[] Sort(T[] arr)
        {
            var result = (T[])arr.Clone();
            Array.Sort(result);
            return result;
        }
    }

    // 约束: class (引用类型) + new() (有无参构造)
    public class Factory<T> where T : class, new()
    {
        public T Create() => new T();  // 可以 new T()
    }

    public class BookEntity
    {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
    }

    // ===== 泛型缓存 =====
    // 每个不同的 T 产生一份独立的静态字段
    public static class GenericCache<T>
    {
        public static string? Value { get; set; }
    }

    // ===== 协变 (out) =====
    public interface ICovariant<out T>
    {
        T Get();
    }

    public class Producer<T> : ICovariant<T>
    {
        public T Get() => default!;
    }

    // ===== 逆变 (in) =====
    public interface IContravariant<in T>
    {
        void Accept(T item);
    }

    public class Consumer<T> : IContravariant<T>
    {
        public void Accept(T item) =>
            Console.WriteLine($"  接收: {item}");
    }

    // ===== 多类型参数 =====
    public class Pair<T1, T2>
    {
        public T1 First { get; }
        public T2 Second { get; }

        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }
}
