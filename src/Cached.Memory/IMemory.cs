namespace Cached.Memory
{
    using Caching;

    /// <summary>
    /// Provides caching, utilizing MemoryCache under the hood.
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.memorycache"/> 
    /// </summary>
    public interface IMemory : ICacheProvider
    {
    }
}
