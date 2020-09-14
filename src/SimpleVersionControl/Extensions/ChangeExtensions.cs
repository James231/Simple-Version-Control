// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace SimpleVersionControl
{
    public static class ChangeExtensions
    {
        /// <summary>
        /// Serializes instance of <see cref="Change"/> to JSON string.
        /// </summary>
        /// <param name="change">Instance of <see cref="Change"/> to serialize.</param>
        /// <returns>Returns JSON string.</returns>
        public static string ToJson(this Change change)
        {
            return JsonConvert.SerializeObject(change, Formatting.Indented);
        }

        /// <summary>
        /// Throws <see cref="ChangeLogValidationException"/> if version references are not valid.
        /// </summary>
        /// <param name="change">Instance of <see cref="Change"/> to check version references on.</param>
        /// <param name="changeLog">The instance of <see cref="ChangeLog"/> listing all versions.</param>
        public static void Validate(this Change change, ChangeLog changeLog)
        {
            if (change.ReleaseVersion == null)
            {
                throw new ChangeLogValidationException("Change has Release version reference missing.");
            }

            bool found = false;
            foreach (VersionRef versionRef in changeLog.Versions)
            {
                if (versionRef.VersionName == change.ReleaseVersion.VersionName && versionRef.RelativeFilePath == change.ReleaseVersion.RelativeFilePath)
                {
                    found = true;
                }
            }

            if (!found)
            {
                throw new ChangeLogValidationException("Change has invalid version reference.");
            }
        }
    }
}