using System;
using System.Collections.Generic;
using System.Linq;
using AMS.Repository;

namespace AMS.Service
{
    public class HelpdeskServicesService
    {
        GenericRepository<HelpdeskService> hdServiceRepository = new GenericRepository<HelpdeskService>();

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
        public void Delete(HelpdeskService service)
        {
            hdServiceRepository.Delete(service);
        }

        public List<HelpdeskService> FindByCategoryAndName(string name, int catId)
        {
            if (null != name)
            {
                return
                    hdServiceRepository.List.Where(s => s.HelpdeskServiceCategoryId == catId && s.Name.ToLower().Contains(name.ToLower()))
                        .ToList();
            }
            return hdServiceRepository.List.Where(s => s.HelpdeskServiceCategoryId == catId)
                    .ToList();
        }
        public List<HelpdeskService> FindByStatusAndName(string name, int status)
        {
            if (null != name)
            {
                return
                    hdServiceRepository.List.Where(s => s.Status == status && s.Name.ToLower().Contains(name.ToLower()))
                        .ToList();
            }
            return hdServiceRepository.List.Where(s => s.HelpdeskServiceCategoryId == status)
                    .ToList();
        }
        public List<HelpdeskService> FindByName(string name)
        {
            return
                hdServiceRepository.List.Where(s => s.Name.ToLower().Contains(name.ToLower())).ToList();
        }
        public List<HelpdeskService> FindByCategoryAndEnable(int catId)
        {
            return
                hdServiceRepository.List.Where(s => s.HelpdeskServiceCategoryId == catId && s.Status == 1).OrderBy(s => s.LastModified).ToList();
        }
    }

}