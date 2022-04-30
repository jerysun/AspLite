using System;
using System.Text.RegularExpressions;

namespace Rf7_CustomAttributes.ValidationAttributes
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
  public class MyPhoneAttribute : ValidationAttributeBase
  {
    public override bool IsValid(object value)
    {
      if (value == null || string.IsNullOrEmpty(value.ToString())) return false;

      string pattern = @"^\(?([+]31|0031|0)-?6(\s?|-?)([0-9]\s{0,3}){8}$";
      Regex regex = new Regex(pattern);
      return regex.IsMatch(value.ToString());
    }
  }
}
