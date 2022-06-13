using System;
using System.IO;
using Newtonsoft.Json.Linq;
using SharpRaven;
using SharpRaven.Data;

namespace NielsEngine
{
    internal static class Common
    {
        internal static string _commonFolder { get; } =
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        internal static string _commonAppFolder { get; } = Path.Combine(_commonFolder, "cpt");
        internal static string _commonAppDataFolder { get; } = Path.Combine(_commonAppFolder, "data");
        internal static string _confPath { get; } = Path.Combine(_commonAppFolder, "tracker.dat");

        internal static JObject tracker;


        internal static void log(Exception exception)
        {
            var ravenClient =
                new RavenClient("https://42f4e5b67dc24504bc31c7c976682596@o413864.ingest.sentry.io/6489294");
            if (tracker != null)
                ravenClient.Tags.Add("Client", $"{tracker["name"]}");
            ravenClient.Capture(new SentryEvent(exception));
        }

        internal static void log(string exception)
        {
            var ravenClient =
                new RavenClient("https://42f4e5b67dc24504bc31c7c976682596@o413864.ingest.sentry.io/6489294");
            if (tracker != null)
                ravenClient.Tags.Add("Client", $"{tracker["name"]}");
            ravenClient.Capture(new SentryEvent(exception));
        }

        internal static void IniTmp()
        {
            try
            {
                if (!Directory.Exists(_commonAppFolder))
                    Directory.CreateDirectory(_commonAppFolder);
                if (!Directory.Exists(_commonAppDataFolder))
                    Directory.CreateDirectory(_commonAppDataFolder);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}