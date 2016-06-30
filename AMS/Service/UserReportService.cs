using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class UserReportService
    {
        GenericRepository<UserReportPost> reportRepository = new GenericRepository<UserReportPost>();
        public List<UserReportPost> GetListUserReportPost()
        {
            return reportRepository.List.OrderByDescending(t => t.ReportId).ToList();// AnLTNM Change because it not found

        }
        public UserReportPost FindUserReportPostById(int id)
        {
            return reportRepository.FindById(id);
        }

        public int CountReport(int id)
        {
            return reportRepository.List.Count(t => t.PostId == id);
        }

        public void AddUserReportPost(UserReportPost obj)
        {
            reportRepository.Add(obj);
        }
    }
}