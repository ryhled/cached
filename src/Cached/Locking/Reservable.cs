namespace Cached.Locking
{
    using System;

    internal sealed class Reservable<T>
    {
        public Reservable(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
        }

        public int Reservations { get; private set; }

        public T Value { get; }

        public void Reserve()
        {
            ++Reservations;
        }

        public bool TryRelease()
        {
            if (Reservations == 0)
            {
                throw new InvalidOperationException("Already released");
            }

            --Reservations;
            return Reservations == 0;
        }
    }
}