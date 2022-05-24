using C17Assemblies;
using System.Reflection;
using System.Runtime.Loader;

//PrintFolderBasedAlc();
ResolveAssemblyDependency();


static void ResolveAssemblyDependency()
{
  const string assemPath = @"D:\prjs_vs2022\ConsoleZone\ClientApp\bin\Debug\net6.0\ClientApp.dll";
  var resolver = new AssemblyDependencyResolver(assemPath);
  var sqlClient = new AssemblyName("Microsoft.Data.SqlClient");
  Console.WriteLine(resolver.ResolveAssemblyToPath(sqlClient));
}

static void PrintFolderBasedAlc()
{
  const string assemPath = @"D:\prjs_vs2022\ProIdentityNet6Book\my-core-identity\IdentityApp\bin\Debug\net6.0";
  FolderBasedALC alc = new FolderBasedALC(assemPath);
  alc.LoadFromAssemblyName(new AssemblyName("IdentityApp"));

  foreach (Assembly a in alc.Assemblies) Console.WriteLine(a.FullName);
}
