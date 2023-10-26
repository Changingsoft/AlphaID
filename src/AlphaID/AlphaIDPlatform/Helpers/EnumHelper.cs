using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AlphaIDPlatform.Helpers;
public class EnumHelper
{
    public static IEnumerable<SelectListItem> GetSelectListItems<TEnum>() where TEnum : struct, Enum
    {
        var type = typeof(TEnum);
        var enumValues = type.GetEnumValues();
        List<SelectListItem> listItems = new();
        foreach (TEnum enumValue in enumValues)
        {
            var member = type.GetMember(enumValue.ToString()).First();
            var displayAttribute = member.GetCustomAttribute<DisplayAttribute>();
            listItems.Add(new SelectListItem()
            {
                Text = displayAttribute?.Name ?? enumValue.ToString(),
                Value = enumValue.ToString(),
            });
        }
        return listItems;
    }
}