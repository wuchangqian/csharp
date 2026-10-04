namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 05: 字符串
    /// 知识点: string 基本操作、string vs StringBuilder、插值、正则表达式、可空字符串
    /// </summary>
    public static class L05_Strings
    {
        public static void Run()
        {
            L01_Basics.PrintHeader("模块 05: 字符串");

            // ===== 1. 字符串基本操作 =====
            string s = "Hello, World!";
            Console.WriteLine($"原始: \"{s}\"");
            Console.WriteLine($"长度: {s.Length}");
            Console.WriteLine($"大写: {s.ToUpper()}");
            Console.WriteLine($"小写: {s.ToLower()}");

            // 索引访问: 返回 char
            Console.WriteLine($"s[0]={s[0]}, s[7]={s[7]}");

            // 子串
            Console.WriteLine($"Substring(7): \"{s.Substring(7)}\"");
            Console.WriteLine($"Substring(0,5): \"{s.Substring(0, 5)}\"");

            // 查找
            Console.WriteLine($"IndexOf('o')={s.IndexOf('o')}");
            Console.WriteLine($"LastIndexOf('o')={s.LastIndexOf('o')}");
            Console.WriteLine($"Contains(\"World\")={s.Contains("World")}");
            Console.WriteLine($"StartsWith(\"Hello\")={s.StartsWith("Hello")}");
            Console.WriteLine($"EndsWith(\"!\")={s.EndsWith("!")}");

            // ===== 2. 字符串拼接 =====
            string a = "Hello", b = "World";
            string plus = a + ", " + b + "!";        // + 拼接
            string concat = string.Concat(a, b);      // Concat
            string joined = string.Join("-", "a", "b", "c"); // Join
            Console.WriteLine($"+: \"{plus}\"");
            Console.WriteLine($"Concat: \"{concat}\"");
            Console.WriteLine($"Join: \"{joined}\"");

            // ===== 3. 字符串插值 (C# 6+) =====
            // $ 前缀, 大括号内写表达式
            string name = "张三";
            int age = 25;
            string interpolated = $"姓名:{name}, 年龄:{age}, 明年:{age + 1}岁";
            Console.WriteLine($"插值: {interpolated}");

            // 逐字插值字符串 ($@)
            string path = $@"C:\Users\{name}\Documents";
            Console.WriteLine($"路径: {path}");

            // ===== 4. 分割与替换 =====
            string csv = "张三,25,银川,程序员";
            string[] parts = csv.Split(',');
            Console.WriteLine($"Split: [{string.Join(" | ", parts)}]");

            string replaced = s.Replace("World", "C#");
            Console.WriteLine($"Replace: \"{replaced}\"");

            // 去空白
            string dirty = "  hello  ";
            Console.WriteLine($"Trim: \"{dirty.Trim()}\"");
            Console.WriteLine($"TrimStart: \"{dirty.TrimStart()}\"");
            Console.WriteLine($"TrimEnd: \"{dirty.TrimEnd()}\"");

            // ===== 5. string 是不可变的 (Immutable) =====
            // 每次操作都创建新字符串, 原字符串不变
            string original = "abc";
            string modified = original.ToUpper();
            Console.WriteLine($"不可变: original=\"{original}\", modified=\"{modified}\"");

            // 频繁拼接会产生大量临时字符串 → 用 StringBuilder
            // 类比: string = 每次抄写全文; StringBuilder = 白板, 可擦改

            // ===== 6. StringBuilder =====
            var sb = new System.Text.StringBuilder();
            sb.Append("第一行\n");
            sb.Append("第二行\n");
            sb.AppendLine("第三行");  // 自动加换行
            sb.AppendFormat("格式化: {0:F2}\n", 3.14159);
            sb.Insert(0, "开头插入: ");
            sb.Replace("行", "行(改)");
            Console.WriteLine($"StringBuilder:\n{sb}");

            // 性能对比
            var sw = System.Diagnostics.Stopwatch.StartNew();
            string slow = "";
            for (int i = 0; i < 10000; i++) slow += "a";  // 10000 次分配
            sw.Stop();
            Console.WriteLine($"string 拼接 10000 次: {sw.ElapsedTicks} ticks");

            sw.Restart();
            var fast = new System.Text.StringBuilder();
            for (int i = 0; i < 10000; i++) fast.Append("a");
            string result = fast.ToString();
            sw.Stop();
            Console.WriteLine($"StringBuilder 10000 次: {sw.ElapsedTicks} ticks");

            // ===== 7. 正则表达式 =====
            string text = "联系电话: 13800138000, 邮箱: test@example.com, 邮编: 750001";

            // 提取手机号
            var phoneMatch = System.Text.RegularExpressions.Regex.Match(text, @"\d{11}");
            Console.WriteLine($"手机号: {phoneMatch.Value}");

            // 提取邮箱
            var emailMatch = System.Text.RegularExpressions.Regex.Match(text, @"[\w.-]+@[\w.-]+\.\w+");
            Console.WriteLine($"邮箱: {emailMatch.Value}");

            // 替换
            string masked = System.Text.RegularExpressions.Regex.Replace(text, @"\d{11}", "***********");
            Console.WriteLine($"脱敏: {masked}");

            // 验证
            bool isValidEmail = System.Text.RegularExpressions.Regex.IsMatch(
                "user@example.com", @"^[\w.-]+@[\w.-]+\.\w+$");
            Console.WriteLine($"邮箱验证: {isValidEmail}");

            Console.WriteLine();
        }
    }
}
