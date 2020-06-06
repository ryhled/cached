namespace Cached.Demo.Net
{
    using Cached.Net;
    using InMemory;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Services;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IFakeService, FakeService>();

            services.AddCached(options =>
            {
                options.AddInMemoryCaching();
                options.AddInMemoryCachedFunction<string, int>(
                    param => param.ToString(), // Generates cache key based on the argument used.
                    (resolver, key, arg) => resolver.GetService<IFakeService>().FunctionGet(key, arg)); // creates the fetch logic for the cached entry.
            });
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapRazorPages());
        }
    }
}