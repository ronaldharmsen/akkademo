using System;
using Autofac;
using Microsoft.Azure.WebJobs.Host;

namespace AkkaWebJob {
    public class CustomJobActivator : IJobActivator
    {
        private readonly IContainer container;

        public CustomJobActivator(IContainer container)
        {
            this.container = container;
        }

        public T CreateInstance<T>()
        {
            return container.Resolve<T>();
        }
    }
}