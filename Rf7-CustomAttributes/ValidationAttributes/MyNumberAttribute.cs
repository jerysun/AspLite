using System;
using System.Text.RegularExpressions;

namespace Rf7_CustomAttributes.ValidationAttributes
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
  public class MyNumberAttribute : ValidationAttributeBase
  {
    public override bool IsValid(object value)
    {
      if (value == null || string.IsNullOrEmpty(value.ToString())) return false;

      string pattern = @"^[0-9]*$";
      Regex regex = new Regex(pattern);
      return regex.IsMatch(value.ToString());
    }
  }
}
