using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using DatingApp.API.Helpers;
using AutoMapper;
using DatingApp.API;
using DatingApp.API.Registry;

namespace DatingApp.API
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
            //adicionando o Entityframework
            services.AddDbContext<DataContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("ConexaoPrincipal")));

            //Adicionando CORS:
            services.AddCors();

            //adicionando as propriedades do appsettings.json para o CloudnarySettings.
            //vale lembrar q é preciso ter a classe no mesmo formato para "deserializar"!
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

            //adicionando AutoMapper;
            services.AddAutoMapper(typeof(DatingRepository).Assembly);

            services.AddControllers();//.AddNewtonsoftJson();

            //adicionando DY
            // services.AddScoped<IAuthRepository, AuthRepository>();
            // services.AddScoped<IDatingRepository, DatingRepository>();
            services.RegistryIoC();
            //Registry.RegistryIoC(services);

            /*
            resolvendo o erro abaixo:
             "System.Text.Json.JsonException: A possible object cycle was detected which is not supported. This can either be due to a cycle or if the object depth is larger than the maximum allowed depth of 32."
            */

            services.AddMvc()                
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(opt => {
                    opt.SerializerSettings.ReferenceLoopHandling = 
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            //adicionando geração do token JWT com IdentityModel.Tokens.Jwt
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                ClockSkew = TimeSpan.Zero
            });

            //adicionando Health Check:
            services.AddHealthChecks();

            //adicionando o Filter que criamos para setar o LastActivity do usuario
            services.AddScoped<LoginUserActivity>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //configurando o Global Exception Handler no .NET Core - faremos isso também na Liquidez WMS!!!
                app.UseExceptionHandler(builder => {
                    builder.Run(async context => 
                    {
                        context.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if(error != null)
                        {
                            context.Response.AdicionarErroAplicacao(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            //usando o CORS:
            app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            //database first para EntityFrameWork
            //context.Database.EnsureCreated();
           

            //usando o Health Check
            app.UseHealthChecks("/check");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
