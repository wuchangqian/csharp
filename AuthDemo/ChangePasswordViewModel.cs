using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpLearningProject.ViewModels;

namespace CSharpLearningProject.AuthDemo;

/// <summary>
/// 修改密码视图模型. 校验旧密码 + 新密码一致后调用 UserService.ChangePassword.
/// 成功回调通知 view 关窗; 失败统一提示, 不区分原因以防探测.
/// </summary>
public partial class ChangePasswordViewModel : ViewModelBase
{
    private readonly Action _onSuccess;

    [ObservableProperty]
    private string _oldPassword = "";

    [ObservableProperty]
    private string _newPassword = "";

    [ObservableProperty]
    private string _confirmPassword = "";

    [ObservableProperty]
    private string _errorMessage = "";

    public ChangePasswordViewModel(Action onSuccess)
    {
        _onSuccess = onSuccess;
    }

    [RelayCommand]
    private void Submit()
    {
        if (string.IsNullOrWhiteSpace(OldPassword)
            || string.IsNullOrWhiteSpace(NewPassword)
            || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ErrorMessage = "请填写所有字段";
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "两次输入的新密码不一致";
            return;
        }

        // 当前未登录则视为会话失效, 统一按失败处理.
        var user = AuthSession.Current;
        if (user is null)
        {
            ErrorMessage = "会话已失效, 请重新登录";
            return;
        }

        if (!UserService.ChangePassword(user.Id, OldPassword, NewPassword))
        {
            // 旧密码错误或用户不存在, 统一提示以防探测.
            ErrorMessage = "旧密码错误";
            return;
        }

        ErrorMessage = "";
        _onSuccess.Invoke();
    }
}
