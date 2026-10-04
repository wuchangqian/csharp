using Avalonia.Controls;

namespace CSharpLearningProject.AuthDemo;

public partial class LoginView : Window
{
    public LoginView() : this(() => { }) { }

    public LoginView(Action onLoginSuccess)
    {
        InitializeComponent();
        DataContext = new LoginViewModel(() =>
        {
            // 登录成功: 先关闭登录窗, 再打开数据窗. 在主循环外再启动数据窗.
            onLoginSuccess();
            Close();
        });
    }
}
