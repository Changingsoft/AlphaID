using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AspNetWebLib.Helpers;

/// <summary>
/// 适用于枚举的帮助器类。
/// </summary>
public class EnumHelper
{
    /// <summary>
    /// 获取特定枚举类型的选择项列表。
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    public static IEnumerable<SelectListItem> GetSelectListItems<TEnum>() where TEnum : struct, Enum
    {
        Type type = typeof(TEnum);
        Array enumValues = type.GetEnumValues();
        List<SelectListItem> listItems = [];
        foreach (TEnum enumValue in enumValues)
        {
            MemberInfo member = type.GetMember(enumValue.ToString()).First();
            var displayAttribute = member.GetCustomAttribute<DisplayAttribute>();
            //displayAttribute
            listItems.Add(new SelectListItem
            {
                Text = displayAttribute?.GetName() ?? enumValue.ToString(),
                Value = enumValue.ToString()
            });
        }

        return listItems;
    }
}