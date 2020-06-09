namespace Cached.Caching
{
    /// <summary>
    ///     Represents the result of an attempt to get a value from the cache.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public sealed class ValueResult<TValue>
    {
        private ValueResult(bool success, TValue value)
        {
            Value = value;
            Succeeded = success;
        }

        internal static ValueResult<TValue> Miss => new ValueResult<TValue>(false, default);

        /// <summary>
        ///     If the attempt to get the value succeeded.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        ///     The retrieved value, or default.
        /// </summary>
        public TValue Value { get; }

        internal static ValueResult<TValue> Hit(TValue value)
        {
            return new ValueResult<TValue>(true, value);
        }
    }
}