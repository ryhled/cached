namespace Cached.Tests.Memory
{
    using System;
    using System.Threading.Tasks;
    using Cached.Caching;
    using Cached.Configuration;
    using Cached.Memory;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;

    public class CachedOptionsExtensionsTests
    {
        public class AddMemoryCachingMethod
        {
            public class Throws
            {
                [Fact]
                public void If_Options_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() =>
                        ((ICachedOptions) null).AddMemoryCaching(builder => { }));
                }
            }

            [Fact]
            public void Adds_Service()
            {
                // Arrange
                var optionsMock = new Mock<ICachedOptions>();

                // Act
                optionsMock.Object.AddMemoryCaching();

                // Assert
                optionsMock.Verify(m => m.AddService(It.IsAny<ServiceBuilder<IMemory>>()), Times.Once);
            }

            [Fact]
            public void Adds_Service_With_Cached_Service()
            {
                // Arrange
                var optionsMock = new Mock<ICachedOptions>();
                var services = new ServiceCollection();
                optionsMock.Setup(m => m.AddService(It.IsAny<ServiceBuilder<IMemory>>()))
                    .Callback((ServiceBuilder<IMemory> builder) => builder.GetBuild()(services));

                // Act
                optionsMock.Object.AddMemoryCaching(
                    builder => builder.AddFunction<string, double>(
                        _ => "",
                        (_, __, ___) => Task.FromResult("")));
                ServiceProvider provider = services.BuildServiceProvider();

                // Assert
                optionsMock.Verify(m => m.AddService(It.IsAny<ServiceBuilder<IMemory>>()), Times.Once);
                Assert.NotNull(provider.GetService<ICached<string, double>>());
            }
        }
    }
}