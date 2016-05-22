using System;
using System.Collections.Generic;
using System.Linq;
using AMS.Reposiroty;

namespace AMS.Service
{
    public class HelpdeskServicesService
    {
         GenericRepository<HelpdeskService>  hdServiceRepository = new GenericRepository<HelpdeskService>();

        public List<HelpdeskService> GetHelpdeskServices()
        {
            return hdServiceRepository.List.ToList();
        }

        public HelpdeskService FindById(int id)
        {
            return hdServiceRepository.FindById(id);
        }

        public void Add(HelpdeskService service)
        {
            hdServiceRepository.Add(service);
        }
        public void Update(HelpdeskService service)
        {
            hdServiceRepository.Update(service);
        }

    }
}