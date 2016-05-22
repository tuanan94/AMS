using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using AMS.Repository;

namespace AMS.Repository
{
    public class HelpdeskSupporterRepository : GenericRepository<HelpdeskRequest>, IHelpdeskSupporterRepository
    {
        public IEnumerable<HelpdeskRequest> ListAll()
        {
            return
                    table.Include(p => p.HelpdeskService).Include(p => p.HelpdeskRequestHelpdeskSupporters).Include(p => p.House);
        }



    }
}