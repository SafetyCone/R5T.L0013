using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;


namespace System
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Establishes that <see cref="ServiceLifetime.Transient"/> is the default.
        /// </summary>
        /// <remarks>
        /// The only consideration when choosing the lifetime for a service is an evaluation of the state associated with the service. If the service's state should last for the application's duration, then singleton. If the state should not last any longer than the service instance, then transient.
        /// This makes transient the safest with regards to the only thing that matters: state.
        /// This is why transient is the default, and any decision to make state longer-lasting must be make consciously.
        /// While the constant newing-up of transient instances does cost some performance, it is minimal relative to the cost of debugging service lifetime errors.
        /// Additionally, a service with a lower lifetime (transient), if injected into a service with a higher lifetime (singleton) will mean that the singleton's transient instance gets upgraded to a singleton instance since it is captured by the singleton.
        /// Thus singletons should only have singleton dependencies, but transients could have transient, scoped, and singleton dependencies. Thus choosing a transient lifetime also avoids needing to debug captured-instance errors.
        /// </remarks>
        public static IServiceCollection AddWithDefaultLifetime<TDefinition, TImplementation>(this IServiceCollection services)
            where TDefinition : class
            where TImplementation : class, TDefinition
        {
            services.AddTransient<TDefinition, TImplementation>();

            return services;
        }

        /// <summary>
        /// Establishes that <see cref="ServiceLifetime.Transient"/> is the default.
        /// </summary>
        /// <remarks>
        /// See <see cref="AddWithDefaultLifetime{TDefinition, TImplementation}(IServiceCollection)"/> for remarks.
        /// </remarks>
        public static IServiceCollection AddWithDefaultLifetime<TImplementation>(this IServiceCollection services)
            where TImplementation : class
        {
            services.AddTransient<TImplementation>();

            return services;
        }

        /// <summary>
        /// Gets an instance using dependency injection.
        /// </summary>
        /// <remarks>
        /// Note that <see cref="ServiceProvider"/> is <see cref="IDisposable"/>, and if any of the service implementation types (not definition types, but implementation types) require disposal, the service provider will dispose them.
        /// The service provider built in this method will be disposed. To dispose of the service provider yourself, use <see cref="GetAddedInstanceWithoutServiceProviderDisposal{TDefinition, TImplementation}(IServiceCollection, out TDefinition)"/>.
        /// Because the returned instance escapes the disposal scope, if any of its service dependency implementations are disposable, they will be disposed by the disposal of the service prover. If the service itself is disposable, it will be disposed. Both of these situations will lead to errors.
        /// </remarks>
        public static TDefinition GetAddedInstanceWithServiceProviderDisposal<TDefinition, TImplementation>(this IServiceCollection services)
            where TDefinition : class
            where TImplementation : class, TDefinition
        {
            services.AddWithDefaultLifetime<TDefinition, TImplementation>();

            using var serviceProvider = services.BuildServiceProvider();

            var instance = serviceProvider.GetRequiredService<TDefinition>();
            return instance;
        }

        /// <summary>
        /// Gets an instance using dependency injection.
        /// </summary>
        /// <remarks>
        /// See <see cref=" GetAddedInstanceWithServiceProviderDisposal{TDefinition, TImplementation}(IServiceCollection)"/> for remarks.
        /// </remarks>
        public static TImplementation GetAddedInstanceWithServiceProviderDisposal<TImplementation>(this IServiceCollection services)
            where TImplementation : class
        {
            services.AddWithDefaultLifetime<TImplementation>();

            using var serviceProvider = services.BuildServiceProvider();

            var instance = serviceProvider.GetRequiredService<TImplementation>();
            return instance;
        }

        /// <summary>
        /// Gets an instance using dependency injection.
        /// </summary>
        /// <remarks>
        /// Note that <see cref="ServiceProvider"/> is <see cref="IDisposable"/>, the service provider is returned to allow the caller to choose when to dispose it.
        /// This method makes it clear that the instance, or its service dependency implementations, are disposable they will not be disposed until the service provider is disposed.
        /// </remarks>
        public static ServiceProvider GetAddedInstanceWithoutServiceProviderDisposal<TDefinition, TImplementation>(this IServiceCollection services,
            out TDefinition instance)
            where TDefinition : class
            where TImplementation : class, TDefinition
        {
            services.AddWithDefaultLifetime<TDefinition, TImplementation>();

            var serviceProvider = services.BuildServiceProvider();

            instance = serviceProvider.GetRequiredService<TDefinition>();

            return serviceProvider;
        }

        public static async Task<ServiceProvider> BuildServiceProvider(this Task<IServiceCollection> gettingServiceCollection)
        {
            var serviceCollection = await gettingServiceCollection;

            var output = serviceCollection.BuildServiceProvider();
            return output;
        }

        /// <summary>
        /// Gets an instance using dependency injection.
        /// </summary>
        /// <remarks>
        /// See <see cref="GetAddedInstanceWithoutServiceProviderDisposal{TDefinition, TImplementation}(IServiceCollection, out TDefinition)"/> for remarks.
        /// </remarks>
        public static ServiceProvider GetAddedInstanceWithoutServiceProviderDisposal<TImplementation>(this IServiceCollection services,
            out TImplementation instance)
            where TImplementation : class
        {
            services.AddWithDefaultLifetime<TImplementation>();

            var serviceProvider = services.BuildServiceProvider();

            instance = serviceProvider.GetRequiredService<TImplementation>();

            return serviceProvider;
        }

        public static ServiceProvider GetServiceInstance<TService>(this IServiceCollection services,
            out TService instance)
            where TService : class
        {
            var serviceProvider = services.BuildServiceProvider();

            instance = serviceProvider.GetRequiredService<TService>();

            return serviceProvider;
        }

        /// <summary>
        /// Chooses <see cref="GetAddedInstanceWithoutServiceProviderDisposal{TDefinition, TImplementation}(IServiceCollection, out TDefinition)"/> as the default.
        /// The default was chosen to communicate service dependency disposal safety.
        /// </summary>
        public static ServiceProvider GetAddedInstance<TDefinition, TImplementation>(this IServiceCollection services,
            out TDefinition instance)
            where TDefinition : class
            where TImplementation : class, TDefinition
        {
            return services.GetAddedInstanceWithoutServiceProviderDisposal<TDefinition, TImplementation>(out instance);
        }

        /// <summary>
        /// Chooses <see cref="GetAddedInstanceWithoutServiceProviderDisposal{TDefinition, TImplementation}(IServiceCollection, out TDefinition)"/> as the default.
        /// The default was chosen to communicate service dependency disposal safety.
        /// </summary>
        public static ServiceProvider GetAddedInstance<TImplementation>(this IServiceCollection services,
            out TImplementation instance)
            where TImplementation : class
        {
            return services.GetAddedInstanceWithoutServiceProviderDisposal(out instance);
        }

        /// <remarks>
        /// -Async name chosen as more natural over making the synchronous name -Synchronous.
        /// </remarks>
        public static void UseAddedInstance<TDefinition, TImplementation>(this IServiceCollection services,
            Action<TDefinition> action)
            where TDefinition : class
            where TImplementation : class, TDefinition
        {
            using var serviceProvider = services.GetAddedInstanceWithoutServiceProviderDisposal<TDefinition, TImplementation>(out var instance);

            action(instance);
        }

        /// <remarks>
        /// -Async name chosen as more natural over making the synchronous name -Synchronous.
        /// </remarks>
        public static void UseAddedInstance<TImplementation>(this IServiceCollection services,
            Action<TImplementation> action)
            where TImplementation : class
        {
            using var serviceProvider = services.GetAddedInstanceWithoutServiceProviderDisposal<TImplementation>(out var instance);

            action(instance);
        }

        /// <remarks>
        /// -Async name chosen as more natural over making the synchronous name -Synchronous.
        /// </remarks>
        public static TOut UseAddedInstance<TDefinition, TImplementation, TOut>(this IServiceCollection services,
            Func<TDefinition, TOut> function)
            where TDefinition : class
            where TImplementation : class, TDefinition
        {
            using var serviceProvider = services.GetAddedInstanceWithoutServiceProviderDisposal<TDefinition, TImplementation>(out var instance);

            var output = function(instance);
            return output;
        }

        /// <remarks>
        /// -Async name chosen as more natural over making the synchronous name -Synchronous.
        /// </remarks>
        public static TOut UseAddedInstance<TImplementation, TOut>(this IServiceCollection services,
            Func<TImplementation, TOut> function)
            where TImplementation : class
        {
            using var serviceProvider = services.GetAddedInstanceWithoutServiceProviderDisposal<TImplementation>(out var instance);

            var output = function(instance);
            return output;
        }

        /// <remarks>
        /// -Async name chosen as more natural over making the synchronous name -Synchronous.
        /// </remarks>
        public static async Task UseAddedInstanceAsync<TDefinition, TImplementation>(this IServiceCollection services,
            Func<TDefinition, Task> action)
            where TDefinition : class
            where TImplementation : class, TDefinition
        {
            using var serviceProvider = services.GetAddedInstanceWithoutServiceProviderDisposal<TDefinition, TImplementation>(out var instance);

            await action(instance);
        }

        /// <remarks>
        /// -Async name chosen as more natural over making the synchronous name -Synchronous.
        /// </remarks>
        public static async Task UseAddedInstanceAsync<TImplementation>(this IServiceCollection services,
            Func<TImplementation, Task> action)
            where TImplementation : class
        {
            using var serviceProvider = services.GetAddedInstanceWithoutServiceProviderDisposal<TImplementation>(out var instance);

            await action(instance);
        }

        /// <remarks>
        /// -Async name chosen as more natural over making the synchronous name -Synchronous.
        /// </remarks>
        public static async Task<TOut> UseAddedInstanceAsync<TDefinition, TImplementation, TOut>(this IServiceCollection services,
            Func<TDefinition, Task<TOut>> function)
            where TDefinition : class
            where TImplementation : class, TDefinition
        {
            using var serviceProvider = services.GetAddedInstanceWithoutServiceProviderDisposal<TDefinition, TImplementation>(out var instance);

            var output = await function(instance);
            return output;
        }

        /// <remarks>
        /// -Async name chosen as more natural over making the synchronous name -Synchronous.
        /// </remarks>
        public static async Task<TOut> UseAddedInstanceAsync<TImplementation, TOut>(this IServiceCollection services,
            Func<TImplementation, Task<TOut>> function)
            where TImplementation : class
        {
            using var serviceProvider = services.GetAddedInstanceWithoutServiceProviderDisposal<TImplementation>(out var instance);

            var output = await function(instance);
            return output;
        }
    }
}