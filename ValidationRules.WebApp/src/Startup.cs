using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NuClear.ValidationRules.WebApp.Configuration;
using NuClear.ValidationRules.WebApp.DataAccess;
using NuClear.ValidationRules.WebApp.Serializers;

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

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddSingleton<DataConnectionFactory>();
            services.AddSingleton<LinkFactory>();
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
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseDeveloperExceptionPage();
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
            }
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //}

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseRequestLocalization(new RequestLocalizationOptions {DefaultRequestCulture = new RequestCulture("ru")});

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
