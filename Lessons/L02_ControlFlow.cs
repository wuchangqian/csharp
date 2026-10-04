namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 02: 控制流
    /// 知识点: if/else, switch, 三元, for, while, do-while, foreach, break/continue
    /// </summary>
    public static class L02_ControlFlow
    {
        public static void Run()
        {
            L01_Basics.PrintHeader("模块 02: 控制流");

            // ===== 1. 条分支 =====
            int score = 85;

            if (score >= 90)
                Console.WriteLine("优秀");
            else if (score >= 80)
                Console.WriteLine("良好");
            else if (score >= 60)
                Console.WriteLine("及格");
            else
                Console.WriteLine("不及格");

            // ===== 2. switch 语句 =====
            string day = DateTime.Now.DayOfWeek.ToString();
            switch (day)
            {
                case "Saturday":
                case "Sunday":       // 多个 case 共用一段代码
                    Console.WriteLine("周末");
                    break;
                case "Monday":
                    Console.WriteLine("周一, 精神满满");
                    break;
                default:              // 默认分支
                    Console.WriteLine($"工作日: {day}");
                    break;
            }

            // C# 8+ switch 表达式 (箭头语法)
            string season = DateTime.Now.Month switch
            {
                3 or 4 or 5 => "春",   // 模式匹配: or 组合
                6 or 7 or 8 => "夏",
                9 or 10 or 11 => "秋",
                12 or 1 or 2 => "冬",
                _ => "未知"            // _ 是 discard, 等同 default
            };
            Console.WriteLine($"当前季节: {season}");

            // ===== 3. 循环 =====

            // for 循环: 已知次数
            Console.Write("for 循环 1~5: ");
            for (int i = 1; i <= 5; i++)
                Console.Write(i + " ");
            Console.WriteLine();

            // while 循环: 先判断后执行
            Console.Write("while 循环偶数: ");
            int n = 0;
            while (n < 10)
            {
                n += 2;
                Console.Write(n + " ");
            }
            Console.WriteLine();

            // do-while 循环: 先执行后判断, 至少执行一次
            Console.Write("do-while: ");
            int k = 5;
            do
            {
                Console.Write(k + " ");
                k--;
            } while (k > 0);
            Console.WriteLine();

            // foreach: 遍历集合/数组
            string[] fruits = { "苹果", "香蕉", "橘子" };
            Console.Write("foreach: ");
            foreach (string fruit in fruits)
                Console.Write(fruit + " ");
            Console.WriteLine();

            // ===== 4. break 和 continue =====
            Console.Write("break 演示 (遇到3停止): ");
            for (int i = 1; i <= 10; i++)
            {
                if (i == 3) break;      // 直接跳出循环
                Console.Write(i + " ");
            }
            Console.WriteLine();

            Console.Write("continue 演示 (跳过3的倍数): ");
            for (int i = 1; i <= 10; i++)
            {
                if (i % 3 == 0) continue; // 跳过本次, 继续下一次
                Console.Write(i + " ");
            }
            Console.WriteLine();

            // ===== 5. 嵌套循环 (九九乘法表) =====
            Console.WriteLine("九九乘法表:");
            for (int i = 1; i <= 9; i++)
            {
                for (int j = 1; j <= i; j++)
                    Console.Write($"{j}×{i}={i * j}\t");
                Console.WriteLine();
            }

            // ===== 6. C# 9+ 模式匹配 =====
            object value = 42;
            string description = value switch
            {
                int i when i > 100 => "大整数",           // when 守卫
                int i when i > 0 => "正整数",
                int i when i == 0 => "零",
                int => "负整数",
                string s => $"字符串: {s}",
                null => "null",
                _ => "其他类型"
            };
            Console.WriteLine($"模式匹配: {description}");

            Console.WriteLine();
        }
    }
}
