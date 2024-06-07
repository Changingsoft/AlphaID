namespace AuthCenterWebApp.Pages.Account;

/// <summary>
///     ��¼ѡ�
/// </summary>
public class LoginOptions
{
    /// <summary>
    /// �����ص�¼
    /// </summary>
    public bool AllowLocalLogin { get; set; } = true;

    /// <summary>
    /// �����ס�û���¼��
    /// </summary>
    public bool AllowRememberLogin { get; set; } = true;

    /// <summary>
    /// ��ס�û���¼�������
    /// </summary>
    public int RememberMeLoginDuration { get; set; } = 30;

    /// <summary>
    /// ����ƾ��ʱ����Ϣ��
    /// </summary>
    public string InvalidCredentialsErrorMessage { get; set; } = "�û�����������Ч";
}