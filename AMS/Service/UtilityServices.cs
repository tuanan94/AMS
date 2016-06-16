using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

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
    }
}