namespace Cached.Tests.InMemory
{
    using System;
    using System.Threading.Tasks;
    using Cached.InMemory;
    using Cached.Net;
    using Configuration;
    using Moq;
    using Xunit;

    public sealed class OptionsExtensionsTests
    {
        public sealed class AddInMemoryCachingMethod
        {
            public sealed class Throws_Exception
            {
                [Fact]
                public void When_Options_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() => ((ICachedOptions) null).AddInMemoryCaching());
                }
            }

            public sealed class Will_Add_Service_In_Singleton_Scope
            {
                [Fact]
                public void When_Valid_Options_Argument_Is_Provided()
                {
                    // Arrange
                    Func<IServiceProvider, InMemoryCacher> createdFactory = null;
                    var optionsMock = new Mock<ICachedOptions>();
                    optionsMock.Setup(o =>
                            o.AddSingletonService<IInMemoryCacher, InMemoryCacher>(
                                It.IsAny<Func<IServiceProvider, InMemoryCacher>>()))
                        .Callback(
                            (Func<IServiceProvider, InMemoryCacher> fetchFactory) => createdFactory = fetchFactory);

                    // Act
                    optionsMock.Object.AddInMemoryCaching();

                    // Assert
                    optionsMock.Verify(
                        o => o.AddSingletonService<IInMemoryCacher, InMemoryCacher>(
                            It.IsAny<Func<IServiceProvider, InMemoryCacher>>()), Times.Once);
                    Assert.NotNull(createdFactory);
                }
            }
        }

        public sealed class AddInMemoryCachedFunctionMethod
        {
            public sealed class Throws_Exception
            {
                [Fact]
                public void When_FetchFactory_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() => new CachedOptionsBuilder()
                        .AddInMemoryCachedFunction<string, string>(p => p, null));
                }

                [Fact]
                public void When_KeyFactory_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() => new CachedOptionsBuilder()
                        .AddInMemoryCachedFunction<string, string>(null, (_, __) => Task.FromResult("")));
                }

                [Fact]
                public void When_Options_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() => ((ICachedOptions) null)
                        .AddInMemoryCachedFunction<string, string>(p => p, (_, __) => Task.FromResult("")));
                }
            }

            public sealed class Will_Add_Service_In_Transient_Scope
            {
                [Fact]
                public void When_Key_And_Fetch_Factories_Are_Provided()
                {
                    // Arrange
                    Func<IServiceProvider, InMemoryCached<string, string>> createdFactory = null;
                    var optionsMock = new Mock<ICachedOptions>();
                    optionsMock.Setup(o =>
                            o.AddTransientService<ICached<string, string>, InMemoryCached<string, string>>(
                                It.IsAny<Func<IServiceProvider, InMemoryCached<string, string>>>()))
                        .Callback((Func<IServiceProvider, InMemoryCached<string, string>> fetchFactory) =>
                            createdFactory = fetchFactory);

                    // Act
                    optionsMock.Object.AddInMemoryCachedFunction<string, string>(arg => arg,
                        (_, __) => Task.FromResult("abc123"));

                    // Assert
                    optionsMock.Verify(
                        o => o.AddTransientService<ICached<string, string>, InMemoryCached<string, string>>(
                            It.IsAny<Func<IServiceProvider, InMemoryCached<string, string>>>()), Times.Once);
                    Assert.NotNull(createdFactory);
                }
            }
        }
    }
}