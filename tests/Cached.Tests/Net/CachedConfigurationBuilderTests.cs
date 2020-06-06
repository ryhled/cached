﻿namespace Cached.Tests.Net
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Cached.Caching;
    using Cached.Net;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;

    public class CachedConfigurationBuilderTests
    {
        public class BuildMethod
        {
            public class TestClass : ICached<string, string>
            {
                public string MyProperty { get; set; }

                public Task<string> GetOrFetchAsync(string arg)
                {
                    return Task.FromResult(string.Empty);
                }
            }

            public class ThrowsException
            {
                [Fact]
                public void When_No_Configurations_Are_Provided()
                {
                    // Arrange
                    var serviceCollectionMock = new Mock<IServiceCollection>();
                    var config = new CachedConfigurationBuilder();

                    // Act, Assert
                    Assert.Throws<InvalidOperationException>(() => config.Build(serviceCollectionMock.Object));
                }

                [Fact]
                public void When_No_Service_Collection_Is_Null()
                {
                    // Arrange
                    var config = new CachedConfigurationBuilder();

                    // Act, Assert
                    Assert.Throws<ArgumentNullException>(() => config.Build(null));
                }
            }

            public class Builds
            {
                [Fact]
                public void When_Configuring_Singleton_Service()
                {
                    // Arrange
                    var options = new CachedConfigurationBuilder();
                    var serviceCollection = new ServiceCollection();
                    options.TryAddTransient(_ => new TestClass {MyProperty = "abc123"});
                    options.TryAddTransient(_ => new TestClass {MyProperty = "cde321"});

                    // Act
                    options.Build(serviceCollection);

                    // Assert
                    Assert.Single(serviceCollection);
                    Assert.Equal(nameof(TestClass), serviceCollection.First().ServiceType.Name);

                    var fetchedObject = (TestClass) serviceCollection.Single()
                        .ImplementationFactory(new Mock<IServiceProvider>().Object);
                    Assert.Equal("abc123", fetchedObject.MyProperty);
                }

                [Fact]
                public void When_Configuring_Transient_Service()
                {
                    // Arrange
                    var options = new CachedConfigurationBuilder();
                    var serviceCollection = new ServiceCollection();
                    options.TryAddTransient(_ => new TestClass {MyProperty = "abc123"});

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