using Microsoft.Data.Sqlite;

namespace CSharpLearningProject.AuthDemo;

/// <summary>
/// SQLite 数据库初始化与连接管理.
/// 数据库文件 auth.db 位于运行目录, 首次运行自动建表并写入种子数据.
/// </summary>
internal static class Database
{
    public const string ConnectionString = "Data Source=auth.db";

    /// <summary>
    /// 建表(若不存在)并写入种子数据(若为空). 重复调用安全.
    /// </summary>
    public static void Initialize()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        CreateSchema(connection);
        SeedIfEmpty(connection);
    }

    private static void CreateSchema(SqliteConnection connection)
    {
        // users: 用户表, username 唯一, 密码存哈希
        const string users = """
            CREATE TABLE IF NOT EXISTS users (
                id            INTEGER PRIMARY KEY AUTOINCREMENT,
                username      TEXT NOT NULL UNIQUE,
                password_hash TEXT NOT NULL,
                created_at    TEXT NOT NULL DEFAULT (datetime('now'))
            );
            """;

        // categories: 自引用树, parent_id 为 NULL 表示一级分类
        const string categories = """
            CREATE TABLE IF NOT EXISTS categories (
                id        INTEGER PRIMARY KEY AUTOINCREMENT,
                name      TEXT NOT NULL,
                parent_id INTEGER,
                FOREIGN KEY (parent_id) REFERENCES categories(id)
            );
            """;

        // items: 分类下的知识点条目
        const string items = """
            CREATE TABLE IF NOT EXISTS items (
                id          INTEGER PRIMARY KEY AUTOINCREMENT,
                category_id INTEGER NOT NULL,
                title       TEXT NOT NULL,
                description TEXT NOT NULL,
                difficulty  TEXT NOT NULL,
                FOREIGN KEY (category_id) REFERENCES categories(id)
            );
            """;

        foreach (var sql in new[] { users, categories, items })
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }

    private static void SeedIfEmpty(SqliteConnection connection)
    {
        // 仅在 admin 不存在时写入种子, 避免重复哈希与重复数据.
        if (Exists(connection, "SELECT 1 FROM users WHERE username = @username", ("@username", "admin")))
            return;

        SeedAdmin(connection);
        SeedCategoriesAndItems(connection);
    }

    private static void SeedAdmin(SqliteConnection connection)
    {
        // bcrypt 哈希明文 123456; 明文不出现在任何持久化或日志中.
        var hash = BCrypt.Net.BCrypt.HashPassword("123456", workFactor: 10);

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO users (username, password_hash) VALUES (@username, @hash)";
        cmd.Parameters.AddWithValue("@username", "admin");
        cmd.Parameters.AddWithValue("@hash", hash);
        cmd.ExecuteNonQuery();
    }

    private static void SeedCategoriesAndItems(SqliteConnection connection)
    {
        // 一级分类 → 子分类 → 条目, 用事务一次性写入.
        using var tx = connection.BeginTransaction();
        try
        {
            // 一级分类
            var basics = InsertCategory(connection, tx, "语言基础", null);
            var oop = InsertCategory(connection, tx, "面向对象", null);
            var advanced = InsertCategory(connection, tx, "进阶特性", null);

            // 子分类
            var typesVars = InsertCategory(connection, tx, "类型与变量", basics);
            var controlFlow = InsertCategory(connection, tx, "控制流", basics);
            var stringsCol = InsertCategory(connection, tx, "字符串与集合", basics);

            var classesObj = InsertCategory(connection, tx, "类与对象", oop);
            var inheritance = InsertCategory(connection, tx, "继承与多态", oop);
            var linq = InsertCategory(connection, tx, "LINQ", oop);

            var asyncTask = InsertCategory(connection, tx, "异步编程", advanced);
            var reflection = InsertCategory(connection, tx, "反射与特性", advanced);

            // 各子分类下的知识点条目
            InsertItem(connection, tx, typesVars, "值类型与引用类型", "struct vs class 的内存语义", "入门");
            InsertItem(connection, tx, typesVars, "var 与显式类型", "隐式类型的使用边界", "入门");
            InsertItem(connection, tx, controlFlow, "if / switch 模式匹配", "switch 表达式与 relational 模式", "入门");
            InsertItem(connection, tx, controlFlow, "for / foreach / while", "循环与迭代器选择", "入门");
            InsertItem(connection, tx, stringsCol, "StringBuilder 与字符串驻留", "减少分配的字符串拼接", "入门");
            InsertItem(connection, tx, stringsCol, "正则表达式", "Regex 常用捕获与替换", "进阶");
            InsertItem(connection, tx, classesObj, "封装与属性", "get/set/init 与 backing field", "入门");
            InsertItem(connection, tx, classesObj, "record 与 with", "不可变数据模型与非破坏更新", "进阶");
            InsertItem(connection, tx, inheritance, "virtual / override / base", "多态与基类调用", "中级");
            InsertItem(connection, tx, inheritance, "接口与抽象类", "何时用接口何时用抽象类", "中级");
            InsertItem(connection, tx, linq, "查询与方法语法", "from-select 与 Where/Select", "中级");
            InsertItem(connection, tx, linq, "GroupBy / Join", "分组连接与聚合", "进阶");
            InsertItem(connection, tx, asyncTask, "async / await", "Task 与同步上下文", "进阶");
            InsertItem(connection, tx, asyncTask, "CancellationToken", "可取消的异步协作", "进阶");
            InsertItem(connection, tx, reflection, "Attribute 与反射读取", "运行时元数据获取", "高级");
            InsertItem(connection, tx, reflection, "动态加载程序集", "Assembly.Load 与类型发现", "高级");

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    private static int InsertCategory(SqliteConnection connection, SqliteTransaction tx, string name, int? parentId)
    {
        using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = "INSERT INTO categories (name, parent_id) VALUES (@name, @parent_id); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@parent_id", (object?)parentId ?? DBNull.Value);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    private static void InsertItem(SqliteConnection connection, SqliteTransaction tx, int categoryId, string title, string description, string difficulty)
    {
        using var cmd = connection.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = "INSERT INTO items (category_id, title, description, difficulty) VALUES (@cid, @title, @desc, @diff)";
        cmd.Parameters.AddWithValue("@cid", categoryId);
        cmd.Parameters.AddWithValue("@title", title);
        cmd.Parameters.AddWithValue("@desc", description);
        cmd.Parameters.AddWithValue("@diff", difficulty);
        cmd.ExecuteNonQuery();
    }

    private static bool Exists(SqliteConnection connection, string sql, params (string Name, object Value)[] parameters)
    {
        using var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        foreach (var (name, value) in parameters)
            cmd.Parameters.AddWithValue(name, value);
        return cmd.ExecuteScalar() is not null;
    }
}
