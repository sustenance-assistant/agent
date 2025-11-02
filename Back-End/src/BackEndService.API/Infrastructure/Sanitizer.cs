using System.Text.RegularExpressions;

namespace BackEndService.API.Infrastructure
{
    public static class Sanitizer
    {
        public static string SanitizeText(string input, int maxLength = 4000)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var trimmed = input.Trim();
            if (trimmed.Length > maxLength) trimmed = trimmed[..maxLength];
            return Regex.Replace(trimmed, "[\u0000-\u001F]", "");
        }
    }
}


