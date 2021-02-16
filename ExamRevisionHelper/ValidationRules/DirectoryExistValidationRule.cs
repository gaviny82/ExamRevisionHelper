using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace ExamRevisionHelper.ValidationRules
{
    class DirectoryExistValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string path = value as string;
            if (string.IsNullOrEmpty(path)) return new ValidationResult(false, "Directory does not exisit.");
            string[] split = path.Split('\\');
            if (split.Length > 1)
            {
                if(split.Last()=="Past Papers")
                    return Directory.Exists(path.Substring(0, path.Length - split.Last().Length - 1)) ? ValidationResult.ValidResult : new ValidationResult(false, "Directory does not exisit.");
            }
            return Directory.Exists(path) ? ValidationResult.ValidResult : new ValidationResult(false, "Directory does not exisit.");
        }
    }
}
