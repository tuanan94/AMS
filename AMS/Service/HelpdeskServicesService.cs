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

        public void Add(HelpdeskService service)
        {
            hdServiceRepository.Add(service);
        }
    }
}