using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac.Integration.Mvc;
using Autofac;
using AccreditTechTest.Controllers;
using AccreditTechTest.Services;
using System.Net.Http;
using Serilog;
using System.Configuration;
using System;

namespace AccreditTechTest
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Setup_Dependencies();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .CreateLogger();
        }

        private void Setup_Dependencies()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(HomeController).Assembly);

            builder.RegisterType<GitHubService>().As<IGitHubService>().InstancePerRequest();
            builder.Register(c =>
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(ConfigurationManager.AppSettings["GitHubApiBaseUrl"])
                };

                client.DefaultRequestHeaders.UserAgent.ParseAdd("TechTest");

                return client;
            }).As<HttpClient>().SingleInstance();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
