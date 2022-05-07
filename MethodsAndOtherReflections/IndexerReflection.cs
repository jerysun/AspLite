using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MethodsAndOtherReflections
{
  public static class IndexerReflection
  {
    public static void IndexerReflectionDemo()
    {
      Type type = typeof(MyClass17);
      PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static |
        BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance);

      foreach(PropertyInfo pi in properties)
      {
        StringBuilder sb = new();
        var result = PropertiesReflection.GetVisibility(pi);
        if (result != String.Empty)
        {
          sb.Append(result);
          sb.Append(" ");
        }
        sb.Append(pi.GetGetMethod(true).IsStatic ? "static " : string.Empty);

        result = PropertiesReflection.GetMethodModifier(type, pi);
        if (result != String.Empty)
        {
          sb.Append(result);
          sb.Append(" ");
        }
        sb.Append(pi.PropertyType.Name);
        sb.Append(" ");

        if (pi.Name == "Item") // indexer property
        {
          sb.Append("this[");
          ParameterInfo[] paras = pi.GetIndexParameters();
          int length = paras.Length - 1;
          for (int i = 0; i <= length; i++)
          {
            sb.Append(paras[i].ParameterType.Name);
            sb.Append(" ");
            sb.Append(paras[i].Name);
            if (i < length) sb.Append(", ");
          }
          sb.Append("]");
        } else
        {
          sb.Append(pi.Name);
          sb.Append(" ");
          sb.Append(PropertiesReflection.GetPropertyConstructor(pi));
        }

        Console.WriteLine(sb.ToString());
      }
    }
  }

  public class MyClass17
  {
    private string[] MyArray;

    public MyClass17()
    {
      MyArray = new string[] { "a", "b", "c", "d", "e" };
    }

    public string this[int index, string search]
    {
      get { return MyArray[index]; }
      set { MyArray[index] = value; }
    }
  }
}
