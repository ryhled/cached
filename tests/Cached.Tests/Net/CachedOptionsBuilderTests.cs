namespace Cached.Tests.Net
{
    using System;
    using Cached.Net;
    using Configuration;
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
                    var config = new CachedOptionsBuilder();

                    // Act, Assert
                    Assert.Throws<InvalidOperationException>(() => config.Build(serviceCollectionMock.Object));
                }

                [Fact]
                public void When_no_service_collection_is_null()
                {
                    // Arrange
                    var config = new CachedOptionsBuilder();

                    // Act, Assert
                    Assert.Throws<ArgumentNullException>(() => config.Build(null));
                }
            }

            public sealed class
                BuildsCorrectly // TODO: Extract descriptors from the servicecollectionmock and ensure that what goes in actually comes out.
            {
                [Fact]
                public void Configurator_only_add_identical_service_once()
                {
                    // Arrange
                    var globalSettings = new CachedSettings(TimeSpan.FromDays(3));
                    var options = new CachedOptionsBuilder {GlobalSettings = globalSettings};
                    var serviceCollection = new ServiceCollection();
                    options.AddTransientService<TestClass, TestClass>(_ => new TestClass {MyProperty = "abc123"});
                    options.AddTransientService<TestClass, TestClass>(_ => new TestClass {MyProperty = "abc123"});

                    // Act
                    options.Build(serviceCollection);

                    // Assert
                    Assert.Single(serviceCollection);
                }

                [Fact]
                public void When_global_configuration_is_provided()
                {
                    // Arrange
                    var globalSettings = new CachedSettings(TimeSpan.FromDays(3));
                    var options = new CachedOptionsBuilder {GlobalSettings = globalSettings};
                    var serviceCollection = new ServiceCollection();
                    options.AddSingletonService<TestClass, TestClass>(_ => new TestClass {MyProperty = "abc123"});

                    // Act
                    options.Build(serviceCollection);

                    // Assert
                    Assert.Single(serviceCollection);
                    Assert.Equal(globalSettings, options.GlobalSettings);
                }

                [Fact]
                public void When_no_settings_are_provided()
                {
                    // Arrange
                    var options = new CachedOptionsBuilder();
                    var serviceCollection = new ServiceCollection();
                    options.AddTransientService<TestClass, TestClass>(_ => new TestClass {MyProperty = "abc123"});

                    // Act
                    options.Build(serviceCollection);

                    // Assert
                    Assert.Single(serviceCollection);
                }
            }
        }
    }
}