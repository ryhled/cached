namespace Cached.Tests.Configuration
{
    using System;
    using Cached.Caching;
    using Cached.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;

    public class CachedOptionsBuilderTests
    {
        public class BuildMethod
        {
            public class Throws
            {
                [Fact]
                public void If_Services_Argument_Is_Null()
                {
                    // Arrange
                    var builder = new CachedOptionsBuilder();
                    
                    // Act, Assert
                    Assert.Throws<ArgumentNullException>(
                        () => builder.Build(null));
                }

                [Fact]
                public void If_No_Cached_Services_Are_Configured()
                {
                    // Arrange
                    var builder = new CachedOptionsBuilder();

                    // Act, Assert
                    Assert.Throws<InvalidOperationException>(
                        () => builder.Build(new Mock<IServiceCollection>().Object));
                }
            }

            public class Builds
            {
                [Fact]
                public void Expected_Service()
                {
                    // Arrange
                    var services = new ServiceCollection();
                    var builder = new CachedOptionsBuilder();
                    builder.AddService(new FakeBuilder());

                    // Act
                    builder.Build(services);
                    ServiceProvider provider = services.BuildServiceProvider();

                    // Assert
                    Assert.Equal("cached service", (string)provider.GetService<object>());
                }

                [Fact]
                public void Without_Removing_PreExisting_Service()
                {
                    // Arrange
                    var services = new ServiceCollection();
                    services.AddScoped<string, string>(provider => "preconditioned");
                    var builder = new CachedOptionsBuilder();
                    builder.AddService(new FakeBuilder());

                    // Act
                    builder.Build(services);

                    // Assert
                    Assert.Equal(2, services.Count);
                }
            }
        }

        internal class FakeBuilder : ServiceBuilder<ICacheProvider>
        {
            internal override Action<IServiceCollection> GetBuild()
            {
                return services => services.AddSingleton<object, object>(provider => "cached service");
            }
        }
    }
}
