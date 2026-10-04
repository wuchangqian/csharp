using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpLearningProject.ViewModels;

namespace CSharpLearningProject.AuthDemo;

/// <summary>
/// 登录视图模型. 验证成功后回调通知 view 关窗并打开数据窗口.
/// ViewModel 不持有 Window 引用, 通过 OnLoginSuccess 回调解耦.
/// </summary>
public partial class LoginViewModel : ViewModelBase
{
    private readonly Action _onLoginSuccess;

    [ObservableProperty]
    private string _username = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _errorMessage = "";

    public LoginViewModel(Action onLoginSuccess)
    {
        _onLoginSuccess = onLoginSuccess;
    }

    [RelayCommand]
    private void Login()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "请输入用户名和密码";
            return;
        }

        // 统一失败提示, 防用户名枚举.
        if (!UserService.Verify(Username, Password))
        {
            ErrorMessage = "用户名或密码错误";
            return;
        }

        ErrorMessage = "";
        _onLoginSuccess.Invoke();
    }
}
