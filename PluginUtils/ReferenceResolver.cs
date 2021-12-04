using System.IO;
using System.Linq;
using System.Reflection;
using PluginUtil.Loger;

namespace PluginUtil
{
    public class ReferenceResolver
    {
        #region static

        public static void ReloadReference()
        {
            var asm = Assembly.GetExecutingAssembly();
            var directory = Path.GetDirectoryName(asm.Location);

            var dlls = Directory.GetFiles(directory)
                                .Where(x => !x.Equals(asm.Location))
                                .Where(x => x.EndsWith(".dll"))
                                .ToList();

            foreach (var dll in dlls)
                try
                {
                    Assembly.LoadFrom(dll);
                }
                catch
                {
                    $"(Dll {Path.GetFileName(dll)} not loaded!)".WriteLog();
                }
        }

        #endregion
    }
}