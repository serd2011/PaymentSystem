using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ApplicationDependencyInjection
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddScoped<Services.IUserService, Services.Impl.UserService>();
            services.AddScoped<Services.IPaymentsService, Services.Impl.PaymentsService>();
        }
    }
}
