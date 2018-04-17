using System;
using System.Diagnostics;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AkkaWebJob
{
    class Program
    {
        static void Main(string[] args)
        {
            IContainer container = SetupDI();
            JobHostConfiguration configuration = CreateWebJobConfiguration(container);

            var host = new JobHost(configuration);
            host.RunAndBlock();
        }

        private static JobHostConfiguration CreateWebJobConfiguration(IContainer container)
        {
            var hostConfig = container.Resolve<IConfiguration>();
            var connection = hostConfig.GetValue<string>("StorageAccount");
            var configuration = new JobHostConfiguration(connection, hostConfig);

            configuration.Queues.MaxPollingInterval = TimeSpan.FromSeconds(10);
            configuration.Queues.BatchSize = 1;
            configuration.JobActivator = new CustomJobActivator(container);

            configuration.UseCore();
            configuration.UseTimers();

            return configuration;
        }

        private static IContainer SetupDI()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);

            //Register classes with webjobs/functions in here:
            containerBuilder.RegisterType<TimeScheduledWebJob>().AsSelf();

            return containerBuilder.Build();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.Configure<WebJobSettings>(configuration);

            // One more thing - tell azure where your azure connection strings are
            Environment.SetEnvironmentVariable("AzureWebJobsDashboard", configuration.GetConnectionString("StorageAccount"));
            Environment.SetEnvironmentVariable("AzureWebJobsStorage", configuration.GetConnectionString("StorageAccount"));
        }
    }
}
