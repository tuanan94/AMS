using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class HelpdeskRequestServices
    {
        GenericRepository<HelpdeskRequest> helpdeskReqRepository = new GenericRepository<HelpdeskRequest>();

        public int Add(HelpdeskRequest hdReg)
        {
            helpdeskReqRepository.Add(hdReg);
            return hdReg.Id;
        }

        public HelpdeskRequest FindById(int id)
        {
            return helpdeskReqRepository.FindById(id);
        }
        public List<HelpdeskRequest> GetHdRequestByHouseId(int houseId)
        {
            return 
                helpdeskReqRepository.List.Where(r => r.HouseId == houseId).OrderBy(r => r.ModifyDate).ToList();
        }
        
        public List<HelpdeskRequest> GetAllHelpdeskRequests()
        {
            return 
                helpdeskReqRepository.List.OrderByDescending(r => r.ModifyDate).ToList();
        }

        public void  Update (HelpdeskRequest hdRequest)
        {
            helpdeskReqRepository.Update(hdRequest);
        }

        
    }
}