using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using OtpNet;
using XIVQuickLauncherOTP.Settings;
using XIVQuickLauncherOTP.ViewModels;

namespace XIVQuickLauncherOTP;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly UserTotpSettings _settings = new();

    private Totp? _userTotp;

    private readonly DispatcherTimer _otpUpdateDispatcherTimer;

    private readonly DispatcherTimer _xivLauncherHttpRequestDispatcherTimer;

    private readonly HttpClient _httpClient;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        Model.SelectedHashingMode = _settings.UserTotpHashMode;
        Model.UserTotpDigits = _settings.UserTotpDigits;
        Model.UserTotpTimeSteps = _settings.UserTotpTimeSteps;
        Model.OtpString = string.Empty;
        Model.IsSendingHttpRequest = _settings.IsSendingHttpRequest;
        if (_settings.UserTotpSecret is { Length: > 0 })
        {
            Model.UserTotpSecret = Base32Encoding.ToString(_settings.UserTotpSecret).ToUpper();
            UpdateTotp();
        }

        _otpUpdateDispatcherTimer = new DispatcherTimer();
        _otpUpdateDispatcherTimer.Tick += otpUpdateDispatcherTimer_Tick;
        _otpUpdateDispatcherTimer.Interval = TimeSpan.Zero;
        _otpUpdateDispatcherTimer.Start();

        _httpClient = new HttpClient();
        _xivLauncherHttpRequestDispatcherTimer = new DispatcherTimer();
        _xivLauncherHttpRequestDispatcherTimer.Tick += xivLauncherHttpRequestDispatcherTimer_Tick;
        _xivLauncherHttpRequestDispatcherTimer.Interval = TimeSpan.FromSeconds(1);

        if (Model.IsSendingHttpRequest)
        {
            _xivLauncherHttpRequestDispatcherTimer.Start();
        }

        Model.PropertyChanged += OnModelPropertyChange;
    }

    private static DateTime DateTimeForceRoundUp(DateTime dt, TimeSpan d)
    {
        return new DateTime((dt.Ticks + d.Ticks) / d.Ticks * d.Ticks, dt.Kind);
    }

    private MainWindowViewModel Model =>
        DataContext as MainWindowViewModel ?? throw new InvalidOperationException();

    private void OnModelPropertyChange(object? sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
        _settings.UserTotpSecret = Model.UserTotpSecretBytes;
        _settings.UserTotpHashMode = Model.SelectedHashingMode;
        _settings.UserTotpDigits = Model.UserTotpDigits;
        _settings.UserTotpTimeSteps = Model.UserTotpTimeSteps;
        _settings.IsSendingHttpRequest = Model.IsSendingHttpRequest;
        _settings.Save();
        UpdateTotp();
        if (propertyChangedEventArgs.PropertyName == nameof(Model.UserTotpTimeSteps))
        {
            _otpUpdateDispatcherTimer.Interval = TimeSpan.Zero;
        }
        else if (propertyChangedEventArgs.PropertyName == nameof(Model.IsSendingHttpRequest))
        {
            if (Model.IsSendingHttpRequest)
            {
                _xivLauncherHttpRequestDispatcherTimer.Start();
            }
            else
            {
                _xivLauncherHttpRequestDispatcherTimer.Stop();
            }
        }
        else
        {
            UpdateTotpString();
        }
    }

    private void UpdateTotp()
    {
        _userTotp = new Totp(Model.UserTotpSecretBytes, Model.UserTotpTimeSteps,
            Model.SelectedHashingMode, Model.UserTotpDigits);
    }

    private void UpdateTotpString()
    {
        Model.OtpString = _userTotp?.ComputeTotp() ?? string.Empty;
    }

    private void otpUpdateDispatcherTimer_Tick(object? sender, EventArgs e)
    {
        UpdateTotpString();

        var now = DateTime.Now;
        var nextRefresh = DateTimeForceRoundUp(now, TimeSpan.FromSeconds(Model.UserTotpTimeSteps));
        _otpUpdateDispatcherTimer.Interval = nextRefresh - now;
        _otpUpdateDispatcherTimer.Start();
    }

    private async void xivLauncherHttpRequestDispatcherTimer_Tick(object? sender, EventArgs e)
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(1));
        try
        {
            await _httpClient.GetAsync(Model.XivLauncherUrl, cts.Token);
        }
        catch (Exception)
        {
            // ignored
        }
    }
}