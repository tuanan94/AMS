using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;
using Microsoft.Owin.Security.Provider;

namespace AMS.Service
{

    public class UtilityServiceServices
    {
        GenericRepository<UtilityService> _utilityServiceRepository = new GenericRepository<UtilityService>();

        public void Add(UtilityService u)
        {
            _utilityServiceRepository.Add(u);
        }
        public void Update(UtilityService u)
        {
            _utilityServiceRepository.Update(u);
        }
        public void Delete(UtilityService u)
        {
            _utilityServiceRepository.Delete(u);
        }
        public void DeleteById(int id)
        {
            var utilSrv = _utilityServiceRepository.FindById(id);
            if (utilSrv != null)
            {
                _utilityServiceRepository.Delete(utilSrv);
            }
        }
        public UtilityService FindById(int id)
        {
            return _utilityServiceRepository.FindById(id);
        }
        public List<UtilityService> GetServicesByType(int type)
        {
            return _utilityServiceRepository.List.Where(s => s.Type == type).ToList();
        }
    }
    public class UtilityServiceRangePriceServices
    {
        GenericRepository<UtilityServiceRangePrice> _UtilityServiceRangePriceRepository = new GenericRepository<UtilityServiceRangePrice>();

        public void Add(UtilityServiceRangePrice u)
        {
            _UtilityServiceRangePriceRepository.Add(u);
        }
        public void Update(UtilityServiceRangePrice u)
        {
            _UtilityServiceRangePriceRepository.Update(u);
        }
        public UtilityServiceRangePrice FindById(int id)
        {
            return _UtilityServiceRangePriceRepository.FindById(id);
        }
        public void DeleteById(int id)
        {
            UtilityServiceRangePrice rangePrice = _UtilityServiceRangePriceRepository.FindById(id);
            if (rangePrice != null) _UtilityServiceRangePriceRepository.Delete(rangePrice);
        }
    }
}