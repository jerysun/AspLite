# AspLite

Write a prototype Framework of Asp.Net where the advanced C# technique Reflection shines!

Implement the DI(Dependency Injection), and allow the access to the RESTful API the same as what ASP.NET Core does: pass parameters then automatically call the Action methods, and yes, finally return the response.

## UI Requirements:
- User can access Controller
- User can access Action
- When accessing Action, pass parameters

## Tech Requirements:
- Instantiate types on demand
- Identify the Constructor Type
- Dynamically instantiate the types based on the constructor parameters' types then inject them to the controller
- Dynamically call the appropriate overloaded methods

Here are some important methods, which are easy to understand due to the detailed comments.

```C#
/// <summary>
/// Check if there's such a controller and return the result and a Type as well
/// </summary>
/// <param name="controllerName">The pure controller name without the postfix "controller"</param>
/// <returns></returns>
private static(bool, Type) HasController(string controllerName) {
  string name = controllerName.EndsWith("Controller") ? controllerName: controllerName + "Controller";

  if (!types.Any(x =>x.Name.ToLower() == name.ToLower())) return (false, null);

  return (true, types.FirstOrDefault(x =>x.Name.ToLower() == name.ToLower()));
}

/// <summary>
/// Check if the controller has such an action method
/// </summary>
/// <param name="type">Controller type</param>
/// <param name="actionName">Action method name</param>
/// <returns></returns>
public static bool HasAction(Type type, string actionName) =>type.GetMethods().Any(m =>m.Name.ToLower() == actionName.ToLower());

/// <summary>
/// Type instantiation via the DI(Dependency Injection)
/// </summary>
/// <param name="type">The type which will be instantiated by using DI</param>
private static object[] CreateType(Type type) {
  // In this case and also most of the time one controller has only one constructor
  ConstructorInfo ctor = type.GetConstructors().FirstOrDefault();
  ParameterInfo[] parms = ctor.GetParameters();

  // object list for DI
  List < object > objects = new();
  foreach(ParameterInfo pi in parms) {
    // Find which type is the interface pi
    Type who = types.FirstOrDefault(x =>x.GetInterfaces().Any(y =>y.Name == pi.ParameterType.Name));

    object created = Activator.CreateInstance(who, new object[] {});
    objects.Add(created);
  }

  return objects.ToArray();
}
```

However the most important method is not listed here:
```C#
private static object SimulateAspNet(Type type, string actionName, params object[] parms);
```

Don't worry, you can always access not only it but also the whole project on my Github site: https://github.com/jerysun/AspLite

Have fun!
Jerry