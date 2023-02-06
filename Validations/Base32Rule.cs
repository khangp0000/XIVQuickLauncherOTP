using System;
using System.Globalization;
using System.Windows.Controls;
using OtpNet;

namespace XIVQuickLauncherOTP.Validations;

public class Base32Rule : ValidationRule
{
    public int MinBytes { get; set; }
    public int MaxBytes { get; set; }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var bytes = new Span<byte>(new byte[MaxBytes + 1]);
        try
        {
            var bytesWritten = Base32Encoding.ToBytes((string)value).Length;
            if (bytesWritten < MinBytes || bytesWritten > MaxBytes)
                return new ValidationResult(false, "Invalid base32 secret.");

            return ValidationResult.ValidResult;
        }
        catch (Exception)
        {
            return new ValidationResult(false, "Invalid base32 secret.");
        }
    }
}