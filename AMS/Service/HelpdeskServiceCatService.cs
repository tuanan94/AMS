using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Reposiroty;

namespace AMS.Service
{
    public class HelpdeskServiceCatService
    {
        GenericRepository<HelpdeskServiceCategory> helpdeskServiceCatRepository = new GenericRepository<HelpdeskServiceCategory>();

        public List<HelpdeskServiceCategory> GetAll()
        {
            return helpdeskServiceCatRepository.List.ToList();
        }
    }
}