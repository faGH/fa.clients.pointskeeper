namespace FrostAura.Clients.PointsKeeper.Shared.Models
{
	public class ApplicationConfig
	{
		public string AppBaseUrl { get; set; }
		public string AdminPinCode { get; set; }
		public int MonitaryTarget { get; set; }
		public int AutoRefreshDelayInSeconds { get; set; }
	}
}

