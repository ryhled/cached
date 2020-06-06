namespace Cached.Tests.Net
{
    using System;
    using Cached.Caching;
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

            [Fact]
            public void Setup_Cached_Based_On_Provided_Services()
            {
                // Arrange
                var services = new ServiceCollection();

                // Act
                services.AddCached(options
                    => options.TryAddSingleton(provider => new Mock<ICacher>().Object));
                ServiceProvider serviceProvider = services.BuildServiceProvider();
                var instance = serviceProvider.GetService<ICacher>();

                // Assert
                Assert.Single(services);
                Assert.NotNull(instance);

                // Teardown
                serviceProvider.Dispose();
            }
        }
    }
}