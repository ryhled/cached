namespace Cached.Tests.Net
{
    using System;
    using System.Threading.Tasks;
    using Cached.Caching;
    using Cached.InMemory;
    using Cached.Net;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;

    public class ServiceExtensionsTests
    {
        public class AddCachedMethod
        {
            public class Throws
            {
                [Fact]
                public void If_No_CacheService_Is_Configured()
                {
                    Assert.Throws<InvalidOperationException>(() =>
                        new Mock<IServiceCollection>().Object.AddCached(options => { }));
                }

                [Fact]
                public void If_Option_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() => new Mock<IServiceCollection>().Object.AddCached(null));
                }

                [Fact]
                public void If_Services_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() => ((IServiceCollection) null).AddCached(options => { }));
                }
            }

            public class AddsCacher
            {
                [Fact]
                public void As_ICacher_Interface()
                {
                    // Arrange
                    var services = new ServiceCollection();

                    // Act
                    services.AddCached(options
                        => options.AddCacher(provider => new TestCacher()));


                    // Assert
                    ServiceProvider serviceProvider = services.BuildServiceProvider();
                    var instance = serviceProvider.GetService<ICacher<IInMemory>>();
                    Assert.NotNull(instance);
                    Assert.IsType<TestCacher>(instance);

                    // Teardown
                    serviceProvider.Dispose();
                }
            }

            private class TestCacher : ICacher<IInMemory>
            {
                public Task<TResponse> GetOrFetchAsync<TResponse>(string key,
                    Func<string, Task<TResponse>> fetchFactory)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}