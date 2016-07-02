using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class ReportService
    {
        GenericRepository<Report> reportRepository = new GenericRepository<Report>();

        public List<Report> GetListReport()
        {
            return reportRepository.List.OrderByDescending(t => t.Id).ToList();
        }
        public Report FindReportById(int id)
        {
            return reportRepository.FindById(id);
        }


        public void AddReport(Report obj)
        {
            reportRepository.Add(obj);
        }
        public void UpdateReport(Report obj)
        {
            reportRepository.Update(obj);
        }
        public List<User> findAllReportedUserByPostId(int id)
        {
            List<User> result = new List<User>();
            List<Report> allReport = GetListReport();
            foreach(Report rp in allReport)
            {
                if(rp.Post!=null && rp.Post.Id == id && rp.User!=null)
                {
                    result.Add(rp.User);   
                }
            }
            return result;
        }

    }
}