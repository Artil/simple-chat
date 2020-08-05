using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ChatDbCore;
using ChatDbCore.Account;
using ChatServer.Hubs;
using ChatServer.Interfaces;
using ChatServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace ChatServer
{
    class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddTransient<IUtility, UtilityService>();
            services.AddTransient<IChat, ChatService>();
            services.AddTransient<IMessage, MessageService>();
            services.AddTransient<IUser, UserService>();

            // ===== Add DbContext ========
            services.AddDbContext<Context>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("ChatDbCore")));

            // ===== Add Identity ========
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<Context>()
            .AddDefaultTokenProviders();

            // ===== Add Jwt Authentication ========
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = Configuration.GetSection("Jwt").GetSection("Issuer").Value,
                        ValidAudience = Configuration.GetSection("Jwt").GetSection("Issuer").Value,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Jwt").GetSection("Key").Value)),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            services.AddCors();
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddSignalR(option => option.MaximumReceiveMessageSize = 100 * 1024 * 1024);
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

            app.UseCors();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}");
            });
        }
    }
}
