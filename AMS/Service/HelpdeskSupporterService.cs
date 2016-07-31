using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Enum;
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

        public HelpdeskRequest GetHelpdeskRequest(int id)
        {
            HelpdeskRequest hr = new HelpdeskRequest();
            return hr = _helpdeskSupporterRepository.FindById(id);
        }

        public bool UpdateHelpdeskRequest(HelpdeskRequest request)
        {
            try
            {
                _helpdeskSupporterRepository.Update(request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        public bool UpdateStatus(int currId, int status)
        {
            HelpdeskRequest currRequest = GetHelpdeskRequest(currId);
            currRequest.Status = status;
            try
            {
                if (status == (int) StatusEnum.Done)
                {
                    currRequest.DoneDate = DateTime.Now;
                }
                _helpdeskSupporterRepository.Update(currRequest);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}