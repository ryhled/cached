namespace Cached.Net
{
    using System;
    using Configuration;
    using Microsoft.Extensions.DependencyInjection;

    internal class NetResolver : IResolver
    {
        private readonly IServiceProvider _provider;

        internal NetResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        public T GetService<T>()
        {
            return _provider.GetService<T>();
        }
    }
}