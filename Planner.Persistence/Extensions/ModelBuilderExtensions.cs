using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace Planner.Persistence.Extensions
{
    public static class ModelBuilderExtensions
	{
		public static void ApplyAllConfigurations(this ModelBuilder modelBuilder)
		{
			var applyConfigurationMethodInfo = modelBuilder
				.GetType()
				.GetMethods(BindingFlags.Instance | BindingFlags.Public)
				.First(m => m.Name.Equals("ApplyConfiguration", StringComparison.OrdinalIgnoreCase));

			var ret = typeof(DatabaseContext).Assembly
				.GetTypes()
				.Select(t => (t, i: t.GetInterfaces().FirstOrDefault(i => i.Name.Equals(typeof(IEntityTypeConfiguration<>).Name, StringComparison.Ordinal)))) // find all interfaces of name IEntityTypeConfiguration<> in the same assembly as DatabaseContext is,
				.Where(it => it.i != null && it.t.IsAbstract == false) // filter out results without interfaces
				.Select(it => (et: it.i.GetGenericArguments()[0], cfgObj: Activator.CreateInstance(it.t))) // transform to second pair of results => (TData of IEntityTypeConfiguration<TData>, an instance of the type from the (type, interface) result in the step before)
				.Select(it => applyConfigurationMethodInfo.MakeGenericMethod(it.et).Invoke(modelBuilder, new[] { it.cfgObj })) // creates a generic method with of signature: TData MethodName(IEntityTypeConfiguration<TData> configuration) and invokes it
				.ToList();
		}
	}
}
