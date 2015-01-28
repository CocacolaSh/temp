using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Configuration;

namespace Ocean.Core.Infrastructure.DependencyManagement
{
    /// <summary>
    /// Configures the inversion of control container with services used by Ocean.
    /// </summary>
    public class ContainerConfigurer
    {
        /// <summary>
        /// Known configuration keys used to configure services.
        /// </summary>
        public static class ConfigurationKeys
        {
            /// <summary>Key used to configure services intended for medium trust.</summary>
            public const string MediumTrust = "MediumTrust";
            /// <summary>Key used to configure services intended for full trust.</summary>
            public const string FullTrust = "FullTrust";
        }

        public virtual void Configure(IEngine engine, ContainerManager containerManager, EventBroker broker, OceanConfig configuration)
        {
            //other dependencies
            containerManager.AddComponentInstance<OceanConfig>(configuration, "ocean.configuration");
            containerManager.AddComponentInstance<IEngine>(engine, "ocean.engine");
            containerManager.AddComponentInstance<ContainerConfigurer>(this, "ocean.containerConfigurer");

            //type finder
            containerManager.AddComponent<ITypeFinder, WebAppTypeFinder>("ocean.typeFinder");

            //register dependencies provided by other assemblies
            var typeFinder = containerManager.Resolve<ITypeFinder>();
            containerManager.UpdateContainer(x =>
            {
                var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
                var drInstances = new List<IDependencyRegistrar>();
                foreach (var drType in drTypes)
                    drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));
                //sort
                drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
                foreach (var dependencyRegistrar in drInstances)
                    dependencyRegistrar.Register(x, typeFinder);
            });

            //event broker
            containerManager.AddComponentInstance(broker);

            //service registration
            containerManager.AddComponent<DependencyAttributeRegistrator>("ocean.serviceRegistrator");
            var registrator = containerManager.Resolve<DependencyAttributeRegistrator>();
            var services = registrator.FindServices();
            var configurations = GetComponentConfigurations(configuration);
            services = registrator.FilterServices(services, configurations);
            registrator.RegisterServices(services);
        }

        protected virtual string[] GetComponentConfigurations(OceanConfig configuration)
        {
            var configurations = new List<string>();
            string trustConfiguration = (CommonHelper.GetTrustLevel() > System.Web.AspNetHostingPermissionLevel.Medium)
                ? ConfigurationKeys.FullTrust
                : ConfigurationKeys.MediumTrust;
            configurations.Add(trustConfiguration);
            return configurations.ToArray();
        }
    }
}