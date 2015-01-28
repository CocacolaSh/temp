using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ocean.Core.Infrastructure.DependencyManagement;
using Autofac;
using Ocean.Core.Infrastructure;

namespace Ocean.Web.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
        
        }

        public int Order
        {
            get { return 2; }
        }
    }
}