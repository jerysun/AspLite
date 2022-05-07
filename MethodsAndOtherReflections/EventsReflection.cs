using System;
using System.Reflection;
using System.Text;

namespace MethodsAndOtherReflections
{
  public static class EventsReflection
  {
    public static void EventsReflectionDemo()
    {
      Type type = typeof(MyClass16);
      EventInfo[] events = type.GetEvents(BindingFlags.Public | BindingFlags.Static |
        BindingFlags.DeclaredOnly | BindingFlags.Instance);
      
      foreach (EventInfo eventInfo in events)
      {
        // Returns the method used to add an event handler delegate to the event source
        // true: includes the non-public methods too
        MethodInfo method = eventInfo.GetAddMethod(true);
        StringBuilder sb = new();

        string result = MethodsReflection.GetVisibility(method);
        if (result != String.Empty)
        {
          sb.Append(result);
          sb.Append(" ");
        }
        sb.Append(method.IsStatic ? "static " : String.Empty);

        result = MethodsReflection.GetMethodModifier(type, method);
        if (result != String.Empty)
        {
          sb.Append(result);
          sb.Append(" ");
        }
        sb.Append("event ");
        sb.Append(eventInfo.EventHandlerType.Name); //EventHandler is just a func signature
        sb.Append(" ");
        sb.Append(eventInfo.Name);
        sb.Append(';');
        Console.WriteLine(sb.ToString());
      }
    }
  }

  public delegate void DeTest();

  public abstract class AE
  {
    public abstract event DeTest TestA;
  }
  public abstract class BE : AE
  {
    public virtual event DeTest TestB;
    public event DeTest TestC;
  }
  public class MyClass16 : BE
  {
    public override event DeTest TestA;
    public override event DeTest TestB;
    new public event DeTest TestC;
  }
}
