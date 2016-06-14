using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Service;

namespace AMS.Controllers
{
    public class ReportController : Controller
    {
        ReportService reportService = new ReportService();
        PostService postService = new PostService();
        UserReportService userReportService = new UserReportService();
        UserServices userServices = new UserServices();
        ImageService imageService = new ImageService();
        //
        // GET: /Report/
        public ActionResult Report()
        {
            List<Report> listReports = reportService.GetListReport();
            List<Post> listPosts = new List<Post>();
            List<int> listCountReport = new List<int>();
            List<User> userReport = new List<User>();
            List<UserReportPost> listusUserReportPosts = userReportService.GetListUserReportPost();
            List<Image> listImage = new List<Image>();
            foreach (var obj in listusUserReportPosts)
            {
               listPosts.Add(postService.findPostById(obj.PostId.Value));
                userReport.Add(userServices.FindById(obj.UserId.Value));
                listImage.Add(imageService.getImageByPostId(obj.PostId.Value));
            }
            foreach (var item in listPosts)
            {
                listCountReport.Add(userReportService.CountReport(item.Id));

            }
            ViewBag.ListImage = listImage;
            ViewBag.ListUser = userReport;
            ViewBag.ListPost = listPosts;
            ViewBag.CountReport = listCountReport;
            ViewBag.ListReport = listReports;
            return View("Report");
        }

        //[HttpPost]
        public ActionResult ApproveReport(int reportId, int postId)
        {
          
            //string[] postId = Request.Form.GetValues("PostId");
            //string[] reportId = Request.Form.GetValues("ReportId");
            Post post = postService.findPostById(postId);
            Report report = reportService.FindReportById(reportId);
            report.Disable = 1;
            reportService.UpdateReport(report);
            post.Disable = 1;
            postService.UpdatePost(post);
            return RedirectToAction("Report");
        }
        //[HttpPost]
        public ActionResult RejectReport(int reportId)
        {
           
         
           // string[] reportId = Request.Form.GetValues("ReportId");
          
            Report report = reportService.FindReportById((reportId));
            report.Disable = 1;
            reportService.UpdateReport(report);
           
            return RedirectToAction("Report");
        }
	}
}