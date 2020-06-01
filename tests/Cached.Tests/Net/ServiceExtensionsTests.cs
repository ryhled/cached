namespace Cached.Tests.Net
{
    using System;
    using Cached.Net;
    using Caching;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;

    public sealed class ServiceExtensionsTests
    {
        public sealed class AddCachedMethod
        {
            public sealed class Throws
            {
                [Fact]
                public void If_No_CacheService_Is_Configured()
                {
                    Assert.Throws<InvalidOperationException>(() =>
                        new Mock<IServiceCollection>().Object.AddCached(options => { }));
                }

                [Fact]
                public void If_Services_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() => ((IServiceCollection) null).AddCached(options => { }));
                }

                [Fact]
                public void If_Option_Argument_Is_Null()
                {
                    Assert.Throws<ArgumentNullException>(() => new Mock<IServiceCollection>().Object.AddCached(null));
                }
            }

            public sealed class AddsCachedAndService
            {
                [Fact]
                public void When_Service_Is_Configured()
                {
                    // Arrange
                    var services = new ServiceCollection();

                    // Act
                    services.AddCached(options =>
                        options.AddTransientService<ITestCacher, TestCacher>(provider => new TestCacher()));

                    // Assert
                    Assert.True(services.Count == 1);
                }
            }

            private interface ITestCacher
            {
            }

            private class TestCacher : ICachedService, ITestCacher
            {
            }
        }
    }
}