using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Infrastructure.DependencyManagement;
using Ocean.Core.Configuration;

namespace Ocean.Core.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the 
    /// various services composing the Ocean engine. Edit functionality, modules
    /// and implementations access most Ocean functionality through this 
    /// interface.
    /// </summary>
    public interface IEngine
    {
        ContainerManager ContainerManager { get; }

        /// <summary>
        /// Initialize components and plugins in the ocean environment.
        /// </summary>
        /// <param name="config">Config</param>
        void Initialize(OceanConfig config);

        T Resolve<T>() where T : class;

        object Resolve(Type type);

        T[] ResolveAll<T>();
    }
}