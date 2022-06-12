using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public static class DALDependencyInjection
    {
        public static void AddDataAccessLayer(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<Context>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
        }
    }
}
