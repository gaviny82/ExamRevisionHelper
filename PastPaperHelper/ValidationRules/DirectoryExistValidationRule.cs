using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PastPaperHelper
{
    class DirectoryExistValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return Directory.Exists(value as string) ? ValidationResult.ValidResult : new ValidationResult(false, "Directory does not exisit.");
        }
    }
}
