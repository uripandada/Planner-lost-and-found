using System.Collections.Generic;
using System.Text;

namespace Planner.Domain.Entities
{

	/// <summary>
	/// Base entity that provides the basic audit properties for allmost every property in the domain.
	/// Additionaly it provides the HotelID, because allmost all of the data will be partitioned by hotel
	/// </summary>
	public abstract class BaseEntity: ChangeTrackingBaseEntity
	{
		public string HotelId { get; set; }
		public Hotel Hotel { get; set; }
	}
}
