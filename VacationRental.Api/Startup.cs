using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Swashbuckle.AspNetCore.Swagger;
using AutoMapper;
using FluentValidation.AspNetCore;

using VacationRental.BLL.Extensions;
using VacationRental.DAL.InMemory.Di;
using VacationRental.Contract.Models;
using VacationRental.Api.Filters;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using VacationRental.BLL.Exceptions;

namespace VacationRental.Api
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
            services.AddMvc(options => options.Filters.Add<ValidationFilter>())
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                        .AddFluentValidation(options => options.RegisterValidatorsFromAssemblyContaining<BookingBindingModel>());
            
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            services.AddInMemoryRepositories();
            services.AddDomainServices();

            services.AddSwaggerGen(opts => opts.SwaggerDoc("v1", new Info { Title = "Vacation rental information", Version = "v1" }));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var exception = context.Features
                                            .Get<IExceptionHandlerPathFeature>()
                                            .Error;
                var response = new { error = exception.Message };
                var result = env.IsDevelopment() 
                                    ? JsonConvert.SerializeObject(new { error = exception.Message, stackTrace = exception.StackTrace }) 
                                    : JsonConvert.SerializeObject(new { error = exception.Message });
                context.Response.ContentType = "application/json";
                switch (exception)
                {
                    case NotFoundExceptionBase _:
                        context.Response.StatusCode = "get".Equals(context.Request.Method, StringComparison.InvariantCultureIgnoreCase) 
                                                                ? StatusCodes.Status404NotFound 
                                                                : StatusCodes.Status400BadRequest;
                        break;
                    case OperationNotAvailableException _:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        break;
                }
                
                await context.Response.WriteAsync(result);
            }));
            
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(opts => opts.SwaggerEndpoint("/swagger/v1/swagger.json", "VacationRental v1"));
        }
    }
}
