using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Common.MediatrCustom
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using MediatR.Pipeline;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.DependencyInjection.Extensions;

	public class MediatrCustomServiceConfiguration
	{
		public Type MediatorImplementationType { get; private set; }
		public ServiceLifetime Lifetime { get; private set; }

		public MediatrCustomServiceConfiguration()
		{
			MediatorImplementationType = typeof(MediatR.Mediator);
			Lifetime = ServiceLifetime.Transient;
		}

		public MediatrCustomServiceConfiguration Using<TMediator>() where TMediator : MediatR.IMediator
		{
			MediatorImplementationType = typeof(TMediator);
			return this;
		}

		public MediatrCustomServiceConfiguration AsSingleton()
		{
			Lifetime = ServiceLifetime.Singleton;
			return this;
		}

		public MediatrCustomServiceConfiguration AsScoped()
		{
			Lifetime = ServiceLifetime.Scoped;
			return this;
		}

		public MediatrCustomServiceConfiguration AsTransient()
		{
			Lifetime = ServiceLifetime.Transient;
			return this;
		}
	}

	/// <summary>
	/// Extensions to scan for MediatR handlers and registers them.
	/// - Scans for any handler interface implementations and registers them as <see cref="ServiceLifetime.Transient"/>
	/// - Scans for any <see cref="IRequestPreProcessor{TRequest}"/> and <see cref="IRequestPostProcessor{TRequest,TResponse}"/> implementations and registers them as transient instances
	/// Registers <see cref="ServiceFactory"/> and <see cref="IMediator"/> as transient instances
	/// After calling AddMediatR you can use the container to resolve an <see cref="IMediator"/> instance.
	/// This does not scan for any <see cref="IPipelineBehavior{TRequest,TResponse}"/> instances including <see cref="RequestPreProcessorBehavior{TRequest,TResponse}"/> and <see cref="RequestPreProcessorBehavior{TRequest,TResponse}"/>.
	/// To register behaviors, use the <see cref="ServiceCollectionServiceExtensions.AddTransient(IServiceCollection,Type,Type)"/> with the open generic or closed generic types.
	/// </summary>
	public static class MediatrCustomServiceCollectionExtensions
	{
		/// <summary>
		/// Registers handlers and mediator types from the specified assemblies
		/// </summary>
		/// <param name="services">Service collection</param>
		/// <param name="assemblies">Assemblies to scan</param>        
		/// <returns>Service collection</returns>
		// A -> C
		public static IServiceCollection AddMediatR(this IServiceCollection services, bool mustImplementInterface, Type interfaceToImplement, params Assembly[] assemblies)
			=> services.AddMediatR(assemblies, configuration: null, mustImplementInterface: mustImplementInterface, interfaceToImplement: interfaceToImplement);

		/// <summary>
		/// Registers handlers and mediator types from the specified assemblies
		/// </summary>
		/// <param name="services">Service collection</param>
		/// <param name="assemblies">Assemblies to scan</param>
		/// <param name="configuration">The action used to configure the options</param>
		/// <returns>Service collection</returns>
		// B -> C
		public static IServiceCollection AddMediatR(this IServiceCollection services, Action<MediatrCustomServiceConfiguration> configuration, bool mustImplementInterface, Type interfaceToImplement, params Assembly[] assemblies)
			=> services.AddMediatR(assemblies, configuration, mustImplementInterface: mustImplementInterface, interfaceToImplement: interfaceToImplement);

		/// <summary>
		/// Registers handlers and mediator types from the specified assemblies
		/// </summary>
		/// <param name="services">Service collection</param>
		/// <param name="assemblies">Assemblies to scan</param>
		/// <param name="configuration">The action used to configure the options</param>
		/// <returns>Service collection</returns>
		// C
		public static IServiceCollection AddMediatR(this IServiceCollection services, IEnumerable<Assembly> assemblies, Action<MediatrCustomServiceConfiguration> configuration, bool mustImplementInterface, Type interfaceToImplement)
		{
			if (!assemblies.Any())
			{
				throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for handlers.");
			}
			var serviceConfig = new MediatrCustomServiceConfiguration();

			configuration?.Invoke(serviceConfig);

			MediatrCustomServiceRegistrar.AddRequiredServices(services, serviceConfig);
			MediatrCustomServiceRegistrar.AddMediatRClasses(services, assemblies, mustImplementInterface, interfaceToImplement);

			return services;
		}

		/// <summary>
		/// Registers handlers and mediator types from the assemblies that contain the specified types
		/// </summary>
		/// <param name="services"></param>
		/// <param name="handlerAssemblyMarkerTypes"></param>        
		/// <returns>Service collection</returns>
		// D -> F 
		public static IServiceCollection AddMediatR(this IServiceCollection services, bool mustImplementInterface, Type interfaceToImplement, params Type[] handlerAssemblyMarkerTypes)
			=> services.AddMediatR(handlerAssemblyMarkerTypes, configuration: null, mustImplementInterface: mustImplementInterface, interfaceToImplement: interfaceToImplement);

		/// <summary>
		/// Registers handlers and mediator types from the assemblies that contain the specified types
		/// </summary>
		/// <param name="services"></param>
		/// <param name="handlerAssemblyMarkerTypes"></param>
		/// <param name="configuration">The action used to configure the options</param>
		/// <returns>Service collection</returns>
		// E -> F
		public static IServiceCollection AddMediatR(this IServiceCollection services, Action<MediatrCustomServiceConfiguration> configuration, bool mustImplementInterface, Type interfaceToImplement, params Type[] handlerAssemblyMarkerTypes)
			=> services.AddMediatR(handlerAssemblyMarkerTypes, configuration, mustImplementInterface: mustImplementInterface, interfaceToImplement: interfaceToImplement);

		/// <summary>
		/// Registers handlers and mediator types from the assemblies that contain the specified types
		/// </summary>
		/// <param name="services"></param>
		/// <param name="handlerAssemblyMarkerTypes"></param>
		/// <param name="configuration">The action used to configure the options</param>
		/// <returns>Service collection</returns>
		// F -> C
		public static IServiceCollection AddMediatR(this IServiceCollection services, IEnumerable<Type> handlerAssemblyMarkerTypes, Action<MediatrCustomServiceConfiguration> configuration, bool mustImplementInterface, Type interfaceToImplement)
			=> services.AddMediatR(handlerAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly), configuration, mustImplementInterface: mustImplementInterface, interfaceToImplement: interfaceToImplement);
	}

	public static class MediatrCustomServiceRegistrar
	{
		public static void AddMediatRClasses(IServiceCollection services, IEnumerable<Assembly> assembliesToScan,
			bool mustImplementInterface,
			Type interfaceToImplement)
		{
			assembliesToScan = assembliesToScan.Distinct().ToArray();

			ConnectImplementationsToTypesClosing(typeof(MediatR.IRequestHandler<,>), services, assembliesToScan, false, mustImplementInterface, interfaceToImplement);
			ConnectImplementationsToTypesClosing(typeof(MediatR.INotificationHandler<>), services, assembliesToScan, true);
			ConnectImplementationsToTypesClosing(typeof(IRequestPreProcessor<>), services, assembliesToScan, true);
			ConnectImplementationsToTypesClosing(typeof(IRequestPostProcessor<,>), services, assembliesToScan, true);
			ConnectImplementationsToTypesClosing(typeof(IRequestExceptionHandler<,,>), services, assembliesToScan, true);
			ConnectImplementationsToTypesClosing(typeof(IRequestExceptionAction<,>), services, assembliesToScan, true);

			var multiOpenInterfaces = new[]
			{
				typeof(MediatR.INotificationHandler<>),
				typeof(IRequestPreProcessor<>),
				typeof(IRequestPostProcessor<,>),
				typeof(IRequestExceptionHandler<,,>),
				typeof(IRequestExceptionAction<,>)
			};

			foreach (var multiOpenInterface in multiOpenInterfaces)
			{
				var concretions = assembliesToScan
					.SelectMany(a => a.DefinedTypes)
					.Where(type => type.FindInterfacesThatClose(multiOpenInterface).Any())
					.Where(type => type.IsConcrete() && type.IsOpenGeneric())
					.ToList();

				foreach (var type in concretions)
				{
					services.AddTransient(multiOpenInterface, type);
				}
			}
		}

		/// <summary>
		/// Helper method use to differentiate behavior between request handlers and notification handlers.
		/// Request handlers should only be added once (so set addIfAlreadyExists to false)
		/// Notification handlers should all be added (set addIfAlreadyExists to true)
		/// </summary>
		/// <param name="openRequestInterface"></param>
		/// <param name="services"></param>
		/// <param name="assembliesToScan"></param>
		/// <param name="addIfAlreadyExists"></param>
		private static void ConnectImplementationsToTypesClosing(Type openRequestInterface,
			IServiceCollection services,
			IEnumerable<Assembly> assembliesToScan,
			bool addIfAlreadyExists,
			bool mustImplementInterface = false,
			Type interfaceToImplement = null)
		{
			var concretions = new List<Type>();
			var interfaces = new List<Type>();
			foreach (var type in assembliesToScan.SelectMany(a => a.DefinedTypes).Where(t => !t.IsOpenGeneric()))
			{
				var typeName = type.Name;
				var interfaceTypes = type.FindInterfacesThatClose(openRequestInterface).ToArray();
				if (!interfaceTypes.Any()) continue;

				if (mustImplementInterface && interfaceToImplement != null)
				{
					if (type.GetInterfaces().All(i => i.FullName != interfaceToImplement.FullName))
					{
						// Continue if the specified interface is not implemented
						continue;
					}
				}

				if (type.IsConcrete())
				{
					concretions.Add(type);
				}

				foreach (var interfaceType in interfaceTypes)
				{
					interfaces.Fill(interfaceType);
				}
			}

			foreach (var @interface in interfaces)
			{
				var exactMatches = concretions.Where(x => x.CanBeCastTo(@interface)).ToList();
				if (addIfAlreadyExists)
				{
					foreach (var type in exactMatches)
					{
						var typeName = type.Name;
						services.AddTransient(@interface, type);
					}
				}
				else
				{
					if (exactMatches.Count > 1)
					{
						exactMatches.RemoveAll(m => !IsMatchingWithInterface(m, @interface));
					}

					foreach (var type in exactMatches)
					{
						var typeName = type.Name;
						services.TryAddTransient(@interface, type);
					}
				}

				if (!@interface.IsOpenGeneric())
				{
					AddConcretionsThatCouldBeClosed(@interface, concretions, services);
				}
			}
		}

		private static bool IsMatchingWithInterface(Type handlerType, Type handlerInterface)
		{
			if (handlerType == null || handlerInterface == null)
			{
				return false;
			}

			if (handlerType.IsInterface)
			{
				if (handlerType.GenericTypeArguments.SequenceEqual(handlerInterface.GenericTypeArguments))
				{
					return true;
				}
			}
			else
			{
				return IsMatchingWithInterface(handlerType.GetInterface(handlerInterface.Name), handlerInterface);
			}

			return false;
		}

		private static void AddConcretionsThatCouldBeClosed(Type @interface, List<Type> concretions, IServiceCollection services)
		{
			foreach (var type in concretions
				.Where(x => x.IsOpenGeneric() && x.CouldCloseTo(@interface)))
			{
				try
				{
					services.TryAddTransient(@interface, type.MakeGenericType(@interface.GenericTypeArguments));
				}
				catch (Exception)
				{
				}
			}
		}

		private static bool CouldCloseTo(this Type openConcretion, Type closedInterface)
		{
			var openInterface = closedInterface.GetGenericTypeDefinition();
			var arguments = closedInterface.GenericTypeArguments;

			var concreteArguments = openConcretion.GenericTypeArguments;
			return arguments.Length == concreteArguments.Length && openConcretion.CanBeCastTo(openInterface);
		}

		private static bool CanBeCastTo(this Type pluggedType, Type pluginType)
		{
			if (pluggedType == null) return false;

			if (pluggedType == pluginType) return true;

			return pluginType.GetTypeInfo().IsAssignableFrom(pluggedType.GetTypeInfo());
		}

		public static bool IsOpenGeneric(this Type type)
		{
			return type.GetTypeInfo().IsGenericTypeDefinition || type.GetTypeInfo().ContainsGenericParameters;
		}

		public static IEnumerable<Type> FindInterfacesThatClose(this Type pluggedType, Type templateType)
		{
			return FindInterfacesThatClosesCore(pluggedType, templateType).Distinct();
		}

		private static IEnumerable<Type> FindInterfacesThatClosesCore(Type pluggedType, Type templateType)
		{
			if (pluggedType == null) yield break;

			if (!pluggedType.IsConcrete()) yield break;

			if (templateType.GetTypeInfo().IsInterface)
			{
				foreach (
					var interfaceType in
					pluggedType.GetInterfaces()
						.Where(type => type.GetTypeInfo().IsGenericType && (type.GetGenericTypeDefinition() == templateType)))
				{
					yield return interfaceType;
				}
			}
			else if (pluggedType.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType &&
					 (pluggedType.GetTypeInfo().BaseType.GetGenericTypeDefinition() == templateType))
			{
				yield return pluggedType.GetTypeInfo().BaseType;
			}

			if (pluggedType.GetTypeInfo().BaseType == typeof(object)) yield break;

			foreach (var interfaceType in FindInterfacesThatClosesCore(pluggedType.GetTypeInfo().BaseType, templateType))
			{
				yield return interfaceType;
			}
		}

		private static bool IsConcrete(this Type type)
		{
			return !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;
		}

		private static void Fill<T>(this IList<T> list, T value)
		{
			if (list.Contains(value)) return;
			list.Add(value);
		}

		public static void AddRequiredServices(IServiceCollection services, MediatrCustomServiceConfiguration serviceConfiguration)
		{
			// Use TryAdd, so any existing ServiceFactory/IMediator registration doesn't get overriden
			services.TryAddTransient<MediatR.ServiceFactory>(p => p.GetService);
			services.TryAdd(new ServiceDescriptor(typeof(MediatR.IMediator), serviceConfiguration.MediatorImplementationType, serviceConfiguration.Lifetime));
			services.TryAdd(new ServiceDescriptor(typeof(MediatR.ISender), sp => sp.GetService<MediatR.IMediator>(), serviceConfiguration.Lifetime));
			services.TryAdd(new ServiceDescriptor(typeof(MediatR.IPublisher), sp => sp.GetService<MediatR.IMediator>(), serviceConfiguration.Lifetime));

			// Use TryAddTransientExact (see below), we dó want to register our Pre/Post processor behavior, even if (a more concrete)
			// registration for IPipelineBehavior<,> already exists. But only once.
			services.TryAddTransientExact(typeof(MediatR.IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
			services.TryAddTransientExact(typeof(MediatR.IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));
			services.TryAddTransientExact(typeof(MediatR.IPipelineBehavior<,>), typeof(RequestExceptionActionProcessorBehavior<,>));
			services.TryAddTransientExact(typeof(MediatR.IPipelineBehavior<,>), typeof(RequestExceptionProcessorBehavior<,>));
		}

		/// <summary>
		/// Adds a new transient registration to the service collection only when no existing registration of the same service type and implementation type exists.
		/// In contrast to TryAddTransient, which only checks the service type.
		/// </summary>
		/// <param name="services">The service collection</param>
		/// <param name="serviceType">Service type</param>
		/// <param name="implementationType">Implementation type</param>
		private static void TryAddTransientExact(this IServiceCollection services, Type serviceType, Type implementationType)
		{
			if (services.Any(reg => reg.ServiceType == serviceType && reg.ImplementationType == implementationType))
			{
				return;
			}

			services.AddTransient(serviceType, implementationType);
		}
	}
}
