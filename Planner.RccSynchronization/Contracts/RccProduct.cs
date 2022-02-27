namespace Planner.RccSynchronization.Contracts
{
	public class RccProduct
	{
		public string ProductId { get; set; }
		public bool IsActive { get; set; }
		public string ExternalName { get; set; }
		public string ServiceId { get; set; }
		public string CategoryId { get; set; }
	}
}
