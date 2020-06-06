namespace Cached.Tests.InMemory
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Cached.Caching;
    using Cached.InMemory;
    using Cached.Net;
    using Configuration;
    using Microsoft.Extensions.Caching.Memory;
    using Moq;
    using Xunit;

    public sealed class OptionsExtensionsTests
    {
        public sealed class AddInMemoryCachingMethod
        {
            public sealed class Throws
            {
                [Fact]
                public void When_Options_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(
                        () => ((ICachedConfigurationBuilder) null).AddInMemoryCaching());
                }
            }

            [Fact]
            public void Adds_Required_Services()
            {
                // Arrange
                var optionsMock = new Mock<ICachedConfigurationBuilder>();

                // Act
                optionsMock.Object.AddInMemoryCaching();

                // Assert
                optionsMock.Verify(m => m.TryAddSingleton<IMemoryCache, MemoryCache>(), Times.Once());
                optionsMock.Verify(m => m.TryAddSingleton(It.IsNotNull<Func<IResolver, IInMemoryCacher>>()),
                    Times.Once());
            }
        }

        public sealed class AddInMemoryCachedFunctionMethod
        {
            public sealed class Throws
            {
                [Fact]
                public void When_FetchFactory_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() => new CachedConfigurationBuilder()
                        .AddInMemoryCachedFunction<string, string>(p => p, null));
                }

                [Fact]
                public void When_KeyFactory_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() => new CachedConfigurationBuilder()
                        .AddInMemoryCachedFunction<string, string>(null, (_, __, ___) => Task.FromResult("")));
                }

                [Fact]
                public void When_Options_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() => ((ICachedConfigurationBuilder) null)
                        .AddInMemoryCachedFunction<string, string>(p => p, (_, __, ___) => Task.FromResult("")));
                }
            }

            [Fact]
            public void Adds_Required_Service()
            {
                // Arrange
                var optionsMock = new Mock<ICachedConfigurationBuilder>();

                // Act
                optionsMock.Object.AddInMemoryCachedFunction<string, double>(
                    arg => arg.ToString(CultureInfo.InvariantCulture),
                    (_, __, ___) => Task.FromResult("abc123"));

                // Assert
                optionsMock.Verify(
                    m => m.TryAddTransient(It.IsNotNull<Func<IResolver, ICached<string, double>>>()),
                    Times.Once());
            }
        }
    }
}