using AMS.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Helper;
using AMS.ViewModel;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Constant;
using AMS.Enum;
using AMS.Models;
using AMS.ViewModel;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

using Newtonsoft.Json;

namespace AMS.Controllers
{
    public class HomeController : Controller
    {
        TestService testService = new TestService();
        UserInHouseService userInHouseService = new UserInHouseService();
        PostService postService = new PostService();
        UserService userService = new UserService();
        PendingMemberService pendingMemberService = new PendingMemberService();
        HelpdeskServiceCatService _helpdeskServiceCat = new HelpdeskServiceCatService();
        HelpdeskServicesService _helpdeskServices = new HelpdeskServicesService();
        UserServices _userServices = new UserServices();
        HelpdeskRequestServices _hdReqServices = new HelpdeskRequestServices();
        HelpdeskRequestLogServices _helpdeskRequestLogServices = new HelpdeskRequestLogServices();
        HdReqHdSupporterServices _hdReqHdSupporterServices = new HdReqHdSupporterServices();
        readonly string parternTime = "dd-MM-yyyy HH:mm";

        public ActionResult Test()
        {
            List<House> allHouse = testService.getAllHouse();
            ViewBag.allHouse = allHouse;
            return View(allHouse);
        }
        [Authorize]

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public void AddByAjax(string Title, int PostId)
        {
            postService.createPost(Title, PostId);

        }
        [HttpPost]
        public ActionResult Index(string Title, int PostId)
        {

            postService.createPost(Title, PostId);
            return RedirectToAction("TimeLine");
        }
        [HttpPost]
        public ActionResult Indexx(ListPostViewModel model, int postId)
        {

            postService.createPost(model.Title, model.Id);
            //return PartialView("TimeLine");
            return RedirectToAction("TimeLine");
        }

        public ActionResult TimeLine()
        {
            // get all post
            IEnumerable<Post> allPost = postService.getAllPost();
            IEnumerable<Post> listComment = new List<Post>();
            ListPostViewModel listPostViewModel = new ListPostViewModel();
            listPostViewModel.listPost = new List<PostViewModel>();
            foreach (var item in allPost)
            {
                PostViewModel postViewModel = new PostViewModel();
                postViewModel.ImgUrl = item.ImgUrl;
                postViewModel.Id = item.Id;
                postViewModel.Title = item.Title;
                postViewModel.CountComment = postService.CountComment(postViewModel.Id);
                if (item.CreateDate.HasValue)
                {
                    postViewModel.CreateDate = item.CreateDate.Value;
                }

                //get list comment belong post
                listComment = postService.getCommentBelongPost(postViewModel.Id);
                if (listComment != null)
                {
                    postViewModel.Post = listComment;
                }
                listPostViewModel.listPost.Add(postViewModel);

            }


            return View(listPostViewModel);
        }

        [HttpPost]
        public ActionResult TimeLine(PostViewModel post, string Title, HttpPostedFileBase Media)
        {
            string mediaUrl = null;


            //if (ModelState.IsValid)
            //{
            if (post.Media != null && post.Media.ContentLength > 0)
            {
                {

                    // Save dir
                    var uploadDir = "~/images/Post/Binh";
                    // File extentions
                    var ext = "";
                    //Modify ANTT
                    ext = Media.FileName.Substring(post.Media.FileName.LastIndexOf(".",
                            StringComparison.Ordinal));

                    // Force it to be jpg
                    var fileName = Util.GetUnixTime() + ".jpg";
                    // Tmp file name
                    var tmpFileName = "tmp" + Util.GetUnixTime() + ext;

                    string serverPath = Server.MapPath(uploadDir);
                    // tmp image path
                    var imagePath = Path.Combine(serverPath, tmpFileName);
                    if (!Directory.Exists(serverPath))
                    {
                        Directory.CreateDirectory(serverPath);
                    }
                    // Save as tmp file
                    post.Media.SaveAs(imagePath);
                    if (Util.ConvertImageToJpg(uploadDir, tmpFileName, ConstantB.DefaultImageQuality, uploadDir,
                        fileName))
                    {
                        mediaUrl = Path.Combine(uploadDir, fileName);


                    }

                    Util.DeleteFile(imagePath);
                }


            }
            Post p = new Post();
            // post.Body = p.Body;
            post.Title = Title;
            post.ImgUrl = mediaUrl;
            post.CreateDate = DateTime.Now;
            postService.createPost(post);
            //}
            return RedirectToAction("TimeLine");
        }

        [HttpPost]
        public ActionResult TimeLinex(ListPostViewModel post)
        {
            string mediaUrl = null;


            //if (ModelState.IsValid)
            //{

            Post p = new Post();
            // post.Body = p.Body;
            p.Title = post.Title;
            p.ImgUrl = post.ImgUrl;

            postService.CreatePosts(p);
            //}
            return RedirectToAction("TimeLine");
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        //BinhHT
        [HttpGet]
        public ActionResult ViewProfile()
        {
            User currentUser = userService.findById(int.Parse(User.Identity.GetUserId()));
            ViewBag.user = currentUser;

            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult ManageMember()
        {
            var currentUser = User.Identity;
            int userId;
            bool isValidID = int.TryParse(currentUser.GetUserId(), out userId);
            if (!isValidID)
            {
                return View("error");
            }
            User user = userService.findById(userId);
            List<User> members = userService.findByHouseId(user.Id);
            ViewBag.currentHouse = user.House;
            ViewBag.members = members;
            return View();
        }

        [HttpGet]
        [Route("Home/HelpdeskRequest/Create/{userId}")]
        public ActionResult CreateNewHdRequest(int userId)
        {

            List<HelpdeskServiceCategory> hdSrvCats = _helpdeskServiceCat.GetAll();
            ViewBag.hdSrvCats = hdSrvCats;
            ViewBag.userId = userId;
            return View("CreateHdRequest");
        }

        [HttpPost]
        [Route("Home/HelpdeskRequest/AddHdRequest")]
        public ActionResult AddHdRequest(HelpdeskRequestModel request)
        {
            MessageViewModels response = new MessageViewModels();
            try
            {
                User u = _userServices.FindById(request.HdReqUserId);
                HelpdeskService hdService = _helpdeskServices.FindById(request.HdServiceId);
                if (u != null && hdService != null)
                {
                    HelpdeskRequest hdRequest = new HelpdeskRequest();

                    hdRequest.HelpdeskServiceId = hdService.Id;
                    hdRequest.HouseId = u.HouseId;//AnTT them vao cho nay 27-5-2016
                    hdRequest.Price = hdService.Price;
                    hdRequest.CreateDate = DateTime.Now;
                    hdRequest.Description = request.HdReqUserDesc;
                    hdRequest.ModifyDate = DateTime.Now;
                    hdRequest.Priority = request.HdReqPrior;
                    hdRequest.Status = 0;
                    hdRequest.Title = request.HdReqTitle;

                    int id = _hdReqServices.Add(hdRequest);
                    HelpdeskRequestLog log = new HelpdeskRequestLog();
                    log.ChangeFromUserId = u.Id;
                    log.ChangeToUserId = u.Id;
                    log.HelpdeskRequestId = id;
                    log.StatusFrom = (int)StatusEnum.Open;
                    log.StatusTo = (int)StatusEnum.Open;
                    log.CreateDate = DateTime.Now;
                    _helpdeskRequestLogServices.Add(log);
                }
                else
                {
                    response.StatusCode = -1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                response.StatusCode = -1;
                return Json(response); ;
            }
            return Json(response);
        }

        [HttpGet]
        [Route("Home/HelpdeskRequest/ViewHistory/{userId}")]
        public ActionResult ViewHistoryHdRequest(int userId)
        {
            User u = _userServices.FindById(userId);
            if (u != null)
            {
                ViewBag.userId = u.Id;
            }
            return View("ViewHistoryHdRequests");
        }

        [HttpGet]
        [Route("Home/HelpdeskRequest/ViewDetail")]
        public ActionResult ViewHdRequestDetail(String hdReqId, String userId)
        {
            if (!userId.IsNullOrWhiteSpace() && !hdReqId.IsNullOrWhiteSpace())
            {
                try
                {
                    User u = _userServices.FindById(Int32.Parse(userId));
                    if (u != null)
                    {
                        HelpdeskRequest hdRequest = _hdReqServices.FindById(Int32.Parse(hdReqId));
                        bool allowViewDetail = false;
                        if (u.RoleId == SLIM_CONFIG.USER_ROLE_MANAGER)
                        {
                            allowViewDetail = true;
                        }
                        else if (u.RoleId == SLIM_CONFIG.USER_ROLE_SUPPORTER)
                        {
                            if (hdRequest.HelpdeskRequestHelpdeskSupporters.Count != 0)
                            {
                                if (u.Id ==
                                    hdRequest.HelpdeskRequestHelpdeskSupporters.OrderByDescending(e => e.CreateDate)
                                        .First()
                                        .User.Id)
                                {
                                    allowViewDetail = true;
                                }
                            }
                        }
                        else if (u.RoleId == (int)SLIM_CONFIG.USER_ROLE_RESIDENT)
                        {

                            /**if (u.UserInHouses.First() != null &&
                                hdRequest.HouseId == u.UserInHouses.First().HouseId.Value)
                            {
                                allowViewDetail = true;
                            }*/ //ANTT
                            if (u.HouseId == hdRequest.HouseId)
                            {
                                allowViewDetail = true;
                            }
                        }
                        if (allowViewDetail)
                        {
                            List<HelpdeskRequestLog> helpdeskRequestLogs =
                                _helpdeskRequestLogServices.GetHelpdeskRequestLog(hdRequest.Id);

                            ViewBag.hdRequest = hdRequest;
                            ViewBag.roleId = u.RoleId;
                            ViewBag.userId = u.Id;
                            ViewBag.helpdeskRequestLogs = helpdeskRequestLogs;
                            return View("_helpdeskRequestDetail");
                        }
                        return RedirectToAction("ViewHistoryHdRequest", new { userId = u.Id });

                    }/*Else return to main page*/
                }
                catch (Exception)
                {
                    return RedirectToAction("ViewHistoryHdRequest", new { userId = userId });
                }/*Return to main page*/

            }/*else return to main page*/
            return RedirectToAction("ViewHistoryHdRequest", new { userId = userId });
        }

        [HttpGet]
        [Route("Home/HelpdeskRequest/GetListRequest")]
        public ActionResult GetListRequest(string userId)
        {
            MessageViewModels reqponse = new MessageViewModels();
            List<HelpdeskRequest> hdReq = null;
            List<HelpdeskRequestTableModel> rows = new List<HelpdeskRequestTableModel>();

            if (!userId.IsNullOrWhiteSpace())
            {
                try
                {
                    User u = _userServices.FindById(Int32.Parse(userId));
                    if (u != null)
                    {

                        if (u.RoleId == SLIM_CONFIG.USER_ROLE_MANAGER)
                        {
                            hdReq = _hdReqServices.GetAllHelpdeskRequests();
                        }
                        else if (u.RoleId == SLIM_CONFIG.USER_ROLE_RESIDENT)
                        {
                            hdReq = _hdReqServices.GetHdRequestByHouseId(u.HouseId.Value); //ANTT
                        }
                        else
                        {
                            List<HelpdeskRequestHelpdeskSupporter> hdReqHdSupporter =
                                _hdReqHdSupporterServices.GetCurrentSupporterHdRequest(u.Id);


                            List<HelpdeskRequest> hdRequests = new List<HelpdeskRequest>();
                            if (hdReqHdSupporter != null || hdReqHdSupporter.Count != 0)
                            {
                                foreach (var s in hdReqHdSupporter)
                                {
                                    hdRequests.Add(s.HelpdeskRequest);
                                }
                                hdReq = hdRequests;
                            }
                        }



                        HelpdeskRequestTableModel row = null;
                        if (null != hdReq && hdReq.Count != 0)
                        {
                            foreach (var req in hdReq)
                            {
                                row = new HelpdeskRequestTableModel();
                                row.HdReqId = req.Id;
                                row.HdReqTitle = req.Title;

                                if (null != req.CreateDate)
                                {
                                    row.HdReqCreateDate = req.CreateDate.Value.ToString(parternTime);
                                }
                                if (null != req.DueDate)
                                {
                                    row.HdReqDeadline = req.DueDate.Value.ToString(parternTime);
                                }
                                row.HdReqSrvName = req.HelpdeskService.Name;
                                row.HdReqHouse = req.House.HouseName;
                                row.HdReqPrior = req.Priority.Value;
                                row.HdReqStatus = req.Status.Value;
                                if (req.Status != (int)StatusEnum.Open)
                                {
                                    User user =
                                        req.HelpdeskRequestHelpdeskSupporters.Where(
                                            hdSup => hdSup.HelpdeskRequestId == req.Id)
                                            .OrderByDescending(hdSup1 => hdSup1.CreateDate)
                                            .First()
                                            .User;
                                    row.HdReqSupporter =
                                    user.Fullname;
                                }
                                rows.Add(row);
                            }
                        }
                    }/*Else return to main page*/
                }
                catch (Exception)
                {
                    reqponse.StatusCode = -1;
                    Json(reqponse, JsonRequestBehavior.AllowGet);
                }/*Return to main page*/

            }/*else return to main page*/
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Home/HelpdeskRequest/UpdateStatus")]
        public ActionResult UpdateStatusHdRequest(HdRequestChangeStatusModel hdReqChngStatus)
        {
            try
            {
                User fromUser = _userServices.FindById(hdReqChngStatus.FromUserId);
                User toUser = _userServices.FindById(hdReqChngStatus.ToUserId);

                if (fromUser != null)
                {
                    HelpdeskRequest hdRequest = _hdReqServices.FindById(hdReqChngStatus.HdReqId);
                    if (null != hdRequest)
                    {
                        bool statusIsChange = true;
                        if (hdRequest.Status == (int)StatusEnum.WaitingForQuotation
                            && hdReqChngStatus.FromStatus == hdRequest.Status
                            && hdReqChngStatus.ToStatus == (int)StatusEnum.QuoutationConfirmed
                            )
                        {
                            hdRequest.Status = (int)StatusEnum.QuoutationConfirmed;
                            hdRequest.ModifyDate = DateTime.Now;
                            _hdReqServices.Update(hdRequest);

                        }
                        else if (hdRequest.Status == (int)StatusEnum.Open
                          && hdReqChngStatus.FromStatus == hdRequest.Status
                           && hdReqChngStatus.ToStatus == (int)StatusEnum.WaitingForQuotation)
                        {
                            hdRequest.Status = (int)StatusEnum.WaitingForQuotation;
                            hdRequest.AssignDate = DateTime.Now;
                            hdRequest.ModifyDate = DateTime.Now;
                            _hdReqServices.Update(hdRequest);


                            HelpdeskRequestHelpdeskSupporter hdReqHdSupporter = new HelpdeskRequestHelpdeskSupporter();
                            hdReqHdSupporter.HelpdeskRequestId = hdRequest.Id;
                            hdReqHdSupporter.HelpdeskSupporterId = hdReqChngStatus.ToUserId; // must check if user is disable
                            hdReqHdSupporter.CreateDate = DateTime.Now;
                            _hdReqHdSupporterServices.Add(hdReqHdSupporter);

                        }
                        else if (hdRequest.Status == (int)StatusEnum.WaitingForQuotation
                         && hdReqChngStatus.FromStatus == hdRequest.Status
                          && hdReqChngStatus.ToStatus == (int)StatusEnum.WaitingQuoutationConfirming)
                        {
                            hdRequest.Status = (int)StatusEnum.WaitingQuoutationConfirming;
                            hdRequest.ModifyDate = DateTime.Now;
                            hdRequest.Price = hdReqChngStatus.Price;
                            hdRequest.DueDate = DateTime.Parse(hdReqChngStatus.DueDate);
                            /*Add them giá vào đây*/
                            _hdReqServices.Update(hdRequest);

                        }
                        else if (hdRequest.Status == (int)StatusEnum.WaitingQuoutationConfirming
                         && hdReqChngStatus.FromStatus == hdRequest.Status
                          && hdReqChngStatus.ToStatus == (int)StatusEnum.QuoutationConfirmed)
                        {
                            hdRequest.Status = (int)StatusEnum.QuoutationConfirmed;
                            hdRequest.ModifyDate = DateTime.Now;
                            _hdReqServices.Update(hdRequest);

                        }
                        else if ((hdRequest.Status == (int)StatusEnum.QuoutationConfirmed
                            || hdRequest.Status == (int)StatusEnum.Reopen)
                        && hdReqChngStatus.FromStatus == hdRequest.Status
                         && hdReqChngStatus.ToStatus == (int)StatusEnum.Processing)
                        {
                            hdRequest.Status = (int)StatusEnum.Processing;
                            hdRequest.ModifyDate = DateTime.Now;
                            _hdReqServices.Update(hdRequest);

                        }
                        else if ((hdRequest.Status == (int)StatusEnum.QuoutationConfirmed
                            || hdRequest.Status == (int)StatusEnum.Reopen)
                        && hdReqChngStatus.FromStatus == hdRequest.Status
                         && hdReqChngStatus.ToStatus == (int)StatusEnum.WaitingForProcess)
                        {
                            hdRequest.Status = (int)StatusEnum.WaitingForProcess;
                            hdRequest.ModifyDate = DateTime.Now;
                            _hdReqServices.Update(hdRequest);

                        }

                        else if (hdRequest.Status == (int)StatusEnum.WaitingForProcess
                        && hdReqChngStatus.FromStatus == hdRequest.Status
                         && hdReqChngStatus.ToStatus == (int)StatusEnum.Processing)
                        {
                            hdRequest.Status = (int)StatusEnum.Processing;
                            hdRequest.ModifyDate = DateTime.Now;
                            _hdReqServices.Update(hdRequest);

                        }
                        else if (hdRequest.Status == (int)StatusEnum.Processing
                        && hdReqChngStatus.FromStatus == hdRequest.Status
                         && hdReqChngStatus.ToStatus == (int)StatusEnum.Done)
                        {
                            hdRequest.Status = (int)StatusEnum.Done;
                            hdRequest.ModifyDate = DateTime.Now;
                            _hdReqServices.Update(hdRequest);

                        }
                        else if (hdRequest.Status == (int)StatusEnum.Done
                        && hdReqChngStatus.FromStatus == hdRequest.Status
                         && hdReqChngStatus.ToStatus == (int)StatusEnum.Reopen)
                        {
                            hdRequest.Status = (int)StatusEnum.Reopen;
                            hdRequest.ModifyDate = DateTime.Now;
                            _hdReqServices.Update(hdRequest);

                        }
                        else if (hdRequest.Status == (int)StatusEnum.Done
                        && hdReqChngStatus.FromStatus == hdRequest.Status
                         && hdReqChngStatus.ToStatus == (int)StatusEnum.Closed)
                        {
                            hdRequest.Status = (int)StatusEnum.Closed;
                            hdRequest.ModifyDate = DateTime.Now;
                            hdRequest.CloseDate = DateTime.Now;
                            _hdReqServices.Update(hdRequest);

                            HelpdeskRequestHelpdeskSupporter hdReqHdSupporter = hdRequest.HelpdeskRequestHelpdeskSupporters
                                .Where(s => s.HelpdeskRequestId == hdRequest.Id).First();
                            hdReqHdSupporter.Status = (int)StatusEnum.Closed;
                            _hdReqHdSupporterServices.Update(hdReqHdSupporter);
                        }
                        else if (hdReqChngStatus.ToStatus == (int)StatusEnum.Reject)
                        {
                            bool isOk = false;
                            if (fromUser.RoleId == SLIM_CONFIG.USER_ROLE_MANAGER)
                            {
                                isOk = true;
                            }
                            else if (fromUser.RoleId == SLIM_CONFIG.USER_ROLE_RESIDENT &&
                                      (hdRequest.Status == (int)StatusEnum.WaitingForQuotation
                                      || hdRequest.Status == (int)StatusEnum.WaitingQuoutationConfirming
                                      || hdRequest.Status == (int)StatusEnum.Open))
                            {
                                isOk = true;
                            }
                            if (isOk)
                            {
                                hdRequest.Status = (int)StatusEnum.Reject;
                                hdRequest.ModifyDate = DateTime.Now;
                                _hdReqServices.Update(hdRequest);

                                if (hdRequest.HelpdeskRequestHelpdeskSupporters.Count != 0)
                                {
                                    HelpdeskRequestHelpdeskSupporter hdReqHdSupporter =
                                        hdRequest.HelpdeskRequestHelpdeskSupporters.OrderByDescending(s => s.CreateDate)
                                            .First();
                                    hdReqHdSupporter.Status = (int)StatusEnum.Reject;
                                    _hdReqHdSupporterServices.Update(hdReqHdSupporter);
                                }
                            }
                            else
                            {
                                statusIsChange = false;
                            }
                        }
                        if (statusIsChange)
                        {
                            HelpdeskRequestLog hdRequestLog = new HelpdeskRequestLog();
                            hdRequestLog.ChangeFromUserId = fromUser.Id;
                            hdRequestLog.HelpdeskRequestId = hdRequest.Id;
                            //                        hdRequestLog.ChangeToUserId = toUser.Id;
                            hdRequestLog.StatusFrom = hdReqChngStatus.FromStatus;
                            hdRequestLog.StatusTo = hdReqChngStatus.ToStatus;
                            hdRequestLog.CreateDate = DateTime.Now;
                            _helpdeskRequestLogServices.Add(hdRequestLog);
                            //                        hdReqId, String userId
                        }

                        return RedirectToAction("ViewHdRequestDetail", new { hdReqId = hdRequest.Id, userId = fromUser.Id });
                    }
                    else
                    {
                        return RedirectToAction("ViewHistoryHdRequest", new { userId = fromUser });
                    }// return message not found.
                }
                /*Else return to main page*/
            }
            catch (Exception)
            {
                return RedirectToAction("ViewHistoryHdRequest", new { userId = hdReqChngStatus.FromUserId });
            }/*Return to main page*/

            return RedirectToAction("ViewHistoryHdRequest", new { userId = hdReqChngStatus.FromUserId });
        }

        [HttpGet]
        [Route("Home/HelpdeskRequest/GetSupporters")]
        public ActionResult GetHelpdeskSupporter(int hdReqId)
        {
            MessageViewModels response = new MessageViewModels();
            HelpdeskRequest hdRequest = _hdReqServices.FindById(hdReqId);
            if (hdRequest != null)
            {
                List<User> supporterList = _userServices.FindUserByRole(SLIM_CONFIG.USER_ROLE_SUPPORTER);
                List<HdSuporterModel> listSupporter = new List<HdSuporterModel>();
                HdSuporterModel model = null;
                foreach (var s in supporterList)
                {
                    model = new HdSuporterModel();
                    model.UserId = s.Id;
                    model.Fullname = s.Fullname;
                    listSupporter.Add(model);
                }
                if (hdRequest.HelpdeskRequestHelpdeskSupporters.Count != 0)
                {
                    User curSupporter =
                        hdRequest.HelpdeskRequestHelpdeskSupporters.OrderByDescending(c => c.CreateDate).First().User;
                    response.Data = new { supporterList = listSupporter, curUserId = curSupporter.Id };
                }
                else
                {
                    response.Data = new { supporterList = listSupporter };

                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Home/HelpdeskRequest/AssignTask")]
        public ActionResult AssignTask(HdRequestChangeStatusModel hdReqChngStatus)
        {
            try
            {
                User fromUser = _userServices.FindById(hdReqChngStatus.FromUserId);
                User toUser = _userServices.FindById(hdReqChngStatus.ToUserId);

                if (fromUser != null && toUser != null)
                {
                    HelpdeskRequest hdRequest = _hdReqServices.FindById(hdReqChngStatus.HdReqId);

                    if (hdRequest.Status == (int)StatusEnum.Open)
                    {
                        hdRequest.Status = (int)StatusEnum.WaitingForQuotation;
                        hdRequest.AssignDate = DateTime.Now;
                        hdRequest.ModifyDate = DateTime.Now;
                        _hdReqServices.Update(hdRequest);

                        HelpdeskRequestHelpdeskSupporter hdReqHdSupporter = new HelpdeskRequestHelpdeskSupporter();
                        hdReqHdSupporter.HelpdeskRequestId = hdRequest.Id;
                        hdReqHdSupporter.HelpdeskSupporterId = hdReqChngStatus.ToUserId; // must check if user is disable
                        hdReqHdSupporter.CreateDate = DateTime.Now;
                        _hdReqHdSupporterServices.Add(hdReqHdSupporter);
                        return RedirectToAction("ViewHdRequestDetail", new { hdReqId = hdRequest.Id, userId = fromUser.Id });
                    }
                    else if (hdRequest.Status != (int)StatusEnum.Reject && hdRequest.Status != (int)StatusEnum.Closed)
                    {
                        int curSupporterId = hdRequest.HelpdeskRequestHelpdeskSupporters.OrderByDescending(e => e.CreateDate).First().HelpdeskSupporterId.Value;

                        if (curSupporterId != hdReqChngStatus.ToUserId)
                        {
                            hdRequest.ModifyDate = DateTime.Now;
                            _hdReqServices.Update(hdRequest);

                            HelpdeskRequestHelpdeskSupporter hdReqHdSupporter = new HelpdeskRequestHelpdeskSupporter();
                            hdReqHdSupporter.HelpdeskRequestId = hdRequest.Id;
                            hdReqHdSupporter.HelpdeskSupporterId = hdReqChngStatus.ToUserId; // must check if user is disable
                            hdReqHdSupporter.CreateDate = DateTime.Now;
                            _hdReqHdSupporterServices.Add(hdReqHdSupporter);
                        }

                        return RedirectToAction("ViewHdRequestDetail", new { hdReqId = hdRequest.Id, userId = fromUser.Id });
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ViewHistoryHdRequest", new { userId = hdReqChngStatus.FromUserId });
            }/*Return to main page*/

            return RedirectToAction("ViewHistoryHdRequest", new { userId = hdReqChngStatus.FromUserId });
        }

        [HttpGet]
        [Route("Home/HelpdeskRequest/AdminView")]
        public ActionResult ViewRequestingHdRequestToManager()
        {

            return View("ViewRequestingHdRequestsOfManager");
        }

        [HttpPost]
        public ActionResult ManageMember(AddMemberViewModel member)
        {
            if (ModelState.IsValid)
            {
                User existUser = userService.findByUsername(member.Username);
                if (existUser != null)
                {
                    ModelState.AddModelError("Username", "Tên người dùng này đã được sử dụng");
                }
                else
                {
                    var curUser = userService.findById(int.Parse(User.Identity.GetUserId()));
                    User newUser = new User();
                    newUser.Creator = curUser.Id;
                    newUser.Fullname = member.Fullname;
                    newUser.Username = curUser.House.HouseName + "_" + member.Username;
                    newUser.Password = member.Password;
                    newUser.Gender = member.Gender;
                    newUser.ProfileImage = member.ImageURL;
                    newUser.RoleId = SLIM_CONFIG.Role_RESIDENT;
                    newUser.HouseId = curUser.HouseId;
                    newUser.IsApproved = SLIM_CONFIG.USER_APPROVE_WAITING;
                    newUser.DateOfBirth = member.DateOfBirth;
                    userService.addUser(newUser);

                }
            }
            else
            {
                ViewBag.adding = true;
            }
            var currentUser = User.Identity;
            int userId;
            bool isValidID = int.TryParse(currentUser.GetUserId(), out userId);
            if (!isValidID)
            {
                return View("error");
            }
            User user = userService.findById(userId);
            List<User> members = userService.findByHouseId(user.Id);
            ViewBag.currentHouse = user.House;
            ViewBag.members = members;
            return View();
            //pendingMemberService.addMemberRequest(member);

        }
    }
}