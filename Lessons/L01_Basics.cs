namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 01: 基础语法
    /// 知识点: 变量声明、数据类型、类型转换、运算符、var、常量
    /// </summary>
    public static class L01_Basics
    {
        public static void Run()
        {
            PrintHeader("模块 01: 基础语法");

            // 1. 值类型 (Value Types) —— 存在栈上，直接存值
            // 类比: 变量 = 贴了标签的盒子，盒子里直接装东西
            int age = 25;                    // 32位整数
            long bigNumber = 9_000_000_000L; // 64位整数, 下划线分隔提高可读性
            double price = 19.99;            // 64位浮点
            float weight = 70.5f;            // 32位浮点, 必须加 f 后缀
            decimal money = 1000.50m;       // 128位高精度, 财务计算专用, 加 m 后缀
            bool isStudent = true;           // 布尔
            char grade = 'A';                // 单个 Unicode 字符, 用单引号
            byte b = 255;                    // 8位无符号整数

            Console.WriteLine($"int: {age}, long: {bigNumber}, double: {price}");
            Console.WriteLine($"float: {weight}, decimal: {money}, bool: {isStudent}, char: {grade}, byte: {b}");

            // 2. 引用类型 (Reference Types) —— 存在堆上，变量存的是地址
            // 类比: 盒子里装的是"遥控器"，真正的对象在别的房间
            string name = "张三";  // string 是引用类型, 但行为类似值类型 (不可变)
            object obj = "可以是任何类型";  // object 是所有类型的基类
            dynamic dyn = 42;     // dynamic 绕过编译期检查, 运行时确定

            Console.WriteLine($"string: {name}, object: {obj}, dynamic: {dyn}");

            // 3. var 关键字 —— 类型推断, 编译器自动推断类型
            // 注意: var 不是"动态类型", 编译后就确定了, 只是不用你写类型名
            var city = "银川";     // 编译后等价于 string city
            var count = 100;      // 编译后等价于 int count
            var pi = 3.14159;     // 编译后等价于 double pi
            Console.WriteLine($"var: {city}({city.GetType()}), {count}({count.GetType()}), {pi}({pi.GetType()})");

            // 4. 常量
            const double PI = 3.14159265;    // const: 编译期常量, 必须在声明时赋值
            readonly示范();

            // 5. 类型转换
            // 隐式转换 (小→大, 不丢精度)
            int small = 100;
            double large = small;   // int → double, 隐式
            Console.WriteLine($"隐式转换: int {small} → double {large}");

            // 显式转换 (大→小, 可能丢精度)
            double d = 9.78;
            int truncated = (int)d;  // 强转, 截断小数
            Console.WriteLine($"显式转换: double {d} → int {truncated}");

            // Convert 类 (安全转换, 四舍五入)
            string numStr = "42";
            int parsed = Convert.ToInt32(numStr);  // 字符串转整数
            double rounded = Convert.ToDouble("3.7");
            Console.WriteLine($"Convert: \"{numStr}\" → {parsed}, \"3.7\" → {rounded}");

            // Parse / TryParse
            int parsed2 = int.Parse("100");        // 失败抛异常
            bool ok = int.TryParse("abc", out int result); // 失败返回 false, 不抛异常
            Console.WriteLine($"Parse: {parsed2}, TryParse(\"abc\"): ok={ok}, result={result}");

            // 6. 运算符
            int a = 10, c = 3;
            Console.WriteLine($"算术: {a}+{c}={a + c}, {a}-{c}={a - c}, {a}*{c}={a * c}, {a}/{c}={a / c}, {a}%{c}={a % c}");
            Console.WriteLine($"关系: {a > c}, {a == c}, {a != c}");
            Console.WriteLine($"逻辑: {a > 5 && c > 1}, {a > 5 || c > 5}, {!isStudent}");
            Console.WriteLine($"位运算: {a & c}, {a | c}, {a ^ c}, ~{a} = {~a}, {a} << 1 = {a << 1}");
            Console.WriteLine($"条件运算符(三元): {(a > c ? "a大" : "c大")}");

            // null 条件运算符 (C# 6+)
            string? maybeNull = null;
            int? length = maybeNull?.Length;  // 如果 maybeNull 为 null, 返回 null, 不抛异常
            Console.WriteLine($"null 条件: maybeNull?.Length = {(length ?? -1)}");  // ?? 提供 null 时的默认值

            // 7. 类型信息
            Console.WriteLine($"name 的类型: {name.GetType()}");
            Console.WriteLine($"age 的类型: {age.GetType()}");
            Type intType = typeof(int);  // typeof 在编译期获取 Type
            Console.WriteLine($"typeof(int): {intType}");

            Console.WriteLine();
        }

        // readonly 示范: 只能在声明时或构造函数中赋值
        // 与 const 区别: const 是编译期常量, readonly 是运行期常量
        static readonly string CreatedAt = DateTime.Now.ToString();
        static void readonly示范()
        {
            Console.WriteLine($"readonly (运行期): CreatedAt = {CreatedAt}");
        }

        // ============================================================
        // 辅助方法
        // ============================================================
        public static void PrintHeader(string title)
        {
            Console.WriteLine();
            Console.WriteLine("════════════════════════════════════════");
            Console.WriteLine($"  {title}");
            Console.WriteLine("════════════════════════════════════════");
        }
    }
}
