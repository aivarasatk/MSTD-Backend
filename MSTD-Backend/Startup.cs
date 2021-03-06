using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MSTD_Backend.Filters;
using MSTD_Backend.Interfaces;
using MSTD_Backend.Models.Response;
using MSTD_Backend.Services;
using Newtonsoft.Json.Converters;

namespace MSTD_Backend
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
            InitializeIoc(services);
            services.AddControllers(options =>
            {
                options.Filters.Add(new ProducesAttribute("application/json"));
                options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ResponseMessage), StatusCodes.Status500InternalServerError));
                options.Filters.Add(typeof(GlobalExceptionHandlerFilter));
            })
            .AddNewtonsoftJson(opt =>
            {
                //response formatting
                opt.UseMemberCasing();
                opt.SerializerSettings.Converters.Add(new StringEnumConverter());
            })
            .AddJsonOptions(opt =>
            {
                //these settings are only used in swagger doc
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                opt.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(string.Format("comments.xml", AppDomain.CurrentDomain.BaseDirectory));
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MSTD torrent entries API", Version = "v1" });
            });
        }

        private void InitializeIoc(IServiceCollection services)
        {
            services.AddSingleton<ILeetxSource, LeetxSource>();
            services.AddSingleton<ILeetxParser, LeetxParser>();

            services.AddSingleton<IThePirateBaySource, ThePirateBaySource>();
            services.AddSingleton<IThePirateBayParser, ThePirateBayParser>();

            services.AddSingleton<IKickassSource, KickassSource>();
            services.AddSingleton<IKickassParser, KickassParser>();

            services.AddSingleton<SourcesHelper>();
            services.AddSingleton<IStateCache, StateCache>();

            services.AddHostedService<SiteHealthChecker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(builder => builder.AllowAnyOrigin());

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
        }
    }
}
