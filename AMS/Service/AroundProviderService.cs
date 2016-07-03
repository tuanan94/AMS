using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class AroundProviderService : GenericRepository<AroundProvider>
    {
        readonly GenericRepository<AroundProvider> _repository = new GenericRepository<AroundProvider>();
        readonly GenericRepository<AroundProviderCategory> _aroundProviderCategoryRepository = new GenericRepository<AroundProviderCategory>();
        public List<AroundProvider> GetAllProviders()
        {
            return _repository.List.ToList();
        }
        public List<AroundProvider> GetAllProviderWithCat(String cat)
        {
            List<AroundProvider> result = new List<AroundProvider>();
            int intCat = -1;
            if (cat == null || cat.Equals("")||int.TryParse(cat,out intCat)==false)

            {
                result = GetAllProviders();
            }
            else
            {
                result = _repository.List.Where(c => c.AroundProviderCategoryId == intCat).ToList();
            }
            return result;
        }

        public List<AroundProvider> GetProvidersByCategory(int categoryId)
        {
            return _repository.List.Where(c => c.AroundProviderCategoryId == categoryId).ToList();
        }

        public AroundProvider GetProvider(int id)
        {
            return _repository.FindById(id);
        }
        public List<AroundProviderCategory> getAllCategory()
        {
            return _aroundProviderCategoryRepository.List.ToList();
        }
    }
}