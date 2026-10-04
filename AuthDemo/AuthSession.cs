namespace CSharpLearningProject.AuthDemo;

/// <summary>
/// 登录会话. 登录成功后由 LoginViewModel 写入 Current,
/// 供主界面读取用户名与修改密码使用.
/// </summary>
public static class AuthSession
{
    /// <summary>
    /// 当前登录用户. 未登录时为 null.
    /// </summary>
    public static User? Current { get; set; }

    /// <summary>
    /// 当前登录用户名. 未登录时返回空串, 避免 NRE.
    /// </summary>
    public static string CurrentUsername => Current?.Username ?? "";
}
