using System;
using System.ComponentModel;
using System.Security.Policy;
using OtpNet;

namespace XIVQuickLauncherOTP.ViewModels;

internal class MainWindowViewModel : INotifyPropertyChanged
{
    public MainWindowViewModel()
    {
        OtpString = string.Empty;
        PropertyChanged += OnSecretChange;
    }

    public string? UserTotpSecret { get; set; }
    public OtpHashMode SelectedHashingMode { get; set; }
    public int UserTotpDigits { get; set; }
    public int UserTotpTimeSteps { get; set; }
    public byte[]? UserTotpSecretBytes { get; private set; }
    public string OtpString { get; set; }
    public string XivLauncherUrl => $"http://localhost:4646/ffxivlauncher/{OtpString}";
    public bool IsSendingHttpRequest { get; set; }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnSecretChange(object? sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        if (propertyChangedEventArgs.PropertyName == nameof(UserTotpSecret) && UserTotpSecret != null)
            UserTotpSecretBytes = Base32Encoding.ToBytes(UserTotpSecret);
    }
}