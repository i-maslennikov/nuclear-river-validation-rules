using System.Collections.Generic;
using System.Globalization;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NuClear.ValidationRules.WebApp.Configuration;
using NuClear.ValidationRules.WebApp.DataAccess;

namespace NuClear.ValidationRules.WebApp
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
            // Add framework services.
            services.AddSingleton<DataConnectionFactory>();
            services.AddScoped<OrderRepositiory>();
            services.AddScoped<UserRepositiory>();
            services.AddScoped<ProjectRepositiory>();
            services.AddScoped<QueryingClient>();

            services.AddOptions();
            services.Configure<LinkFactorySettings>(Configuration.GetSection("LinkFactorySettings"));
            services.Configure<ConnectionStringSettings>(Configuration.GetSection("ConnectionStringSettings"));
            services.Configure<QueryingHostSettings>(Configuration.GetSection("QueryingHostSettings"));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug(LogLevel.Trace);

            app.UseRequestLocalization(new RequestLocalizationOptions
                                           {
                                               DefaultRequestCulture = new RequestCulture("ru-RU"),
                                               SupportedCultures = new List<CultureInfo> { new CultureInfo("ru-RU") },
                                               SupportedUICultures = new List<CultureInfo> { new CultureInfo("ru-RU") },
                                           });

            app.UseDeveloperExceptionPage();
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
            }
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
