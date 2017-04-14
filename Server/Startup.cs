using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using T2Stats.Models;
using T2Stats.Services;
using T2Stats.Middleware;
using Microsoft.Extensions.Options;

namespace T2Stats
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.default.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<ApplicationDbContext>(options => 
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<EventIngestionService>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Apply migrations
            app.ApplicationServices.GetRequiredService<ApplicationDbContext>().Database.Migrate();

            // Add authentication
            // This isn't really the right way.
            // See http://stackoverflow.com/questions/31687955/authorizing-a-user-depending-on-the-action-name/31688792#31688792
            app.UseMiddleware<TribesNextAuthenticationMiddleware>(
                new OptionsWrapper<TribesNextAuthenticationMiddlewareOptions>(
                    new TribesNextAuthenticationMiddlewareOptions() {
                        AuthenticationServerExponent = Configuration["TribesNextAuthentication:AuthenticationServerExponent"],
                        AuthenticationServerModulus = Configuration["TribesNextAuthentication:AuthenticationServerModulus"]
                    }
                )
            );

            // Add MVC / file serving
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
