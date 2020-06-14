namespace Cached.Tests.Integration.Memory
{
    using System.Threading.Tasks;
    using Caching;
    using MemoryCache;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public sealed class InMemoryCachedIntegrationTests
    {
        public InMemoryCachedIntegrationTests()
        {
            var services = new ServiceCollection();
            services.AddCached(config =>
            {
                config.AddMemoryCaching(options =>
                {
                    options.AddFunction<string, int>(
                        arg => arg.ToString(),
                        (provider, key, arg) => Task.FromResult("Value for key " + key));
                });
            });

            ServiceProvider scope = services.BuildServiceProvider();
            _inMemoryCached = scope.GetService<ICached<string, int>>();
        }

        private readonly ICached<string, int> _inMemoryCached;

        [Fact]
        public async Task InMemoryCached_Will_Fetch_Or_Cache_Based_On_Key()
        {
            // Arrange, Act
            var result1 = await _inMemoryCached.GetOrFetchAsync(1);
            var result2 = await _inMemoryCached.GetOrFetchAsync(2);
            var result3 = await _inMemoryCached.GetOrFetchAsync(1);
            var result4 = await _inMemoryCached.GetOrFetchAsync(2);

            // Assert
            Assert.Equal("Value for key 1", result1);
            Assert.Equal("Value for key 2", result2);
            Assert.Equal("Value for key 1", result3);
            Assert.Equal("Value for key 2", result4);
        }
    }
}