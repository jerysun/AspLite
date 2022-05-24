using Plugin.Common;
using Plugin.Host;
using System.Reflection;

const bool UseCollectibleContext = true;

MainEntry();

static void MainEntry()
{
  const string capitalizer = @"D:\prjs_vs2022\CsInNutShell10Zone\AspLite\Capitalizer\bin\Debug\net6.0\Capitalizer.dll";
  Console.WriteLine(TransformText("big apple", capitalizer));
}

static string TransformText(string text, string pluginPath)
{
  var alc = new PluginLoadContext(pluginPath, UseCollectibleContext);
  try
  {
    Assembly assem = alc.LoadFromAssemblyPath(pluginPath);

    // Locate the type in the assembly that implements ITextPlugin
    Type pluginType = assem.ExportedTypes.Single(t => typeof(ITextPlugin).IsAssignableFrom(t));

    // Instantiate the ITextPlugin implementation
    ITextPlugin plugin = (ITextPlugin)Activator.CreateInstance(pluginType)!;
    if (plugin != null) return plugin.TransformText(text);
    return String.Empty;
  }
  finally
  {
    if (UseCollectibleContext) alc.Unload();
  }
}