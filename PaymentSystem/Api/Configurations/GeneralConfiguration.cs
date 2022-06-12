using Application;
using DAL;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;

namespace API.Configurations
{
    public static class GeneralConfiguration
    {
        public static IServiceCollection ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.Conventions.Add(new VersionByNamespaceConvention());
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });
            return services;
        }

        public static IServiceCollection ConfigureHeaders(this IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            return services;
        }

        public static IServiceCollection ConfigureDependancies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationLayer();
            services.AddDataAccessLayer(configuration.GetConnectionString("PaymentSystem"));
            return services;
        }
    }
}
