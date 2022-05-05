using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MethodsAndOtherReflections
{
	public static class FieldsReflection
	{
		public static void FieldsDemo()
		{
			Type type = typeof(MyClass13);
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.GetField | BindingFlags.Static | BindingFlags.Instance);
			IEnumerable<FieldInfo> fields1 = type.GetRuntimeFields();

			foreach (var item in fields)
			{
				StringBuilder sb = new();
				sb.Append(GetVisibility(item) + " ");
				sb.Append(GetRead(item) + " ");
				sb.Append(item.FieldType.Name + " ");
				sb.Append(item.Name + " ");
				Console.WriteLine(sb.ToString());
			}
			Console.WriteLine("");
			foreach (var item in fields1)
			{
				StringBuilder sb = new();
				sb.Append(GetVisibility(item) + " ");
				sb.Append(GetRead(item) + " ");
				sb.Append(item.FieldType.Name + " ");
				sb.Append(item.Name + " ");
				Console.WriteLine(sb.ToString());
			}
		}
		public static string GetRead(FieldInfo field)
		{
			if (field == null) return string.Empty;
			return
				field.IsLiteral ? "const" :
				field.IsStatic && field.IsInitOnly ? "readonly static" :
				field.IsStatic ? "static" :
				field.IsInitOnly ? "readonly" :
				string.Empty;
		}

		public static string GetVisibility(FieldInfo field)
		{
			if (field == null) return string.Empty;
			return
				field.IsPublic ? "public" :
				field.IsPrivate ? "private" :
				field.IsFamilyOrAssembly ? "protected internal" :
				field.IsAssembly ? "internal" :
				field.IsFamily ? "protected" :
				string.Empty;
		}
	}

	public class MyClass13
	{
		public int a;
		internal int b;
		protected int c;
		protected internal int d;
		private int e;
		public readonly static float f = 1;
	}
}
