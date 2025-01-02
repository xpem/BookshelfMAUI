using System.Globalization;
using System.Text;

namespace Services.Utils
{
    public static class StringExtensions
    {
        public static string Truncate(this string txt, int maxLength)
        {
            if (!string.IsNullOrEmpty(txt) && txt.Length > maxLength)
            {
                return txt[..maxLength];
            }

            return txt;
        }

        public static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString.EnumerateRunes())
            {
                var unicodeCategory = Rune.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
