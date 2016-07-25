using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class AroundProviderService
    {
        GenericRepository<AroundProvider> _arroundProviderRepository = new GenericRepository<AroundProvider>();

        public List<AroundProvider> GetAllProviders()
        {
            return _arroundProviderRepository.List.ToList();
        }

        public List<AroundProvider> GetAllProviderWithCat(String cat)
        {
            List<AroundProvider> result = new List<AroundProvider>();
            int intCat = -1;
            if (cat == null || cat.Equals("") || int.TryParse(cat, out intCat) == false)
            {
                result = GetAllProviders();
            }
            else
            {
                result = _arroundProviderRepository.List.Where(c => c.AroundProviderCategoryId == intCat).ToList();
            }
            return result;
        }

        public List<AroundProvider> GetTopTenproviderOrderByView()
        {
            return _arroundProviderRepository.List.OrderByDescending(pro => pro.ClickCount).Take(10).ToList();
        }

        public List<AroundProvider> GetProvidersByCategory(int categoryId)
        {
            return _arroundProviderRepository.List.Where(c => c.AroundProviderCategoryId == categoryId).ToList();
        }

        public AroundProvider GetProvider(int id)
        {
            return _arroundProviderRepository.FindById(id);
        }

        public void Update(AroundProvider provider)
        {
            _arroundProviderRepository.Update(provider);
        }

        public AroundProvider FindById(int id)
        {
            return _arroundProviderRepository.FindById(id);
        }
        public void Delete(AroundProvider e)
        {
            _arroundProviderRepository.Delete(e);
        }
        public void DeleteById(int id)
        {
            AroundProvider e = _arroundProviderRepository.FindById(id);
            if (e != null)
            {
                _arroundProviderRepository.Delete(e);
            }
        }
        public void Add(AroundProvider e)
        {
            _arroundProviderRepository.Add(e);
        }
        public AroundProvider FindByIdAfterAdd(AroundProvider e)
        {
            return _arroundProviderRepository.FindByIdAfterAdd(e, e.Id);
        }
    }
}