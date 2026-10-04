namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 12: LINQ
    /// 知识点: 查询语法、方法语法、Where/Select/OrderBy/GroupBy/Join/聚合、延迟执行
    /// </summary>
    public static class L12_LINQ
    {
        // 数据源
        record Student(string Name, int Age, string Major, double Score);
        record Course(string Name, string Major, int Credits);

        static List<Student> students = new()
        {
            new Student("张三", 20, "计算机", 85.5),
            new Student("李四", 22, "数学", 92.0),
            new Student("王五", 21, "计算机", 78.5),
            new Student("赵六", 23, "物理", 88.0),
            new Student("钱七", 20, "数学", 95.5),
            new Student("孙八", 22, "计算机", 76.0),
            new Student("周九", 21, "物理", 81.5),
            new Student("吴十", 20, "计算机", 90.0),
        };

        static List<Course> courses = new()
        {
            new Course("数据结构", "计算机", 4),
            new Course("高等数学", "数学", 5),
            new Course("大学物理", "物理", 4),
            new Course("操作系统", "计算机", 3),
        };

        public static void Run()
        {
            L01_Basics.PrintHeader("模块 12: LINQ");

            // LINQ 两种写法: 查询语法 vs 方法语法
            // 查询语法像 SQL, 方法语法用 Lambda 链式调用
            // 编译后完全等价, 方法语法更灵活

            // ===== 1. Where (过滤) =====
            Console.WriteLine("=== Where: 计算机专业学生 ===");

            // 方法语法
            var csStudents = students.Where(s => s.Major == "计算机");
            foreach (var s in csStudents)
                Console.WriteLine($"  {s.Name}, {s.Score}");

            // 查询语法 (等价)
            var csStudents2 = from s in students
                              where s.Major == "计算机"
                              select s;

            // ===== 2. Select (投影/转换) =====
            Console.WriteLine("\n=== Select: 只取姓名 ===");
            var names = students.Select(s => s.Name);
            Console.WriteLine($"  [{string.Join(", ", names)}]");

            // 匿名类型 (匿名对象)
            var summaries = students.Select(s => new { s.Name, s.Score, Pass = s.Score >= 60 });
            foreach (var s in summaries)
                Console.WriteLine($"  {s.Name}: {s.Score} ({(s.Pass ? "及格" : "不及格")})");

            // ===== 3. OrderBy (排序) =====
            Console.WriteLine("\n=== OrderBy: 按分数降序 ===");
            var sorted = students.OrderByDescending(s => s.Score).ThenBy(s => s.Name);
            foreach (var s in sorted)
                Console.WriteLine($"  {s.Name}: {s.Score}");

            // ===== 4. GroupBy (分组) =====
            Console.WriteLine("\n=== GroupBy: 按专业分组 ===");
            var grouped = students.GroupBy(s => s.Major);
            foreach (var g in grouped)
            {
                Console.WriteLine($"  {g.Key} ({g.Count()}人):");
                foreach (var s in g)
                    Console.WriteLine($"    {s.Name} {s.Score}");
            }

            // ===== 5. 聚合函数 =====
            Console.WriteLine("\n=== 聚合 ===");
            Console.WriteLine($"  总人数: {students.Count()}");
            Console.WriteLine($"  最高分: {students.Max(s => s.Score)}");
            Console.WriteLine($"  最低分: {students.Min(s => s.Score)}");
            Console.WriteLine($"  平均分: {students.Average(s => s.Score):F2}");
            Console.WriteLine($"  总分: {students.Sum(s => s.Score)}");

            // ===== 6. Join (连接) =====
            Console.WriteLine("\n=== Join: 学生 ↔ 课程 ===");
            var joined = from s in students
                         join c in courses on s.Major equals c.Major
                         select new { s.Name, s.Major, Course = c.Name, c.Credits };

            foreach (var item in joined)
                Console.WriteLine($"  {item.Name} ({item.Major}) → {item.Course} ({item.Credits}学分)");

            // 方法语法 Join
            var joined2 = students.Join(
                courses,
                s => s.Major,
                c => c.Major,
                (s, c) => new { s.Name, Course = c.Name, c.Credits });

            // ===== 7. GroupJoin (左连接) =====
            Console.WriteLine("\n=== GroupJoin: 专业 → 学生列表 ===");
            var groupJoin = from c in courses
                            join s in students on c.Major equals s.Major into studentGroup
                            select new
                            {
                                Course = c.Name,
                                Students = studentGroup
                            };

            foreach (var g in groupJoin)
            {
                Console.WriteLine($"  {g.Course}:");
                foreach (var s in g.Students)
                    Console.WriteLine($"    {s.Name}");
            }

            // ===== 8. 延迟执行 (Deferred Execution) =====
            Console.WriteLine("\n=== 延迟执行 ===");
            var query = students.Where(s => s.Score > 80);
            Console.WriteLine($"  (1) query 创建, 尚未执行");

            students.Add(new Student("新同学", 20, "计算机", 85.0));
            Console.WriteLine($"  (2) 添加新学生后遍历 query:");
            foreach (var s in query)
                Console.WriteLine($"    {s.Name} {s.Score}");  // 包含新添加的学生

            // 强制立即执行
            var snapshot = students.Where(s => s.Score > 80).ToList();  // ToList 立即执行
            students.Add(new Student("另一新同学", 20, "计算机", 82.0));
            Console.WriteLine($"  (3) snapshot 仍为 {snapshot.Count} 人 (已固定)");

            // ===== 9. First / Single / Any / All =====
            Console.WriteLine("\n=== 元素操作 ===");
            var first = students.First(s => s.Major == "数学");
            Console.WriteLine($"  First(数学): {first.Name}");

            var firstOrDefault = students.FirstOrDefault(s => s.Major == "化学");
            Console.WriteLine($"  FirstOrDefault(化学): {(firstOrDefault == null ? "null" : firstOrDefault.Name)}");

            bool anyCs = students.Any(s => s.Major == "计算机");
            Console.WriteLine($"  Any(计算机): {anyCs}");

            bool allPassed = students.All(s => s.Score >= 60);
            Console.WriteLine($"  All(及格): {allPassed}");

            // ===== 10. Distinct / Except / Intersect / Union =====
            Console.WriteLine("\n=== 集合操作 ===");
            var majors = students.Select(s => s.Major).Distinct();
            Console.WriteLine($"  Distinct 专业: [{string.Join(", ", majors)}]");

            var ages1 = new[] { 1, 2, 3, 4 };
            var ages2 = new[] { 3, 4, 5, 6 };
            Console.WriteLine($"  Intersect: [{string.Join(", ", ages1.Intersect(ages2))}]");
            Console.WriteLine($"  Union: [{string.Join(", ", ages1.Union(ages2))}]");
            Console.WriteLine($"  Except: [{string.Join(", ", ages1.Except(ages2))}]");

            // ===== 11. Skip / Take (分页) =====
            Console.WriteLine("\n=== 分页 (Skip/Take) ===");
            var page = students.OrderBy(s => s.Name).Skip(2).Take(3);
            foreach (var s in page)
                Console.WriteLine($"  {s.Name}");

            // ===== 12. ToLookup (多值字典) =====
            Console.WriteLine("\n=== ToLookup ===");
            var lookup = students.ToLookup(s => s.Age);
            foreach (var g in lookup)
            {
                Console.WriteLine($"  {g.Key}岁: {string.Join(", ", g.Select(s => s.Name))}");
            }

            Console.WriteLine();
        }
    }
}
