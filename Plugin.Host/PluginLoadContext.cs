using Plugin.Common;
using System.Reflection;
using System.Runtime.Loader;

namespace Plugin.Host
{
  public class PluginLoadContext : AssemblyLoadContext
  {
    private readonly AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string pluginPath, bool collectible)
    // Give it a friendly name to help with debugging:
    : base(name: Path.GetFileName(pluginPath), collectible)
    {
      _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
      if (assemblyName.Name == typeof(ITextPlugin).Assembly.GetName().Name)
        return null; // See below

      string? target = _resolver.ResolveAssemblyToPath(assemblyName);
      if (!string.IsNullOrEmpty(target)) return LoadFromAssemblyPath(target);
      return null;//Could be a framework assembly. Allow the default context to resolve.
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
      string? path = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
      return path == null ? IntPtr.Zero : LoadUnmanagedDllFromPath(unmanagedDllName);
    }
  }
}
