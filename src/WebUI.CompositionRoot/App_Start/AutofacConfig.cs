using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using SubstituteProxy;

namespace WebUI.CompositionRoot
{
    public static class AutofacConfig
    {
        public static void RegisterDependency()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.Load("WebUI"));
            builder.RegisterAssemblyTypes(Assembly.Load("SubstituteProxy"))
                .AsSelf()
                .SingleInstance();
            builder.RegisterType<SubstituteProxyService>()
                .AsImplementedInterfaces()
                .SingleInstance();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}