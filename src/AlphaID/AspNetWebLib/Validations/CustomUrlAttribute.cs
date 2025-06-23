using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AspNetWebLib.Validations
{
    public class CustomUrlAttribute : ValidationAttribute
    {
        private static readonly Regex s_urlRegex = new Regex(
            @"^(http|https)://([\w-]+(\.[\w-]+)+|localhost)(:\d+)?(/[\w- ./?%&=]*)?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true; // Not required, use [Required] attribute for required fields
            }

            return value is string url && s_urlRegex.IsMatch(url);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(Resources.UrlValidation, name);
        }
    }
}