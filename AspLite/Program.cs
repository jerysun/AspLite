using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AspLite
{
  internal class Program
  {
    private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
    private static readonly Type[] types = assembly.GetTypes();

    static void Main(string[] args)
    {
      while(true)
      {
        Console.WriteLine("Hi, please input the controller name without the postfix Controller(in this case, simply just input Asp):");
        string input = Console.ReadLine();

        var hasController = HasController(input);

        // Output the status code "404" if failed and let the app continue
        if (!hasController.Item1)
        {
          Console.WriteLine("404");
          continue;
        }

        Console.WriteLine("The controller is found, please input the Action name(in this case, simply just input AspAction):");
        input = Console.ReadLine();

        Type type = hasController.Item2;
        bool hasAction = HasAction(type, input);
        if (!hasAction)
        {
          Console.WriteLine("404");
          continue;
        }

        Console.WriteLine("The route URL is found, please input arguments(Please press the Enter key after each input. If you want to end the input, please input the digit 0 then press Enter):");

        // Be ready to accept the arguments
        List<object> parms = new();
        
        while(true)
        {
          string parm = Console.ReadLine();
          if (parm == "0") break;
          parms.Add(parm);
        }
        Console.WriteLine("The input is finished, sending the Http Request...\n");

        object response = SimulateAspNet(type, input, parms.ToArray());
        Console.WriteLine($"The response result: {response}");
        Console.ReadKey();
      }
    }

    /// <summary>
    /// Check if there's such a controller and return the result and a Type as well
    /// </summary>
    /// <param name="controllerName">The pure controller name without the postfix "controller"</param>
    /// <returns></returns>
    private static (bool, Type) HasController(string controllerName)
    {
      string name = controllerName.EndsWith("Controller") ? controllerName : controllerName + "Controller";

      if (!types.Any(x => x.Name.ToLower() == name.ToLower())) return (false, null);

      return (true, types.FirstOrDefault(x => x.Name.ToLower() == name.ToLower()));
    }

    /// <summary>
    /// Check if the controoler has such an action method
    /// </summary>
    /// <param name="type">Controller type</param>
    /// <param name="actionName">Action method name</param>
    /// <returns></returns>
    public static bool HasAction(Type type, string actionName) =>
      type.GetMethods().Any(m => m.Name.ToLower() == actionName.ToLower());

    /// <summary>
    /// Type instantiation via the DI(Dependency Injection)
    /// </summary>
    /// <param name="type">The type which will be instantiated by using DI</param>
    private static object[] CreateType(Type type)
    {
      // In this case and also most of the time one contorller has only one constructor
      ConstructorInfo ctor = type.GetConstructors().FirstOrDefault();
      ParameterInfo[] parms = ctor.GetParameters();

      // object list for DI
      List<object> objects = new();
      foreach(ParameterInfo pi in parms)
      {
        // Find which type is the interface pi
        Type who = types
          .FirstOrDefault(x => x.GetInterfaces()
          .Any(y => y.Name == pi.ParameterType.Name));
        
        object created = Activator.CreateInstance(who, new object[] { });
        objects.Add(created);
      }

      return objects.ToArray();
    }

    /// <summary>
    /// Create the type instance by using DI, and call its Action method
    /// </summary>
    /// <param name="type">type to instantiate</param>
    /// <param name="actionName">Action method to call</param>
    /// <param name="parms">The parameters of the calling method</param>
    /// <returns></returns>
    private static object SimulateAspNet(Type type, string actionName, params object[] parms)
    {
      // Get the overloading Action method with the same number of parameters
      MethodInfo method = type.GetMethods().FirstOrDefault(x =>
        x.Name.ToLower() == actionName.ToLower() && x.GetParameters().Length == parms.Length);

      // Return the code 405 "Method Not Allowed" if not found
      if (method == null) return "405";

      // Get the dependent objects in ctor prams: inject
      object[] inject = CreateType(type);
      // Inject the dependency and instantiate the type - the controller object
      object typeInstance = Activator.CreateInstance(type, inject);
      
      try
      {
        object result = method.Invoke(typeInstance, parms);
        return result;
      }
      catch
      {
        return "500";
      }
    }
  }
}
