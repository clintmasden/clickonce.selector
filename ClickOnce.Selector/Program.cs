using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ClickOnce.Selector
{
    internal class Program
    {
        private static IConfigurationRoot _configurationRoot;

        private static void Main(string[] entryArguments)
        {
            var arguments = entryArguments.OfType<string>().ToList();

            _configurationRoot = GetConfigurationBuilder();

            var publishDirectory = _configurationRoot.GetSection("appSettings:publishDirectory").Value;

            if (arguments.Count == 0)
            {
                var stableVersion = _configurationRoot.GetSection("appSettings:stableVersion").Value;
                Console.WriteLine($"Launching Stable Version: {stableVersion}");

                LaunchStableExecutable(publishDirectory, stableVersion);
            }
            else if (arguments.Count >= 1)
            {
                var isBleeding = !string.IsNullOrWhiteSpace(arguments[0]) && arguments[0].ToLower().Trim() == "b";

                if (isBleeding)
                {
                    Console.WriteLine($"Launching Bleeding Version");

                    LaunchBleedingExecutable(publishDirectory);
                }
            }
        }

        private static IConfigurationRoot GetConfigurationBuilder()
        {
            var basePath = Directory.GetCurrentDirectory();

            var configurationBuilder = new ConfigurationBuilder()
               .SetBasePath(basePath)
               .AddJsonFile("appsettings.json")
               .Build();

            return configurationBuilder;
        }

        private static void LaunchStableExecutable(string publishDirectory, string publishVersion)
        {
            var publishDirectoryManifestFiles = Directory.GetFiles(publishDirectory, "*.manifest", SearchOption.AllDirectories).ToList();

            var publishVersionFormatted = publishVersion.Replace(".", "_");

            var manifestFile = publishDirectoryManifestFiles.Where(l => l.Contains(publishVersionFormatted)).SingleOrDefault();
            var manifestDirectory = Path.GetDirectoryName(manifestFile);

            var ExecutableName = Path.GetFileNameWithoutExtension(manifestFile);
            var ExecutableFile = Path.Combine(manifestDirectory, ExecutableName);

            StartProcess(ExecutableFile, manifestDirectory);
        }

        private static void LaunchBleedingExecutable(string publishDirectory)
        {
            var publishDirectoryManifestFiles = Directory.GetFiles(publishDirectory, "*.manifest", SearchOption.AllDirectories).ToList();

            string manifestFile = string.Empty;
            DateTime manifestLastWriteTime = DateTime.MinValue;

            foreach (var publishManifestFile in publishDirectoryManifestFiles)
            {
                FileInfo manifestFileInformation = new FileInfo(publishManifestFile);

                if (manifestFileInformation.LastWriteTime > manifestLastWriteTime)
                {
                    manifestFile = publishManifestFile;
                    manifestLastWriteTime = manifestFileInformation.LastWriteTime;
                }
            }

            var manifestDirectory = Path.GetDirectoryName(manifestFile);

            var ExecutableName = Path.GetFileNameWithoutExtension(manifestFile);
            var ExecutableFile = Path.Combine(manifestDirectory, ExecutableName);

            StartProcess(ExecutableFile, manifestDirectory);
        }

        private static void StartProcess(string fileName, string workingDirectory)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                WorkingDirectory = workingDirectory,
            };

            Console.WriteLine($"Executing: {fileName}");

            var process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            System.Threading.Thread.Sleep(1000);
        }
    }
}