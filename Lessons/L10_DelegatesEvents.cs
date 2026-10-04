namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 10: 委托、事件、Lambda 表达式
    /// 知识点: 自定义委托、Action/Func、事件、事件触发、Lambda、闭包
    /// </summary>
    public static class L10_DelegatesEvents
    {
        public static void Run()
        {
            L01_Basics.PrintHeader("模块 10: 委托 / 事件 / Lambda");

            // ===== 1. 自定义委托 =====
            // delegate = 类型安全的函数指针
            // 类比: 遥控器上的按钮, 按下就执行某个功能
            MathOp add = Add;
            MathOp multiply = Multiply;

            Console.WriteLine($"Add(3,5) = {add(3, 5)}");
            Console.WriteLine($"Multiply(3,5) = {multiply(3, 5)}");

            // 委托可以组合 (多播)
            MathOp combined = add + multiply;
            combined(2, 3);  // 依次调用 Add 和 Multiply

            // 移除
            combined -= add;
            combined(2, 3);

            // ===== 2. Action 和 Func =====
            // Action: 无返回值的泛型委托
            // Action (0参数), Action<T> (1参数), Action<T1,T2> (2参数)...
            Action<string> printLine = s => Console.WriteLine(s);
            printLine("Action<string> 调用");

            // Func: 有返回值的泛型委托
            // Func<int> (0参数返回int), Func<int,int> (1参数返回int)...
            Func<int, int> square = x => x * x;
            Console.WriteLine($"Square(5) = {square(5)}");

            Func<int, int, int> max = (a, b) => a > b ? a : b;
            Console.WriteLine($"Max(3,7) = {max(3, 7)}");

            // Predicate<T>: 等价于 Func<T,bool>
            Predicate<int> isEven = n => n % 2 == 0;
            Console.WriteLine($"IsEven(4) = {isEven(4)}");

            // ===== 3. Lambda 表达式 =====
            // (参数) => 表达式或语句块
            // 1. 无参数
            Action greet = () => Console.WriteLine("Hello!");
            greet();

            // 2. 单参数 (可省略括号)
            Func<int, int> inc = x => x + 1;
            Console.WriteLine($"Inc(10) = {inc(10)}");

            // 3. 多参数
            Func<int, int, int> subtract = (a, b) => a - b;
            Console.WriteLine($"Sub(10-3) = {subtract(10, 3)}");

            // 4. 语句块 Lambda
            Func<int, string> classify = n =>
            {
                if (n < 0) return "负";
                if (n == 0) return "零";
                return "正";
            };
            Console.WriteLine($"Classify(-5) = {classify(-5)}");

            // 5. 丢弃 (discard)
            Func<int, int, int> firstParam = (_, b) => b * 2;
            Console.WriteLine($"Discard(忽略第一个, 6) = {firstParam(999, 6)}");

            // ===== 4. 闭包 (Closure) =====
            // Lambda 捕获外部变量, 延长其生命周期
            int counter = 0;
            Action increment = () => counter++;
            increment();
            increment();
            increment();
            Console.WriteLine($"闭包: counter = {counter}");  // 3

            // ===== 5. 方法作为参数传递 =====
            int[] numbers = { 1, 2, 3, 4, 5 };
            var evens = Array.FindAll(numbers, n => n % 2 == 0);
            Console.WriteLine($"偶数: [{string.Join(", ", evens)}]");

            int sum = Array.TrueForAll(numbers, n => n > 0) ? numbers.Sum() : 0;
            Console.WriteLine($"总和: {sum}");

            // ===== 6. 事件 (Event) =====
            // 事件 = 受限的委托, 只能在声明类内部触发
            // 类比: 微信公众号, 关注(订阅)后, 公众号发文章你就能收到
            Console.WriteLine("\n--- 事件演示 ---");
            var account = new BankAccount("张三", 1000);

            // 订阅事件 (注册回调)
            account.BalanceChanged += (sender, e) =>
            {
                Console.WriteLine($"  [短信通知] {e.AccountOwner} 余额变更: {e.OldBalance} → {e.NewBalance} (差: {e.Delta:+0;-0;0})");
            };

            account.BalanceChanged += (sender, e) =>
            {
                if (e.NewBalance < 0)
                    Console.WriteLine($"  [预警] {e.AccountOwner} 账户余额为负!");
            };

            // 触发事件
            account.Deposit(500);
            account.Withdraw(2000);
            account.Deposit(300);

            // 取消订阅
            // account.BalanceChanged -= handler;

            Console.WriteLine();
        }

        // 自定义委托
        public delegate int MathOp(int a, int b);

        static int Add(int a, int b) => a + b;
        static int Multiply(int a, int b) => a * b;
    }

    // ===== 事件相关类型 =====

    // 1. 事件参数类 (继承 EventArgs)
    public class BalanceChangedEventArgs : EventArgs
    {
        public string AccountOwner { get; }
        public decimal OldBalance { get; }
        public decimal NewBalance { get; }
        public decimal Delta => NewBalance - OldBalance;

        public BalanceChangedEventArgs(string owner, decimal oldBal, decimal newBal)
        {
            AccountOwner = owner;
            OldBalance = oldBal;
            NewBalance = newBal;
        }
    }

    // 2. 事件发布者
    public class BankAccount
    {
        public string Owner { get; }
        public decimal Balance { get; private set; }

        // 事件声明: event + 委托类型
        // EventHandler<T> 是 .NET 标准事件委托
        public event EventHandler<BalanceChangedEventArgs>? BalanceChanged;

        public BankAccount(string owner, decimal initialBalance)
        {
            Owner = owner;
            Balance = initialBalance;
        }

        public void Deposit(decimal amount)
        {
            var old = Balance;
            Balance += amount;
            OnBalanceChanged(old, Balance);
        }

        public void Withdraw(decimal amount)
        {
            var old = Balance;
            Balance -= amount;
            OnBalanceChanged(old, Balance);
        }

        // 触发事件的虚方法 (子类可重写触发逻辑)
        // 命名约定: On + 事件名
        protected virtual void OnBalanceChanged(decimal old, decimal @new)
        {
            // ?. 避免 null 引用 (没人订阅时不会报错)
            BalanceChanged?.Invoke(this, new BalanceChangedEventArgs(Owner, old, @new));
        }
    }
}
