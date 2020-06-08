namespace Cached.Distributed
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    internal static class SerializationExtensions
    {
        internal static byte[] ToByteArray(this object item)
        {
            if (item == null)
            {
                return null;
            }

            var formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, item);
                return stream.ToArray();
            }
        }

        internal static bool TryParseObject<T>(this byte[] objectBytes, out T item)
        {
            if (objectBytes != null)
            {
                var formatter = new BinaryFormatter();
                using (var stream = new MemoryStream(objectBytes))
                {
                    if (formatter.Deserialize(stream) is T castItem)
                    {
                        item = castItem;
                        return true;
                    }
                }
            }

            item = default;
            return false;
        }
    }
}