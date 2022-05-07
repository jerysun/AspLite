using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace MethodsAndOtherReflections
{
	public static class MethodsReflection
	{
		public static void MethodsReflectionDemo()
		{
			Type type = typeof(MyClass14);
			MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static |
				BindingFlags.DeclaredOnly | BindingFlags.Instance);

			foreach (MethodInfo method in methods)
			{
				StringBuilder sb = new();
				sb.Append(GetVisibility(method));
				sb.Append(" ");
				sb.Append(method.IsStatic ? "static " : string.Empty);
				sb.Append(GetMethodModifier(type, method));
				sb.Append(" ");
				sb.Append(GetReturn(method));
				sb.Append(" ");
				sb.Append(GetMethodName(method));
				sb.Append(" (");
				sb.Append(GetParams(method));
				sb.Append(")");
				Console.WriteLine(sb.ToString());
			}
		}

		public static string GetVisibility(MethodInfo method)
		{
			if (method == null) return string.Empty;
			return
				method.IsPublic ? "public" :
				method.IsPrivate ? "private" :
				method.IsAssembly ? "internal" :
				method.IsFamily ? "protected" :
				method.IsFamilyOrAssembly ? "protected internal" : string.Empty;
		}

		// virtual override abstract new
		public static string GetMethodModifier(Type type, MethodInfo method)
		{
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
				var flags = method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;
				flags |= method.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
				var paramTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

				if (method.DeclaringType.BaseType.GetMethod(method.Name, flags, null, paramTypes, null) == null)
        {
					// It's not defined in its base type, but only in the current type, so it's just an ordinary method
					return String.Empty;
				} else
        {
					return "new";
        }
			}
		}

		public static string GetReturn(MethodInfo method)
		{
			Type returnType = method.ReturnType;
			ParameterInfo returnParam = method.ReturnParameter;

			if (returnType == typeof(void)) return "void";

			if (returnType.IsValueType)
			{
				// judge if the return form will be (type1, type2)
				if (returnParam.ParameterType.IsGenericType)
				{
					Type[] types = returnParam.ParameterType.GetGenericArguments();
					StringBuilder sb = new();
					sb.Append("(");
					for (int i = 0; i < types.Length; i++)
					{
						sb.Append(types[i].Name);
						if (i < types.Length - 1) sb.Append(",");
					}
					sb.Append(")");
					return sb.ToString();
				}
				return returnType.Name;
			}

			// Here some complex types such as array, generic are not processed
			return returnType.Name;
		}

		public static string GetAsync(MethodInfo method) => method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) == null ? string.Empty : "async ";

		// Judge if it's a generic method, then return the generic arguments
		public static string GetMethodName(MethodInfo method)
		{
			if (!method.IsGenericMethod) return method.Name;
			Type[] types = method.GetGenericArguments();
			StringBuilder sb = new();
			sb.Append(method.Name);
			sb.Append("<");
			for (int i = 0; i < types.Length; i++)
			{
				sb.Append(types[i].Name);
				if (i < types.Length - 1) sb.Append(",");
			}
			sb.Append(">");
			return sb.ToString();
		}

		// Parse the method parameters
		public static string GetParams(MethodInfo method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length == 0) return string.Empty;

			int length = parameters.Length - 1;
			StringBuilder sb = new();
			for (int i = 0; i <= length; i++)
			{
				sb.Append(InRefOutParams(parameters[i]));
				sb.Append(" ");
				sb.Append(GetParamType(parameters[i]));
				sb.Append(" ");
				sb.Append(parameters[i].Name);

				string optArgument = GetDefaultValue(parameters[i]);
				if (!string.IsNullOrEmpty(optArgument))
				{
					sb.Append(" = ");
					sb.Append(optArgument);
				}
				if (i < length) sb.Append(", ");
			}

			return sb.ToString();
		}

		public static string InRefOutParams(ParameterInfo parameter)
		{
			// In compiled IL there is always a symbol & after in, ref, out
			if (parameter.ParameterType.Name.EndsWith("&"))
			{
				return parameter.IsIn ? "in" : parameter.IsOut ? "out" : "ref";
			}

			return (parameter.GetCustomAttributes().Any(x => x.GetType() == typeof(ParamArrayAttribute))) ? "params" : string.Empty;
		}

		public static string GetParamType(ParameterInfo parameter)
		{
			string typeName = parameter.ParameterType.Name;
			if (typeName.EndsWith("&"))
			{
				typeName = typeName.Substring(0, typeName.Length - 1);
			}
			return typeName;
		}

		// It it's an optional parameter, aka the Default value
		public static string GetDefaultValue(ParameterInfo parameter)
		{
			if (!parameter.IsOptional) return string.Empty;
			object? value = parameter.DefaultValue;
			return value != null ? value.ToString()! : string.Empty;
		}
	}

	#region use cases
	interface A
	{
		void TestA();
	}

	public abstract class B
	{
		public abstract void TestB();
	}

	public abstract class C : B
	{
		public virtual void TestC() { }
		public virtual void TestD() { }
	}

	public class MyClass14 : C//, A
	{
		public void TestA()
		{
			throw new NotImplementedException();
		}

		public override void TestB()
		{
			throw new NotImplementedException();
		}

		public override void TestC()
		{
			base.TestC();
		}

		new public void TestD() { }

		public (bool, bool) TestE()
		{
			return (true, true);
		}

		public string TestF<T>(T t)
		{
			return t!.GetType().Name;
		}

		public string TestG(in string a, ref string aa, out string b, string c = "666")
		{
			b = "666";
			return string.Empty;
		}

		public string TestH(params string[] d)
		{
			return string.Empty;
		}
	}
	#endregion
}
