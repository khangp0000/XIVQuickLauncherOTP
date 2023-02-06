using System.Configuration;
using OtpNet;

namespace XIVQuickLauncherOTP.Settings;

internal sealed class UserTotpSettings : ApplicationSettingsBase
{
    [UserScopedSetting]
    public byte[]? UserTotpSecret
    {
        get => (byte[])this[nameof(UserTotpSecret)];
        set => this[nameof(UserTotpSecret)] = value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("Sha1")]
    public OtpHashMode UserTotpHashMode
    {
        get => (OtpHashMode)this[nameof(UserTotpHashMode)];
        set => this[nameof(UserTotpHashMode)] = value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("6")]
    [IntegerValidator(MinValue = 6, MaxValue = 8,
        ExcludeRange = false)]
    public int UserTotpDigits
    {
        get => (int)this[nameof(UserTotpDigits)];
        set => this[nameof(UserTotpDigits)] = value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("30")]
    [IntegerValidator(MinValue = 30, MaxValue = 90,
        ExcludeRange = false)]
    public int UserTotpTimeSteps
    {
        get => (int)this[nameof(UserTotpTimeSteps)];
        set => this[nameof(UserTotpTimeSteps)] = value;
    }
    
    [UserScopedSetting]
    [DefaultSettingValue("false")]
    public bool IsSendingHttpRequest
    {
        get => (bool)this[nameof(IsSendingHttpRequest)];
        set => this[nameof(IsSendingHttpRequest)] = value;
    }
}