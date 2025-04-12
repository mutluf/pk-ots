using System.Reflection;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Ots.Api.Impl.Cqrs;
using Ots.Api.Impl.Service;
using Ots.Api.Impl.Validation;
using Ots.Api.Mapper;
using Ots.Api.Middleware;
using Ots.Base;

namespace Ots.Api;

public class Startup
{
    public IConfiguration Configuration { get; }
    public static JwtConfig JwtConfig { get; private set; }
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        JwtConfig = Configuration.GetSection("JwtConfig").Get<JwtConfig>();
        services.AddSingleton<JwtConfig>(JwtConfig);

        services.AddControllers().AddFluentValidation(x =>
        {
            x.RegisterValidatorsFromAssemblyContaining<CustomerValidator>();
        });

        services.AddSingleton(new MapperConfiguration(x => x.AddProfile(new MapperConfig())).CreateMapper());

        services.AddSwaggerGen();
        services.AddDbContext<OtsDbContext>(options =>
        {
            options.UseSqlServer(Configuration.GetConnectionString("MsSqlConnection"));
        });

        services.AddMediatR(x => x.RegisterServicesFromAssemblies(typeof(CreateCustomerCommand).GetTypeInfo().Assembly));

        services.AddScoped<ScopedService>();
        services.AddTransient<TransientService>();
        services.AddSingleton<SingletonService>();

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITokenService, TokenService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<HeartBeatMiddleware>();
        app.UseMiddleware<ErrorHandlerMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        app.Use((context, next) =>
        {
            if (!string.IsNullOrEmpty(context.Request.Path) && context.Request.Path.Value.Contains("favicon"))
            {
                return next();
            }
            var singletenService = context.RequestServices.GetRequiredService<SingletonService>();
            var scopedService = context.RequestServices.GetRequiredService<ScopedService>();
            var transientService = context.RequestServices.GetRequiredService<TransientService>();

            singletenService.Counter++;
            scopedService.Counter++;
            transientService.Counter++;

            return next();
        });
        app.Run(async context =>
        {
            var singletenService = context.RequestServices.GetRequiredService<SingletonService>();
            var scopedService = context.RequestServices.GetRequiredService<ScopedService>();
            var transientService = context.RequestServices.GetRequiredService<TransientService>();

            if (!string.IsNullOrEmpty(context.Request.Path) && !context.Request.Path.Value.Contains("favicon"))
            {
                singletenService.Counter++;
                scopedService.Counter++;
                transientService.Counter++;
            }

            await context.Response.WriteAsync($"SingletonService: {singletenService.Counter}\n");
            await context.Response.WriteAsync($"TransientService: {transientService.Counter}\n");
            await context.Response.WriteAsync($"ScopedService: {scopedService.Counter}\n");
        });

    }
}