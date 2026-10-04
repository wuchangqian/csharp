using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 15: 文件 I/O 与序列化
    /// 知识点: File/Directory、Stream、StreamReader/Writer、JSON 序列化、Path
    /// </summary>
    public static class L15_FileIO
    {
        // 设置工作目录
        static string WorkDir = Path.Combine(Path.GetTempPath(), "CSharpLearning");

        public static void Run()
        {
            L01_Basics.PrintHeader("模块 15: 文件 I/O 与序列化");

            Directory.CreateDirectory(WorkDir);
            Console.WriteLine($"  工作目录: {WorkDir}");

            // ===== 1. File 类 (简单读写) =====
            string filePath = Path.Combine(WorkDir, "note.txt");

            // 写入 (覆盖)
            File.WriteAllText(filePath, "第一行内容\n");
            // 追加
            File.AppendAllText(filePath, "第二行内容\n");
            File.AppendAllText(filePath, "第三行内容\n");

            // 读取全部
            string content = File.ReadAllText(filePath);
            Console.WriteLine($"\n=== File.ReadAllText ===\n{content}");

            // 读取行
            string[] lines = File.ReadAllLines(filePath);
            Console.WriteLine($"=== File.ReadAllLines ({lines.Length} 行) ===");
            for (int i = 0; i < lines.Length; i++)
                Console.WriteLine($"  [{i}] {lines[i]}");

            // ===== 2. Path 类 =====
            Console.WriteLine($"\n=== Path ===");
            Console.WriteLine($"  Path.GetFileName: {Path.GetFileName(filePath)}");
            Console.WriteLine($"  Path.GetDirectoryName: {Path.GetDirectoryName(filePath)}");
            Console.WriteLine($"  Path.GetExtension: {Path.GetExtension(filePath)}");
            Console.WriteLine($"  Path.GetFileNameWithoutExtension: {Path.GetFileNameWithoutExtension(filePath)}");
            Console.WriteLine($"  Path.GetTempFileName(): {Path.GetTempFileName()}");
            Console.WriteLine($"  Path.Combine: {Path.Combine("a", "b", "c.txt")}");

            // ===== 3. FileStream + StreamReader/Writer =====
            // 适合大文件, 逐行/逐块处理
            string logPath = Path.Combine(WorkDir, "log.txt");
            using (var writer = new StreamWriter(logPath, append: true))
            {
                for (int i = 0; i < 5; i++)
                    writer.WriteLine($"[{DateTime.Now:HH:mm:ss}] 日志条目 {i + 1}");
            }

            using (var reader = new StreamReader(logPath))
            {
                Console.WriteLine("\n=== StreamReader 逐行 ===");
                string? line;
                int lineNum = 0;
                while ((line = reader.ReadLine()) != null)
                    Console.WriteLine($"  {++lineNum}: {line}");
            }

            // ===== 4. Directory 类 =====
            Console.WriteLine($"\n=== Directory ===");
            string subDir = Path.Combine(WorkDir, "subdir", "data");
            Directory.CreateDirectory(subDir);

            // 创建几个文件
            for (int i = 0; i < 3; i++)
                File.WriteAllText(Path.Combine(subDir, $"file{i}.txt"), $"内容 {i}");

            // 列出目录
            string[] files = Directory.GetFiles(subDir);
            Console.WriteLine($"  文件:");
            foreach (var f in files)
                Console.WriteLine($"    {Path.GetFileName(f)}");

            string[] dirs = Directory.GetDirectories(WorkDir);
            Console.WriteLine($"  子目录:");
            foreach (var d in dirs)
                Console.WriteLine($"    {Path.GetFileName(d)}");

            // ===== 5. JSON 序列化 (System.Text.Json) =====
            Console.WriteLine($"\n=== JSON 序列化 ===");
            var books = new List<BookData>
            {
                new("978-001", "C#入门", "张三", 2024, 5),
                new("978-002", "NET进阶", "李四", 2023, 3)
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,           // 格式化输出
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // 驼峰命名
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            // 序列化
            string json = JsonSerializer.Serialize(books, options);
            Console.WriteLine($"序列化:\n{json}");

            // 写入 JSON 文件
            string jsonPath = Path.Combine(WorkDir, "books.json");
            File.WriteAllText(jsonPath, json);
            Console.WriteLine($"\n已写入: {jsonPath}");

            // 反序列化
            string readJson = File.ReadAllText(jsonPath);
            var deserialized = JsonSerializer.Deserialize<List<BookData>>(readJson);
            Console.WriteLine($"\n反序列化 ({deserialized?.Count} 本):");
            foreach (var b in deserialized!)
                Console.WriteLine($"  {b.Title} - {b.Author} ({b.Year})");

            // ===== 6. JsonSerializerOptions 常用选项 =====
            Console.WriteLine($"\n=== JSON 单对象 ===");
            var single = new BookData("978-003", "设计模式", "王五", 2022, 2);
            string singleJson = JsonSerializer.Serialize(single, options);
            Console.WriteLine(singleJson);

            var parsed = JsonSerializer.Deserialize<BookData>(singleJson);
            Console.WriteLine($"解析: {parsed?.Title} by {parsed?.Author}");

            // ===== 7. 文件信息 FileInfo =====
            Console.WriteLine($"\n=== FileInfo ===");
            var fi = new FileInfo(jsonPath);
            Console.WriteLine($"  文件名: {fi.Name}");
            Console.WriteLine($"  大小: {fi.Length} bytes");
            Console.WriteLine($"  创建时间: {fi.CreationTime:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"  扩展名: {fi.Extension}");

            // ===== 8. 清理 =====
            Console.WriteLine($"\n=== 清理临时目录 ===");
            // Directory.Delete(WorkDir, recursive: true);
            // Console.WriteLine($"  已删除: {WorkDir}");

            Console.WriteLine();
        }

        // 用于 JSON 序列化
        record BookData(string Isbn, string Title, string Author, int Year, int Copies);
    }
}
