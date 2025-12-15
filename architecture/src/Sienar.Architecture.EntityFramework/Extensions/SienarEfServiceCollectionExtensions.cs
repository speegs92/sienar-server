using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Sienar.Extensions;

public static class SienarEfServiceCollectionExtensions
{
	/// <summary>
	/// Registers a <see cref="DbContext"/> as an <see cref="IDbContext"/>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="optionsAction">The options configuration, if any</param>
	/// <typeparam name="TContext">The type of the context</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddDbContextForSienar<TContext>(
		this IServiceCollection self,
		Action<DbContextOptionsBuilder>? optionsAction = null)
		where TContext : DbContext, IDbContext
		=> AddDbContextForSienar<IDbContext, TContext>(self, optionsAction);

	/// <summary>
	/// Registers a <see cref="DbContext"/> as an <see cref="IDbContext"/> and as a <c>TContext</c>
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="optionsAction">The options configuration, if any</param>
	/// <typeparam name="TContext">The type of the context</typeparam>
	/// <typeparam name="TContextImplementation">The implementation type of the context</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddDbContextForSienar<TContext, TContextImplementation>(
		this IServiceCollection self,
		Action<DbContextOptionsBuilder>? optionsAction = null)
		where TContext : IDbContext
		where TContextImplementation : DbContext, TContext
	{
		self.AddDbContext<TContext, TContextImplementation>(optionsAction);

		if (typeof(TContext) != typeof(IDbContext))
		{
			self.AddScoped<IDbContext>(sp => sp.GetRequiredService<TContext>());
		}

		// Add the TContextImplementation to DI as all its interfaces
		// but skip any Microsoft-provided interfaces
		// Because IDbContext is in the Microsoft.EntityFrameworkCore namespace,
		// it will also be skipped here
		var interfaces = typeof(TContextImplementation)
			.GetInterfaces()
			.Where(
				i => i.Namespace is not null &&
				!i.Namespace.StartsWith("Microsoft") &&
				!i.Namespace.StartsWith("System"));

		foreach (var i in interfaces)
		{
			self.AddScoped(i, sp => sp.GetRequiredService<TContext>());
		}

		return self;
	}

	/// <summary>
	/// Adds the necessary services to use an entity via an EF repository
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">The application type</param>
	/// <typeparam name="TEntity">The type of the entity</typeparam>
	/// <typeparam name="TFilterProcessor">The type of the filter processor</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddEfEntity<TEntity, TFilterProcessor>(
		this IServiceCollection self,
		ApplicationType appType)
		where TEntity : EntityBase
		where TFilterProcessor : class, IEfFilterProcessor<TEntity>
	{
		self.AddBeforeCreateActionHook<ConcurrencyStampUpdater<TEntity>, TEntity>(appType);
		self.AddBeforeUpdateActionHook<ConcurrencyStampUpdater<TEntity>, TEntity>(appType);
		self.TryAddScoped<IStateValidator<TEntity>, ConcurrencyStampValidator<TEntity>>();
		self.TryAddScoped<IEfFilterProcessor<TEntity>, TFilterProcessor>();

		return self;
	}

	/// <summary>
	/// Adds the necessary services to use an entity via Entity Framework
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">The application type</param>
	/// <typeparam name="TDto">The type of the DTO</typeparam>
	/// <typeparam name="TDtoToEntityMapper">The type of the DTO-to-entity mapper</typeparam>
	/// <typeparam name="TEntityToDtoMapper">The type of the entity-to-DTO mapper</typeparam>
	/// <typeparam name="TEntity">The type of the entity</typeparam>
	/// <typeparam name="TFilterProcessor">The type of the filter processor</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddEfEntity<
		TDto,
		TDtoToEntityMapper,
		TEntityToDtoMapper,
		TEntity,
		TFilterProcessor>(
		this IServiceCollection self,
		ApplicationType appType)
		where TDto : class, new()
		where TDtoToEntityMapper : class, IMapper<TDto, TEntity>
		where TEntityToDtoMapper : class, IMapper<TEntity, TDto>
		where TEntity : EntityBase, new()
		where TFilterProcessor : class, IEfFilterProcessor<TEntity>
		=> AddEfEntity<TDto, TEntityToDtoMapper, TDto, TDtoToEntityMapper, TDto, TDtoToEntityMapper, TEntity, TFilterProcessor>(self, appType);

	/// <summary>
	/// Adds the necessary services to use an entity via Entity Framework
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <param name="appType">The application type</param>
	/// <typeparam name="TViewDto">The type of the view DTO</typeparam>
	/// <typeparam name="TEntityToViewDtoMapper">The type of the entity-to-view-DTO mapper</typeparam>
	/// <typeparam name="TAddDto">The type of the add DTO</typeparam>
	/// <typeparam name="TAddDtoToEntityMapper">The type of the add-DTO-to-entity mapper</typeparam>
	/// <typeparam name="TEditDto">The type of the edit DTO</typeparam>
	/// <typeparam name="TEditDtoToEntityMapper">The type of the edit-DTO-to-entity mapper</typeparam>
	/// <typeparam name="TEntity">The type of the entity</typeparam>
	/// <typeparam name="TFilterProcessor">The type of the filter processor</typeparam>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddEfEntity<
		TViewDto,
		TEntityToViewDtoMapper,
		TAddDto,
		TAddDtoToEntityMapper,
		TEditDto,
		TEditDtoToEntityMapper,
		TEntity,
		TFilterProcessor>(
		this IServiceCollection self,
		ApplicationType appType)
		where TViewDto : class, new()
		where TEntityToViewDtoMapper : class, IMapper<TEntity, TViewDto>
		where TAddDto : class, new()
		where TAddDtoToEntityMapper : class, IMapper<TAddDto, TEntity>
		where TEditDto : class, new()
		where TEditDtoToEntityMapper : class, IMapper<TEditDto, TEntity>
		where TEntity : EntityBase, new()
		where TFilterProcessor : class, IEfFilterProcessor<TEntity>
	{
		self.TryAddScoped<IMapper<TEntity, TViewDto>, TEntityToViewDtoMapper>();
		self.TryAddScoped<IMapper<TAddDto, TEntity>, TAddDtoToEntityMapper>();

		if (typeof(TEditDtoToEntityMapper) != typeof(TAddDtoToEntityMapper))
		{
			self.TryAddScoped<IMapper<TEditDto, TEntity>, TEditDtoToEntityMapper>();
		}

		self
			.AddBeforeCreateActionHook<ConcurrencyStampUpdater<TEntity>, TEntity>(appType)
			.AddBeforeUpdateActionHook<ConcurrencyStampUpdater<TEntity>, TEntity>(appType);
		self.TryAddScoped<IStateValidator<TEntity>, ConcurrencyStampValidator<TEntity>>();
		self.TryAddScoped<IEfFilterProcessor<TEntity>, TFilterProcessor>();

		return self;
	}

	/// <summary>
	/// Adds the core services necessary for Sienar to work with Entity Framework
	/// </summary>
	/// <param name="self">The service collection</param>
	/// <returns>The service collection</returns>
	public static IServiceCollection AddEntityFramework(this IServiceCollection self)
	{
		self.TryAddScoped(typeof(IEntityReadActor<>), typeof(EfEntityReadActor<>));
		self.TryAddScoped(typeof(IEntityReadAllActor<>), typeof(EfEntityReadAllActor<>));
		self.TryAddScoped(typeof(IEntityCreateActor<>), typeof(EfEntityCreateActor<>));
		self.TryAddScoped(typeof(IEntityUpdateActor<>), typeof(EfEntityUpdateActor<>));
		self.TryAddScoped(typeof(IEntityDeleteActor<>), typeof(EfEntityDeleteActor<>));

		return self;
	}
}