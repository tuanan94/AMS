using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Enum
{
    [Flags]
    public enum StatusEnum
    {
        Open = 1,
        WaitingForQuotation = 2,
        WaitingQuoutationConfirming = 3,
        QuoutationConfirmed = 4,
        WaitingForProcess = 5,
        Processing = 6,
        Done = 7,
        Closed = 8,
        Reopen = 9,
        Reject = 10
    }
}