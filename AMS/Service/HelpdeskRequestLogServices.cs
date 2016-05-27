using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class HelpdeskRequestLogServices
    {
        GenericRepository<HelpdeskRequestLog> logRepository = new GenericRepository<HelpdeskRequestLog>();

        public void Add(HelpdeskRequestLog log)
        {
            logRepository.Add(log);
        }

        public HelpdeskRequestLog FindLastHelpdeskRequestLog(int id)
        {
           return logRepository.List.Where(l => l.HelpdeskRequestId == id).OrderByDescending(l => l.CreateDate).First();
        }

        public List<HelpdeskRequestLog> GetHelpdeskRequestLog(int id)
        {
            return logRepository.List.Where(l => l.HelpdeskRequestId == id).OrderBy(l => l.CreateDate).ToList();
        }
    }
}