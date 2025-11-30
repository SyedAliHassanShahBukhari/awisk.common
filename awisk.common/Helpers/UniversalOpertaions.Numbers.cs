namespace awisk.common.Helpers
{
    public static partial class UniversalOpertaions
    {
        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
            => value.CompareTo(min) < 0 ? min : (value.CompareTo(max) > 0 ? max : value);

        public static bool Between<T>(this T value, T min, T max, bool inclusive = true) where T : IComparable<T>
            => inclusive ? value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0
                         : value.CompareTo(min) > 0 && value.CompareTo(max) < 0;

        public static double SafeDivide(double n, double d, double fallback = 0d) => d == 0d ? fallback : n / d;
    }
}
