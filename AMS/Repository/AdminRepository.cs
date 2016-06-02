using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using AMS.Repository;


namespace AMS.Repository
{
    public class AdminRepository : GenericRepository<House>
    {
        public List<House> findByHouseID(int Id)
        {
            var result = (from r in db.Houses where r.Id == Id select r).ToList();
            return result;
        }
    }
}