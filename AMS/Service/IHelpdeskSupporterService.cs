using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Service
{
    interface IHelpdeskSupporterService
    {
        IEnumerable<HelpdeskRequest> ListAllRequest();
        HelpdeskRequest GetHelpdeskRequest(int id);
        bool UpdateHelpdeskRequest(HelpdeskRequest request);
        bool UpdateStatus(int currId, int status);
    }
}
