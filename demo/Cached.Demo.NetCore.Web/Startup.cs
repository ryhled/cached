namespace Cached.Demo.Net
{
    using System;
    using Memory;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Caching.Memory;
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
                options.AddMemoryCaching(builder =>
                {
                    builder.Options = new MemoryCacheEntryOptions
                        {AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)};
                    builder.AddFunction<string, int>(
                        param => param.ToString(), // Generates cache key based on the argument used.
                        (provider, key, arg) => provider.GetService<IFakeService>().FunctionGet(key, arg)
                    ); // creates the fetch logic for the cached entry.));
                });
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