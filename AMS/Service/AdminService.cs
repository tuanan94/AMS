using AMS.Reposiroty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Service
{
    public class AdminService
    {
        GenericRepository<House> allHouseInfo = new GenericRepository<House>();
        // get house's info
        // return list house's info
        public List<House> getAllHouseInfo()
        {
            return allHouseInfo.List.ToList();
        }
    }
}