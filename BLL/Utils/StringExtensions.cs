namespace Services.Handlers
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
    }
}
