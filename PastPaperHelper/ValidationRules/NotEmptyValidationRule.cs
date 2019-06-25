using System.Globalization;
using System.Windows.Controls;

namespace PastPaperHelper
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString()) ? new ValidationResult(false, "Keyword is required.") : ValidationResult.ValidResult;
        }
    }
}
