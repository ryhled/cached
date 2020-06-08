namespace Cached.Memory.Configuration
{
    using Cached.Configuration;
    using Microsoft.Extensions.Caching.Memory;

    /// <summary>
    /// Options used to configure the memory-based caching.
    /// </summary>
    public sealed class MemoryOptions : ServiceOptions<IMemoryCacher>
    {
        internal MemoryOptions()
        {
        }

        /// <summary>
        /// MemoryCache entry options that overrides the global MemoryCAche settings.
        /// </summary>
        public MemoryCacheEntryOptions Options { get; set; }
    }
}
