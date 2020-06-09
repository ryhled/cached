namespace Cached.Tests.Caching
{
    using Cached.Caching;
    using Xunit;

    public class ValueResultTests
    {
        public class Miss
        {
            [Fact]
            public void Create_Failed_Result_Instance_With_Empty_Value()
            {
                // Arrange, Act
                var result = ValueResult<int>.Miss;

                // Assert
                Assert.False(result.Succeeded);
                Assert.IsType<int>(result.Value);
                Assert.Equal(default, result.Value);
            }
        }

        public class Hit
        {
            [Fact]
            public void Create_Successful_Result_Instance_With_Matching_Value()
            {
                // Arrange
                const string value = "abc123";

                // Act
                var result = ValueResult<string>.Hit(value);

                // Assert
                Assert.True(result.Succeeded);
                Assert.Equal(value, result.Value);
            }
        }
    }
}
