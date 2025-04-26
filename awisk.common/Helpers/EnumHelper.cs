using awisk.common.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace awisk.common.Helpers
{
    public static class EnumHelper
    {
        public static IEnumerable<ListItemResponseDto<T>> GetSelectListFromEnum<T>() where T : Enum
        {
            return [.. Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new ListItemResponseDto<T>
                {
                    Id = e,
                    Value = GetEnumDescription(e)
                })];
        }
        public static IEnumerable<ListItemResponseDto<T>> GetTextSelectListFromEnum<T>() where T : Enum
        {
            return [.. Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new ListItemResponseDto<T>
                {
                    Id = e,
                    Value = e.ToString()
                })];
        }

        private static string GetEnumDescription<T>(T enumValue) where T : Enum
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            var descriptionAttribute = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute;
            return descriptionAttribute?.Description ?? enumValue.ToString();
        }
        public static string ToDescription(this Enum enumValue)
        {
            var field = enumValue?.GetType().GetField(enumValue.ToString());
            if (field != null && Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                return attribute.Description;
            }
            throw new ArgumentException("Item not found.", nameof(enumValue));
        }
    }
}
