// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace SimpleVersionControl
{
    public class VersionRef
    {
        public VersionRef()
        {
        }

        public VersionRef(VersionController versionController, string relativePath, string versionName)
        {
            VersionController = versionController;
            RelativeFilePath = relativePath;
            VersionName = versionName;
        }

        [JsonIgnore]
        public VersionController VersionController { get; internal set; }

        /// <summary>
        /// A unique name used to identify this version.
        /// </summary>
        [JsonProperty("version")]
        public string VersionName { get; set; }

        /// <summary>
        /// Relative file path to the JSON file for this version in the filesystem.
        /// </summary>
        [JsonProperty("relative_file_path")]
        public string RelativeFilePath { get; set; }
    }
}
