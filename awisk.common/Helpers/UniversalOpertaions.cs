namespace awisk.common.Helpers
{
    public static partial class UniversalOpertaions
    {
        public static string EmptyGuidStr () => Guid.Empty.ToString();
        public static string NewGuidStr() => Guid.NewGuid().ToString();
        public static Guid NewGuid() => Guid.NewGuid();
        public static string IfNullEmptyString(this string? value) => value ?? EmptyGuidStr();
        public static string IfNullEmptyString(this object? value) => value?.ToString() ?? EmptyGuidStr();
    }
}
