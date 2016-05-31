using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class ServiceChargeSevices
    {
        GenericRepository<ServiceFee> _serviceChargeRepository = new GenericRepository<ServiceFee>();

        public void Add(ServiceFee serviceFee)
        {
            _serviceChargeRepository.Add(serviceFee);
        }
    }
}