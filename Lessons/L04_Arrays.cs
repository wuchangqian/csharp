namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 04: 数组
    /// 知识点: 一维数组、多维数组、交错数组、Array 类、数组作为参数/返回值
    /// </summary>
    public static class L04_Arrays
    {
        public static void Run()
        {
            L01_Basics.PrintHeader("模块 04: 数组");

            // ===== 1. 一维数组 =====
            // 声明 + 初始化
            int[] scores = new int[5];           // 长度 5, 默认值 0
            int[] nums = { 10, 20, 30, 40, 50 }; // 集合初始化器
            int[] nums2 = new[] { 1, 2, 3 };     // 隐式类型数组

            Console.Write("一维数组: ");
            foreach (var n in nums) Console.Write(n + " ");
            Console.WriteLine($"长度={nums.Length}");

            // 索引访问 (从 0 开始)
            Console.WriteLine($"nums[0]={nums[0]}, nums[4]={nums[4]}");

            // 修改
            nums[0] = 100;
            Console.WriteLine($"修改后 nums[0]={nums[0]}");

            // ===== 2. 多维数组 (矩形数组) =====
            // 所有行长度相同, 用逗号分隔维度
            // 类比: Excel 表格, 行列固定
            int[,] grid = new int[3, 4];  // 3行4列
            int[,] matrix = {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };

            Console.WriteLine("二维数组 (3×3):");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    Console.Write($"{matrix[i, j]}\t");
                Console.WriteLine();
            }
            Console.WriteLine($"总元素数: {matrix.Length}, 维度数: {matrix.Rank}");

            // ===== 3. 交错数组 (Jagged Array) =====
            // 数组的数组, 每行长度可以不同
            // 类比: 不规则锯齿, 每行长短不一
            int[][] jagged = new int[3][];
            jagged[0] = new int[] { 1, 2 };
            jagged[1] = new int[] { 3, 4, 5, 6 };
            jagged[2] = new int[] { 7, 8, 9 };

            Console.WriteLine("交错数组:");
            for (int i = 0; i < jagged.Length; i++)
            {
                Console.Write($"  行{i}: ");
                foreach (var v in jagged[i]) Console.Write(v + " ");
                Console.WriteLine();
            }

            // ===== 4. Array 类常用方法 =====
            int[] arr = { 5, 3, 8, 1, 9, 2 };

            // 排序
            Array.Sort(arr);
            Console.WriteLine($"排序后: [{string.Join(", ", arr)}]");

            // 反转
            Array.Reverse(arr);
            Console.WriteLine($"反转后: [{string.Join(", ", arr)}]");

            // 查找
            int index = Array.IndexOf(arr, 8);
            Console.WriteLine($"IndexOf(8) = {index}");

            // 二分查找 (需要先排序)
            Array.Sort(arr);
            int idx = Array.BinarySearch(arr, 5);
            Console.WriteLine($"BinarySearch(5) = {idx}");

            // 复制
            int[] copy = new int[arr.Length];
            Array.Copy(arr, copy, arr.Length);
            Console.WriteLine($"副本: [{string.Join(", ", copy)}]");

            // Resize (改变大小)
            Array.Resize(ref arr, 8);
            Console.WriteLine($"Resize 后长度: {arr.Length}");

            // Clear (置默认值)
            Array.Clear(arr, 5, 3);  // 从索引5开始清空3个元素
            Console.WriteLine($"Clear 后: [{string.Join(", ", arr)}]");

            // ===== 5. 数组作为参数和返回值 =====
            int[] original = { 1, 2, 3, 4, 5 };
            int[] doubled = DoubleArray(original);
            Console.WriteLine($"翻倍: [{string.Join(", ", doubled)}]");

            // params 数组参数 (见模块03)
            // 注意: 数组是引用类型, 方法内修改会影响调用方
            ModifyArray(original);
            Console.WriteLine($"ModifyArray 后: [{string.Join(", ", original)}]");

            Console.WriteLine();
        }

        static int[] DoubleArray(int[] source)
        {
            int[] result = new int[source.Length];
            for (int i = 0; i < source.Length; i++)
                result[i] = source[i] * 2;
            return result;
        }

        static void ModifyArray(int[] arr)
        {
            // 数组是引用类型, 修改元素影响调用方
            for (int i = 0; i < arr.Length; i++)
                arr[i] += 100;
            // 但重新赋值不影响调用方 (只改了局部变量指向)
            // arr = new int[] { 0 };
        }
    }
}
