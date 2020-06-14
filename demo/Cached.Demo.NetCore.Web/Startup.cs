namespace Cached.Demo.NetCore.Web
{
    using System;
    using MemoryCache;
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
                options.AddMemoryCaching(builder =>
                {
                    builder.Options.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15);
                    builder.AddFunction<string, int>(
                        param => param.ToString(),
                        (provider, key, arg) => provider.GetService<IFakeService>().FunctionGet(key, arg)
                    );
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