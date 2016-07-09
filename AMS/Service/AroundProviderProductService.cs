using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using AMS.Repository;
using AMS.Helper;

namespace AMS.Service
{
    public class AroundProviderProductService
    {
        GenericRepository<AroundProviderProduct> _repository = new GenericRepository<AroundProviderProduct>();
        public List<AroundProviderProduct> GetAroundProviderProduct(int providerId)
        {
            return _repository.List.Where(p => p.AroundProviderId == providerId).ToList();
        }
        public void Add(AroundProviderProduct e)
        {
            _repository.Add(e);
        }
        public void Update(AroundProviderProduct e)
        {
            _repository.Update(e);
        }
        public void Delete(AroundProviderProduct e)
        {
            _repository.Delete(e);
        }
        public void DeleteById(int id)
        {
            AroundProviderProduct e = _repository.FindById(id);
            if (e != null)
            {
                _repository.Update(e);
            }
        }
        public AroundProviderProduct FindById(int id)
        {
            return _repository.FindById(id);
        }
    }
    public class AroundProviderCategoryService
    {
        GenericRepository<AroundProviderCategory> _repository = new GenericRepository<AroundProviderCategory>();
        public List<AroundProviderCategory> GetAllOrderByProviderClickCount()
        {
            return _repository.List.OrderByDescending(cat => (cat.AroundProviders.Sum(prov => prov.ClickCount))).ToList();
        }
        public List<AroundProviderCategory> GetAll()
        {
            return _repository.List.ToList();
        }
        public bool CheckNameIsExited(string name)
        {
            return _repository.List.Any(proCat => proCat.Name.Equals(name));
        }
        public bool CheckNewCatNameIsExited(int catId, string name)
        {
            return _repository.List.Any(proCat => proCat.Name.Equals(name) && proCat.Id != catId);
        }
        public void Add(AroundProviderCategory e)
        {
            _repository.Add(e);
        }
        public void Update(AroundProviderCategory e)
        {
            _repository.Update(e);
        }
        public void Delete(AroundProviderCategory e)
        {
            _repository.Delete(e);
        }
        public AroundProviderCategory FindById(int id)
        {
            return _repository.FindById(id);
        }

//        public List<AroundProviderCategory> GetAllGroupByClickCount()
//        {
//            return _repository.List.s.ToList();
//        }
    }
}