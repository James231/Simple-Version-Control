﻿// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Threading.Tasks;

namespace SimpleVersionControl
{
    public interface IFileSaver
    {
        Task<bool> SaveFile(string relativePath, string content);

        Task<bool> PublishFromTemp();

        Task CancelPublish();
    }
}
