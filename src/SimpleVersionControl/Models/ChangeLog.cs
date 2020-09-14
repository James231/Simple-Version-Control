// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleVersionControl
{
    public class ChangeLog
    {
        public ChangeLog()
        {
        }

        public ChangeLog(VersionController versionController, List<VersionRef> versions, object additionalData = null)
        {
            VersionController = versionController;
            Versions = versions;
            if (additionalData != null)
            {
                AdditionalData = JObject.FromObject(additionalData);
            }
        }

        [JsonIgnore]
        public VersionController VersionController { get; internal set; }

        /// <summary>
        /// List of versions of the application.
        /// </summary>
        [JsonProperty("versions")]
        public List<VersionRef> Versions { get; set; }

        /// <summary>
        /// Store any additional data with each Change in this <see cref="JObject"/>.
        /// </summary>
        [JsonProperty("additional_data")]
        public JObject AdditionalData { get; set; }

        /// <summary>
        /// Returns instance of <see cref="ChangeLog"/> from JSON string.
        /// </summary>
        /// <param name="json">JSON string to deserialize.</param>
        /// <param name="versionController">Version Controller reference used to cache the ChangeLog object.</param>
        /// <returns>Returns ChangeLog.</returns>
        public static ChangeLog FromJson(string json, VersionController versionController)
        {
            ChangeLog changeLog = JsonConvert.DeserializeObject<ChangeLog>(json);
            if (changeLog != null)
            {
                changeLog.VersionController = versionController;
                changeLog.Versions.ToList().ForEach(v => v.VersionController = versionController);
            }

            return changeLog;
        }
    }
}
