namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 03: 方法
    /// 知识点: 方法定义、重载、ref/out/in 参数、params、可选参数、命名参数、表达式体方法、局部函数
    /// </summary>
    public static class L03_Methods
    {
        public static void Run()
        {
            L01_Basics.PrintHeader("模块 03: 方法");

            // 1. 基本方法调用
            int sum = Add(3, 5);
            Console.WriteLine($"Add(3, 5) = {sum}");

            // 2. 方法重载 (Overloading): 同名方法, 参数不同
            Console.WriteLine($"Add(3.14, 2.86) = {Add(3.14, 2.86)}");
            Console.WriteLine($"Add(1, 2, 3) = {Add(1, 2, 3)}");

            // 3. 可选参数 + 命名参数
            Greet("张三");                          // 用默认值
            Greet("李四", "你好");                  // 覆盖默认值
            Greet(name: "王五", greeting: "早上好"); // 命名参数, 顺序无关

            // 4. ref 参数: 传入前必须赋值, 方法内可修改, 修改影响调用方
            int x = 10;
            Triple(ref x);   // ref: 双向传递
            Console.WriteLine($"ref 后 x = {x}");  // x 变成 30

            // 5. out 参数: 传入前不需要赋值, 方法内必须赋值
            // 常用于方法返回多个值
            Divide(17, 5, out int quotient, out int remainder);
            Console.WriteLine($"17 ÷ 5 = {quotient} 余 {remainder}");

            // TryParse 模式: out 配合 bool 返回
            if (int.TryParse("42", out int parsed))
                Console.WriteLine($"解析成功: {parsed}");

            // 6. in 参数: 只读引用传递 (C# 7.2+)
            // 类似 ref 但只读, 方法内不能修改
            // 好处: 避免大结构体的拷贝, 同时保证不被修改
            var big = new BigStruct { Data = 999 };
            PrintBig(in big);

            // 7. params 参数: 可变数量参数
            Console.WriteLine($"Sum(1,2,3,4,5) = {Sum(1, 2, 3, 4, 5)}");
            Console.WriteLine($"Sum() = {Sum()}");  // 空数组, 结果 0

            // 8. 表达式体方法 (Expression-bodied members)
            // 用 => 语法, 适合简单方法
            int doubled = DoubleIt(21);
            Console.WriteLine($"DoubleIt(21) = {doubled}");

            // 9. 局部函数 (Local Function): 在方法内部定义的方法
            // 适合只在某方法内使用的辅助逻辑
            int fib10 = CalculateFib(10);
            Console.WriteLine($"斐波那契第10项 = {fib10}");

            // 10. 递归
            Console.WriteLine($"5! = {Factorial(5)}");

            Console.WriteLine();
        }

        // ===== 方法定义 =====

        // 基本方法: 返回类型 + 方法名 + 参数列表
        static int Add(int a, int b) => a + b;  // 表达式体语法

        // 重载: 参数类型不同
        static double Add(double a, double b) => a + b;

        // 重载: 参数个数不同
        static int Add(int a, int b, int c) => a + b + c;

        // 可选参数: 参数有默认值
        // 注意: 可选参数必须在所有必选参数之后
        static void Greet(string name, string greeting = "你好")
            => Console.WriteLine($"{greeting}, {name}!");

        // ref 参数: 双向引用
        static void Triple(ref int value) => value *= 3;

        // out 参数: 方法内必须赋值
        // 类比: ref 是"我给你一个已有值的盒子, 你可以改它"
        //        out 是"我给你一个空盒子, 你必须往里放东西"
        static void Divide(int dividend, int divisor, out int quotient, out int remainder)
        {
            quotient = dividend / divisor;
            remainder = dividend % divisor;
        }

        // in 参数: 只读引用
        struct BigStruct { public int Data; }
        static void PrintBig(in BigStruct s) => Console.WriteLine($"BigStruct.Data = {s.Data}");

        // params: 可变参数, 编译器自动包装成数组
        static int Sum(params int[] numbers)
        {
            int total = 0;
            foreach (var n in numbers) total += n;
            return total;
        }

        // 表达式体方法
        static int DoubleIt(int x) => x * 2;

        // 局部函数示例
        static int CalculateFib(int n)
        {
            // 局部函数: 只在 CalculateFib 内可见
            return FibInternal(n);

            int FibInternal(int k) =>
                k <= 1 ? k : FibInternal(k - 1) + FibInternal(k - 2);
        }

        // 递归
        static int Factorial(int n) => n <= 1 ? 1 : n * Factorial(n - 1);
    }
}
