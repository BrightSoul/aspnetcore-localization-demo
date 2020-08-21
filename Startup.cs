using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Routing;
using AspnetcoreLocalizationDemo.Models.Localization;

namespace AspnetcoreLocalizationDemo
{
    public class Startup
    {

        // Elenco delle Culture supportate
        private readonly List<CultureInfo> supportedCultures = new List<CultureInfo> {
            new CultureInfo("en"),
            new CultureInfo("it"),
            new CultureInfo("fr")
        };
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //Come da articolo su https://www.aspitalia.com/script/1333/Usare-File-Risorse-ASP.NET-Core.aspx
            services.AddLocalization(options =>
            {
                //Indichiamo la cartella in cui si trovano i nostri file resx
                options.ResourcesPath = "Resources";
            });
            services.AddTransient<IStringLocalizer, ResourceBasedLocalizer>();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(supportedCultures[0].TwoLetterISOLanguageName);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                // Aggiungo questo culture provider che determinerà la Culture corrente in base al frammento "language"
                // presente nel route pattern (vedi la chiamata a endpoints.MapControllerRoute che si trova in questo file, più in basso)
                options.RequestCultureProviders.Insert(0, new RouteRequestCultureProvider(supportedCultures));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            // ASP.NET Core selezionerà la Culture per la richiesta corrente in base a dei RequestCultureProvider.
            // Vedi /Models/Localization/RouteRequestCultureProvider.cs
            app.UseRequestLocalization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{language=en}/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
