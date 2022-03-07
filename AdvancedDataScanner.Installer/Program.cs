using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Octokit;

namespace AdvancedDataScannerInstaller
{
    internal class Program
    {
        private static bool _exit = false;
        private static GitHubClient client;
        private static bool _isConnection = true;

        static void Main (string[] args)
        {
            client = new GitHubClient(new ProductHeaderValue("FireDragon91245"));
            _isConnection = TestConnectionAsync().GetAwaiter().GetResult();
            while (!_exit)
            {
                Console.Write(">");
                String[] cmd;
                if (args.Length > 0)
                {
                    cmd = args;
                }
                else
                {
                    cmd = Console.ReadLine().Split(' ');
                }

                if (!_isConnection)
                {
                    Console.WriteLine("Could not achive a connection try 'reconect' to refresh connection!");
                    if (cmd[0].Equals("reconect", StringComparison.OrdinalIgnoreCase))
                    {
                        _isConnection = TestConnectionAsync().GetAwaiter().GetResult();
                        if (_isConnection)
                        {
                            Console.WriteLine("Reconectet!");
                        }
                        else
                        {
                            Console.WriteLine("No Connection Achivable!");
                        }
                    }
                }
                else
                {
                    if (cmd[0].Equals("newest", StringComparison.OrdinalIgnoreCase))
                    {
                        GetNewestVersion();
                    }
                    else if (cmd[0].Equals("versions", StringComparison.OrdinalIgnoreCase))
                    {
                        GetAllVersions();
                    }
                    else if (cmd[0].Equals("installnewest", StringComparison.OrdinalIgnoreCase))
                    {
                        InstallNewest();
                    }
                    else if (cmd[0].Equals("installcustom", StringComparison.OrdinalIgnoreCase))
                    {
                        InstallCustom(cmd);
                    }
                    else if (cmd[0].Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        _exit = true;
                    }
                    else if (cmd[0].Equals("srcnewest", StringComparison.OrdinalIgnoreCase))
                    {
                        DownloadNewestSrc();
                    }
                    else if (cmd[0].Equals("srccustom", StringComparison.OrdinalIgnoreCase))
                    {
                        DownloadCustomSrc(cmd);
                    }
                    else if (cmd[0].Equals("help", StringComparison.OrdinalIgnoreCase))
                    {
                        using (StreamReader reader = new StreamReader("./help.txt"))
                        {
                            Console.WriteLine(reader.ReadToEnd());
                        }
                    }
                }
            }
        }

        private static async Task<bool> TestConnectionAsync ()
        {
            bool connection = true;
            try
            {
                var response = await client.Repository.Release.GetAll("FireDragon91245", "AdvancedDataScanner");
            }
            catch
            {
                connection = false;
            }
            return connection;
        }

        private static async void GetNewestVersion ()
        {
            var releases = await client.Repository.Release.GetAll("FireDragon91245", "AdvancedDataScanner");
            Console.WriteLine("Newest Advanced Data Scanner Release: " + releases.First().TagName);
        }

        private static async void GetAllVersions ()
        {
            var releases = await client.Repository.Release.GetAll("FireDragon91245", "AdvancedDataScanner");
            Console.WriteLine("Found " + releases.Count + " Version(s)");
            foreach (Release release in releases)
            {
                Console.WriteLine("-> " + release.TagName);
            }
        }

        private static async void InstallNewest ()
        {
            var releases = await client.Repository.Release.GetAll("FireDragon91245", "AdvancedDataScanner");
            string newest = $"https://github.com/FireDragon91245/AdvancedDataScanner/releases/download/{releases.First().TagName.ToLower()}/AdvancedDataScanner.exe";
            Console.WriteLine($"Downloading From: {newest}...");
            using (var webclient = new WebClient())
            {
                Uri uri = new Uri(newest);
                webclient.DownloadProgressChanged += DownloadPercentage;
                webclient.DownloadDataCompleted += DownloadFinish;
                webclient.DownloadFileAsync(uri, $"./AdvancedDataScanner {releases.First().TagName}.exe");
            }
        }

        private static void DownloadPercentage (object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine($"Downloading... {e.ProgressPercentage}% [{Math.Round(e.BytesReceived / 1048576f, 2)}MB / {Math.Round(e.TotalBytesToReceive / 1048576f, 2)}MB]");
        }

        private static void DownloadFinish (object sender, DownloadDataCompletedEventArgs e)
        {
            Console.WriteLine("");
            Console.WriteLine("Download Finished and was saved in the same folder as this executable");
        }

        private static async void InstallCustom (string[] args)
        {
            if (args.Length < 2)
                return;
            var releases = await client.Repository.Release.GetAll("FireDragon91245", "AdvancedDataScanner");
            bool targetVersionAvaidable = false;
            foreach (Release release in releases)
            {
                if (args[1].Equals(release.TagName, StringComparison.OrdinalIgnoreCase))
                {
                    targetVersionAvaidable = true;
                }
            }
            if (!targetVersionAvaidable)
            {
                Console.WriteLine("Targetet Version not avaidable");
            }
            else
            {
                string version = $"https://github.com/FireDragon91245/AdvancedDataScanner/releases/download/{args[1].ToLower()}/AdvancedDataScanner.exe";
                using (var webclient = new WebClient())
                {
                    Uri uri = new Uri(version);
                    webclient.DownloadProgressChanged += DownloadPercentage;
                    webclient.DownloadDataCompleted += DownloadFinish;
                    webclient.DownloadFileAsync(uri, $"./AdvancedDataScanner {releases.First().TagName}.exe");
                }
            }
        }

        private static async void DownloadNewestSrc ()
        {
            var releases = await client.Repository.Release.GetAll("FireDragon91245", "AdvancedDataScanner");
            string newest = $"https://github.com/FireDragon91245/AdvancedDataScanner/archive/refs/tags/{releases.First().TagName.ToLower()}.zip";
            Console.WriteLine($"Downloading From: {newest}...");
            using (var webclient = new WebClient())
            {
                Uri uri = new Uri(newest);
                webclient.DownloadProgressChanged += DownloadPercentage;
                webclient.DownloadDataCompleted += DownloadFinish;
                webclient.DownloadFileAsync(uri, $"./AdvancedDataScannerSrc {releases.First().TagName}.zip");
            }
        }

        private static async void DownloadCustomSrc (string[] args)
        {
            if (args.Length < 2)
                return;
            var releases = await client.Repository.Release.GetAll("FireDragon91245", "AdvancedDataScanner");
            bool targetVersionAvaidable = false;
            foreach (Release release in releases)
            {
                if (args[1].Equals(release.TagName, StringComparison.OrdinalIgnoreCase))
                {
                    targetVersionAvaidable = true;
                }
            }
            if (!targetVersionAvaidable)
            {
                Console.WriteLine("Targetet Version not avaidable");
            }
            else
            {
                string version = $"https://github.com/FireDragon91245/AdvancedDataScanner/archive/refs/tags/{args[1].ToLower()}.zip";
                using (var webclient = new WebClient())
                {
                    Uri uri = new Uri(version);
                    webclient.DownloadProgressChanged += DownloadPercentage;
                    webclient.DownloadDataCompleted += DownloadFinish;
                    webclient.DownloadFileAsync(uri, $"./AdvancedDataScannerSrc {releases.First().TagName}.zip");
                }
            }
        }
    }
}
