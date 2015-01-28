using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Infrastructure;
using System.Reflection;

namespace Ocean.Framework.EmbeddedViews
{
    public class EmbeddedViewResolver : IEmbeddedViewResolver
    {
        ITypeFinder _typeFinder;
        public EmbeddedViewResolver(ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
        }

        public EmbeddedViewTable GetEmbeddedViews()
        {
            var assemblies = _typeFinder.GetAssemblies();
            if (assemblies == null || assemblies.Count == 0) return null;

            var table = new EmbeddedViewTable();

            foreach (var assembly in assemblies)
            {
                var names = GetNamesOfAssemblyResources(assembly);
                if (names == null || names.Length == 0) continue;

                foreach (var name in names)
                {
                    var key = name.ToLowerInvariant();
                    if (!key.Contains(".views.")) continue;

                    table.AddView(name, assembly.FullName);
                }
            }

            return table;
        }

        private static string[] GetNamesOfAssemblyResources(Assembly assembly)
        {
            //GetManifestResourceNames will throw a NotSupportedException when run on a dynamic assembly
            try
            {
                return assembly.GetManifestResourceNames();
            }
            catch
            {
                return new string[] { };
            }
        }
    }
}
