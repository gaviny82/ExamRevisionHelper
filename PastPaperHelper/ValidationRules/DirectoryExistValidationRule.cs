using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace PastPaperHelper.ValidationRules
{
    class DirectoryExistValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string path = value as string;
            if (string.IsNullOrEmpty(path)) return new ValidationResult(false, "Directory does not exisit.");
            string[] split = path.Split('\\');
            return Directory.Exists(path.Substring(0,path.Length-split.Last().Length-1)) ? ValidationResult.ValidResult : new ValidationResult(false, "Directory does not exisit.");
        }
    }
}
