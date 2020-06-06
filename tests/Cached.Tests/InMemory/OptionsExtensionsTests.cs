namespace Cached.Tests.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Cached.Caching;
    using Cached.InMemory;
    using Cached.Net;
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
                        () => ((ICachedConfigurationBuilder)null).AddInMemoryCaching());
                }
            }

            //public sealed class WillAddCacher
            //{
            //    [Fact]
            //    public void When_Valid_Options_Argument_Is_Provided()
            //    {
            //        // Arrange
            //        Func<IServiceProvider, IInMemoryCacher> createdFactory = null;
            //        var optionsMock = new Mock<ICachedConfigurationBuilder>();
            //        optionsMock.Setup(o =>
            //                o.TryAddCacher(
            //                    It.IsAny<Func<IResolver, IInMemoryCacher>>()))
            //            .Callback(
            //                (Func<IServiceProvider,IInMemoryCacher> fetchFactory) => createdFactory = fetchFactory);

            //        // Act
            //        optionsMock.Object.AddInMemoryCaching();

            //        // Assert
            //        optionsMock.Verify(
            //            o => o.TryAddCacher(It.IsAny<Func<IResolver, IInMemoryCacher>>()),
            //            Times.Once);
            //        Assert.NotNull(createdFactory);
            //    }
            //}
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
                    Assert.Throws<ArgumentNullException>(() => ((ICachedConfigurationBuilder)null)
                        .AddInMemoryCachedFunction<string, string>(p => p, (_, __, ___) => Task.FromResult("")));
                }
            }

            public sealed class WillAddCached
            {
                [Fact]
                public void When_Key_And_Fetch_Factories_Are_Provided()
                {
                    // Arrange
                    Func<IResolver, ICached<string, string>> createdFactory = null;
                    var optionsMock = new Mock<ICachedConfigurationBuilder>();
                    optionsMock.Setup(o =>
                            o.TryAddCached<ICached<string, string>, ICached<string, string>, string, string>(
                                It.IsAny<Func<IResolver, ICached<string, string>>>()))
                        .Callback((Func<IResolver, ICached<string, string>> fetchFactory) =>
                            createdFactory = fetchFactory);

                    // Act
                    optionsMock.Object.AddInMemoryCachedFunction<string, string>(arg => arg,
                        (_, __, ___) => Task.FromResult("abc123"));

                    // Assert
                    optionsMock.Verify(
                        o => o.TryAddCached<ICached<string, string>, ICached<string, string>, string, string>(
                            It.IsAny<Func<IResolver, ICached<string, string>>>()), Times.Once);
                    Assert.NotNull(createdFactory);
                }
            }
        }
    }
}