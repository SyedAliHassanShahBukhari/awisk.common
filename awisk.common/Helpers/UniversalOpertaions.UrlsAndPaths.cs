using System.Linq;

namespace awisk.common.Helpers
{
    public static partial class UniversalOpertaions
    {
        public static string CombineUrl(string baseUrl, params string[] parts)
        {
            var url = (baseUrl ?? string.Empty).TrimEnd('/');
            foreach (var p in parts) url += "/" + (p ?? string.Empty).Trim('/');
            return url;
        }

        public static string EnsureTrailingSlash(this string? s)
            => string.IsNullOrEmpty(s) ? "/" : (s!.EndsWith('/') ? s : s + "/");

        public static string CombinePath(params string[] paths)
            => System.IO.Path.Combine([.. paths.Where(p => !string.IsNullOrWhiteSpace(p))]);

        public static bool EnsureDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            return true;
        }
    }
}
