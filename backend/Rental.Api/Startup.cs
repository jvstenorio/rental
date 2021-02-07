using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rental.Api.Middlewares;
using Rental.Domain.Errors;
using Rental.Domain.Mappers;
using System;
using System.IO;
using System.Linq;

namespace Rental.Api
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
            services.AddAutoMapper(new Type[]
            {
                typeof(MapperProfile)
            });

            services.AddElasticsearch(Configuration);
            services.AddApplications();
            services.AddRepositories(Configuration);

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Rental",
                    Description = "API - Rental",
                    Version = "v1"
                });

                var apiPath = Path.Combine(AppContext.BaseDirectory, "Rental.Api.xml");

                c.IncludeXmlComments(apiPath);
            });

            services.Configure<ApiBehaviorOptions>(o =>
            {
                o.InvalidModelStateResponseFactory = actionContext =>
                new BadRequestObjectResult(GetErrorFromModelState(actionContext.ModelState));
            });

            services.AddControllers();

            services.Configure<ApiBehaviorOptions>(o =>
            {
                o.InvalidModelStateResponseFactory = actionContext =>
                new BadRequestObjectResult(GetErrorFromModelState(actionContext.ModelState));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UsePathBase("/app");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = string.Empty;
                c.SwaggerEndpoint("/app/swagger/v1/swagger.json", "API Rental");
                
            });

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private ErrorModel GetErrorFromModelState(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
        {

            string errors = string.Empty;
            foreach (var state in modelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    var errorMessage = state.Value.Errors
                        .Select(e => e.ErrorMessage)
                        .Aggregate((acc, e) => $"{acc}\n{e}");
                    errors = errors + errorMessage;
                }
            }

            return new ErrorModel
            {
                Message = errors
            };

        }
    }
}
