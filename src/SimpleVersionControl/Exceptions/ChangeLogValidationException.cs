// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;

namespace SimpleVersionControl
{
    public class ChangeLogValidationException : Exception
    {
        public ChangeLogValidationException(string msg)
            : base(msg)
        {
        }
    }
}
