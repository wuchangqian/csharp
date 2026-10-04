namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 06: 集合
    /// 知识点: List, Dictionary, HashSet, Queue, Stack, SortedList, SortedDictionary, ObservableCollection
    /// </summary>
    public static class L06_Collections
    {
        public static void Run()
        {
            L01_Basics.PrintHeader("模块 06: 集合");

            // ===== 1. List<T> —— 动态数组 =====
            // 类比: 可变长的数组, 自动扩容
            var list = new List<string> { "苹果", "香蕉" };
            list.Add("橘子");
            list.Add("葡萄");
            list.AddRange(new[] { "西瓜", "梨" });
            Console.WriteLine($"List: [{string.Join(", ", list)}]");
            Console.WriteLine($"Count={list.Count}, Capacity={list.Capacity}");

            list.Insert(0, "芒果");
            Console.WriteLine($"Insert(0, mango): [{string.Join(", ", list)}]");

            list.Remove("香蕉");
            list.RemoveAt(0);
            Console.WriteLine($"Remove后: [{string.Join(", ", list)}]");

            bool contains = list.Contains("橘子");
            Console.WriteLine($"Contains(橘子): {contains}");

            // 查找
            int idx = list.FindIndex(s => s.StartsWith("橘"));
            Console.WriteLine($"FindIndex(以橘开头): {idx}");

            // 转换
            List<int> lengths = list.ConvertAll(s => s.Length);
            Console.WriteLine($"各水果字数: [{string.Join(", ", lengths)}]");

            // ===== 2. Dictionary<TKey, TValue> —— 键值对 =====
            // 类比: 字典, 通过词条查释义
            var dict = new Dictionary<string, int>
            {
                ["张三"] = 25,
                ["李四"] = 30
            };
            dict.Add("王五", 28);
            dict["赵六"] = 22;

            Console.WriteLine("\nDictionary:");
            foreach (var kv in dict)
                Console.WriteLine($"  {kv.Key}: {kv.Value}岁");

            // 查找
            if (dict.TryGetValue("张三", out int zAge))
                Console.WriteLine($"张三年龄: {zAge}");

            // 安全访问 (不存在则返回默认值)
            int unknown = dict.TryGetValue("无名氏", out int u) ? u : -1;
            Console.WriteLine($"无名氏: {unknown}");

            // 判断包含键/值
            Console.WriteLine($"ContainsKey(李四): {dict.ContainsKey("李四")}");
            Console.WriteLine($"ContainsValue(30): {dict.ContainsValue(30)}");

            // ===== 3. HashSet<T> —— 去重集合 =====
            // 类比: 数学集合, 元素唯一, 快速判断存在性
            var set = new HashSet<int> { 1, 2, 3, 4, 5 };
            set.Add(3);  // 重复添加无效
            set.Add(6);
            Console.WriteLine($"\nHashSet: [{string.Join(", ", set)}]");

            // 集合运算
            var set2 = new HashSet<int> { 3, 4, 5, 6, 7 };
            Console.WriteLine($"set2: [{string.Join(", ", set2)}]");

            // 交集
            set.IntersectWith(set2);
            Console.WriteLine($"交集: [{string.Join(", ", set)}]");  // 3,4,5

            // 重置再做并集
            var s1 = new HashSet<int> { 1, 2, 3 };
            var s3 = new HashSet<int> { 3, 4, 5 };
            s1.UnionWith(s3);
            Console.WriteLine($"并集: [{string.Join(", ", s1)}]");  // 1,2,3,4,5

            // ===== 4. Queue<T> —— 队列 (先进先出 FIFO) =====
            // 类比: 排队买饭, 先来的先买到
            var queue = new Queue<string>();
            queue.Enqueue("第一位");
            queue.Enqueue("第二位");
            queue.Enqueue("第三位");

            Console.WriteLine("\nQueue:");
            Console.WriteLine($"Peek (看队头): {queue.Peek()}");
            Console.WriteLine($"Dequeue (出队): {queue.Dequeue()}");
            Console.WriteLine($"Dequeue (出队): {queue.Dequeue()}");

            // ===== 5. Stack<T> —— 栈 (后进先出 LIFO) =====
            // 类比: 叠盘子, 最后放的最先拿
            var stack = new Stack<int>();
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);

            Console.WriteLine("\nStack:");
            Console.WriteLine($"Peek (看栈顶): {stack.Peek()}");
            Console.WriteLine($"Pop (出栈): {stack.Pop()}");
            Console.WriteLine($"Pop (出栈): {stack.Pop()}");

            // ===== 6. SortedList 和 SortedDictionary =====
            // SortedList: 键排序的键值对, 内部用两个数组
            var sorted = new SortedList<string, int>
            {
                ["banana"] = 2,
                ["apple"] = 1,
                ["cherry"] = 3
            };
            Console.WriteLine("\nSortedList (按键排序):");
            foreach (var kv in sorted)
                Console.WriteLine($"  {kv.Key}: {kv.Value}");

            // ===== 7. 集合初始化器 =====
            // 对象初始化器 + 集合初始化器
            var point = new Point { X = 10, Y = 20 };  // 对象初始化器
            var points = new List<Point>
            {
                new Point { X = 1, Y = 1 },
                new Point { X = 2, Y = 2 },
                new() { X = 3, Y = 3 }  // C# 9+ target-typed new
            };
            Console.WriteLine($"\nPoints: {points.Count} 个");

            // ===== 8. 元组 (Tuple) =====
            // 多值返回
            var (name, age) = GetPerson();
            Console.WriteLine($"元组: {name}, {age}岁");

            Console.WriteLine();
        }

        class Point
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        static (string name, int age) GetPerson() => ("张三", 25);
    }
}
