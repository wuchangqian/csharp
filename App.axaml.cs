using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CSharpLearningProject.AuthDemo;
using CSharpLearningProject.ViewModels;
using CSharpLearningProject.Views;

namespace CSharpLearningProject;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // 启动闸门: 必须先登录, 成功后才创建主窗口.
            // 建库 (建表/种子) 同步执行, 确保登录前 users 表存在.
            Database.Initialize();

            var login = new LoginView(onLoginSuccess: () =>
            {
                // 登录成功: 切换主窗口为 MainWindow, 然后关闭登录窗.
                var main = new MainWindow
                {
                    DataContext = new MainViewModel(),
                };
                desktop.MainWindow = main;
                main.Show();
            });

            // 登录窗作为初始主窗口. 登录窗关闭时若主窗口已 Show, 应用不退出.
            desktop.MainWindow = login;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
