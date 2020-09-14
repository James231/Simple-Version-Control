// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleVersionControl
{
    public class Change
    {
        public Change()
        {
        }

        public Change(VersionController versionController, string guid, string relativePath, string title, string description, VersionRef releaseVersion, object additionalData = null)
        {
            VersionController = versionController;
            Guid = guid;
            Title = title;
            Description = description;
            ReleaseVersion = releaseVersion;
            if (additionalData != null)
            {
                AdditionalData = JObject.FromObject(additionalData);
            }
        }

        [JsonIgnore]
        public VersionController VersionController { get; internal set; }

        /// <summary>
        /// A random Guid used to identify this change.
        /// </summary>
        [JsonProperty("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// A short title representing this change.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// A description of the change.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// A reference to the version this change was included in.
        /// </summary>
        [JsonProperty("release_version")]
        public VersionRef ReleaseVersion { get; set; }

        /// <summary>
        /// Store any additional data with each Change in this <see cref="JObject"/>.
        /// </summary>
        [JsonProperty("additional_data")]
        public JObject AdditionalData { get; set; }

        /// <summary>
        /// Returns instance of <see cref="Change"/> from JSON string.
        /// </summary>
        /// <param name="json">JSON string to deserialize.</param>
        /// <param name="versionController">Version Controller reference used to cache the change object.</param>
        /// <returns>Returns Change.</returns>
        public static Change FromJson(string json, VersionController versionController)
        {
            Change change = JsonConvert.DeserializeObject<Change>(json);
            if (change != null)
            {
                change.VersionController = versionController;
                change.ReleaseVersion.VersionController = versionController;
            }

            return change;
        }
    }
}
