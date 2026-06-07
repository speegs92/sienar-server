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
	[ExcludeFromCodeCoverage]
	public static IServiceCollection AddSienarCoreUtilities(this IServiceCollection self)
	{
		self.TryAddScoped(typeof(IStatusActor<>), typeof(DefaultStatusActor<>));
		self.TryAddScoped(typeof(IGeneralActor<,>), typeof(DefaultGeneralActor<,>));
		self.TryAddScoped(typeof(IResultActor<>), typeof(DefaultResultActor<>));
		self.TryAddScoped(typeof(IAccessValidationRunner<>), typeof(DefaultAccessValidationRunner<>));
		self.TryAddScoped(typeof(IStateValidationRunner<>), typeof(DefaultStateValidationRunner<>));
		self.TryAddScoped(typeof(IBeforeActionRunner<,>), typeof(DefaultBeforeActionRunner<,>));
		self.TryAddScoped(typeof(IAfterActionRunner<,>), typeof(DefaultAfterActionRunner<,>));
		self.TryAddScoped<IMenuGenerator, DefaultMenuGenerator>();
		self.TryAddScoped<IBotDetector, DefaultBotDetector>();
		self.TryAddScoped(typeof(IMapper<,>), typeof(DefaultMapper<,>));
		self.TryAddScoped<IOperationResultNotifier, DefaultOperationResultNotifier>();

		return self;
	}

	/// <summary>
	/// Adds Sienar Blazor utilities to the DI contaziner
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddSienarBlazorUtilities(this IServiceCollection self)
	{
		self.TryAddScoped<SienarAuthenticationStateProvider>();
		self.TryAddScoped<AuthenticationStateProvider>(
			sp => sp.GetRequiredService<SienarAuthenticationStateProvider>());
		self.TryAddScoped<IDelegateHandler, DefaultDelegateHandler>();
		self.TryAddScoped<IScheduler, DefaultScheduler>();

		return self;
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
	/// <typeparam name="TValidator">the validator implementation</typeparam>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddAccessValidator<TValidator, TRequest>(this IServiceCollection self)
		where TValidator : class, IAccessValidator<TRequest>
		=> self.AddScoped<IAccessValidator<TRequest>, TValidator>();

	/// <summary>
	/// Adds a state validator for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TValidator">the validator implementation</typeparam>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddStateValidator<TValidator, TRequest>(this IServiceCollection self)
		where TValidator : class, IStateValidator<TRequest>
		=> self.AddScoped<IStateValidator<TRequest>, TValidator>();

	/// <summary>
	/// Adds an before-create hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddBeforeCreateActionHook<THook, TEntity>(this IServiceCollection self)
		where THook : class, IBeforeCreateAction<TEntity>
		where TEntity : IEntity
		=> self.AddScoped<IBeforeCreateAction<TEntity>, THook>();

	/// <summary>
	/// Adds an before-update hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddBeforeUpdateActionHook<THook, TEntity>(this IServiceCollection self)
		where THook : class, IBeforeUpdateAction<TEntity>
		where TEntity : IEntity
		=> self.AddScoped<IBeforeUpdateAction<TEntity>, THook>();

	/// <summary>
	/// Adds an before-delete hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddBeforeDeleteActionHook<THook, TEntity>(this IServiceCollection self)
		where THook : class, IBeforeDeleteAction<TEntity>
		where TEntity : IEntity
		=> self.AddScoped<IBeforeDeleteAction<TEntity>, THook>();

	/// <summary>
	/// Adds an before general action hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TRequest">The request type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddBeforeGeneralActionHook<THook, TRequest>(
		this IServiceCollection self)
		where THook : class, IBeforeGeneralAction<TRequest>
		where TRequest : IRequest
		=> self.AddScoped<IBeforeGeneralAction<TRequest>, THook>();

	/// <summary>
	/// Adds an before status action hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TRequest">The request type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddBeforeStatusActionHook<THook, TRequest>(
		this IServiceCollection self)
		where THook : class, IBeforeStatusAction<TRequest>
		where TRequest : IRequest
		=> self.AddScoped<IBeforeStatusAction<TRequest>, THook>();

	/// <summary>
	/// Adds an after-read hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterReadActionHook<THook, TEntity>(this IServiceCollection self)
		where THook : class, IAfterReadAction<TEntity>
		where TEntity : IEntity
		=> self.AddScoped<IAfterReadAction<TEntity>, THook>();

	/// <summary>
	/// Adds an after-read-all hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterReadAllActionHook<THook, TEntity>(this IServiceCollection self)
		where THook : class, IAfterReadAllAction<TEntity>
		where TEntity : IEntity
		=> self.AddScoped<IAfterReadAllAction<TEntity>, THook>();

	/// <summary>
	/// Adds an after-create hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterCreateActionHook<THook, TEntity>(
		this IServiceCollection self)
		where THook : class, IAfterCreateAction<TEntity>
		where TEntity : IEntity
		=> self.AddScoped<IAfterCreateAction<TEntity>, THook>();

	/// <summary>
	/// Adds an after-update hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterUpdateActionHook<THook, TEntity>(this IServiceCollection self)
		where THook : class, IAfterUpdateAction<TEntity>
		where TEntity : IEntity
		=> self.AddScoped<IAfterUpdateAction<TEntity>, THook>();

	/// <summary>
	/// Adds an after-delete hook for the given <c>TEntity</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TEntity">The entity type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterDeleteActionHook<THook, TEntity>(this IServiceCollection self)
		where THook : class, IAfterDeleteAction<TEntity>
		where TEntity : IEntity
		=> self.AddScoped<IAfterDeleteAction<TEntity>, THook>();

	/// <summary>
	/// Adds an after general action hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TRequest">The request type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterGeneralActionHook<THook, TRequest>(this IServiceCollection self)
		where THook : class, IAfterGeneralAction<TRequest>
		where TRequest : IRequest
		=> self.AddScoped<IAfterGeneralAction<TRequest>, THook>();

	/// <summary>
	/// Adds an after status action hook for the given <c>TRequest</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TRequest">The request type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterStatusActionHook<THook, TRequest>(this IServiceCollection self)
		where THook : class, IAfterStatusAction<TRequest>
		where TRequest : IRequest
		=> self.AddScoped<IAfterStatusAction<TRequest>, THook>();

	/// <summary>
	/// Adds an after result action hook for the given <c>TResult</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <typeparam name="THook">The hook implementation</typeparam>
	/// <typeparam name="TResult">The result type</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddAfterResultActionHook<THook, TResult>(this IServiceCollection self)
		where THook : class, IAfterResultAction<TResult>
		where TResult : IResult
		=> self.AddScoped<IAfterResultAction<TResult>, THook>();

	/// <summary>
	/// Adds a general processor
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="TResult">the data type of the result</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddGeneralProcessor<TProcessor, TRequest, TResult>(this IServiceCollection self)
		where TProcessor : class, IGeneralProcessor<TRequest, TResult>
		where TRequest : IRequest
		where TResult : IResult
		=> self.AddScoped<IGeneralProcessor<TRequest, TResult>, TProcessor>();

	/// <summary>
	/// Adds a general processor
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <typeparam name="TResult">the data type of the result</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection TryAddGeneralProcessor<TProcessor, TRequest, TResult>(this IServiceCollection self)
		where TProcessor : class, IGeneralProcessor<TRequest, TResult>
		where TRequest : IRequest
		where TResult : IResult
	{
		self.TryAddScoped<IGeneralProcessor<TRequest, TResult>, TProcessor>();
		return self;
	}

	/// <summary>
	/// Adds a status processor (<c>IProcessor&lt;TRequest, bool&gt;</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddStatusProcessor<TProcessor, TRequest>(this IServiceCollection self)
		where TProcessor : class, IStatusProcessor<TRequest>
		where TRequest : IRequest
		=> self.AddScoped<IStatusProcessor<TRequest>, TProcessor>();

	/// <summary>
	/// Adds a status processor (<c>IProcessor&lt;TRequest, bool&gt;</c>
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <typeparam name="TRequest">the data type of the request</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection TryAddStatusProcessor<TProcessor, TRequest>(this IServiceCollection self)
		where TProcessor : class, IStatusProcessor<TRequest>
		where TRequest : IRequest
	{
		self.TryAddScoped<IStatusProcessor<TRequest>, TProcessor>();
		return self;
	}


	/// <summary>
	/// Adds a result processor (<c>IProcessor&lt;TRequest&gt;</c>)
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <typeparam name="TResult">the data type of the result</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddResultProcessor<TProcessor, TResult>(this IServiceCollection self)
		where TProcessor : class, IResultProcessor<TResult>
		where TResult : IResult
		=> self.AddScoped<IResultProcessor<TResult>, TProcessor>();

	/// <summary>
	/// Adds a result processor (<c>IProcessor&lt;TRequest&gt;</c>)
	/// </summary>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <typeparam name="TResult">the data type of the result</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection TryAddResultProcessor<TProcessor, TResult>(this IServiceCollection self)
		where TProcessor : class, IResultProcessor<TResult>
		where TResult : IResult
	{
		self.TryAddScoped<IResultProcessor<TResult>, TProcessor>();
		return self;
	}

	/// <summary>
	/// Adds a startup processor to the startup service collection
	/// </summary>
	/// <remarks>
	/// This method should only be used when registering startup services. The registered <c>IStatusProcessor&lt;Startup&gt;</c> implementations are only requested once, at app startup. As such, this should only be used within a call to <see cref="SienarAppBuilder.AddStartupServices"/> or the <see cref="IConfigurer{TOptions}.Configure"/> method of a configuration class referenced by an <see cref="AppConfigurerAttribute"/>.
	/// </remarks>
	/// <param name="self">the service collection</param>
	/// <typeparam name="TProcessor">the processor implementation</typeparam>
	/// <returns>the service collection</returns>
	public static IServiceCollection AddStartupProcessor<TProcessor>(this IServiceCollection self)
		where TProcessor : class, IStatusProcessor<Startup>
		=> self.AddSingleton<IStatusProcessor<Startup>, TProcessor>();
}