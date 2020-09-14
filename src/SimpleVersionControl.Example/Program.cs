// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

// For More Examples see the GitHub Readme

using System;
using System.Threading.Tasks;

namespace SimpleVersionControl.Example
{
    public class Program
    {
        public static int Main(string[] args)
        {
            // Needs to execute in async method
            return MainAsync(args).Result;
        }

        private static async Task<int> MainAsync(string[] args)
        {
            string currentVersion = "1.0.0";
            string baseUrl = "https://start-menu-manager.jam-es.com/versions";

            // Create instance of VersionController by passing in this version of the application, and the URL of the directory containing changelog file:
            VersionController versionController = new VersionController(currentVersion, baseUrl);
            Console.WriteLine($"This Version: {currentVersion}");

            // Check this is the latest version
            bool isLatestVersion = await versionController.IsLatestVersion();
            Console.WriteLine($"Is Latest Version: {isLatestVersion}");

            // If not latest, you can check this version is still meant to function correctly
            bool isFunctioningVersion = await versionController.IsFunctioningVersion();
            Console.WriteLine($"Is Version Functioning: {isFunctioningVersion}");

            // The ChangeLog object contains a list of all versions that have been released
            // We can list version properties like this:
            ChangeLog changeLog = await versionController.GetChangeLog();
            Console.WriteLine();
            Console.WriteLine("All Versions:");
            foreach (VersionRef versionRef in changeLog.Versions)
            {
                Version version = await versionRef.GetVersion();
                Console.WriteLine($"Version Name: {version.VersionName}");
                Console.WriteLine($"Version Description: {version.Description}");
                Console.WriteLine($"Version Download Link: {version.DownloadLink}");
                Console.WriteLine($"Version Release Date: {version.ReleaseDate}");

                // You can get all changes in a version with version.Changes
            }

            // Get individual versions with:
            VersionRef firstAppVersionRef = await versionController.GetFirstVersionRef();
            Version latestAppVersionRef = await versionController.GetLatestVersion();
            VersionRef specificVersionRef = await versionController.GetVersionRef("1.0.0b4");
            Console.WriteLine();
            Console.WriteLine($"First Version: {firstAppVersionRef.VersionName}");
            Console.WriteLine($"Latest Version: {latestAppVersionRef.VersionName}");
            Console.WriteLine($"Found Version 1.0.0b4: {specificVersionRef != null}");

            // Convert references to Version objects to get all the properties
            Version firstVersion = await firstAppVersionRef.GetVersion();
            Console.WriteLine($"First Version Description: {firstVersion.Description}");

            // ChangeLog, Version and Change objects can all have additional data stored within them (set in the WPF app)
            // You can access this data by deserializing the AdditionalData property (it is a JObject)
            // For example, to retrieve additional data from a version
            ExtraVersionData extraData = firstVersion.AdditionalData.ToObject<ExtraVersionData>();

            // ^ The version files with the given URI don't actually use additional data so this is null

            // You could use this to order lists of versions or changes using Linq:
            /*
                List<Task<Version>> getVersionTasks = changeLog.Versions.Select(async v => await v.GetVersion()).ToList();
                IEnumerable<Version> versions = await Task.WhenAll(getVersionTasks);
                versions.OrderBy(v => v.AdditionalData.ToObject<ExtraVersionData>().Importance);
                // It is gerenally a good idea to surround this in try{}catch{} in case ToObject throws an error
            */

            Console.ReadKey();

            return 0;
        }

        private class ExtraVersionData
        {
            public int Importance { get; set; }
        }
    }
}
