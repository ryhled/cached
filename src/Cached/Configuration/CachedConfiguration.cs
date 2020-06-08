namespace Cached.Configuration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Configuration for setting up Cached support. 
    /// </summary>
    public sealed class CachedConfiguration
    {
        internal readonly List<Action<IServiceCollection>> Services
            = new List<Action<IServiceCollection>>();
    }
}
