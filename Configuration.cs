using System.Text.Json.Serialization;

namespace GooglePlayPublisherCli
{
	public class Configuration
	{
		[JsonPropertyName("applicationFile")]
		public string ApplicationFile { get; set; } = null!;

		[JsonPropertyName("name")]
		public string Name { get; set; } = null!;

		[JsonPropertyName("package")]
		public string Package { get; set; } = null!;

		[JsonPropertyName("serviceAccountFile")]
		public string ServiceAccountFile { get; set; } = null!;

		[JsonPropertyName("track")]
		public TrackTypes? Track { get; set; }

		[JsonPropertyName("trackStatus")]
		public TrackStatusTypes? TrackStatus { get; set; }
	}
}