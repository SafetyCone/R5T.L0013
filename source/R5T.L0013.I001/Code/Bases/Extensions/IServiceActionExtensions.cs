using System;

using Microsoft.Extensions.DependencyInjection;

using R5T.T0062;
using R5T.T0063;


namespace R5T.L0013.I001
{
    public static class IServiceActionExtensions
    {
        /// <summary>
        /// Adds the <see cref="IServiceCollection"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<IServiceCollection> AddServiceCollectionAction(this IServiceAction _)
        {
            var serviceAction = _.New<IServiceCollection>(services => services.AddServiceCollection());
            return serviceAction;
        }
    }
}
