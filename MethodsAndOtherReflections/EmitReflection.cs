using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace MethodsAndOtherReflections
{
  public static class EmitReflection
  {
    public static void EmitReflectionDemo()
    {
      WriteEnum();
      Console.WriteLine("");
      DynamicClassStaticMethod();
      DynamicInstanceMethod();
    }

    public static void WriteEnum()
    {
      TypeInfo? info = DynamicEnum();
      if (info == null) return;

      var myEnum = Activator.CreateInstance(info);
      Console.WriteLine($"{(info.IsPublic ? "public" : "private")} {(info.IsEnum ? "enum" : "class")} {info.Name}");
      Console.WriteLine("{");

      var names = Enum.GetNames(info);
      int[] values = (int[])Enum.GetValues(info);

      for (int i = 0; i < names.Length; i++)
      {
        Console.WriteLine($"  {names[i]} = {values[i]}{(i < names.Length - 1 ? "," : String.Empty)}");
      }
      Console.WriteLine("}");
    }

    public static (ModuleBuilder, Type?) EmitSetup()
    {
      // Assembly
      AssemblyName assemblyName = new AssemblyName("MyTest");
      AssemblyBuilder asmBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
      assemblyName.Version = new Version("1.0.0");
      assemblyName.CultureName = CultureInfo.CurrentCulture.Name;
      assemblyName.SetPublicKeyToken(new Guid().ToByteArray());

      // Module, TypeBuilder
      ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule("MyTest");
      TypeBuilder typeBuilder = moduleBuilder.DefineType("MyTest.MyClass", TypeAttributes.Public);
      Type? type = typeBuilder.CreateType();

      #region info test
      /*
      if (type != null)
      {
        Console.WriteLine($"Assembly Info: {type.Assembly.FullName}");
        Console.WriteLine($"Namespace: {type.Namespace}, Type: {type.Name}");
      }
      */
      #endregion
      return (moduleBuilder, type);
    }

    /*
     To build dynamically such a Enum:
     namespace MyTest {
       public enum MyEnum {
         Top = 1,
         Bottom = 2,
         Left = 4,
         Right = 8,
         All = 16
       }
     }
     */
    public static TypeInfo? DynamicEnum()
    {
      ModuleBuilder moduleBuilder = EmitSetup().Item1;
      EnumBuilder enumBuilder = moduleBuilder.DefineEnum("MyTest.MyEnum", TypeAttributes.Public, typeof(int));

      enumBuilder.DefineLiteral("Top", 0);
      enumBuilder.DefineLiteral("Bottom", 1);
      enumBuilder.DefineLiteral("Left", 2);
      enumBuilder.DefineLiteral("Right", 4);
      enumBuilder.DefineLiteral("All", 8);
      TypeInfo? type = enumBuilder.CreateTypeInfo();
      return type;
    }

    public static void DynamicClassStaticMethod()
    {
      DynamicMethod dyn = new DynamicMethod("Foo", null, null, typeof(MyClass19));
      ILGenerator ilGenerator = dyn.GetILGenerator();
      ilGenerator.EmitWriteLine("Hello World!");
      ilGenerator.Emit(OpCodes.Ret);
      dyn.Invoke(null, null); // The first argument being null means it's a static method
    }

    public static void DynamicInstanceMethod()
    {
      object? MyClass19Inst = Activator.CreateInstance(typeof(MyClass19));
      if (MyClass19Inst == null) return;

      // DynamicMethod dyn = new DynamicMethod("Add", typeof(int), new Type[] {typeof(int), typeof(int)});
      DynamicMethod dyn = new DynamicMethod("Add", typeof(int), new Type[] {typeof(int), typeof(int)}, typeof(MyClass19));
      ILGenerator ilCode = dyn.GetILGenerator();

      ilCode.Emit(OpCodes.Ldarg_0); //a, put the var of index 0 onto the stack
      ilCode.Emit(OpCodes.Ldarg_1); //b, put the var of index 1 onto the stack
      ilCode.Emit(OpCodes.Add);     // Add(a,b), then put the result onto the stack
      ilCode.Emit(OpCodes.Ret);

      // option 1
      Func<int, int, int> test = (Func<int, int, int>) dyn.CreateDelegate(typeof(Func<int, int, int>));
      Console.WriteLine($"Use dyn.CreateDelegate: test(1,2) = {test(1,2)}");

      // option 2
      // int sum = (int) dyn.Invoke(null, BindingFlags.Public, null, new object[] {1,2}, CultureInfo.CurrentCulture)!;
      int sum = (int) dyn.Invoke(MyClass19Inst, BindingFlags.Public, null, new object[] {1,2}, CultureInfo.CurrentCulture)!;
      Console.WriteLine($"Use dyn.Invoke: Invoke(1,2) = {sum}");
    }
  }

  public class MyClass19 {}
}
