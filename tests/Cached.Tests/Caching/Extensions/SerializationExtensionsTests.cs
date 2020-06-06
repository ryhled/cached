namespace Cached.Tests.Caching.Extensions
{
    using System.Collections.Generic;
    using Cached.Extensions;
    using Xunit;

    public class SerializationExtensionsTests
    {
        [Fact]
        public void Serializes_And_Deserializes_Object_Of_Correct_Type()
        {
            // Arrange
            var item = new HashSet<int> {1, 4, 8};

            // Act
            var bytes = item.ToByteArray();
            var couldDeserialize = bytes.TryParseObject(out HashSet<int> deserialized);

            //Assert
            Assert.True(couldDeserialize);
            Assert.NotNull(deserialized);
            Assert.Equal(item, deserialized);
        }

        [Fact]
        public void Fails_To_Serialize_Object_As_Incorrect_Type()
        {
            // Arrange
            var item = new HashSet<int> { 1, 4, 8 };

            // Act
            var bytes = item.ToByteArray();
            var couldDeserialize = bytes.TryParseObject(out List<int> deserialized);

            //Assert
            Assert.False(couldDeserialize);
            Assert.Null(deserialized);
        }

        public class ToByteArrayMethod
        {
            public class ReturnsNull
            {
                [Fact]
                public void When_Item_Is_Null()
                {
                    // Arrange, Act
                    var result = ((object) null).ToByteArray();

                    // Assert
                    Assert.Null(result);

                }
            }
        }

        public class TryParseObjectMethod
        {
            [Fact]
            public void When_ByteArray_Is_Null()
            {
                // Arrange, Act
                var result = ((byte[])null).TryParseObject(out object item);

                // Assert
                Assert.False(result);
                Assert.Null(item);
            }
        }
    }
}
