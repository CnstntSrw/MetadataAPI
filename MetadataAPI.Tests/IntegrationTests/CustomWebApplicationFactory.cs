using Microsoft.AspNetCore.Hosting;

namespace MetadataAPI.Tests.IntegrationTests
{
    using MetadataAPI.Infrastructure.Persistent;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private ApplicationDbContext _db;
        private IServiceScope _scope;

        public ApplicationDbContext Db
        {
            get => _db;
            set => _db = value;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing database registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                // Load real connection string from appsettings.json
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = config.GetConnectionString("LocalConnection");

                // Register PostgreSQL DB for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(connectionString));

                // Ensure database is created before running tests
                var provider = services.BuildServiceProvider();
                _scope = provider.CreateScope();
                _db = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                _db.Database.EnsureCreated();
            });
        }
        public void Dispose()
        {
            _scope?.Dispose();
        }
    }

}
