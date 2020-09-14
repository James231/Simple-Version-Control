// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;

namespace SimpleVersionControl
{
    public static class VersionRefExtensions
    {
        /// <summary>
        /// Asynchronously retrieves <see cref="Version"/> object from the filesystem given a reference to it.
        /// </summary>
        /// <param name="versionRef">Reference to the JSON file.</param>
        /// <param name="cache">Should the resulting Version be cached?</param>
        /// <returns>Returns Version object.</returns>
        public static async Task<Version> GetVersion(this VersionRef versionRef, bool cache = true)
        {
            Version version = versionRef.VersionController.CachedVersions.FirstOrDefault(v => v.VersionName == versionRef.VersionName);
            if (version != null)
            {
                return version;
            }

            string versionJson = await versionRef.VersionController.FileProvider.GetFile(versionRef.RelativeFilePath);
            if (versionJson == null)
            {
                return null;
            }

            version = Version.FromJson(versionJson, versionRef.VersionController);
            if (version != null && cache)
            {
                versionRef.VersionController.CachedVersions.Add(version);
            }

            return version;
        }
    }
}
