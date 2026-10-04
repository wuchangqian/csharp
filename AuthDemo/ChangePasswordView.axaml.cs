using Avalonia.Controls;

namespace CSharpLearningProject.AuthDemo;

public partial class ChangePasswordView : Window
{
    public ChangePasswordView() : this(() => { }) { }

    public ChangePasswordView(Action onSuccess)
    {
        InitializeComponent();
        DataContext = new ChangePasswordViewModel(() =>
        {
            // 修改成功: 回调刷新主界面, 再关窗.
            onSuccess();
            Close();
        });
    }
}
