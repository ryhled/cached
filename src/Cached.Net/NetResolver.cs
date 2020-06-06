namespace Cached.Net
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    internal class NetResolver : IResolver
    {
        private readonly IServiceProvider _provider;

        internal NetResolver(IServiceProvider provider)
        {
            _provider = provider;
        }

        public T GetService<T>() => _provider.GetService<T>();
    }
}