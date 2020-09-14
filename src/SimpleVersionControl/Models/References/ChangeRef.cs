// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace SimpleVersionControl
{
    public class ChangeRef
    {
        public ChangeRef()
        {
        }

        public ChangeRef(VersionController versionController, string guid, string relativePath)
        {
            VersionController = versionController;
            Guid = guid;
            RelativeFilePath = relativePath;
        }

        [JsonIgnore]
        public VersionController VersionController { get; internal set; }

        /// <summary>
        /// A random Guid used to identify this change.
        /// </summary>
        [JsonProperty("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// Relative file path to the JSON file for this change in the filesystem.
        /// </summary>
        [JsonProperty("relative_file_path")]
        public string RelativeFilePath { get; set; }
    }
}
