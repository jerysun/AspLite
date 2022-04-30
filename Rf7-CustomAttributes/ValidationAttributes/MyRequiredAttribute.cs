using System;

namespace Rf7_CustomAttributes.ValidationAttributes
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
  public class MyRequiredAttribute : ValidationAttributeBase
  {
    public override bool IsValid(object value)
    {
      if (value == null || string.IsNullOrEmpty(value.ToString())) return false;
      return true;
    }
  }
}
