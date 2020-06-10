namespace Cached.Tests.Configuration
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Cached.Caching;
    using Cached.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public abstract class ServiceBuilderTestsBase<TProvider> where TProvider : ICacheProvider
    {
        protected readonly ServiceBuilder<TProvider> ServiceBuilder;

        protected ServiceBuilderTestsBase(ServiceBuilder<TProvider> serviceBuilder)
        {
            ServiceBuilder = serviceBuilder;
        }

        [Fact]
        public void AddFunctionMethod_Throws_If_KeyFactory_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ServiceBuilder.AddFunction<string, string>(
                    null, 
                    (provider, key, arg) => Task.FromResult("")));
        }

        [Fact]
        public void AddFunctionMethod_Throws_If_FetchFactory_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ServiceBuilder.AddFunction<string, string>(
                    arg => "",
                    null));
        }

        [Fact]
        public void AddFunctionMethod_Does_Not_Clear_PreExisting_Services()
        {
            // Arrange
            ServiceBuilder.AddFunction<string, string>(
                arg => "",
                (provider, key, arg) => Task.FromResult(""));
            var services = new ServiceCollection();
            services.AddSingleton<object, object>(provider => "123");

            // Act
            ServiceBuilder.GetBuild()(services);

            // Assert
            Assert.Equal("123", (string)services.BuildServiceProvider().GetService<object>());
        }
    }
}
