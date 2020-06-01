namespace Cached.Tests.Net
{
    using System;
    using System.Linq;
    using Cached.Net;
    using Caching;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;

    public sealed class CachedOptionsBuilderTests
    {
        public sealed class BuildMethod
        {
            public class TestClass : ICachedService
            {
                public string MyProperty { get; set; }
            }

            public sealed class ThrowsException
            {
                [Fact]
                public void When_no_configurations_are_provided()
                {
                    // Arrange
                    var serviceCollectionMock = new Mock<IServiceCollection>();
                    var config = new CachedConfigurationBuilder();

                    // Act, Assert
                    Assert.Throws<InvalidOperationException>(() => config.Build(serviceCollectionMock.Object));
                }

                [Fact]
                public void When_no_service_collection_is_null()
                {
                    // Arrange
                    var config = new CachedConfigurationBuilder();

                    // Act, Assert
                    Assert.Throws<ArgumentNullException>(() => config.Build(null));
                }
            }

            public sealed class BuildsCorrectly
            {
                [Fact]
                public void When_Configuring_Singleton_Service()
                {
                    // Arrange
                    var options = new CachedConfigurationBuilder();
                    var serviceCollection = new ServiceCollection();
                    options.AddTransientService<TestClass, TestClass>(_ => new TestClass {MyProperty = "abc123"});
                    options.AddTransientService<TestClass, TestClass>(_ => new TestClass {MyProperty = "cde321"});

                    // Act
                    options.Build(serviceCollection);

                    // Assert
                    Assert.Single(serviceCollection);
                    Assert.Equal(nameof(TestClass), serviceCollection.First().ServiceType.Name);

                    var fetchedObject = (TestClass) serviceCollection.First()
                        .ImplementationFactory(new Mock<IServiceProvider>().Object);
                    Assert.Equal("abc123", fetchedObject.MyProperty);
                }

                [Fact]
                public void When_Configuring_Transient_Service()
                {
                    // Arrange
                    var options = new CachedConfigurationBuilder();
                    var serviceCollection = new ServiceCollection();
                    options.AddTransientService<TestClass, TestClass>(_ => new TestClass {MyProperty = "abc123"});

                    // Act
                    options.Build(serviceCollection);

                    // Assert
                    Assert.Single(serviceCollection);
                    Assert.Equal(nameof(TestClass), serviceCollection.First().ServiceType.Name);
                }
            }
        }
    }
}