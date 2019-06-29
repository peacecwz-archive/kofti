using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Kofti.Manager.Data;
using Kofti.Manager.Infrastructure;
using Kofti.Models;
using Kofti.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kofti.Manager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<KoftiOptions>(Configuration.GetSection("Kofti"));

            services.AddDbContext<KoftiDbContext>(
                options => { options.UseNpgsql(Configuration.GetConnectionString("DbConnection")); },
                ServiceLifetime.Singleton);

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new CustomViewLocator());
            });
            services.AddAutoMapper();
            services
                .AddKofti()
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.InitKoftiServer(async message => await LoadConfig(app, message));

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        async Task LoadConfig(IApplicationBuilder app, KoftiInitMessage message)
        {
            //TODO (peacecwz): Refactor here for pretty codes
            var configService = app.ApplicationServices.GetRequiredService<IConfigService>();
            var dbContext = app.ApplicationServices.GetRequiredService<KoftiDbContext>();
            var configs = await dbContext.Configs
                .Include(x => x.Application)
                .Where(enttiy =>
                    enttiy.IsActive && !enttiy.IsDeleted && enttiy.Application.Name == message.ApplicationName)
                .ToListAsync();

            await configService.PublishAsync(message.ApplicationName,
                configs.ToDictionary(x => x.Key, x => (object) x.Value));
        }
    }
}