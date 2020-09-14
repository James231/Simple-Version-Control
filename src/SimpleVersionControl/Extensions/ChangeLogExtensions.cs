// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Linq;
using Newtonsoft.Json;

namespace SimpleVersionControl
{
    public static class ChangeLogExtensions
    {
        /// <summary>
        /// Serializes instance of <see cref="ChangeLog"/> to JSON string.
        /// </summary>
        /// <param name="changeLog">Instance of <see cref="ChangeLog"/> to serialize.</param>
        /// <returns>Returns JSON string.</returns>
        public static string ToJson(this ChangeLog changeLog)
        {
            return JsonConvert.SerializeObject(changeLog, Formatting.Indented);
        }

        /// <summary>
        /// Throws <see cref="ChangeLogValidationException"/> if version names are not unique.
        /// </summary>
        /// <param name="changeLog">Instance of <see cref="ChangeLog"/> to check version names from.</param>
        public static void Validate(this ChangeLog changeLog)
        {
            changeLog.Versions.RemoveAll(item => item == null);
            bool versionNamesUnique = changeLog.Versions.Select(v => v.VersionName).Distinct().Count() == changeLog.Versions.Count;
            if (!versionNamesUnique)
            {
                throw new ChangeLogValidationException("Duplicate Version Name Found. Version Names should be unique.");
            }
        }
    }
}
