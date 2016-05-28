using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Enum;
using AMS.Repository;

namespace AMS.Service
{
    public class HdReqHdSupporterServices
    {
        private GenericRepository<HelpdeskRequestHelpdeskSupporter> hdReqHdsupporterRepository =
            new GenericRepository<HelpdeskRequestHelpdeskSupporter>();

        public void Add(HelpdeskRequestHelpdeskSupporter hdReqHdsupporter)
        {
            hdReqHdsupporterRepository.Add(hdReqHdsupporter);
        }

        public void Update (HelpdeskRequestHelpdeskSupporter hdReqHdsupporter)
        {
            hdReqHdsupporterRepository.Update(hdReqHdsupporter);
        }
//        public List<HelpdeskRequestHelpdeskSupporter> GetCurrentSupporterHdRequest(int hdSupporterId)
//        {
//          return  hdReqHdsupporterRepository.List.Where(
//                s =>
//                    s.HelpdeskSupporterId == hdSupporterId &&
//                    (s.HelpdeskRequest.Status != (int) StatusEnum.Closed &&
//                     s.HelpdeskRequest.Status != (int) StatusEnum.Reject)).ToList();
//        }
        public List<HelpdeskRequestHelpdeskSupporter> GetCurrentSupporterHdRequest(int hdSupporterId)
        {
          return  hdReqHdsupporterRepository.List.Where(
                s => s.HelpdeskSupporterId == hdSupporterId && s.HelpdeskRequest.Status != (int)StatusEnum.Closed &&
                     s.HelpdeskRequest.Status != (int)StatusEnum.Reject && 
                    s.HelpdeskRequest.HelpdeskRequestHelpdeskSupporters.OrderByDescending(ee => ee.CreateDate).First().HelpdeskSupporterId == hdSupporterId
                    ).ToList();
        }
        
    }
}