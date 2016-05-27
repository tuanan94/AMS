using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class HelpdeskServiceCatService
    {
        GenericRepository<HelpdeskServiceCategory> helpdeskServiceCatRepository = new GenericRepository<HelpdeskServiceCategory>();

        public List<HelpdeskServiceCategory> GetAll()
        {
            return helpdeskServiceCatRepository.List.ToList();
        }
        public HelpdeskServiceCategory FindById(int id)
        {
            return helpdeskServiceCatRepository.FindById(id);
        }
        public void Add(HelpdeskServiceCategory hdSrvCategory)
        {
            helpdeskServiceCatRepository.Add(hdSrvCategory);
        }
        public void Update(HelpdeskServiceCategory hdSrvCategory)
        {
            helpdeskServiceCatRepository.Update(hdSrvCategory);
        }
        public void Delete(HelpdeskServiceCategory hdSrvCategory)
        {
            helpdeskServiceCatRepository.Delete(hdSrvCategory);
        }
        public List<HelpdeskServiceCategory> FindByName(string name)
        {
            return helpdeskServiceCatRepository.List.Where(c => c.Name.ToLower().Contains(name.ToLower())).ToList();
        }
    }
}