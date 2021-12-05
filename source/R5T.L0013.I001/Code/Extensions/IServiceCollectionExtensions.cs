using System;

using Microsoft.Extensions.DependencyInjection;

using R5T.T0063;


namespace R5T.L0013.I001
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IServiceCollection"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddServiceCollection(this IServiceCollection services)
        {
            services.AddSingleton<IServiceCollection>(_ => services);

            return services;
        }
    }
}