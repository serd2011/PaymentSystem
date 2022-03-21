using Microsoft.EntityFrameworkCore;

namespace API.Configurations
{
    public static class ConnectionConfiguration
    {
        public static IServiceCollection ConfigureConnection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<API.Infrastructure.Database.Context>(
                options => {
                    var connetionString = configuration.GetConnectionString("PaymentSystem");
                    options.UseMySql(connetionString, ServerVersion.AutoDetect(connetionString));
                });
            return services;
        }
    }
}
