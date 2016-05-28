using AMS.Constant;
using AMS.Enum;
using AMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Service
{
    public class HelpdeskRequestHelpdeskSupporterService : IHelpdeskRequestHelpdeskSupporterService
    {
        GenericRepository<HelpdeskRequestHelpdeskSupporter> _repository = new GenericRepository<HelpdeskRequestHelpdeskSupporter>();
        public List<HelpdeskRequestHelpdeskSupporter> GetCurrentHelpdeskRequest(int helpdeskSupporterId)
        {
            return _repository.List.Where(
               s => s.HelpdeskSupporterId == helpdeskSupporterId && s.HelpdeskRequest.Status != (int)StatusEnumNew.Done &&
                    s.HelpdeskRequest.Status != (int)StatusEnumNew.Close && s.HelpdeskRequest.Status != (int)StatusEnumNew.Cancel &&
                   s.HelpdeskRequest.HelpdeskRequestHelpdeskSupporters.OrderByDescending(ee => ee.CreateDate).First().HelpdeskSupporterId == helpdeskSupporterId
                   ).ToList();
        }
    }
}