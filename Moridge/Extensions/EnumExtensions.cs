using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Moridge.BusinessLogic;

namespace Moridge.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the DataAnnotation "[Display]" for the given enum value.
        /// From http://stackoverflow.com/a/26455406/4660537.
        /// </summary>
        /// <param name="enumValue">enum value to get display name for</param>
        /// <returns>display name for the value</returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                .GetName();
        }
    }
}