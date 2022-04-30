using System;
using System.Text.RegularExpressions;

namespace Rf7_CustomAttributes.ValidationAttributes
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
  public class MyEmailAttribute : ValidationAttributeBase
  {
    public override bool IsValid(object value)
    {
      if (value == null || string.IsNullOrEmpty(value.ToString())) return false;

      string pattern = @"^[A-Za-z0-9\u4e00-\u9fa5]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$";
      Regex regex = new Regex(pattern);
      return regex.IsMatch(value.ToString());
    }
  }
}
