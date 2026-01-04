namespace Sienar.Extensions;

/// <summary>
/// Contains <see cref="IServiceCollection"/> extension methods for the <c>Sienar.Utils</c> assembly
/// </summary>
public static class SienarUtilsServiceCollectionExtensions
{
	/// <summary>
	/// Adds universal Sienar utilities to the DI container
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="appType">the application type</param>
	/// <returns>the service collection</returns>
	[ExcludeFromCodeCoverage]
	public static IServiceCollection AddSienarCoreUtilities(
		this IServiceCollection self,
		ApplicationType appType)
	{
		self.TryAddScoped(typeof(IStatusActor<>), typeof(DefaultStatusActor<>), appType);
		self.TryAddScoped(typeof(IGeneralActor<,>), typeof(DefaultGeneralActor<,>), appType);
		self.TryAddScoped(typeof(IResultActor<>), typeof(DefaultResultActor<>), appType);
		self.TryAddScoped(typeof(IAccessValidationRunner<>), typeof(DefaultAccessValidationRunner<>), appType);
		self.TryAddScoped(typeof(IStateValidationRunner<>), typeof(DefaultStateValidationRunner<>), appType);
		self.TryAddScoped(typeof(IBeforeActionRunner<,>), typeof(DefaultBeforeActionRunner<,>), appType);
		self.TryAddScoped(typeof(IAfterActionRunner<,>), typeof(DefaultAfterActionRunner<,>), appType);
		self.TryAddScoped<IMenuGenerator, DefaultMenuGenerator>();
		self.TryAddScoped<IBotDetector, DefaultBotDetector>(appType);
		self.TryAddScoped(typeof(IMapper<,>), typeof(DefaultMapper<,>));
		self.TryAddScoped<IOperationResultNotifier, DefaultOperationResultNotifier>();

		return self.AddStatusProcessor<StartupProcessor, Startup>(appType);
	}

	/// <summary>
	/// Checks if a <c>TOptions</c> has already been configured, and if not, adds the supplied default configuration
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="config">the default configuration to apply if no existing configuration was found</param>
	/// <typeparam name="TOptions">the type of the options class to configure</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection ApplyDefaultConfiguration<TOptions>(
		this IServiceCollection self,
		IConfiguration config)
		where TOptions : class
	{
		if (!self.Any(sd => sd.ServiceType == typeof(IConfigureOptions<TOptions>)))
		{
			self.Configure<TOptions>(config);
		}

		return self;
	}

	/// <summary>
	/// Adds a configurer of type <c>IConfigurer&lt;TOptions&gt;</c> for the given <c>TOptions</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TConfigurer">the type of the configurer</typeparam>
	/// <typeparam name="TOptions">the type of the options class to configure</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddConfigurer<TConfigurer, TOptions>(this IServiceCollection self)
		where TConfigurer : class, IConfigurer<TOptions>
		where TOptions : class
		=> self.AddScoped<IConfigurer<TOptions>, TConfigurer>();

	/// <summary>
	/// Adds a configurer of type <c>IConfigurer&lt;TOptions&gt;</c> for the given <c>TOptions</c> if one hasn't already been registered
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TConfigurer">the type of the configurer</typeparam>
	/// <typeparam name="TOptions">the type of the options class to configure</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection TryAddConfigurer<TConfigurer, TOptions>(this IServiceCollection self)
		where TConfigurer : class, IConfigurer<TOptions>
		where TOptions : class
	{
		self.TryAddScoped<IConfigurer<TOptions>, TConfigurer>();
		return self;
	}

	/// <summary>
	/// Adds an access validator for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="TValidator">the validator implementation</typeparam>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddAccessValidator<TValidator, TRequest>(
		this IServiceCollection self,
		ApplicationType appType)
		where TValidator : class, IAccessValidator<TRequest>
		=> AddScoped<IAccessValidator<TRequest>, TValidator>(self, appType);

	/// <summary>
	/// Adds a state validator for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="TValidator">the validator implementation</typeparam>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddStateValidator<TValidator, TRequest>(
		this IServiceCollection self,
		ApplicationType appType)
		where TValidator : class, IStateValidator<TRequest>
		=> AddScoped<IStateValidator<TRequest>, TValidator>(self, appType);

	/// <summary>
	/// Adds an before-create hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddBeforeCreateActionHook<THook, TEntity>(
		this IServiceCollection self,
		ApplicationType appType)
		where THook : class, IBeforeCreateAction<TEntity>
		where TEntity : IEntity
		=> AddScoped<IBeforeCreateAction<TEntity>, THook>(self, appType);

	/// <summary>
	/// Adds an before-update hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddBeforeUpdateActionHook<THook, TEntity>(
		this IServiceCollection self,
		ApplicationType appType)
		where THook : class, IBeforeUpdateAction<TEntity>
		where TEntity : IEntity
		=> AddScoped<IBeforeUpdateAction<TEntity>, THook>(self, appType);

	/// <summary>
	/// Adds an before-delete hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddBeforeDeleteActionHook<THook, TEntity>(
		this IServiceCollection self,
		ApplicationType appType)
		where THook : class, IBeforeDeleteAction<TEntity>
		where TEntity : IEntity
		=> AddScoped<IBeforeDeleteAction<TEntity>, THook>(self, appType);

	/// <summary>
	/// Adds an before general action hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TRequest">The request type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddBeforeGeneralActionHook<THook, TRequest>(
		this IServiceCollection self,
		ApplicationType appType)
		where THook : class, IBeforeGeneralAction<TRequest>
		where TRequest : IRequest
		=> AddScoped<IBeforeGeneralAction<TRequest>, THook>(self, appType);

	/// <summary>
	/// Adds an before status action hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TRequest">The request type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddBeforeStatusActionHook<THook, TRequest>(
		this IServiceCollection self,
		ApplicationType appType)
		where THook : class, IBeforeStatusAction<TRequest>
		where TRequest : IRequest
		=> AddScoped<IBeforeStatusAction<TRequest>, THook>(self, appType);

	/// <summary>
	/// Adds an after-read hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterReadActionHook<THook, TEntity>(
		this IServiceCollection self,
		ApplicationType appType)
		where THook : class, IAfterReadAction<TEntity>
		where TEntity : IEntity
		=> AddScoped<IAfterReadAction<TEntity>, THook>(self, appType);

	/// <summary>
	/// Adds an after-read-all hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterReadAllActionHook<THook, TEntity>(
		this IServiceCollection self,
		ApplicationType appType)
		where THook : class, IAfterReadAllAction<TEntity>
		where TEntity : IEntity
		=> AddScoped<IAfterReadAllAction<TEntity>, THook>(self, appType);

	/// <summary>
	/// Adds an after-create hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterCreateActionHook<THook, TEntity>(
		this IServiceCollection self,
		ApplicationType appType)
		where THook : class, IAfterCreateAction<TEntity>
		where TEntity : IEntity
		=> AddScoped<IAfterCreateAction<TEntity>, THook>(self, appType);

	/// <summary>
	/// Adds an after-update hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterUpdateActionHook<THook, TEntity>(
		this IServiceCollection self,
		ApplicationType appType)
		where THook : class, IAfterUpdateAction<TEntity>
		where TEntity : IEntity
		=> AddScoped<IAfterUpdateAction<TEntity>, THook>(self, appType);

	/// <summary>
	/// Adds an after-delete hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterDeleteActionHook<THook, TEntity>(
		this IServiceCollection self,
		ApplicationType appType)
		where THook : class, IAfterDeleteAction<TEntity>
		where TEntity : IEntity
		=> AddScoped<IAfterDeleteAction<TEntity>, THook>(self, appType);

	/// <summary>
	/// Adds an after general action hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TRequest">The request type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterGeneralActionHook<THook, TRequest>(
		this IServiceCollection self,
		ApplicationType appType)
		where THook : class, IAfterGeneralAction<TRequest>
		where TRequest : IRequest
		=> AddScoped<IAfterGeneralAction<TRequest>, THook>(self, appType);

	/// <summary>
	/// Adds an after status action hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TRequest">The request type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterStatusActionHook<THook, TRequest>(
		this IServiceCollection self,
		ApplicationType appType)
		where THook : class, IAfterStatusAction<TRequest>
		where TRequest : IRequest
		=> AddScoped<IAfterStatusAction<TRequest>, THook>(self, appType);

	/// <summary>
	/// Adds an after result action hook for the given <c>TResult</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TResult">The result type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterResultActionHook<THook, TResult>(
		this IServiceCollection self,
		ApplicationType appType)
		where THook : class, IAfterResultAction<TResult>
		where TResult : IResult
		=> AddScoped<IAfterResultAction<TResult>, THook>(self, appType);

	/// <summary>
	/// Adds a general processor
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="TResult">the data type of the result</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddGeneralProcessor<TProcessor, TRequest, TResult>(
		this IServiceCollection self,
		ApplicationType appType)
		where TProcessor : class, IGeneralProcessor<TRequest, TResult>
		where TRequest : IRequest
		where TResult : IResult
		=> AddScoped<IGeneralProcessor<TRequest, TResult>, TProcessor>(self, appType);

	/// <summary>
	/// Adds a general processor
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="TResult">the data type of the result</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection TryAddGeneralProcessor<TProcessor, TRequest, TResult>(
		this IServiceCollection self,
		ApplicationType appType)
		where TProcessor : class, IGeneralProcessor<TRequest, TResult>
		where TRequest : IRequest
		where TResult : IResult
		=> TryAddScoped<IGeneralProcessor<TRequest, TResult>, TProcessor>(self, appType);

	/// <summary>
	/// Adds a status processor (<c>IProcessor&lt;TRequest, bool&gt;</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddStatusProcessor<TProcessor, TRequest>(
		this IServiceCollection self,
		ApplicationType appType)
		where TProcessor : class, IStatusProcessor<TRequest>
		where TRequest : IRequest
		=> AddScoped<IStatusProcessor<TRequest>, TProcessor>(self, appType);

	/// <summary>
	/// Adds a status processor (<c>IProcessor&lt;TRequest, bool&gt;</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection TryAddStatusProcessor<TProcessor, TRequest>(
		this IServiceCollection self,
		ApplicationType appType)
		where TProcessor : class, IStatusProcessor<TRequest>
		where TRequest : IRequest
		=> TryAddScoped<IStatusProcessor<TRequest>, TProcessor>(self, appType);

	/// <summary>
	/// Adds a result processor (<c>IProcessor&lt;TRequest&gt;</c>)
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <typeparam name="TResult">the data type of the result</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddResultProcessor<TProcessor, TResult>(
		this IServiceCollection self,
		ApplicationType appType)
		where TProcessor : class, IResultProcessor<TResult>
		where TResult : IResult
		=> AddScoped<IResultProcessor<TResult>, TProcessor>(self, appType);

	/// <summary>
	/// Adds a result processor (<c>IProcessor&lt;TRequest&gt;</c>)
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <param name="appType">the application type</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <typeparam name="TResult">the data type of the result</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection TryAddResultProcessor<TProcessor, TResult>(
		this IServiceCollection self,
		ApplicationType appType)
		where TProcessor : class, IResultProcessor<TResult>
		where TResult : IResult
		=> TryAddScoped<IResultProcessor<TResult>, TProcessor>(self, appType);

	/// <summary>
	/// Adds a scoped service in a way that supports the application type of the app registering the service
	/// </summary>
	/// <remarks>
	/// This method inspects the <see cref="ApplicationType"/> and adds the given service in a way which is closest to the idea of a scoped-lifetime service for the given <see cref="ApplicationType"/>. An <see cref="ApplicationType.Client">ApplicationType.Client</see>, for example, will add such a service as <see cref="ServiceLifetime.Transient">ServiceLifetime.Transient</see> because Blazor WASM does not functionally distinguish between scoped- and singleton-lifetime services.
	/// </remarks>
	/// <param name="self">The service collection</param>
	/// <param name="appType">The application type</param>
	/// <typeparam name="TService">The type of the service</typeparam>
	/// <typeparam name="TImplementation">The type of the service's implementation</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddScoped<TService, TImplementation>(
		this IServiceCollection self,
		ApplicationType appType)
		where TImplementation : class, TService
		=> AddScoped(self, typeof(TService), typeof(TImplementation), appType);

	/// <summary>
	/// Adds a scoped service in a way that supports the application type of the app registering the service
	/// </summary>
	/// <remarks>
	/// This method inspects the <see cref="ApplicationType"/> and adds the given service in a way which is closest to the idea of a scoped-lifetime service for the given <see cref="ApplicationType"/>. An <see cref="ApplicationType.Client">ApplicationType.Client</see>, for example, will add such a service as <see cref="ServiceLifetime.Transient">ServiceLifetime.Transient</see> because Blazor WASM does not functionally distinguish between scoped- and singleton-lifetime services.
	/// </remarks>
	/// <param name="self">The service collection</param>
	/// <param name="serviceType">The type of the service</param>
	/// <param name="implementationType">The type of the service's implementation</param>
	/// <param name="appType">The application type</param>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddScoped(
		this IServiceCollection self,
		Type serviceType,
		Type implementationType,
		ApplicationType appType)
	{
		var lifetime = CreateTraditionallyScopedServiceLifetime(appType);

		var descriptor = new ServiceDescriptor(
			serviceType,
			implementationType,
			lifetime);

		self.Add(descriptor);

		return self;
	}

	/// <summary>
	/// Tries to add a scoped service in a way that supports the application type of the app registering the service
	/// </summary>
	/// <remarks>
	/// This method inspects the <see cref="ApplicationType"/> and adds the given service in a way which is closest to the idea of a scoped-lifetime service for the given <see cref="ApplicationType"/>. An <see cref="ApplicationType.Client">ApplicationType.Client</see>, for example, will add such a service as <see cref="ServiceLifetime.Transient">ServiceLifetime.Transient</see> because Blazor WASM does not functionally distinguish between scoped- and singleton-lifetime services.
	/// </remarks>
	/// <param name="self">The service collection</param>
	/// <param name="appType">The application type</param>
	/// <typeparam name="TService">The type of the service</typeparam>
	/// <typeparam name="TImplementation">The type of the service's implementation</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection TryAddScoped<TService, TImplementation>(
		this IServiceCollection self,
		ApplicationType appType)
		=> TryAddScoped(self, typeof(TService), typeof(TImplementation), appType);

	/// <summary>
	/// Tries to add a scoped service in a way that supports the application type of the app registering the service
	/// </summary>
	/// <remarks>
	/// This method inspects the <see cref="ApplicationType"/> and adds the given service in a way which is closest to the idea of a scoped-lifetime service for the given <see cref="ApplicationType"/>. An <see cref="ApplicationType.Client">ApplicationType.Client</see>, for example, will add such a service as <see cref="ServiceLifetime.Transient">ServiceLifetime.Transient</see> because Blazor WASM does not functionally distinguish between scoped- and singleton-lifetime services.
	/// </remarks>
	/// <param name="self">The service collection</param>
	/// <param name="serviceType">The type of the service</param>
	/// <param name="implementationType">The type of the service's implementation</param>
	/// <param name="appType">The application type</param>
	/// <returns>The service collection</returns>
	public static IServiceCollection TryAddScoped(
		this IServiceCollection self,
		Type serviceType,
		Type implementationType,
		ApplicationType appType)
	{
		var lifetime = CreateTraditionallyScopedServiceLifetime(appType);

		var descriptor = new ServiceDescriptor(
			serviceType,
			implementationType,
			lifetime);

		self.TryAdd(descriptor);

		return self;
	}

	private static ServiceLifetime CreateTraditionallyScopedServiceLifetime(ApplicationType appType)
		=> appType switch
		{
			ApplicationType.Client => ServiceLifetime.Transient,
			_ => ServiceLifetime.Scoped
		};
}