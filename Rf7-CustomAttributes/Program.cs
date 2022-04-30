using Rf7_CustomAttributes.ValidationProcessing;
using System;
using System.Collections.Generic;

namespace Rf7_CustomAttributes
{
  internal class Program
  {
    static void Main(string[] args)
    {
      List<object> users = new ()
      {
        new User { Id = 0 },
        new User
        {
          Id = 1,
          Name = "Stupid_Craftsman",
          Phone = "0627225110",
          Email = "666@qq.com"
        },
        new User
        {
          Id = 2,
          Name = "Lindsey Graham",
          Phone = "6666666",
          Email = "CNN@CNN.CNN"
        }
      };

      AttributesProcessing.Analyze(users);
      Console.ReadKey();
    }
  }
}
