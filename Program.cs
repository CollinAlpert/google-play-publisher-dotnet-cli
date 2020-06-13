using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.AndroidPublisher.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace GooglePlayPublisherCli
{
	class Program
	{
		private static readonly Regex LanguageCodeRegex = new Regex("[a-z]{2}-[A-Z]{2}", RegexOptions.Compiled | RegexOptions.Singleline);

		private static readonly IDictionary<TrackStatusTypes, string> TrackStatusTypesStringMapping = new Dictionary<TrackStatusTypes, string>
			{
				[TrackStatusTypes.Completed] = "completed",
				[TrackStatusTypes.Draft] = "draft",
				[TrackStatusTypes.Halted] = "halted",
				[TrackStatusTypes.InProgress] = "inProgress"
			};

		private static readonly IDictionary<TrackTypes, string> TrackTypesStringMapping = new Dictionary<TrackTypes, string>
			{
				[TrackTypes.Alpha] = "alpha",
				[TrackTypes.Beta] = "beta",
				[TrackTypes.Production] = "production"
			};

		public static async Task Main(string[] args)
		{
			if (args.Length != 1 || !File.Exists(args[0]))
			{
				Console.WriteLine("The configuration file could not be found.");

				return;
			}

			await using Stream fileStream = new FileStream(args[0], FileMode.Open);
			var configuration = await JsonSerializer.DeserializeAsync<Configuration>(fileStream).ConfigureAwait(false);
			configuration.Track ??= GetTrack();
			configuration.TrackStatus ??= GetTrackStatus();

			var success = IsInputValid(configuration);
			if (!success)
			{
				return;
			}

			var service = await CreateServiceAsync(configuration.Name, configuration.ServiceAccountFile).ConfigureAwait(false);
			var edit = await service.Edits.Insert(null, configuration.Package).ExecuteAsync().ConfigureAwait(false);

			var versionCode = await UploadApplicationFileAsync(edit.Id, service, configuration.Package, configuration.ApplicationFile).ConfigureAwait(false);

			Console.WriteLine("Please enter language code for release notes (e.g. en-US)");
			var languageCode = GetLanguageCode();
			
			var releaseNotes = string.Join('\n', GetMultilineInput("Please enter release notes. Press Ctrl+d when done."));

			var track = new Track
				{
					Releases = new SingletonList<TrackRelease>(
						new TrackRelease
							{
								Name = $"Release {versionCode}",
								ReleaseNotes = new List<LocalizedText>
									{
										new LocalizedText
											{
												Language = languageCode,
												Text = releaseNotes
											}
									},
								Status = TrackStatusTypesStringMapping[configuration.TrackStatus.Value],
								VersionCodes = new SingletonList<long?>(versionCode),
							})
				};

			await service.Edits.Tracks.Update(track, configuration.Package, edit.Id, TrackTypesStringMapping[configuration.Track.Value]).ExecuteAsync().ConfigureAwait(false);
			var commit = await service.Edits.Commit(configuration.Package, edit.Id).ExecuteAsync().ConfigureAwait(false);

			Console.WriteLine($"Release with id {commit.Id} committed!");
		}

		private static async Task<int?> UploadApplicationFileAsync(string editId, AndroidPublisherService service, string packageName, string applicationFile)
		{
			if (applicationFile.EndsWith(".apk"))
			{
				await service.Edits.Apks.Upload(packageName, editId, new FileStream(applicationFile, FileMode.Open), "application/vnd.android.package-archive").UploadAsync().ConfigureAwait(false);
				var apks = await service.Edits.Apks.List(packageName, editId).ExecuteAsync().ConfigureAwait(false);

				return apks.Apks.Last().VersionCode;
			}

			if (applicationFile.EndsWith(".aab"))
			{
				await service.Edits.Bundles.Upload(packageName, editId, new FileStream(applicationFile, FileMode.Open), "application/octet-stream").UploadAsync().ConfigureAwait(false);
				var bundles = await service.Edits.Bundles.List(packageName, editId).ExecuteAsync().ConfigureAwait(false);

				return bundles.Bundles.Last().VersionCode;
			}

			throw new ArgumentOutOfRangeException(nameof(applicationFile));
		}

		private static IEnumerable<string> GetMultilineInput(string prompt)
		{
			Console.WriteLine(prompt);

			string input;
			while (!string.IsNullOrEmpty(input = Console.ReadLine()))
			{
				yield return input;
			}
		}

		private static string GetLanguageCode()
		{
			var code = Console.ReadLine();
			if (LanguageCodeRegex.IsMatch(code))
			{
				return code;
			}

			return GetLanguageCode();
		}

		private static bool IsInputValid(Configuration configuration)
		{
			if (string.IsNullOrWhiteSpace(configuration.Name) || string.IsNullOrWhiteSpace(configuration.Package))
			{
				Console.WriteLine("Missing configuration data.");

				return false;
			}

			if (!File.Exists(configuration.ApplicationFile))
			{
				Console.WriteLine("The application file does not exist.");

				return false;
			}

			if (!File.Exists(configuration.ServiceAccountFile))
			{
				Console.WriteLine("The service account file does not exist.");

				return false;
			}

			return true;
		}

		private static async Task<AndroidPublisherService> CreateServiceAsync(string appName, string serviceAccountFile)
		{
			var credentials = await GoogleCredential.FromFileAsync(serviceAccountFile, CancellationToken.None).ConfigureAwait(false);
			credentials = credentials.CreateScoped(new SingletonList<string>(AndroidPublisherService.Scope.Androidpublisher));

			return new AndroidPublisherService(
				new BaseClientService.Initializer
					{
						HttpClientInitializer = credentials,
						ApplicationName = appName
					});
		}

		private static TrackStatusTypes GetTrackStatus()
		{
			Console.WriteLine("Choose number of track to publish to:");
			Console.WriteLine("1 = Completed, 2 = Draft, 3 = Halted, 4 = InProgress");
			var track = Console.ReadLine();
			if (int.TryParse(track, out var trackNumber) && trackNumber >= 1 && trackNumber <= 4)
			{
				return (TrackStatusTypes)trackNumber;
			}

			return GetTrackStatus();
		}

		private static TrackTypes GetTrack()
		{
			Console.WriteLine("Choose number of track to publish to:");
			Console.WriteLine("1 = Alpha, 2 = Beta, 3 = Production");
			var track = Console.ReadLine();
			if (int.TryParse(track, out var trackNumber) && trackNumber >= 1 && trackNumber <= 3)
			{
				return (TrackTypes)trackNumber;
			}

			return GetTrack();
		}
	}
}