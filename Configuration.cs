using System.Text.Json.Serialization;

namespace GooglePlayPublisherCli
{
	public class Configuration
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("package")]
		public string Package { get; set; }

		[JsonPropertyName("serviceAccountFile")]
		public string ServiceAccountFile { get; set; }

		[JsonPropertyName("applicationFile")]
		public string ApplicationFile { get; set; }
		
		[JsonPropertyName("track")]
		public TrackTypes? Track { get; set; }
		
		[JsonPropertyName("trackStatus")]
		public TrackStatusTypes? TrackStatus { get; set; }
	}
}