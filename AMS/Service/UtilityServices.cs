using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class UtilityCategoryServices
    {
        GenericRepository<UtilityServiceCategory> _utilityServiceCatRepository = new GenericRepository<UtilityServiceCategory>();

        public UtilityServiceCategory FindByType(int type)
        {
            List<UtilityServiceCategory> utilSrvCategories =
                _utilityServiceCatRepository.List.Where(uCat => uCat.Type.Value == type).ToList();
            return utilSrvCategories.Count == 0 ? null : utilSrvCategories.First();
        }

        public void Add(UtilityServiceCategory u)
        {
            _utilityServiceCatRepository.Add(u);
        }
        public void Update(UtilityServiceCategory u)
        {
            _utilityServiceCatRepository.Update(u);
        }
        public UtilityServiceCategory FindById(int id)
        {
            return _utilityServiceCatRepository.FindById(id);
        }
    }

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
        public List<UtilityService> GetServicesByCatId(int id)
        {
            return _utilityServiceRepository.List.Where(s => s.CategoryId == id).ToList();
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