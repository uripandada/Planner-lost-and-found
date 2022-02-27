namespace Planner.Application.UserManagement.Models
{
	public class SaveAvatarData
    {
        public bool HasChanged { get; set; }
        public string FileName { get; set; }
        public byte[] File { get; set; }
    }
}
