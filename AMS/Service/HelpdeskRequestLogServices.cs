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

        public HelpdeskRequestLog findById(int id)
        {
            return logRepository.FindById(id);
        }

        public void Add(HelpdeskRequestLog log)
        {
            logRepository.Add(log);
        }
        public void Update(HelpdeskRequestLog log)
        {
            logRepository.Update(log);
        }

        public HelpdeskRequestLog FindLastHelpdeskRequestLog(int id)
        {
           return logRepository.List.Where(l => l.HelpdeskRequestId == id).OrderByDescending(l => l.CreateDate).First();
        }

        public List<HelpdeskRequestLog> GetHelpdeskRequestLog(int id)
        {
            return logRepository.List.Where(l => l.HelpdeskRequestId == id).OrderBy(l => l.CreateDate).ToList();
        }

        //GiangLVT - Log history: who did made change
        public List<HelpdeskRequestLog> GetHelpdeskRequestLogByUser(int userId)
        {
            return logRepository.List.Where(l => l.ChangeFromUserId == userId).OrderBy(l => l.CreateDate).ToList();
        }
    }
}