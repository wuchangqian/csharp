namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 16: 高级类型
    /// 知识点: Record、模式匹配、可空引用类型、struct、enum、扩展方法
    /// </summary>
    public static class L16_AdvancedTypes
    {
        public static void Run()
        {
            L01_Basics.PrintHeader("模块 16: 高级类型");

            // ===== 1. Record (C# 9+) =====
            // 不可变引用类型, 自带值相等 + 解构 + ToString + Copy
            // 类比: 一张照片, 拍完就改不了, 想换就拍新的
            var p1 = new Person("张三", 25);
            var p2 = new Person("张三", 25);
            Console.WriteLine($"Record: p1 == p2? {p1 == p2}");  // true, 值相等
            Console.WriteLine($"ToString: {p1}");

            // with 表达式: 基于现有 record 创建副本, 修改部分字段
            var p3 = p1 with { Age = 26 };
            Console.WriteLine($"with: {p3} (p1 仍为 {p1.Age}岁)");

            // 解构 (Deconstruct)
            var (name, age) = p1;
            Console.WriteLine($"解构: name={name}, age={age}");

            // record struct (C# 10+): 值类型 record
            var point = new Point2D(3, 4);
            Console.WriteLine($"Record struct: {point}, Magnitude={Math.Sqrt(point.X * point.X + point.Y * point.Y):F2}");

            // ===== 2. 模式匹配 (Pattern Matching) =====
            Console.WriteLine("\n=== 模式匹配 ===");

            // is 模式
            object obj = 42;
            if (obj is int i)
                Console.WriteLine($"  is int: {i}");

            // switch 表达式 + 类型模式
            string desc = obj switch
            {
                int => "整数",
                string => "字符串",
                bool => "布尔",
                null => "null",
                _ => "其他"
            };
            Console.WriteLine($"  switch: {desc}");

            // 属性模式
            var person = new { Name = "李四", Age = 17 };
            string category = person switch
            {
                { Age: < 18 } => "未成年",
                { Age: >= 18 and < 60 } => "成年",
                { Age: >= 60 } => "老年",
                _ => "未知"
            };
            Console.WriteLine($"  属性模式: {person.Name} → {category}");

            // 列表模式 (C# 11+)
            int[] numbers = { 1, 2, 3, 4, 5 };
            string listPattern = numbers switch
            {
                [1, 2, 3, ..] => "以1,2,3开头",
                [.., 4, 5] => "以4,5结尾",
                [1, ..] => "以1开头",
                _ => "其他"
            };
            Console.WriteLine($"  列表模式: {listPattern}");

            // 元组模式
            var (x, y) = (5, 10);
            string pos = (x, y) switch
            {
                (0, 0) => "原点",
                (0, _) => "Y轴",
                (_, 0) => "X轴",
                ( > 0, > 0) => "第一象限",
                _ => "其他象限"
            };
            Console.WriteLine($"  元组模式 ({x},{y}): {pos}");

            // ===== 3. 可空引用类型 (NRT, C# 8+) =====
            // 编译器警告可能的 null 引用
            // 在 csproj 中 <Nullable>enable</Nullable> 开启
            Console.WriteLine("\n=== 可空引用类型 ===");
            string? maybeNull = null;
            string notNull = "必定有值";

            // 非空变量赋 null 会警告
            // string mustHaveValue = null;  // 警告 CS8600

            // 可空变量使用前应检查
            if (maybeNull != null)
            {
                Console.WriteLine($"  长度: {maybeNull.Length}");  // 检查后不再警告
            }

            // ! (null 抑制运算符): "我保证不为 null, 别警告"
            int len = maybeNull!.Length;  // 运行时可能 NullReferenceException
            Console.WriteLine($"  ! 抐制: len={len}");  // 如果 maybeNull 真为 null 会崩

            // ===== 4. struct (值类型) =====
            Console.WriteLine("\n=== struct ===");
            var p = new Point2D(10, 20);
            var q = new Point2D(10, 20);

            // struct 赋值是值拷贝 (不是引用)
            var r = p;
            r.X = 999;
            Console.WriteLine($"  p.X={p.X}, r.X={r.X}");  // p 不受影响

            // struct 默认值相等
            Console.WriteLine($"  p == q? {p == q}");

            // ===== 5. enum (枚举) =====
            Console.WriteLine("\n=== enum ===");
            var day = Weekday.Monday;
            Console.WriteLine($"  enum: {day} (={(int)day})");

            // Flags 枚举 (位运算组合)
            var perms = Permission.Read | Permission.Write;
            Console.WriteLine($"  Flags: {perms}");
            Console.WriteLine($"  包含 Read? {perms.HasFlag(Permission.Read)}");
            Console.WriteLine($"  包含 Execute? {perms.HasFlag(Permission.Execute)}");

            // ===== 6. 扩展方法 (Extension Method) =====
            // 给已有类型"添加"方法, 不修改原类型
            // 类比: 给别人的类打"补丁"
            Console.WriteLine("\n=== 扩展方法 ===");
            string text = "hello world";
            Console.WriteLine($"  \"{text}\" → \"{text.Capitalize()}\"");
            Console.WriteLine($"  反转: \"{text.Reverse()}\"");

            int number = 42;
            Console.WriteLine($"  {number} 是否偶数? {number.IsEven()}");
            Console.WriteLine($"  {number} 平方 = {number.Squared()}");

            // ===== 7. ref struct 和 Span =====
            // Span<T>: 安全的内存切片, 无堆分配
            // 用于高性能场景
            Console.WriteLine("\n=== Span ===");
            int[] array = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Span<int> slice = array.AsSpan(2, 4);  // 从索引2取4个
            Console.Write("  Span slice: ");
            foreach (var v in slice) Console.Write(v + " ");
            Console.WriteLine();

            // 修改 Span 会影响原数组
            slice[0] = 99;
            Console.WriteLine($"  修改后 array[2] = {array[2]}");

            Console.WriteLine();
        }

        // ===== Record 定义 =====
        public record Person(string Name, int Age);
        public record struct Point2D(int X, int Y);

        // ===== enum 定义 =====
        public enum Weekday { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday }

        [Flags]
        public enum Permission
        {
            None = 0,
            Read = 1,
            Write = 2,
            Execute = 4,
            All = Read | Write | Execute
        }

    }

    // ===== 扩展方法 (必须在顶级静态类中) =====
    public static class StringExtensions
    {
        public static string Capitalize(this string s) =>
            string.IsNullOrEmpty(s) ? s :
            char.ToUpper(s[0]) + s[1..];

        public static string Reverse(this string s) =>
            new string(s.Reverse().ToArray());
    }

    public static class IntExtensions
    {
        public static bool IsEven(this int n) => n % 2 == 0;
        public static int Squared(this int n) => n * n;
    }
}
