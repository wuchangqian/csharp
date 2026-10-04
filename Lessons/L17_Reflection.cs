using System.Reflection;

namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 17: 特性 (Attribute) 与反射 (Reflection)
    /// 知识点: 内置特性、自定义特性、Type、MethodInfo、PropertyInfo、特性查询
    /// </summary>
    public static class L17_Reflection
    {
        public static void Run()
        {
            L01_Basics.PrintHeader("模块 17: 特性与反射");

            // ===== 1. 内置常用特性 =====
            // [Obsolete]: 标记为过时
            // [Serializable]: 可序列化
            // [DllImport]: 调用非托管 DLL
            // [Conditional]: 条件编译

            // OldMethod();  // 会警告

            // ===== 2. 自定义特性 =====
            // 所有特性继承自 Attribute
            // AttributeUsage 限制特性的使用范围

            // ===== 3. 获取类型信息 (反射) =====
            Type type = typeof(BookRecord);
            Console.WriteLine($"类型: {type.FullName}");
            Console.WriteLine($"基类: {type.BaseType?.Name}");
            Console.WriteLine($"是否类: {type.IsClass}");
            Console.WriteLine($"是否公开: {type.IsPublic}");

            // 获取特性
            var tableAttr = type.GetCustomAttribute<TableAttribute>();
            if (tableAttr != null)
                Console.WriteLine($"Table特性: 表名={tableAttr.Name}");

            // ===== 4. 获取属性 (Property) =====
            Console.WriteLine("\n=== 属性 ===");
            PropertyInfo[] props = type.GetProperties();
            foreach (var prop in props)
            {
                Console.Write($"  {prop.PropertyType.Name} {prop.Name}");

                // 查询属性的 Column 特性
                var colAttr = prop.GetCustomAttribute<ColumnAttribute>();
                if (colAttr != null)
                {
                    Console.Write($" [Column: {colAttr.Name}");
                    if (colAttr.IsPrimaryKey) Console.Write(", PK");
                    if (!colAttr.AllowNull) Console.Write(", NOT NULL");
                    Console.Write("]");
                }
                Console.WriteLine();
            }

            // ===== 5. 获取方法 (Method) =====
            Console.WriteLine("\n=== 方法 ===");
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
                Console.WriteLine($"  {method.ReturnType.Name} {method.Name}({string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name))})");

            // ===== 6. 动态创建实例 =====
            Console.WriteLine("\n=== 动态创建实例 ===");
            // 通过反射创建对象 (调用无参构造)
            object? instance = Activator.CreateInstance(type);
            Console.WriteLine($"  创建: {instance}");

            // 设置属性值
            var titleProp = type.GetProperty("Title");
            titleProp?.SetValue(instance, "反射设置的标题");
            Console.WriteLine($"  设置Title后: {instance}");

            // 获取属性值
            var titleVal = titleProp?.GetValue(instance);
            Console.WriteLine($"  读取Title: {titleVal}");

            // 调用方法
            var displayMethod = type.GetMethod("Display");
            displayMethod?.Invoke(instance, null);

            // ===== 7. 反射 + 泛型方法 =====
            Console.WriteLine("\n=== 反射调用泛型方法 ===");
            var processor = new DataProcessor();
            Type procType = typeof(DataProcessor);

            // MakeGenericMethod: 构造具体的泛型方法
            var genericMethod = procType.GetMethod("Process")!.MakeGenericMethod(typeof(string));
            genericMethod.Invoke(processor, new object[] { "泛型反射数据" });

            // ===== 8. 简单 ORM 映射 (特性 + 反射的应用) =====
            Console.WriteLine("\n=== 简单 ORM 映射演示 ===");
            var book = new BookRecord { Id = 1, Title = "C#反射", Author = "张三", Price = 59.9m };
            string sql = SimpleOrm.GenerateInsertSql(book);
            Console.WriteLine(sql);
        }

        // [Obsolete("请使用 NewMethod 替代", error: false)]
        static void OldMethod() { }
    }

    // ===== 自定义特性 =====

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        public string Name { get; }
        public TableAttribute(string name) => Name = name;
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; }
        public bool IsPrimaryKey { get; set; }
        public bool AllowNull { get; set; } = true;
        public ColumnAttribute(string name) => Name = name;
    }

    // 使用特性标注的实体类
    [Table("Books")]
    public class BookRecord
    {
        [Column("id", AllowNull = false)]
        public int Id { get; set; }

        [Column("title", AllowNull = false)]
        public string Title { get; set; } = "";

        [Column("author")]
        public string Author { get; set; } = "";

        [Column("price")]
        public decimal? Price { get; set; }

        public void Display() =>
            Console.WriteLine($"  [Book] {Id}: {Title} by {Author}, ¥{Price}");
    }

    // 泛型方法示例
    public class DataProcessor
    {
        public void Process<T>(T data) =>
            Console.WriteLine($"  Process<{typeof(T).Name}>: {data}");
    }

    // ORM 映射: 通过反射读特性, 生成 SQL
    public static class SimpleOrm
    {
        public static string GenerateInsertSql(object entity)
        {
            Type type = entity.GetType();
            var tableAttr = type.GetCustomAttribute<TableAttribute>();
            string tableName = tableAttr?.Name ?? type.Name;

            var columns = new List<string>();
            var values = new List<string>();

            foreach (var prop in type.GetProperties())
            {
                var colAttr = prop.GetCustomAttribute<ColumnAttribute>();
                if (colAttr == null) continue;

                columns.Add(colAttr.Name);
                var value = prop.GetValue(entity);
                values.Add(value switch
                {
                    null => "NULL",
                    string s => $"'{s}'",
                    _ => value.ToString() ?? "NULL"
                });
            }

            return $"INSERT INTO {tableName} ({string.Join(", ", columns)})\nVALUES ({string.Join(", ", values)});";
        }
    }
}
