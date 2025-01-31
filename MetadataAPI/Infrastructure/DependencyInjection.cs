using MetadataAPI.Application.Interfaces;
using MetadataAPI.Application.Queries.GetClinicalTrialById;
using MetadataAPI.Infrastructure.Interfaces;
using MetadataAPI.Infrastructure.Persistent;
using MetadataAPI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace MetadataAPI.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());


            return services;
        }
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetClinicalTrialByIdQueryHandler).Assembly));
            services.AddScoped<IJsonSchemaValidator, JsonSchemaValidator>();

            return services;
        }
    }
}
