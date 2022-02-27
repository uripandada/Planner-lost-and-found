namespace Planner.Domain.Entities
{
	public class RccProduct
	{
		public string Id { get; set; }
		public bool IsActive { get; set; }
		public string ExternalName { get; set; }
		/// <summary>
		/// External Identifier. Nothing to do with the RcNext directly.
		/// </summary>
		public string ServiceId { get; set; }
		/// <summary>
		/// External Identifier. Nothing to do with the RcNext directly.
		/// </summary>
		public string CategoryId { get; set; }
	}
}
