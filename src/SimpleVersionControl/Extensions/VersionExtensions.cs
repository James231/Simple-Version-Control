// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Linq;
using Newtonsoft.Json;

namespace SimpleVersionControl
{
    public static class VersionExtensions
    {
        /// <summary>
        /// Serializes instance of <see cref="Version"/> to JSON string.
        /// </summary>
        /// <param name="version">Instance of <see cref="Version"/> to serialize.</param>
        /// <returns>Returns JSON string.</returns>
        public static string ToJson(this Version version)
        {
            return JsonConvert.SerializeObject(version, Formatting.Indented);
        }

        /// <summary>
        /// Throws <see cref="ChangeLogValidationException"/> if change guids are not unique.
        /// </summary>
        /// <param name="version">Instance of <see cref="Version"/> to check change guids from.</param>
        public static void Validate(this Version version)
        {
            version.Changes.RemoveAll(item => item == null);
            bool changeNamesUnique = version.Changes.Select(c => c.Guid).Distinct().Count() == version.Changes.Count;
            if (!changeNamesUnique)
            {
                throw new ChangeLogValidationException("Duplicate Change GUID Name Found. Change GUIDs should be unique.");
            }
        }
    }
}