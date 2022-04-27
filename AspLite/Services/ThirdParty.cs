using System;

namespace AspLite.Services
{
  public class ThirdParty : IThirdParty
  {
    public string Add(string a, string b)
    {
      Console.WriteLine("Add method is called.");
      return a + b;
    }
  }
}
