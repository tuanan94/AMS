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
//        GenericRepository<HelpdeskRequestHelpdeskSupporter> _repository = new GenericRepository<HelpdeskRequestHelpdeskSupporter>();
//        public List<HelpdeskRequestHelpdeskSupporter> GetCurrentHelpdeskRequest(int helpdeskSupporterId)
//        {
//            return _repository.List.Where(
//               s => s.HelpdeskSupporterId == helpdeskSupporterId && s.HelpdeskRequest.Status != (int)StatusEnum.Done &&
//                    s.HelpdeskRequest.Status != (int)StatusEnum.Close && s.HelpdeskRequest.Status != (int)StatusEnum.Cancel &&
//                   s.HelpdeskRequest.HelpdeskRequestHelpdeskSupporters.OrderByDescending(ee => ee.CreateDate).First().HelpdeskSupporterId == helpdeskSupporterId
//                   ).ToList();
//        }
//
//        public List<HelpdeskRequestHelpdeskSupporter> GetHelpdeskRequestById(int helpdeskSupporterId)
//        {
//            return _repository.List.Where(
//               s => s.HelpdeskSupporterId == helpdeskSupporterId).OrderByDescending(ee => ee.CreateDate).ToList();
//        }
    }
}