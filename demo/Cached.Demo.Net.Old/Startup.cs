namespace Cached.Demo.NetCore31
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Cached.InMemory;
    using Net;
    using System;
    using System.Threading.Tasks;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCached(options =>
            {
                options.AddInMemoryCaching();
                options.AddInMemoryCachedFunction<string, string>(param =>
                    param,
                    async (provider, param) =>
                    {
                        await Task.Delay(2000);
                        return $"{DateTime.Now} (from function cache)";
                    });
            });
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapRazorPages());
        }
    }
}