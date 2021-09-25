using CRM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using LightInject;
using LightInject.Mvc;
using System.Web.Services;
using System.Security.Claims;
using System.Threading;
using System.Web.Helpers;

namespace CRM.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            //GlobalFilters.Filters.Add(new AuthorizeAttribute());
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var container = new LightInject.ServiceContainer();
            container.RegisterControllers();
            container.Register(typeof(Model1Container), new PerRequestLifeTime());

            container.Register(typeof(CRM.Service.Admin.SettingService), new PerRequestLifeTime());
            container.Register(typeof(CRM.Service.Common.CommonService), new PerRequestLifeTime());
            container.Register(typeof(CRM.Service.Admin.NewRequestService), new PerRequestLifeTime());
            container.Register(typeof(CRM.Service.Admin.WorkingRequestService), new PerRequestLifeTime());
            container.Register(typeof(CRM.Service.Admin.CompletedRequestService), new PerRequestLifeTime());
            container.Register(typeof(CRM.Service.Admin.NotCompletedRequestService), new PerRequestLifeTime());
            container.Register(typeof(CRM.Service.Admin.UsersService), new PerRequestLifeTime());
            container.Register(typeof(CRM.Service.Admin.CalendarService), new PerRequestLifeTime());
            container.EnableMvc();

        }
    }
}
