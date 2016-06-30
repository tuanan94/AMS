using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using AMS.Repository;
using AMS.Helper;

namespace AMS.Service
{
    public class AroundProviderProductService : GenericRepository<AroundProviderProduct>
    {
        readonly GenericRepository<AroundProviderProduct> _repository = new GenericRepository<AroundProviderProduct>();
        public List<AroundProviderProduct> GetAroundProviderProduct(int providerId)
        {
            return _repository.List.Where(p => p.AroundProviderId == providerId).ToList();
        }

    }
}