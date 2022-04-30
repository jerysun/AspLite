using Rf7_CustomAttributes.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rf7_CustomAttributes.ValidationProcessing
{
  public static class AttributesProcessing
  {

    public static void Analyze(List<object> list)
    {
      foreach (var item in list)
      {
        Console.WriteLine("\nCheck if the properties are valid");

        // Get the type of the object instance
        Type type = item.GetType();
        PropertyInfo[] properties = type.GetProperties();

        foreach (PropertyInfo pi in properties)
        {
          Console.WriteLine($"Property: {pi.Name}, Value: {pi.GetValue(item)}");

          // Get all attributes imposed on this property
          IEnumerable<Attribute> attrList = pi.GetCustomAttributes();
          if (attrList != null)
          {
            foreach (Attribute attr in attrList)
            {
              if (!IsValidationAttributeBase(attr)) continue;

              var result = StartValidation(attr, pi, item);
              Console.WriteLine(result.Item1 ? "The verification of " + attr.GetType().Name + " is passed.\n"
                                             : "The verification of " + attr.GetType().Name + " failed, error: " + result.Item2);
            }
          }
          Console.WriteLine("*****Properties division line*****\n");
        }
        Console.WriteLine("########Objects division line#######");
      }
    }

    /// <summary>
    /// Validate this attribute and check if it inherits ValidationAttributeBase
    /// </summary>
    /// <param name="attr">The attribute constraining the property</param>
    /// <param name="property">The property to validate</param>
    /// <param name="obj">The object instance where the property exists</param>
    /// <returns></returns>
    private static (bool, string) StartValidation(Attribute attr, PropertyInfo property, object obj)
    {
      // Get the property Value from the bound object
      object propertyValue = property.GetValue(obj);

      // Get the InValid method and its params from the attribute instance
      MethodInfo attrMethod = attr.GetType().GetMethod("IsValid", new Type[] { typeof(object) });

      bool checkResult = (bool)attrMethod.Invoke(attr, new object[] { propertyValue });
      if (checkResult) return (true, null);

      PropertyInfo attrProperty = attr.GetType().GetProperty("ErrorMessage");
      string errorMessage = (string)attrProperty.GetValue(attr);
      return (false, errorMessage);
    }

    private static bool IsValidationAttributeBase(Attribute attribute)
    {
      Type type = attribute.GetType();
      return type.BaseType == typeof(ValidationAttributeBase);
    }
  }
}
