using Microsoft.Data.Sqlite;

namespace CSharpLearningProject.AuthDemo;

/// <summary>
/// 用户登录验证. 所有查询参数化, 密码用 bcrypt 比对, 不输出密码或哈希.
/// </summary>
internal static class UserService
{
    /// <summary>
    /// 按用户名查询用户. 未找到返回 null.
    /// </summary>
    public static User? FindByUsername(string username)
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        // 参数化查询, 防 SQL 注入.
        cmd.CommandText = "SELECT id, username, password_hash FROM users WHERE username = @username";
        cmd.Parameters.AddWithValue("@username", username);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            return null;

        return new User(
            Id: reader.GetInt32(0),
            Username: reader.GetString(1),
            PasswordHash: reader.GetString(2));
    }

    /// <summary>
    /// 按 id 查询用户. 未找到返回 null.
    /// </summary>
    public static User? FindById(int id)
    {
        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, username, password_hash FROM users WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            return null;

        return new User(
            Id: reader.GetInt32(0),
            Username: reader.GetString(1),
            PasswordHash: reader.GetString(2));
    }

    /// <summary>
    /// 修改密码. 先校验旧密码 (bcrypt 比对), 通过后写入新密码哈希.
    /// 返回 false 表示旧密码错误或用户不存在; 不区分两者以防探测.
    /// </summary>
    public static bool ChangePassword(int userId, string oldPassword, string newPassword)
    {
        var user = FindById(userId);
        if (user is null)
            return false;

        // 校验旧密码, 常量时间比对.
        if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
            return false;

        // 新密码哈希, workFactor 10 与种子数据一致.
        var newHash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 10);

        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "UPDATE users SET password_hash = @hash WHERE id = @id";
        cmd.Parameters.AddWithValue("@hash", newHash);
        cmd.Parameters.AddWithValue("@id", userId);

        return cmd.ExecuteNonQuery() > 0;
    }

    /// <summary>
    /// 验证用户名 + 密码. 成功返回 true; 用户名不存在或密码不匹配都返回 false
    /// (统一失败, 防止用户名枚举).
    /// </summary>
    public static bool Verify(string username, string password)
    {
        var user = FindByUsername(username);
        if (user is null)
            return false;

        // bcrypt.Verify 内部处理盐值与重哈希比较, 常量时间比对.
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }
}
