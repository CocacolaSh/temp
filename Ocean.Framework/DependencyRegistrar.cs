using System.Linq;
using Ocean.Core.Infrastructure.DependencyManagement;
using Autofac;
using Autofac.Integration.Mvc;
using Ocean.Core.Infrastructure;
using System.Web;
using Ocean.Framework.Mvc.Routes;
using Ocean.Core.Plugins;
using Ocean.Data;
using Ocean.Core.Data;
using Ocean.Services.Cms;
using Ocean.Core;
using Ocean.Framework.Themes;
using Ocean.Core.Fakes;
using Ocean.Core.Configuration;
using Autofac.Core;
using System.Reflection;
using System.Collections.Generic;
using System;
using Autofac.Builder;
using Ocean.Services;
using Ocean.Core.Caching;
using Ocean.Core.Logging;
using Ocean.Core.Utility;
using Ocean.Services.Tasks;
using Ocean.Framework.EmbeddedViews;

namespace Ocean.Framework
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //HTTP context and other related stuff
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerHttpRequest();

            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerHttpRequest();

            //cache manager
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("ocean_cache_static").SingleInstance();
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().Named<ICacheManager>("ocean_cache_per_request").InstancePerHttpRequest();

            //controllers
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            //data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();
            builder.Register(c => dataSettingsManager.LoadSettings()).As<DataSettings>().InstancePerDependency();
            builder.Register(x => new EfDataProviderManager(x.Resolve<DataSettings>())).As<BaseDataProviderManager>().InstancePerDependency();
            builder.Register(x => (IEfDataProvider)x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IDataProvider>().InstancePerDependency();
            builder.Register(x => (IEfDataProvider)x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IEfDataProvider>().InstancePerDependency();

            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                var efDataProviderManager = new EfDataProviderManager(dataSettingsManager.LoadSettings());
                var dataProvider = (IEfDataProvider)efDataProviderManager.LoadDataProvider();
                dataProvider.InitConnectionFactory();
                builder.Register<IDbContext>(c => new OceanObjectContext(dataProviderSettings.DataConnectionString)).InstancePerHttpRequest();
            }
            else
            {
                builder.Register<IDbContext>(c => new OceanObjectContext(dataSettingsManager.LoadSettings().DataConnectionString)).InstancePerHttpRequest();
            }

            //builder.Register<IDbContext>(c => new OceanObjectContext("EFDbContext")).InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();

            //service
            builder.RegisterType<EnumTypeService>().As<IEnumTypeService>().InstancePerHttpRequest();
            builder.RegisterType<EnumDataService>().As<IEnumDataService>().InstancePerHttpRequest();
            builder.RegisterType<AdminService>().As<IAdminService>().InstancePerHttpRequest();
            builder.RegisterType<AdminLoggerService>().As<IAdminLoggerService>().InstancePerHttpRequest();
            builder.RegisterType<PermissionOrganizationService>().As<IPermissionOrganizationService>().InstancePerHttpRequest();
            builder.RegisterType<PermissionModuleService>().As<IPermissionModuleService>().InstancePerHttpRequest();
            builder.RegisterType<PermissionModuleCodeService>().As<IPermissionModuleCodeService>().InstancePerHttpRequest();
            builder.RegisterType<PermissionRoleService>().As<IPermissionRoleService>().InstancePerHttpRequest();
            builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerHttpRequest();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerHttpRequest();
            builder.RegisterType<LoanService>().As<ILoanService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("ocean_cache_static"))
                .InstancePerHttpRequest();
            builder.RegisterType<LoanAssignLoggerService>().As<ILoanAssignLoggerService>().InstancePerHttpRequest();
            builder.RegisterType<MpCenterService>().As<IMpCenterService>().InstancePerHttpRequest();
            builder.RegisterType<MpMaterialService>().As<IMpMaterialService>().InstancePerHttpRequest();
            builder.RegisterType<MpMaterialItemService>().As<IMpMaterialItemService>().InstancePerHttpRequest();
            builder.RegisterType<MpUserGroupService>().As<IMpUserGroupService>().InstancePerHttpRequest();
            builder.RegisterType<MpReplyService>().As<IMpReplyService>().InstancePerHttpRequest();
            builder.RegisterType<MpUserService>().As<IMpUserService>().InstancePerHttpRequest();
            builder.RegisterType<FunongbaoService>().As<IFunongbaoService>().InstancePerHttpRequest();
            builder.RegisterType<FunongbaoApplyService>().As<IFunongbaoApplyService>().InstancePerHttpRequest();
            builder.RegisterType<BranchService>().As<IBranchService>().InstancePerHttpRequest();
            builder.RegisterType<MobileCodeService>().As<IMobileCodeService>().InstancePerHttpRequest();
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().InstancePerHttpRequest();
            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerHttpRequest();
            builder.RegisterType<MpQrSceneService>().As<IMpQrSceneService>().InstancePerHttpRequest();

            //KF
            builder.RegisterType<KfNumberService>().As<IKfNumberService>().InstancePerHttpRequest();
            builder.RegisterType<KfMeetingService>().As<IKfMeetingService>().InstancePerHttpRequest();
            builder.RegisterType<KfMeetingMessageService>().As<IKfMeetingMessageService>().InstancePerHttpRequest();

            //configuration and setting
            builder.RegisterGeneric(typeof(ConfigurationProvider<>)).As(typeof(IConfigurationProvider<>));
            builder.RegisterSource(new SettingsSource());
            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerHttpRequest();

            //theme helper
            builder.RegisterType<MobileDeviceHelper>().As<IMobileDeviceHelper>().InstancePerHttpRequest();
            builder.RegisterType<ThemeProvider>().As<IThemeProvider>().InstancePerHttpRequest();
            builder.RegisterType<ThemeContext>().As<IThemeContext>().InstancePerHttpRequest();

            //Visual Plugins
            builder.RegisterType<PluginBaseService>().As<IPluginBaseService>().InstancePerHttpRequest();
            builder.RegisterType<PluginBaseStyleService>().As<IPluginBaseStyleService>().InstancePerHttpRequest();
            builder.RegisterType<PluginService>().As<IPluginService>().InstancePerHttpRequest();
            builder.RegisterType<PluginResultService>().As<IPluginResultService>().InstancePerHttpRequest();
            builder.RegisterType<PluginUsedService>().As<IPluginUsedService>().InstancePerHttpRequest();
            builder.RegisterType<PluginAllowUserService>().As<IPluginAllowUserService>().InstancePerHttpRequest();
            builder.RegisterType<PluginSceneApllyCodeAllowerService>().As<IPluginSceneApllyCodeAllowerService>().InstancePerHttpRequest();
            builder.RegisterType<PluginSceneResultService>().As<IPluginSceneResultService>().InstancePerHttpRequest();
            builder.RegisterType<PluginSceneVerifyCodeDetailService>().As<IPluginSceneVerifyCodeDetailService>().InstancePerHttpRequest();

            //plugins
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().InstancePerHttpRequest();
            builder.RegisterType<EmbeddedViewResolver>().As<IEmbeddedViewResolver>().SingleInstance();
            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();
            builder.RegisterType<WidgetService>().As<IWidgetService>().InstancePerHttpRequest();
            Log4NetImpl.SetConfig(FileHelper.GetMapPath("~/config/log4net.config"));

            //pos
            builder.RegisterType<PosApplyService>().As<IPosApplyService>().InstancePerHttpRequest();
            builder.RegisterType<PosAuthService>().As<IPosAuthService>().InstancePerHttpRequest();
            builder.RegisterType<PosService>().As<IPosService>().InstancePerHttpRequest();

            //vote
            builder.RegisterType<VoteItemService>().As<IVoteItemService>().InstancePerHttpRequest();
            builder.RegisterType<VoteInfoService>().As<IVoteInfoService>().InstancePerHttpRequest();
            builder.RegisterType<VoteBaseService>().As<IVoteBaseService>().InstancePerHttpRequest();

            //scoresys
            builder.RegisterType<ScoreUserService>().As<IScoreUserService>().InstancePerHttpRequest();
            builder.RegisterType<ScoreTradeInfoService>().As<IScoreTradeInfoService>().InstancePerHttpRequest();
            builder.RegisterType<ScoreStoreItemService>().As<IScoreStoreItemService>().InstancePerHttpRequest();
            builder.RegisterType<ScorePluginResultService>().As<IScorePluginResultService>().InstancePerHttpRequest();
            builder.RegisterType<ScoreConsumeInfoService>().As<IScoreConsumeInfoService>().InstancePerHttpRequest();

            //xypluginuser
            builder.RegisterType<XYPluginUserService>().As<IXYPluginUserService>().InstancePerHttpRequest();


            //baoxian
            builder.RegisterType<ComplainService>().As<IComplainService>().InstancePerHttpRequest();
            builder.RegisterType<DrivingLicenseService>().As<IDrivingLicenseService>().InstancePerHttpRequest();
            builder.RegisterType<VehicleLicenseService>().As<IVehicleLicenseService>().InstancePerHttpRequest();
        }

        public int Order
        {
            get { return 0; }
        }
    }

    public class SettingsSource : IRegistrationSource
    {
        static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod(
            "BuildRegistration",
            BindingFlags.Static | BindingFlags.NonPublic);

        public IEnumerable<IComponentRegistration> RegistrationsFor(
                Service service,
                Func<Service, IEnumerable<IComponentRegistration>> registrations)
        {
            var ts = service as TypedService;
            if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
            {
                var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                yield return (IComponentRegistration)buildMethod.Invoke(null, null);
            }
        }

        static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
        {
            return RegistrationBuilder
                .ForDelegate((c, p) => c.Resolve<IConfigurationProvider<TSettings>>().Settings)
                .InstancePerHttpRequest()
                .CreateRegistration();
        }

        public bool IsAdapterForIndividualComponents { get { return false; } }
    }
}