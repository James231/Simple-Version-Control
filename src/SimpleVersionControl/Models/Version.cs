// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleVersionControl
{
    public class Version
    {
        public Version()
        {
        }

        public Version(VersionController versionController, string versionName, string description, string downloadLink, DateTime? releaseDate, List<ChangeRef> changes, object additionalData = null)
        {
            VersionController = versionController;
            VersionName = versionName;
            Description = description;
            DownloadLink = downloadLink;
            ReleaseDate = releaseDate;
            Changes = changes;
            if (additionalData != null)
            {
                AdditionalData = JObject.FromObject(additionalData);
            }
        }

        [JsonIgnore]
        public VersionController VersionController { get; internal set; }

        /// <summary>
        /// A unique name used to identify this version.
        /// </summary>
        [JsonProperty("version")]
        public string VersionName { get; set; }

        /// <summary>
        /// Description of the version.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Link to download this version from.
        /// </summary>
        [JsonProperty("download_link")]
        public string DownloadLink { get; set; }

        /// <summary>
        /// Timestamp for the release of this version.
        /// </summary>
        [JsonProperty("release_date")]
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// Is this version still functioning?
        /// </summary>
        [JsonProperty("is_functioning")]
        public bool IsFunctioning { get; set; } = true;

        /// <summary>
        /// List of changes made in this version.
        /// </summary>
        [JsonProperty("changes")]
        public List<ChangeRef> Changes { get; set; }

        /// <summary>
        /// Store any additional data with each Version in this <see cref="JObject"/>.
        /// </summary>
        [JsonProperty("additional_data")]
        public JObject AdditionalData { get; set; }

        /// <summary>
        /// Returns instance of <see cref="Version"/> from JSON string.
        /// </summary>
        /// <param name="json">JSON string to deserialize.</param>
        /// <param name="versionController">Version Controller reference used to cache the version object.</param>
        /// <returns>Returns Version.</returns>
        public static Version FromJson(string json, VersionController versionController)
        {
            Version version = JsonConvert.DeserializeObject<Version>(json);
            if (version != null)
            {
                version.VersionController = versionController;
                version.Changes.ToList().ForEach(v => v.VersionController = versionController);
            }

            return version;
        }
    }
}
