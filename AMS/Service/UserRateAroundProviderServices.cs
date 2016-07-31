using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class UserRateAroundProviderServices
    {
        GenericRepository<UserRateAroundProvider> _userRateAroundServiceRepository = new GenericRepository<UserRateAroundProvider>();

        public void Add(UserRateAroundProvider e)
        {
            _userRateAroundServiceRepository.Add(e);
        }
        public void Reload(UserRateAroundProvider e)
        {
            _userRateAroundServiceRepository.Reload(e);
        }
    }
}