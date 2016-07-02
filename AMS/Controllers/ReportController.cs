using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Service;
using Microsoft.AspNet.Identity;

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
            List<ReportedPostItem> reportedPostItems = new List<ReportedPostItem>();// Những items trả qua view để hiện lên
            List<Report> listReports = reportService.GetListReport();
            foreach (Report p in listReports)
            {
                if (p.PostId != null)
                {
                    int index = reportedPostItems.FindIndex(m => m.post.Id == p.PostId.Value);
                    if (index == -1)
                    {

                        ReportedPostItem reportedPostItem = new ReportedPostItem();
                        reportedPostItem.post = p.Post;
                        reportedPostItem.users = reportService.findAllReportedUserByPostId(p.PostId.Value);
                        reportedPostItems.Add(reportedPostItem);
                    }
                }
             

            }
            ViewBag.reportedPostItems = reportedPostItems;
            return View("Report");
        }

        [Authorize]
        [HttpPost]
        public bool addReport(int postId, String reportContent)
        {
            Report p = new Report();
            p.CreatedDate = DateTime.Now;
            p.UserId = int.Parse(User.Identity.GetUserId());
            p.PostId = postId;
            p.ReportContent = reportContent;
            reportService.AddReport(p);
            return true;
        }

        [Authorize]
        [HttpPost]
        public bool hiddenPost(int postid)
        {
            Post p = postService.findPostById(postid);
            if (p == null)
            {
                return false;
            }
            p.Status = SLIM_CONFIG.POST_STATUS_HIDE;
            postService.UpdatePost(p);
            return true;

        }
        //[HttpPost]
        public ActionResult ApproveReport(int reportId, int postId)
        {

            //string[] postId = Request.Form.GetValues("PostId");
            //string[] reportId = Request.Form.GetValues("ReportId");
            Post post = postService.findPostById(postId);
            Report report = reportService.FindReportById(reportId);
           // report.Disable = 1;
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
           // report.Disable = 1;
            reportService.UpdateReport(report);
           
            return RedirectToAction("Report");
            
        }
    }
    public class ReportedPostItem{
        public Post post { get; set; }
        public List<User> users { get; set; }

        public ReportedPostItem()
        {
            post = null;
            users = new List<User>();
        }

        public override bool Equals(object obj)
        {
            if(post == null)
            {
                return false;
            }
            if(obj == null)
            {
                return false;
            }
            return post.Id == ((Post)obj).Id;
        }
        public override int GetHashCode()
        {
            return post.GetHashCode();
        }
    }
}