using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class HelpdeskRequestCatService
    {
        GenericRepository<HelpdeskRequestCategory> helpdeskRequestCatRepository = new GenericRepository<HelpdeskRequestCategory>();

        public List<HelpdeskRequestCategory> GetAll()
        {
            return helpdeskRequestCatRepository.List.ToList();
        }
        public HelpdeskRequestCategory FindById(int id)
        {
            return helpdeskRequestCatRepository.FindById(id);
        }
        public void Add(HelpdeskRequestCategory hdReqCategory)
        {
            helpdeskRequestCatRepository.Add(hdReqCategory);
        }
        public void Update(HelpdeskRequestCategory hdReqCategory)
        {
            helpdeskRequestCatRepository.Update(hdReqCategory);
        }
        public void Delete(HelpdeskRequestCategory hdReqCategory)
        {
            helpdeskRequestCatRepository.Delete(hdReqCategory);
        }
        public List<HelpdeskRequestCategory> FindByName(string name)
        {
            return helpdeskRequestCatRepository.List.Where(c => c.Name.ToLower().Contains(name.ToLower())).ToList();
        }
    }
}