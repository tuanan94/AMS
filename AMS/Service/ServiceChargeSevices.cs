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

        public ServiceFee FindById(int id)
        {
            return _serviceChargeRepository.FindById(id);
        }
        public void Add(ServiceFee serviceFee)
        {
            _serviceChargeRepository.Add(serviceFee);
        }
        public void Update(ServiceFee serviceFee)
        {
            _serviceChargeRepository.Update(serviceFee);
        }
        public void Delete(ServiceFee serviceFee)
        {
            _serviceChargeRepository.Delete(serviceFee);
        }
        public void DeleteById(int id)
        {
            ServiceFee s = _serviceChargeRepository.FindById(id);
            if (s != null)
            {
                _serviceChargeRepository.Delete(s);
            }
        }
    }
}