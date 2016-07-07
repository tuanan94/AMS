using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class ServiceChargeSevices
    {
        GenericRepository<UtilityService> _serviceChargeRepository = new GenericRepository<UtilityService>();

        public UtilityService FindById(int id)
        {
            return _serviceChargeRepository.FindById(id);
        }
        public void Add(UtilityService serviceFee)
        {
            _serviceChargeRepository.Add(serviceFee);
        }
        public void Update(UtilityService serviceFee)
        {
            _serviceChargeRepository.Update(serviceFee);
        }
        public void Delete(UtilityService serviceFee)
        {
            _serviceChargeRepository.Delete(serviceFee);
        }
        public void DeleteById(int id)
        {
            UtilityService s = _serviceChargeRepository.FindById(id);
            if (s != null)
            {
                _serviceChargeRepository.Delete(s);
            }
        }
    }
}