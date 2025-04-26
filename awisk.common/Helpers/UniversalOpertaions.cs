namespace awisk.common.Helpers
{
    public static partial class UniversalOpertaions
    {
        public static string EmptyGuidStr { get; set; } = Guid.Empty.ToString();
        public static string NewGuidStr { get; set; } = Guid.NewGuid().ToString();
        public static Guid NewGuid { get; set; } = Guid.NewGuid();
        public static string IfNullEmptyString(this string? value) => value ?? EmptyGuidStr;
        public static string IfNullEmptyString(this object? value) => value?.ToString() ?? EmptyGuidStr;
    }
}
