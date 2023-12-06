using System;
using System.Linq;

namespace AuthCraft.Common.Extensions;

public static class EnumExtensions
{
    /// <summary>The get enum description.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The <see cref="string"/>.</returns>
    public static string GetEnumDescription(this Enum value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        var field = value.GetType().GetField(value.ToString());
        var attributes = field.GetCustomAttributes(false);

        // Description is in a hidden Attribute class called DisplayAttribute
        // Not to be confused with DisplayNameAttribute
        dynamic displayAttribute = null;

        if (attributes != null && attributes.Any())
        {
            displayAttribute = attributes.ElementAt(0);
        }

        // return description
        return displayAttribute?.Description ?? "Description Not Found";
    }
}
