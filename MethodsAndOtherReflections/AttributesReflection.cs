using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MethodsAndOtherReflections
{
  public static class AttributesReflection
  {
    public static void AttrReflectionDemo()
    {
      Type type = typeof(List<>);
      string[] list = GetAttrs(type.GetCustomAttributesData());
      foreach (var item in list) Console.WriteLine(item);
    }

    public static string[] GetAttrs(IList<CustomAttributeData> attrs)
    {
      List<string> attrResult = new();
      StringBuilder sb = new();

      foreach(CustomAttributeData item in attrs)
      {
        Type attrType = item.GetType();
        sb.Append("[");
        sb.Append(item.AttributeType.Name);
        // so-called positional_parameters
        IList<CustomAttributeTypedArgument> positionals = item.ConstructorArguments;
        // so-called named_parameters
        IList<CustomAttributeNamedArgument> nameds = item.NamedArguments;

        if (positionals.Count == 0 && nameds.Count == 0)
        {
          sb.Append("]");
          attrResult.Add(sb.ToString());
          sb.Clear();
          continue;
        }

        sb.Append("(");
        if (positionals.Count > 0)
        {
          sb.Append(string.Join(", ", positionals.ToArray()));
        }
        if (positionals.Count > 0 && nameds.Count > 0) sb.Append(", ");

        if (nameds.Count > 0)
        {
          sb.Append(string.Join(", ", nameds.ToArray()));
        }
        sb.Append(")");
        attrResult.Add(sb.ToString());
        sb.Clear();
      }
      return attrResult.ToArray();
    }
  }
}
