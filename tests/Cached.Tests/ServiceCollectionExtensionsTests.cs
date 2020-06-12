namespace Cached.Tests
{
    using System;
    using Cached.Caching;
    using Cached.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;

    public class ServiceCollectionExtensionsTests
    {
        public class AddCachedMethod
        {
            public class Throws
            {
                [Fact]
                public void If_No_Services_Are_Configured()
                {
                    Assert.Throws<InvalidOperationException>(() =>
                        new Mock<IServiceCollection>().Object.AddCached(options => { }));
                }

                [Fact]
                public void If_Options_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() =>
                        new Mock<IServiceCollection>().Object.AddCached(null));
                }

                [Fact]
                public void If_Services_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(
                        () => ((IServiceCollection) null).AddCached(options => { }));
                }
            }

            [Fact]
            public void Adds_Cached()
            {
                // Arrange
                var services = new ServiceCollection();

                // Act
                services.AddCached(options => options.AddService(new FakeServiceBuilder()));
                ServiceProvider provider = services.BuildServiceProvider();

                // Assert
                Assert.Single(services);
                Assert.IsType<string>(provider.GetService<string>());
            }
        }
    }

    public class FakeServiceBuilder : ServiceBuilder<ICacheProvider>
    {
        internal override Action<IServiceCollection> GetBuild()
        {
            return services => { services.AddTransient(provider => ""); };
        }
    }
}