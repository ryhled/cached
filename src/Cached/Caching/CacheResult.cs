namespace Cached.Caching
{
    /// <summary>
    ///     Represents the result of an attempt to get a value from the cache.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public sealed class CacheResult<T>
    {
        private CacheResult(bool success, T value)
        {
            Value = value;
            Succeeded = success;
        }

        internal static CacheResult<T> Miss => new CacheResult<T>(false, default);

        /// <summary>
        ///     If the attempt to get the value succeeded.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        ///     The retrieved value, or default.
        /// </summary>
        public T Value { get; }

        internal static CacheResult<T> Hit(T value)
        {
            return new CacheResult<T>(true, value);
        }
    }
}