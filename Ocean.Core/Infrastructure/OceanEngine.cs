using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ocean.Core.Infrastructure.DependencyManagement;
using Ocean.Core.Configuration;
using System.Configuration;
using Autofac;

namespace Ocean.Core.Infrastructure
{
    public class OceanEngine : IEngine
    {
        #region Fields

        private ContainerManager _containerManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates an instance of the content engine using default settings and configuration.
        /// </summary>
        public OceanEngine()
            : this(EventBroker.Instance, new ContainerConfigurer())
        {
        }

        public OceanEngine(EventBroker broker, ContainerConfigurer configurer)
        {
            var config = ConfigurationManager.GetSection("OceanConfig") as OceanConfig;
            InitializeContainer(configurer, broker, config);
        }

        #endregion

        #region Utilities

        private void RunStartupTasks()
        {
            var typeFinder = _containerManager.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }

        private void InitializeContainer(ContainerConfigurer configurer, EventBroker broker, OceanConfig config)
        {
            var builder = new ContainerBuilder();

            _containerManager = new ContainerManager(builder.Build());
            configurer.Configure(this, _containerManager, broker, config);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize components and plugins in the ocean environment.
        /// </summary>
        /// <param name="config">Config</param>
        public void Initialize(OceanConfig config)
        {
            bool databaseInstalled = false;// DataSettingsHelper.DatabaseIsInstalled();
            if (databaseInstalled)
            {
                //startup tasks
                RunStartupTasks();
            }
        }

        public T Resolve<T>() where T : class
        {
            return ContainerManager.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        #endregion

        #region Properties

        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion
    }
}