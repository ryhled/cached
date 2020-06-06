namespace Cached.Tests.Net
{
    using System;
    using System.Linq;
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

            public class AddsCacherService
            {
                [Fact]
                public void And_Resolves_It_Using_Correct_Interface()
                {
                    // Arrange
                    var services = new ServiceCollection();

                    // Act
                    services.AddCached(options
                        => options.AddCacher(provider => new Mock<IInMemoryCacher>().Object));
                    ServiceProvider serviceProvider = services.BuildServiceProvider();
                    var instance = serviceProvider.GetService<IInMemoryCacher>();

                    // Assert
                    Assert.Single(services);
                    Assert.NotNull(instance);

                    // Teardown
                    serviceProvider.Dispose();
                }
            }
        }
    }
}