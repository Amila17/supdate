using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Supdate.Util
{
  public static class EnumUtil
  {
    public static string DisplayName(this Enum value)
    {
      Type enumType = value.GetType();
      var enumValue = Enum.GetName(enumType, value);
      MemberInfo member = enumType.GetMember(enumValue)[0];

      var attributes = member.GetCustomAttributes(typeof(DisplayAttribute), false);
      return attributes.Length > 0 ? ((DisplayAttribute)attributes[0]).Name : value.ToString();
    }
  }
}
