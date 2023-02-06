using System;
using System.Globalization;
using System.Windows.Controls;

namespace XIVQuickLauncherOTP.Validations;

public class IntRangeRule : ValidationRule
{
    public int Min { get; set; }
    public int Max { get; set; }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var val = 0;

        try
        {
            if (((string)value).Length > 0)
                val = int.Parse((string)value);
        }
        catch (Exception)
        {
            return new ValidationResult(false,
                $"Please enter a number in the range: {Min}-{Max}.");
        }

        if (val < Min || val > Max)
            return new ValidationResult(false,
                $"Please enter a number in the range: {Min}-{Max}.");

        return ValidationResult.ValidResult;
    }
}