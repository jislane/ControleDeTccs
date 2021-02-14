using KissLog;
using KissLog.AspNetCore;
using KissLog.CloudListeners.Auth;
using KissLog.CloudListeners.RequestLogsListener;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SistemaDeControleDeTCCs.Data;
using SistemaDeControleDeTCCs.Extentions.Filters;
using SistemaDeControleDeTCCs.Models;
using SistemaDeControleDeTCCs.Services;
using System.Net;
using System.Net.Mail;
using ILogger = KissLog.ILogger;

namespace SistemaDeControleDeTCCs
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

            services.AddControllersWithViews();

            services.AddDbContext<SistemaDeControleDeTCCsContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("SistemaDeControleDeTCCsContextConnection")));

            services.AddIdentity<Usuario, IdentityRole>().AddEntityFrameworkStores<SistemaDeControleDeTCCsContext>().AddDefaultTokenProviders();
            services.AddRazorPages();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Professor",
                    builder => builder.RequireRole("Professor"));
                options.AddPolicy("Coordenador",
                    builder => builder.RequireRole("Coordenador"));
                options.AddPolicy("Administrador",
                    builder => builder.RequireRole("Administrador"));
                options.AddPolicy("Aluno",
                    builder => builder.RequireRole("Aluno"));
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            services.AddScoped<PopularBancoDados>();

            services.AddScoped<SmtpClient>(options =>
            {
                SmtpClient smtp = new SmtpClient()
                {
                    Host = Configuration.GetValue<string>("Email:ServerSMTP"),
                    Port = Configuration.GetValue<int>("Email:ServerPort"),
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(Configuration.GetValue<string>("Email:Username"), Configuration.GetValue<string>("Email:Password")),
                    EnableSsl = true
                };

                return smtp;
            });

            services.AddScoped<SenderEmail>();


            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(AuditoriaILoggerFilter));
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ILogger>((context) =>
            {
                return Logger.Factory.Get();
            });
            services.AddScoped<AuditoriaILoggerFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, PopularBancoDados popularBanco)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                popularBanco.Popular();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                //app.UseExceptionHandler("/Logger/Index");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseKissLogMiddleware(options =>
            {

                options.Listeners.Add(new RequestLogsApiListener(new Application(
                 Configuration["KissLog.OrganizationId"],    //  "b1c2c0c7-8ff6-4b34-af84-f3b76edacb4c"
                 Configuration["KissLog.ApplicationId"])     //  "33162e72-dc5b-4ff7-9690-1e8d4a31f868"
             )
                {
                    ApiUrl = Configuration["KissLog.ApiUrl"]    //  "https://api.kisslog.net"
                });
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
