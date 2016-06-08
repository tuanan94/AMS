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
        public List<AroundProvider> GetAllProviders()
        {
            return _repository.List.ToList();
        }

        public List<AroundProvider> GetProvidersByCategory(int categoryId)
        {
            return _repository.List.Where(c => c.AroundProviderCategoryId == categoryId).ToList();
        }

        public AroundProvider GetProvider(int id)
        {
            return _repository.FindById(id);
        }
    }
}