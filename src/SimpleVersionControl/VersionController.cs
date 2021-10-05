// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleVersionControl
{
    public class VersionController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionController"/> class.
        /// </summary>
        /// <param name="currentVersion">String representing the current version of the application.</param>
        /// <param name="baseUrl">Base Uri where version JSON files can be found.</param>
        /// <param name="httpClient">HttpClient to retrieve JSON files with.</param>
        public VersionController(string currentVersion, string baseUrl, HttpClient httpClient = null)
            : this(currentVersion, new HttpFileProvider(baseUrl, httpClient))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionController"/> class.
        /// </summary>
        /// <param name="currentVersion">String representing the current version of the application.</param>
        /// <param name="fileProvider">Service to retrieve JSON strings from paths.</param>
        public VersionController(string currentVersion, IFileProvider fileProvider)
        {
            CurrentVersion = currentVersion;
            FileProvider = fileProvider;
        }

        /// <summary>
        /// String representing the current version of the application.
        /// </summary>
        public string CurrentVersion { get; internal set; }

        /// <summary>
        /// Service to retrieve JSON strings from paths.
        /// </summary>
        public IFileProvider FileProvider { get; internal set; }

        internal ChangeLog CachedChangeLog { get; set; }

        internal List<Change> CachedChanges { get; set; } = new List<Change>();

        internal List<Version> CachedVersions { get; set; } = new List<Version>();

        /// <summary>
        /// Is this the latest version of the application?
        /// </summary>
        /// <returns>True if this is the latest version.</returns>
        public async Task<bool> IsLatestVersion()
        {
            if (await GetChangeLog() == null)
            {
                return false;
            }

            string latestVersionString = CachedChangeLog.Versions.First().VersionName;
            return latestVersionString == CurrentVersion;
        }

        /// <summary>
        /// Is this version of the application still functioning? Or has it been broken by later updates.
        /// </summary>
        /// <returns>True if this version is still functioning.</returns>
        public async Task<bool> IsFunctioningVersion()
        {
            Version curVersion = await GetVersion();
            if (curVersion == null)
            {
                return false;
            }

            return curVersion.IsFunctioning;
        }

        /// <summary>
        /// Retrieves instance of <see cref="Version"/> representing the current version of the application.
        /// </summary>
        /// <returns>Returns current application version object.</returns>
        public async Task<Version> GetVersion()
        {
            return await GetVersion(CurrentVersion);
        }

        /// <summary>
        /// Retrieves instance of <see cref="Version"/> representing a version of the application.
        /// </summary>
        /// <param name="versionString">String representing a version of the application.</param>
        /// <returns>Returns application version object.</returns>
        public async Task<Version> GetVersion(string versionString)
        {
            VersionRef versionRef = await GetVersionRef(versionString);
            if (versionRef == null)
            {
                return null;
            }

            return await versionRef.GetVersion();
        }

        /// <summary>
        /// Retrieves instance of <see cref="VersionRef"/> which references a version of the application in the filesystem.
        /// </summary>
        /// <param name="versionString">String representing a version of the application.</param>
        /// <returns>Returns object referencing application version.</returns>
        public async Task<VersionRef> GetVersionRef(string versionString)
        {
            if (await GetChangeLog() == null)
            {
                return null;
            }

            return CachedChangeLog.Versions.FirstOrDefault(v => v.VersionName == versionString);
        }

        /// <summary>
        /// Retrieves instance of <see cref="Version"/> representing the latest version of the application.
        /// </summary>
        /// <returns>Returns latest application version object.</returns>
        public async Task<Version> GetLatestVersion()
        {
            VersionRef versionRef = await GetLatestVersionRef();
            if (versionRef == null)
            {
                return null;
            }

            return await versionRef.GetVersion();
        }

        /// <summary>
        /// Retrieves instance of <see cref="VersionRef"/> which references the latest version of the application in the filesystem.
        /// </summary>
        /// <returns>Returns object referencing latest application version.</returns>
        public async Task<VersionRef> GetLatestVersionRef()
        {
            if (await GetChangeLog() == null)
            {
                return null;
            }

            return CachedChangeLog.Versions.First();
        }

        /// <summary>
        /// Retrieves instance of <see cref="Version"/> representing the first version of the application.
        /// </summary>
        /// <returns>Returns latest application version object.</returns>
        public async Task<Version> GetFirstVersion()
        {
            VersionRef versionRef = await GetLatestVersionRef();
            if (versionRef == null)
            {
                return null;
            }

            return await versionRef.GetVersion();
        }

        /// <summary>
        /// Retrieves instance of <see cref="VersionRef"/> which references the first version of the application in the filesystem.
        /// </summary>
        /// <returns>Returns object referencing first application version.</returns>
        public async Task<VersionRef> GetFirstVersionRef()
        {
            if (await GetChangeLog() == null)
            {
                return null;
            }

            return CachedChangeLog.Versions.Last();
        }

        /// <summary>
        /// Retrieves application changelog which lists all versions.
        /// </summary>
        /// <returns>Returns object representing application ChangeLog.</returns>
        public async Task<ChangeLog> GetChangeLog()
        {
            if (CachedChangeLog != null)
            {
                return CachedChangeLog;
            }

            string changelogContent = await FileProvider.GetFile("changelog.json");
            if (string.IsNullOrEmpty(changelogContent))
            {
                return null;
            }

            ChangeLog changeLog = ChangeLog.FromJson(changelogContent, this);
            if (changeLog != null)
            {
                CachedChangeLog = changeLog;
            }

            return changeLog;
        }

        /// <summary>
        /// Retrieves list of all application versions between two given versions.
        /// </summary>
        /// <param name="versionOne">Name of first version.</param>
        /// <param name="versionTwo">Name of second version.</param>
        /// <param name="includeLast">Set to true to include second version in returned IEnumerable.</param>
        /// <returns>List of application version references. Null if version doesn't exist.</returns>
        public async Task<IEnumerable<VersionRef>> GetVersionsBetween(string versionOne, string versionTwo, bool includeLast = false)
        {
            VersionRef refOne = await GetVersionRef(versionOne);
            VersionRef refTwo = await GetVersionRef(versionTwo);
            if (refOne == null || refTwo == null)
            {
                return null;
            }

            return await GetVersionsBetween(refOne, refTwo, includeLast);
        }

        /// <summary>
        /// Retrieves list of all application versions between two given versions.
        /// </summary>
        /// <param name="versionOne">Reference to first version.</param>
        /// <param name="versionTwo">Reference to second version.</param>
        /// <param name="includeLast">Set to true to include second version in returned IEnumerable.</param>
        /// <returns>List of application version references.</returns>
        public async Task<IEnumerable<VersionRef>> GetVersionsBetween(VersionRef versionOne, VersionRef versionTwo, bool includeLast = false)
        {
            if (await GetChangeLog() == null)
            {
                return null;
            }

            if (versionOne.VersionName == versionTwo.VersionName)
            {
                return new List<VersionRef>();
            }

            int indexOfOne = CachedChangeLog.Versions.IndexOf(versionOne);
            int indexOfTwo = CachedChangeLog.Versions.IndexOf(versionTwo);
            int minIndex = Math.Min(indexOfOne, indexOfTwo);
            int maxIndex = Math.Max(indexOfOne, indexOfTwo);
            int rangeStart = includeLast ? minIndex : minIndex + 1;
            int rangeEnd = maxIndex - rangeStart;
            return CachedChangeLog.Versions.GetRange(rangeStart, rangeEnd);
        }

        /// <summary>
        /// Retrieves list of all application changes between two given versions.
        /// </summary>
        /// <param name="versionOne">Name of first version.</param>
        /// <param name="versionTwo">Name of second version.</param>
        /// <param name="includeLast">Set to true to include changes of second version in returned IEnumerable.</param>
        /// <returns>List of references to application changes. Null if version doesn't exist.</returns>
        public async Task<IEnumerable<ChangeRef>> GetChangesBetween(string versionOne, string versionTwo, bool includeLast = false)
        {
            VersionRef refOne = await GetVersionRef(versionOne);
            VersionRef refTwo = await GetVersionRef(versionTwo);
            if (refOne == null || refTwo == null)
            {
                return null;
            }

            return await GetChangesBetween(refOne, refTwo, includeLast);
        }

        /// <summary>
        /// Retrieves list of all application changes between two given versions.
        /// </summary>
        /// <param name="versionOne">Reference to first version.</param>
        /// <param name="versionTwo">Reference to second version.</param>
        /// <param name="includeLast">Set to true to include changes of second version in returned IEnumerable.</param>
        /// <returns>List of references to application changes.</returns>
        public async Task<IEnumerable<ChangeRef>> GetChangesBetween(VersionRef versionOne, VersionRef versionTwo, bool includeLast = false)
        {
            IEnumerable<VersionRef> versionsBetween = await GetVersionsBetween(versionOne, versionTwo, includeLast);
            if (versionsBetween == null)
            {
                return null;
            }

            List<ChangeRef> changes = new List<ChangeRef>();
            foreach (VersionRef versionRef in versionsBetween)
            {
                Version version = await versionRef.GetVersion();
                changes.AddRange(version.Changes);
            }

            return changes;
        }

        /// <summary>
        /// Writes ChangeLog, Version and Change files to filesystem.
        /// </summary>
        /// <returns>True if was successful, false otherwise.</returns>
        public async Task<bool> Serialize()
        {
            if (await GetChangeLog() == null)
            {
                return false;
            }

            return await Serialize(CachedChangeLog);
        }

        /// <summary>
        /// Writes ChangeLog, Version and Change files to filesystem.
        /// </summary>
        /// <param name="changeLog">ChangeLog to serialize.</param>
        /// <returns>True if was successful, false otherwise.</returns>
        public async Task<bool> Serialize(ChangeLog changeLog)
        {
            if (FileProvider is IFileSaver)
            {
                return await Serialize(changeLog, (IFileSaver)FileProvider);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Writes ChangeLog, Version and Change files using given instance of <see cref="IFileSaver"/>
        /// </summary>
        /// <param name="changeLog">ChangeLog to serialize.</param>
        /// <param name="saver">Service to save json strings to file paths.</param>
        /// <returns>True if was successful, false otherwise.</returns>
        public async Task<bool> Serialize(ChangeLog changeLog, IFileSaver saver)
        {
            if (changeLog == null)
            {
                return false;
            }

            try
            {
                changeLog.Validate();
            }
            catch (ChangeLogValidationException)
            {
                await saver.CancelPublish();
                throw;
            }

            if (!(await saver.SaveFile(Path.Combine("temp", "changelog.json"), changeLog.ToJson())))
            {
                return false;
            }

            foreach (VersionRef versionRef in changeLog.Versions)
            {
                Version version = await versionRef.GetVersion(false);
                version.Changes.RemoveAll(c => c == null);
                if (version == null)
                {
                    continue;
                }

                try
                {
                    version.Validate();
                }
                catch (ChangeLogValidationException)
                {
                    await saver.CancelPublish();
                    throw;
                }

                if (!(await saver.SaveFile(Path.Combine("temp", version.VersionName, "version.json"), version.ToJson())))
                {
                    return false;
                }

                foreach (ChangeRef changeRef in version.Changes)
                {
                    Change change = await changeRef.GetChange(false);
                    if (change == null)
                    {
                        continue;
                    }

                    try
                    {
                        change.Validate(changeLog);
                    }
                    catch (ChangeLogValidationException)
                    {
                        await saver.CancelPublish();
                        throw;
                    }

                    if (!(await saver.SaveFile(Path.Combine("temp", version.VersionName, "changes", $"{change.Guid}.json"), change.ToJson())))
                    {
                        return false;
                    }
                }
            }

            bool success = await saver.PublishFromTemp();

            // Clear caches
            CachedChangeLog = null;
            CachedVersions = new List<Version>();
            CachedChanges = new List<Change>();

            return success;
        }

        public void CacheVersion(Version version)
        {
            if (CachedVersions.FirstOrDefault(v => v.VersionName == version.VersionName) == null)
            {
                CachedVersions.Add(version);
            }
        }

        public void CacheChange(Change change)
        {
            if (CachedChanges.FirstOrDefault(c => c.Guid == change.Guid) == null)
            {
                CachedChanges.Add(change);
            }
        }
    }
}
