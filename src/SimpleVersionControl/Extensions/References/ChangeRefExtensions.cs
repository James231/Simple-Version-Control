// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;

namespace SimpleVersionControl
{
    public static class ChangeRefExtensions
    {
        /// <summary>
        /// Asynchronously retrieves <see cref="Change"/> object from the filesystem given a reference to it.
        /// </summary>
        /// <param name="changeRef">Reference to the JSON file.</param>
        /// <param name="cache">Should the resulting Change be cached?</param>
        /// <returns>Returns Change object.</returns>
        public static async Task<Change> GetChange(this ChangeRef changeRef, bool cache = true)
        {
            Change change = changeRef.VersionController.CachedChanges.FirstOrDefault(c => c.Guid == changeRef.Guid);
            if (change != null)
            {
                return change;
            }

            string changeJson = await changeRef.VersionController.FileProvider.GetFile(changeRef.RelativeFilePath);
            if (changeJson == null)
            {
                return null;
            }

            change = Change.FromJson(changeJson, changeRef.VersionController);
            if (change != null && cache)
            {
                changeRef.VersionController.CachedChanges.Add(change);
            }

            return change;
        }
    }
}
