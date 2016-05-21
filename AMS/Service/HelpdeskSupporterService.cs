using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class HelpdeskSupporterService : IHelpdeskSupporterService
    {
        private HelpdeskSupporterRepository _helpdeskSupporterRepository;
        public HelpdeskSupporterService()
        {
            _helpdeskSupporterRepository = new HelpdeskSupporterRepository();
        }

        public IEnumerable<HelpdeskRequest> ListAllRequest()
        {
            return _helpdeskSupporterRepository.ListAll().ToList();
        }
    }
}