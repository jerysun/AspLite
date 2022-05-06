using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MethodsAndOtherReflections
{
  public static class PropertiesReflection
  {
		public static void PropReflectionDemo()
    {
			Type type = typeof(MyClass15);
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static | 
				BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance);
			
			foreach (PropertyInfo property in properties)
      {
				StringBuilder sb = new();
				sb.Append(GetVisibility(property));
				sb.Append(" ");
				//true: It indeed returns a non-public Getter
				sb.Append(property.GetGetMethod(true).IsStatic ? "static " : string.Empty);
				var modifier = GetMethodModifier(type, property);
				if (modifier != string.Empty)
				{
					sb.Append(modifier);
					sb.Append(" ");
				}
				sb.Append(property.PropertyType.ToString());
				sb.Append(" ");
				sb.Append(property.Name);
				sb.Append(" ");
				sb.Append(GetPropertyConstrucotr(property));
        Console.WriteLine(sb.ToString());
      }
    }

		public static string GetVisibility(PropertyInfo property)
		{
			MethodInfo method = property.GetGetMethod(true);//true: It indeed returns a non-public Getter
			string result = method.IsPublic ? "public" :
				method.IsPrivate ? "private" :
				method.IsFamilyOrAssembly ? "protected internal" :
				method.IsAssembly ? "internal" :
				method.IsFamily ? "protected" : string.Empty;
			return result;
		}

		// virtual override abstract new
		public static string GetMethodModifier(Type type, PropertyInfo property)
		{
			MethodInfo method = property.GetGetMethod(true);//true: It indeed returns a non-public Getter
			// No hiding related information, hence no modifiers
			if (!method.IsHideBySig) return string.Empty;

			if (method.IsAbstract) return "abstract";

			// virtual, overrice, interface
			if (method.IsVirtual)
			{
				// A method implementing the interface
				if (method.IsFinal) return string.Empty;

				// not overridden, hence virtual
				if (method.Equals(method.GetBaseDefinition())) return "virtual";
				else return "override";
			}
			else
			{// new - brutally hide/shield
			 // if it's defined within the current type, then it's just an ordinary method
				if (type == method.DeclaringType) return string.Empty;
				return "new";
			}
		}

		public static string GetPropertyConstrucotr(PropertyInfo property)
    {
			StringBuilder sb = new();
			sb.Append("{ ");

			if (property.CanRead)
      {
				sb.Append("get; ");
      }

			if (property.CanWrite)
      {
				sb.Append("set; ");
      }

			sb.Append("}");
			return sb.ToString();
    }
	}

	public abstract class Ab
	{
		public abstract int TestA { get; set; }
		public virtual int TestB { get; set; }
		public virtual int TestC { get; set; }
	}

	public class MyClass15 : Ab
	{
		public int a { get; set; }
		internal int b { get; set; }
		protected int c { get; set; }
		protected internal int d { get; set; }
		private int e { get; set; }
		public static float f { get; set; } = 1;
		public override int TestA { get; set; }
		public override int TestB { get => base.TestB; set => base.TestB = value; }
		new public int TestC { get => base.TestC; set => base.TestC = value; }
	}
}
