using Avalonia.Threading;

namespace CSharpLearningProject.AuthDemo;

/// <summary>
/// AuthDemo 入口: 由 MainViewModel 模块注册表调用.
/// 模块 Run() 在后台线程执行, 故建库可在当前线程, 开窗必须切回 UI 线程.
/// </summary>
internal static class AuthDemoEntry
{
    public static void Run()
    {
        Console.WriteLine("正在打开 SQLite + 登录验证窗口...");

        // 建库在后台线程同步执行, 完成后切 UI 线程开窗.
        Database.Initialize();

        Dispatcher.UIThread.Post(ShowLogin);
    }

    private static void ShowLogin()
    {
        // 通过回调让 view 自己关窗, ViewModel 不持有 Window 引用.
        var window = new LoginView(() => new DataView().Show());
        window.Show();
    }
}
