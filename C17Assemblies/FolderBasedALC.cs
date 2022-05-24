using System.Reflection;
using System.Runtime.Loader;

namespace C17Assemblies
{
  public class FolderBasedALC : AssemblyLoadContext
  {
    readonly string _folder;
    public FolderBasedALC(string folder) => _folder = folder;

    protected override Assembly? Load(AssemblyName assemblyName)
    {
      string targetPath = Path.Combine(_folder, assemblyName.Name + ".dll");

      if (File.Exists(targetPath))
        return LoadFromAssemblyPath(targetPath);
      return null;
    }
  }
}
