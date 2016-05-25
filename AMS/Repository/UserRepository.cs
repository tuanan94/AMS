using AMS.Reposiroty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Repository
{
    public class UserRepository : GenericRepository<User>
    {
        public User findByUsername(String username)
        {
            if(username == null)
            {
                return null;
            }
            var result = (from r in db.Users where username.Equals(r.Username) select r).FirstOrDefault();
            return result;
        }
        public List<User> findByHouseID(int houseId)
        {
            var result = (from r in db.Users where r.HouseId == houseId select r).ToList();
            return result;
        }
       
    }
}