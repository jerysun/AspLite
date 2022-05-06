using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MethodsAndOtherReflections
{
  public static class ConstructorReflection
  {
    public static void CtorReflectionDemo()
    {
      Type type = typeof(List<int>);
      ConstructorInfo[] methods = type.GetConstructors(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Instance);

      foreach (ConstructorInfo item in methods)
      {
        StringBuilder builder = new StringBuilder();
        builder.Append(GetVisibility(item) + " ");
        builder.Append(item.IsStatic ? "static " : string.Empty);
        builder.Append(GetMethodName(item) + " ");
        builder.Append("(" + GetParams(item) + ")");
        Console.WriteLine(builder.ToString());
      }
      Console.ReadKey();
    }

    public static string GetVisibility(ConstructorInfo method)
    {
      return
        method.IsPublic ? "public" :
        method.IsPrivate ? "private" :
        method.IsAssembly ? "internal" :
        method.IsFamily ? "protected" :
        method.IsFamilyOrAssembly ? "protected internal" : string.Empty;
    }

    // Judge if it's a generic ctor, then return the generic arguments
    public static string GetMethodName(ConstructorInfo method)
    {
      if (!method.IsGenericMethod) return method.Name;
      Type[] types = method.GetGenericArguments();

      StringBuilder sb = new();
      sb.Append(method.Name);
      sb.Append("<");
      for (int i = 0; i < types.Length; i++)
      {
        sb.Append(types[i].Name);
        if (i < types.Length - 1) sb.Append(", ");
      }
      sb.Append(">");
      return sb.ToString();
    }

    public static string GetParams(ConstructorInfo method)
    {
      ParameterInfo[] parameters = method.GetParameters();
      if (parameters.Length == 0) return string.Empty;

      int length = parameters.Length - 1;
      StringBuilder str = new StringBuilder();
      for (int i = 0; i <= length; i++)
      {
        str.Append(InRefOutParams(parameters[i]) + " ");
        // Here the complex type won't be processed
        str.Append(GetParamType(parameters[i]));
        str.Append(" ");
        str.Append(parameters[i].Name);

        var defaultVal = GetDefaultValue(parameters[i]);
        if (defaultVal != string.Empty)
        {
          str.Append(" = ");
          str.Append(defaultVal);
        }

        if (i < length) str.Append(", ");
      }
      return str.ToString();
    }

    public static string InRefOutParams(ParameterInfo parameter)
    {
      // In compiled IL, there's a & symbol after the parameter with the prefix in, ref, out
      if (parameter.ParameterType.Name.EndsWith("&"))
      {
        return
        parameter.IsIn ? "in" : parameter.IsOut ? "out" : "ref";
      }
      if (parameter.GetCustomAttributes().Any(x => x.GetType() == typeof(ParamArrayAttribute))) return "params";
      return string.Empty;
    }

    public static string GetParamType(ParameterInfo parameter)
    {
      string typeName = parameter.ParameterType.Name;
      if (typeName.EndsWith("&")) typeName = typeName.Substring(0, typeName.Length - 1);
      return typeName;
    }

    // Get the default value if it's an optional parameter
    public static string GetDefaultValue(ParameterInfo parameter)
    {
      if (!parameter.IsOptional) return string.Empty;
      object value = parameter.DefaultValue;
      return value.ToString();
    }
  }
}
