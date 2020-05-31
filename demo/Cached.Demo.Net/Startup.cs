namespace Cached.Demo.Net
{
    using System;
    using System.Threading.Tasks;
    using Cached.Net;
    using Configuration;
    using InMemory;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCached(options =>
            {
                options.GlobalSettings =
                    new CachedSettings(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(20));
                options.AddInMemoryCaching();
                options.AddInMemoryCachedFunction<string, string>(param =>
                        param,
                    async (_, arg) =>
                    {
                        await Task.Delay(500);
                        return DateTime.Now + $" [cached function for: {arg}]";
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