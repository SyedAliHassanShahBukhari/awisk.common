namespace awisk.common.Helpers
{
    public static partial class UniversalOpertaions
    {
        // Guid
        public static Guid EmptyGuid => Guid.Empty;
        public static Guid NewGuid() => Guid.NewGuid();
        public static string EmptyGuidStr() => Guid.Empty.ToString();
        public static string NewGuidStr() => Guid.NewGuid().ToString();
        public static bool IsEmpty(this Guid g) => g == Guid.Empty;
        public static bool IsNotEmpty(this Guid g) => g != Guid.Empty;

        // Original aliases (kept for compatibility)
        public static bool IsStringNullOrEmpty(this string? v) => string.IsNullOrWhiteSpace(v);
        public static bool IsStringNoNullOrEmpty(this string? v) => !string.IsNullOrWhiteSpace(v);
        public static string IfNullEmptyString(this string? v) => v ?? string.Empty;
        public static string IfNullEmptyString(this object? v) => v?.ToString() ?? string.Empty;
    }
}
