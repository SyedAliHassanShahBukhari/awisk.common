using awisk.common.DTOs.Responses;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace awisk.common.Helpers
{
    public static partial class EnumHelper
    {
        private static readonly ConcurrentDictionary<(Type EnumType, string MemberName), string> _descriptionCache = new();

        private static string GetCachedDescription(Type enumType, string memberName)
        {
            return _descriptionCache.GetOrAdd((enumType, memberName), static key =>
            {
                var field = key.EnumType.GetField(key.MemberName);
                return field?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? key.MemberName;
            });
        }

        public static IEnumerable<ListItemResponseDto<T>> GetSelectListFromEnum<T>() where T : Enum =>
            Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new ListItemResponseDto<T>
                {
                    Id = e,
                    Value = GetEnumDescription(e)
                });

        public static IEnumerable<ListItemResponseDto<T>> GetTextSelectListFromEnum<T>() where T : Enum =>
            Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new ListItemResponseDto<T>
                {
                    Id = e,
                    Value = e.ToString()
                });

        private static string GetEnumDescription<T>(T enumValue) where T : Enum =>
            GetCachedDescription(typeof(T), enumValue.ToString());

        /// <summary>
        /// Gets the description attribute value of an enum, or returns the enum name if no description is found.
        /// </summary>
        public static string ToDescription(this Enum enumValue)
        {
            ArgumentNullException.ThrowIfNull(enumValue);
            return GetCachedDescription(enumValue.GetType(), enumValue.ToString());
        }

        // ───────────────────────────────────────────────
        // 🆕 Additional Helpers
        // ───────────────────────────────────────────────

        /// <summary>
        /// Returns an enum value from its description or name.
        /// </summary>
        public static T FromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                {
                    if (string.Equals(attr.Description, description, StringComparison.OrdinalIgnoreCase))
                    {
                        return (T)field.GetValue(null)!;
                    }
                }

                if (string.Equals(field.Name, description, StringComparison.OrdinalIgnoreCase))
                {
                    return (T)field.GetValue(null)!;
                }
            }
            throw new ArgumentException($"'{description}' is not a valid description or name for enum {typeof(T).Name}");
        }

        /// <summary>
        /// Converts an enum to (int value, string description) tuples.
        /// </summary>
        public static IEnumerable<(int Value, string Description)> ToTupleList<T>() where T : Enum =>
            Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => (Convert.ToInt32(e), GetEnumDescription(e)));

        /// <summary>
        /// Converts an enum to a dictionary: int → string description.
        /// </summary>
        public static Dictionary<int, string> ToDictionary<T>() where T : Enum =>
            Enum.GetValues(typeof(T))
                .Cast<T>()
                .ToDictionary(e => Convert.ToInt32(e), GetEnumDescription);

        /// <summary>
        /// Returns all enum names as string list.
        /// </summary>
        public static List<string> GetNames<T>() where T : Enum =>
            [.. Enum.GetNames(typeof(T))];

        /// <summary>
        /// Returns all enum values as list of T.
        /// </summary>
        public static List<T> GetValues<T>() where T : Enum =>
            [.. Enum.GetValues(typeof(T)).Cast<T>()];

        /// <summary>
        /// Safely parse enum from string, fallback if invalid.
        /// </summary>
        public static T ParseSafe<T>(string? value, T fallback = default) where T : struct, Enum =>
            Enum.TryParse(value, true, out T result) ? result : fallback;

        /// <summary>
        /// Gets the integer value of an enum.
        /// </summary>
        public static int ToInt(this Enum enumValue) => Convert.ToInt32(enumValue);

        /// <summary>
        /// Returns true if the enum has the specified flag (for [Flags] enums).
        /// </summary>
        public static bool HasFlagFast<T>(this T value, T flag) where T : Enum =>
            (Convert.ToInt64(value) & Convert.ToInt64(flag)) != 0;

        /// <summary>
        /// Returns combined flags as comma-separated description.
        /// Works with [Flags] enums to get descriptions of all set flags.
        /// </summary>
        public static string ToCombinedDescription(this Enum flagsEnum)
        {
            if (flagsEnum != null)
            {
                var values = Enum.GetValues(flagsEnum.GetType()).Cast<Enum>();
                var active = values.Where(v => Convert.ToInt64(v) != 0 && flagsEnum.HasFlag(v));
                var descriptions = active.Select(v => v.ToDescription());
                return string.Join(", ", descriptions);
            }

            throw new ArgumentNullException(nameof(flagsEnum));
        }

        /// <summary>
        /// Converts enum to a JSON-friendly list (id, value).
        /// </summary>
        public static IEnumerable<object> ToJsonList<T>() where T : Enum =>
            Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new { id = Convert.ToInt32(e), value = GetEnumDescription(e) });

        /// <summary>
        /// Checks if a given integer exists as a valid value in enum T.
        /// </summary>
        public static bool IsDefined<T>(int value) where T : Enum =>
            Enum.IsDefined(typeof(T), value);
    }
}
