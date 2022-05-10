using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MethodsAndOtherReflections
{
  //NOTE: The enum mentioned below is not this Color enum but GenericParameterAttributes Enum,
  //https://docs.microsoft.com/en-us/dotnet/api/system.reflection.genericparameterattributes?view=net-6.0
  public enum Color
  {
    Red = 0,
    Yellow = 1,
    Blue = 2,
    Orange = 3,
    None = 4
  }

  public class BaseClass { }

  public interface BaseInterFace { }

  public class MyClass17<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    where T1 : struct
    where T2 : class
    where T3 : notnull
    where T4 : unmanaged
    where T5 : new()
    where T6 : BaseClass
    where T7 : BaseInterFace
    where T8 : T2
    // Combinations
    where T9 : class, new()
    where T10 : BaseClass, BaseInterFace, new()
  {
  }

  public static class GenericsReflection
  {
    public static void GenReflectionDemo()
    {
      Type type = typeof(MyClass17<,,,,,,,,,,>);
      Console.WriteLine(GetGenericString(type));
      Console.ReadKey();
    }

    // Parse a generic type including its constraints
    public static string GetGenericString(Type type)
    {
      if (!type.IsGenericType) return string.Empty;

      StringBuilder className = new();
      className.Append(type.Name.Split("`")[0]);
      className.Append("<");

      Type[] types = ((System.Reflection.TypeInfo)type).GenericTypeParameters;
      for (int i = 0; i < types.Length; i++)
      {
        className.Append(types[i].Name);
        if (i < types.Length -1) className.Append(", ");
      }
      className.Append(">");

      StringBuilder sb = new();
      //There can be multiple "where"s together in one generic class, see also the
      //definition of MyClass17 above
      foreach(Type item in types)
      {
        //something like struct, notnull, new() OR your own custom classes, interfaces
        Type[] genericTypes = item.GetGenericParameterConstraints();

        //enum values, for instance:
        //4 | 16 == 20 == 10100 == 0x14 == GenericParameterAttributes.ReferenceTypeConstraint | GenericParameterAttributes.DefaultConstructorConstraint
        GenericParameterAttributes genericAttrs = item.GenericParameterAttributes;

        int length = types.Length - 1;
        // Zero constraint
        if (genericTypes.Length == 0 &&
            genericAttrs == GenericParameterAttributes.None &&
            item.GetCustomAttributes().ToArray().Length == 0)
        {
          sb.Clear();
          continue;
        }

        sb.Append("\nwhere ");
        sb.Append(item.Name);
        sb.Append(" : ");

        // There exist such possibilities when there's only ONE constraint:
        // struct, unmanaged, <BaseClass>, <Interface>, T:U
        if(genericTypes.Length == 1)
        {
          sb.Append(GetGenericType(genericTypes[0]).Item1);
        } else if(genericTypes.Length == 0) // No Type Constraint
        {
          sb.Append(GetGenericType(genericAttrs).Item1);
        }
        // It becomes very complex when the Type Constraints mix with the
        // SpecialConstraintMask, which can have different combinations
        // and orders. They can be categorized into 3 color groups:
        // Yellow, Blue and Orange.
        else
        {
          List<string> color = new();

          //Yellow
          bool a = HasYellow(genericTypes);
          bool b = HasYellow(genericAttrs);
          if (a)
          {
            var result = genericTypes.FirstOrDefault(x => !x.IsInterface && x.IsSubclassOf(typeof(object)));
            if (result != null) color.Add(result.Name);
          }
          else if (b)
            color.Add(GetGenericType(genericAttrs).Item1);

          // Blue
          color.Add(GetGenericType(genericTypes.Where(x => !(!x.IsInterface && x.IsSubclassOf(typeof(object)))).ToArray()));

          // Orange
          if (genericAttrs.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
            color.Add(GetGenericType(genericAttrs).Item1);

          sb.Append(string.Join(", ", color.ToArray()));
        }

        className.Append(sb.ToString());
        sb.Clear();
        className.Append(" ");
      }

      return className.ToString().Trim();
    }

    public static bool HasYellow(Type[] types) => types.Any(x => !x.IsInterface && x.IsSubclassOf(typeof(object)));

    public static bool HasYellow(GenericParameterAttributes attributes) =>
      attributes == GenericParameterAttributes.ReferenceTypeConstraint ||
      attributes == GenericParameterAttributes.None;

    //struct, <BaseClass>, <BaseInterface>, T:U
    public static (string, Color) GetGenericType(Type type)
    {
      // If the constraint itself is a generic, there's no further processing
      if (type.Name == "ValueType")
      {
        return ("struct", Color.Red);
      }
      else if (type.IsInterface)
        return (type.Name, Color.Blue);
      else if (type.IsSubclassOf(typeof(object)))
        return (type.Name, Color.Yellow);
      else
        return (type.Name, Color.Blue);
    }

    // All kinds of combination, struct, <BaseClass>, <Interface>, T:U
    public static string GetGenericType(Type[] types)
    {
      int length = types.Length - 1;
      StringBuilder sb = new();
      for (int i = 0; i <= length; i++)
      {
        if (types[i].Name == "ValueType")
        {
          sb.Append("struct");
        }
        else //The constraint can be generic or whatever, here we don't do the
             //further processing, just leave it as it is
          sb.Append(types[i].Name);

        if (i < length) sb.Append(", ");
      }
      return sb.ToString();
    }

    // class, notnull, new() and all kinds of combinations
    public static (string, Color) GetGenericType(GenericParameterAttributes attributes)
    {
      /*
       * Here is a list of the return results of different constraints:
       * ReferenceTypeConstraint: class
       * None: notnull
       * NotNullableValueTypeConstraint: unmanaged
       * DefaultConstructorConstraint: unmanaged
       * DefaultConstructorConstraint: new()
       */
      switch (attributes)
      {
        case GenericParameterAttributes.ReferenceTypeConstraint:
          return ("class", Color.Yellow);
        case GenericParameterAttributes.None:
          return ("notnull", Color.Yellow);
        case GenericParameterAttributes.DefaultConstructorConstraint:
          return ("new()", Color.Orange);
        default:
          StringBuilder sb = new();
          if (attributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
            sb.Append("class, ");
          if (attributes.HasFlag(GenericParameterAttributes.None))
            sb.Append("notnull, ");
          if (attributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
            sb.Append("new(), ");

          string str = sb.ToString();
          return (str.Substring(0, str.Length - 2), Color.None);
      }
    }
  }
}
