namespace LogApp.App_Start
{
    using Autofac;
    using Autofac.Integration.Mvc;
    using Database.Configuration.Interfaces;
    using Database.Configuration.Implementations;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;

    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            // Register dependencies in controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterFilterProvider();

            builder.RegisterSource(new ViewRegistrationSource());

            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(x => x.Namespace.EndsWith(".Implementations"))
                .AsImplementedInterfaces();

            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerDependency();

            builder.RegisterType<LogAppDbContext>().As<ILogAppDbContext>().InstancePerHttpRequest();

            LoadAndRegisterAssemblies(builder);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static void LoadAndRegisterAssemblies(ContainerBuilder builder)
        {
            LoadAssemblies();
            RegisterAssemblies(builder);
        }

        private static void LoadAssemblies()
        {
            var assembliesNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            foreach (var assemblyName in assembliesNames)
            {
                Assembly.Load(assemblyName);
            }
        }

        private static void RegisterAssemblies(ContainerBuilder builder)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly != null)
                {
                    builder.RegisterAssemblyTypes(assembly)
                        .Where(x =>
                        {
                            if (x != null && x.Namespace != null)
                                return x.Namespace.EndsWith(".Implementations");
                            return false;
                        })
                        .AsImplementedInterfaces();
                }
            }
        }
    }
}