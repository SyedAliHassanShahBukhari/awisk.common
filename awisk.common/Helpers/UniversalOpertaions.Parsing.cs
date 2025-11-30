
namespace awisk.common.Helpers
{
    public static partial class UniversalOpertaions
    {
        public static bool TryGuid(string? s, out Guid value) => Guid.TryParse(s, out value);

        public static int ToInt(this string? s, int def = 0) => int.TryParse(s, out var v) ? v : def;
        public static long ToLong(this string? s, long def = 0) => long.TryParse(s, out var v) ? v : def;
        public static double ToDouble(this string? s, double def = 0) => double.TryParse(s, out var v) ? v : def;
        public static decimal ToDecimal(this string? s, decimal def = 0m) => decimal.TryParse(s, out var v) ? v : def;
        public static bool ToBool(this string? s, bool def = false) => bool.TryParse(s, out var v) ? v : def;

        public static DateTime? ToDateTimeOrNull(this string? s)
            => DateTime.TryParse(s, out var d) ? d : null;

        public static TEnum ToEnum<TEnum>(this string? s, TEnum def = default) where TEnum : struct
            => Enum.TryParse<TEnum>(s, true, out var v) ? v : def;

        public static T SafeGet<T>(this T? value, T fallback) where T : class => value ?? fallback;
        public static T? NullIf<T>(this T? value, Func<T, bool> predicate)
            => value is not null && predicate(value) ? default : value;
    }
}
