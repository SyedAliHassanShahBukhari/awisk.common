using System.Net.Mail;
using System.Text;

namespace awisk.common.Helpers
{
    public static partial class UniversalOpertaions
    {
        public static string NullToEmpty(this string? s) => s ?? string.Empty;
        public static string TrimSafe(this string? s) => s?.Trim() ?? string.Empty;
        public static string? EmptyToNull(this string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        public static string Left(this string? s, int len)
            => string.IsNullOrEmpty(s) || len <= 0 ? string.Empty : (s!.Length <= len ? s : s[..len]);

        public static string Right(this string? s, int len)
            => string.IsNullOrEmpty(s) || len <= 0 ? string.Empty : (s!.Length <= len ? s : s[^len..]);

        public static string ToSlug(this string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            var sb = new StringBuilder(s.Length);
            foreach (var ch in s.ToLowerInvariant())
            {
                sb.Append(char.IsLetterOrDigit(ch) ? ch : (char.IsWhiteSpace(ch) || ch is '-' or '_' ? '-' : '\0'));
            }

            var slug = sb.ToString().Replace("\0", string.Empty).Trim('-');
            return string.Join("-", slug.Split('-', StringSplitOptions.RemoveEmptyEntries));
        }

        public static string ToSafeFileName(this string? s, char replacement = '_')
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            var invalid = System.IO.Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s!)
            {
                sb.Append(invalid.Contains(ch) ? replacement : ch);
            }

            return sb.ToString();
        }

        public static string UrlEncode(this string? s) => Uri.EscapeDataString(s ?? string.Empty);
        public static string UrlDecode(this string? s) => Uri.UnescapeDataString(s ?? string.Empty);

        public static bool IsDigits(this string? s) => !string.IsNullOrEmpty(s) && s.All(char.IsDigit);
        public static bool IsValidEmail(this string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try { _ = new MailAddress(email); return true; } catch { return false; }
        }

        public static bool IsJson(this string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return false;
            }

            s = s.Trim();
            return (s.StartsWith('{') && s.EndsWith('}')) || (s.StartsWith('[') && s.EndsWith(']'));
        }

        public static bool IsUrl(this string? s)
            => Uri.TryCreate(s, UriKind.Absolute, out var u) && (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps);
    }
}
