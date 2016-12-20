using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebCache
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = "Sample";
                options.Configuration = "localhost";
            });

            services.AddSession();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                    {
                        string path = context.File.PhysicalPath;
                        if (path.EndsWith(".css") || path.EndsWith(".js") 
                            || path.EndsWith(".gif") || path.EndsWith(".jpg") 
                            || path.EndsWith(".png") || path.EndsWith(".svg")
                            || path.EndsWith(".ico"))
                        {
                            context.Context.Response.Headers["Cache-Control"] =
                                            "private, max-age=43200";

                            context.Context.Response.Headers["Expires"] =
                                    DateTime.UtcNow.AddHours(12).ToString("R");
                        }
                    }
            });

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
