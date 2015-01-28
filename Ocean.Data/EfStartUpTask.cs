using Ocean.Core;
using Ocean.Core.Data;
using Ocean.Core.Infrastructure;

namespace Ocean.Data
{
    public class EfStartUpTask : IStartupTask
    {
        public void Execute()
        {
            var settings = EngineContext.Current.Resolve<DataSettings>();
            if (settings != null && settings.IsValid())
            {
                var provider = EngineContext.Current.Resolve<IEfDataProvider>();
                if (provider == null)
                    throw new OceanException("No EfDataProvider found");
                provider.SetDatabaseInitializer();
            }
        }

        public int Order
        {
            //ensure that this task is run first 
            get { return -1000; }
        }
    }
}