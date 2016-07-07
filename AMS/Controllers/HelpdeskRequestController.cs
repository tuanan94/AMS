using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Constant;
using AMS.Enum;
using AMS.Models;
using AMS.Service;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace AMS.Controllers
{
    public class HelpdeskRequestController : Controller
    {
        HelpdeskServiceCatService _helpdeskServiceCat = new HelpdeskServiceCatService();
        HelpdeskServicesService _helpdeskServices = new HelpdeskServicesService();
        UserServices _userServices = new UserServices();
        HelpdeskRequestServices _hdReqServices = new HelpdeskRequestServices();
        HelpdeskRequestLogServices _helpdeskRequestLogServices = new HelpdeskRequestLogServices();

        [Authorize]
        [HttpGet]
        [Route("Home/HelpdeskRequest/Create")]
        public ActionResult CreateNewHdRequest()
        {

            List<HelpdeskServiceCategory> hdSrvCats = _helpdeskServiceCat.GetAll();
            ViewBag.hdSrvCats = hdSrvCats;
            User u = _userServices.FindById(int.Parse(User.Identity.GetUserId()));
            if (u == null)
            {
                return View("error");
            }
            ViewBag.curUser = u;
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
                    hdRequest.Status = (int)StatusEnum.Open;
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

        [HttpPost]
        [Route("Home/HelpdeskRequest/UpdateHdRequest")]
        public ActionResult UpdateHdRequest(HelpdeskRequestModel request)
        {
            try
            {
                User u = _userServices.FindById(request.HdReqUserId);
                HelpdeskRequest hdRequest = _hdReqServices.FindById(request.HdReqId);
                if (u != null && hdRequest != null)
                {
                    HelpdeskService hdService = _helpdeskServices.FindById(request.HdServiceId);
                    if (hdService != null && hdRequest.HelpdeskServiceId != hdService.Id)
                    {
                        hdRequest.HelpdeskServiceId = hdService.Id;
                        hdRequest.Price = hdService.Price;
                    }
                    hdRequest.Title = request.HdReqTitle;
                    hdRequest.Description = request.HdReqUserDesc;
                    hdRequest.ModifyDate = DateTime.Now;
                    _hdReqServices.Update(hdRequest);
                    return RedirectToAction("ViewHdRequestDetail", new { hdReqId = hdRequest.Id, userId = u.Id });
                }
                // return to homepage
            }
            catch (Exception e)
            {
                return RedirectToAction("ViewHistoryHdRequest", new { userId = request.HdReqUserId });
            }
            return RedirectToAction("ViewHistoryHdRequest", new { userId = request.HdReqUserId });
        }

        [Authorize]
        [HttpGet]
        [AutoRedirect.MandatorySurveyRedirect]
        [Route("Home/HelpdeskRequest/ViewHistory")]
        public ActionResult ViewHistoryHdRequest()
        {
            User u = _userServices.FindById(int.Parse(User.Identity.GetUserId()));
            if (u == null)
            {
                return View("error");

            }
            ViewBag.roleId = u.RoleId;
            return View("ViewHistoryHdRequests");
        }

        [Authorize]
        [HttpGet]
        [Route("Management/HelpdeskRequest/ManageHdRequest")]
        public ActionResult ManageHdRequest()
        {
            User u = _userServices.FindById(int.Parse(User.Identity.GetUserId()));
            if (u == null)
            {
                return View("error");

            }
            ViewBag.roleId = u.RoleId;
            return View("ManageHdRequests");
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
                            if (hdRequest.SupporterId != null)
                            {
                                allowViewDetail = true;
                            }
                        }
                        else if (u.RoleId == (int)SLIM_CONFIG.USER_ROLE_RESIDENT || u.RoleId == (int)SLIM_CONFIG.USER_ROLE_HOUSEHOLDER)
                        {

                            /**if (u.UserInHouses.First() != null &&
                                hdRequest.HouseId == u.UserInHouses.First().HouseId.Value)
                            {
                                allowViewDetail = true;
                            }*/
                            //ANTT
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
                            return View("ViewHistoryHdRequestDetail");
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
                        else if (u.RoleId == SLIM_CONFIG.USER_ROLE_RESIDENT || u.RoleId == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER)
                        {
                            hdReq = _hdReqServices.GetHdRequestByHouseId(u.HouseId.Value); //ANTT
                        }
                        else
                        {
                            List<HelpdeskRequest> hdRequests = _hdReqServices.GetAllHdRequestBySupporterId(u.Id);
                            hdReq = hdRequests;
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
                                    row.HdReqCreateDate = req.CreateDate.Value.ToString(AmsConstants.DateTimeFormat);
                                }
                                if (null != req.DueDate)
                                {
                                    row.HdReqDeadline = req.DueDate.Value.ToString(AmsConstants.DateTimeFormat);
                                }
                                row.HdReqSrvName = req.HelpdeskService.Name;
                                row.HdReqHouse = req.House.HouseName;
                                row.HdReqPrior = req.Priority.Value;
                                row.HdReqStatus = req.Status.Value;

                                if (req.User1 != null)
                                {
                                    row.HdReqSupporter = req.User1.Fullname;
                                }
                                else
                                {
                                    row.HdReqSupporter = null;
                                }

                                rows.Add(row);
                            }
                        }
                    }/*Else return to main page*/
                }
                catch (Exception)
                {
                    reqponse.StatusCode = -1;
                    return Json(reqponse, JsonRequestBehavior.AllowGet);
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
                        if (hdRequest.Status == (int)StatusEnum.Open
                            && hdReqChngStatus.FromStatus == hdRequest.Status
                            && hdReqChngStatus.ToStatus == (int)StatusEnum.Processing
                            )
                        {
                            hdRequest.Status = (int)StatusEnum.Processing;
                            hdRequest.ModifyDate = DateTime.Now;
                            _hdReqServices.Update(hdRequest);

                        }
                        //                        else if (hdRequest.Status == (int)StatusEnum.Open
                        //                          && hdReqChngStatus.FromStatus == hdRequest.Status
                        //                           && hdReqChngStatus.ToStatus == (int)StatusEnum.WaitingForQuotation)
                        //                        {
                        //                            hdRequest.Status = (int)StatusEnum.WaitingForQuotation;
                        //                            hdRequest.AssignDate = DateTime.Now;
                        //                            hdRequest.ModifyDate = DateTime.Now;
                        //                            _hdReqServices.Update(hdRequest);
                        //
                        //
                        //                            HelpdeskRequestHelpdeskSupporter hdReqHdSupporter = new HelpdeskRequestHelpdeskSupporter();
                        //                            hdReqHdSupporter.HelpdeskRequestId = hdRequest.Id;
                        //                            hdReqHdSupporter.HelpdeskSupporterId = hdReqChngStatus.ToUserId; // must check if user is disable
                        //                            hdReqHdSupporter.CreateDate = DateTime.Now;
                        //                            _hdReqHdSupporterServices.Add(hdReqHdSupporter);
                        //
                        //                        }
                        else if (hdRequest.Status == (int)StatusEnum.Processing
                         && hdReqChngStatus.FromStatus == hdRequest.Status
                          && hdReqChngStatus.ToStatus == (int)StatusEnum.Done)
                        {
                            hdRequest.Status = (int)StatusEnum.Done;
                            hdRequest.ModifyDate = DateTime.Now;
                            //                            hdRequest.Price = hdReqChngStatus.Price;
                            //                            hdRequest.DueDate = DateTime.Parse(hdReqChngStatus.DueDate);
                            /*Add them giá vào đây*/
                            _hdReqServices.Update(hdRequest);
                        }
                        else if (hdRequest.Status == (int)StatusEnum.Done
                         && hdReqChngStatus.FromStatus == hdRequest.Status
                          && hdReqChngStatus.ToStatus == (int)StatusEnum.Close)
                        {
                            hdRequest.Status = (int)StatusEnum.Close;
                            hdRequest.CloseDate = DateTime.Now;
                            hdRequest.ModifyDate = DateTime.Now;
                            _hdReqServices.Update(hdRequest);
                        }
                        else if (hdReqChngStatus.ToStatus == (int)StatusEnum.Cancel)
                        {
                            bool isOk = false;
                            if (fromUser.RoleId == SLIM_CONFIG.USER_ROLE_MANAGER &&
                                (hdRequest.Status == (int)StatusEnum.Open || hdRequest.Status == (int)StatusEnum.Processing))
                            {
                                isOk = true;
                            }
                            else if (fromUser.RoleId == SLIM_CONFIG.USER_ROLE_RESIDENT || fromUser.RoleId == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER &&
                                      (hdRequest.Status == (int)StatusEnum.Open || hdRequest.Status == (int)StatusEnum.Processing))
                            {
                                isOk = true;
                            }
                            if (isOk)
                            {
                                hdRequest.Status = (int)StatusEnum.Cancel;
                                hdRequest.ModifyDate = DateTime.Now;
                                _hdReqServices.Update(hdRequest);
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
                List<User> supporterList = _userServices.GetAllSupporter();
                List<HdSuporterModel> listSupporter = new List<HdSuporterModel>();
                HdSuporterModel model = null;
                foreach (var s in supporterList)
                {
                    model = new HdSuporterModel();
                    model.UserId = s.Id;
                    model.Fullname = s.Fullname;
                    listSupporter.Add(model);
                }
                if (hdRequest.SupporterId != null)
                {
                    response.Data = new { supporterList = listSupporter, curUserId = hdRequest.SupporterId };
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

                    if (hdRequest.Status != (int)StatusEnum.Done && hdRequest.Status != (int)StatusEnum.Close)
                    {
                        HelpdeskRequestLog hdRequestLog = new HelpdeskRequestLog();
                        hdRequestLog.StatusFrom = (int)StatusEnum.AssignTask;
                        hdRequestLog.StatusTo = (int)StatusEnum.AssignTask;

                        if (hdRequest.Status == (int)StatusEnum.Open)
                        {
                            hdRequest.AssignDate = DateTime.Now;
                            hdRequest.Status = (int)StatusEnum.Processing;

                            hdRequestLog.StatusFrom = (int)StatusEnum.Open;
                            hdRequestLog.StatusTo = (int)StatusEnum.Processing;
                        }
                        hdRequest.ModifyDate = DateTime.Now;
                        hdRequest.SupporterId = toUser.Id;
                        _hdReqServices.Update(hdRequest);

                        hdRequestLog.ChangeFromUserId = fromUser.Id;
                        hdRequestLog.HelpdeskRequestId = hdRequest.Id;
                        hdRequestLog.ChangeToUserId = toUser.Id;

                        hdRequestLog.CreateDate = DateTime.Now;
                        _helpdeskRequestLogServices.Add(hdRequestLog);

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

        [HttpPost]
        [Route("Home/HelpdeskRequest/SetDuedate")]
        public ActionResult SetDuedate(HdRequestChangeStatusModel hdReqChngStatus)
        {
            try
            {
                User fromUser = _userServices.FindById(hdReqChngStatus.FromUserId);

                if (fromUser != null)
                {
                    HelpdeskRequest hdRequest = _hdReqServices.FindById(hdReqChngStatus.HdReqId);

                    if (hdRequest.Status != (int)StatusEnum.Done && hdRequest.Status != (int)StatusEnum.Close && fromUser.RoleId == SLIM_CONFIG.USER_ROLE_MANAGER)
                    {
                        hdRequest.DueDate = DateTime.Parse(hdReqChngStatus.DueDate);
                        hdRequest.ModifyDate = DateTime.Now;
                        _hdReqServices.Update(hdRequest);

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
        [Route("Home/HelpdeskRequest/GetHdReqInfoDetail/{hdReqId}")]
        public ActionResult GetHdReqInfoDetail(int hdReqId)
        {
            MessageViewModels response = new MessageViewModels();

            HelpdeskRequest hdRequest = _hdReqServices.FindById(hdReqId);
            if (null != hdRequest)
            {
                HdReqDetailInfo hdReqDetailInfo = new HdReqDetailInfo();
                hdReqDetailInfo.SelectedHdSrvCatId = hdRequest.HelpdeskService.HelpdeskServiceCategoryId.Value;
                hdReqDetailInfo.SelectedHdSrvId = hdRequest.HelpdeskService.Id;
                hdReqDetailInfo.SelectedHdSrvPrice = hdRequest.Price.Value;

                List<HelpdeskServiceCategory> hdSrvCats = _helpdeskServiceCat.GetAll();

                List<HelpdeskServiceCatModel> hdSrvCatModels = new List<HelpdeskServiceCatModel>();
                HelpdeskServiceCatModel hdSrvCatModel = null;
                foreach (var cat in hdSrvCats)
                {
                    hdSrvCatModel = new HelpdeskServiceCatModel();
                    hdSrvCatModel.Id = cat.Id;
                    hdSrvCatModel.Name = cat.Name;
                    hdSrvCatModels.Add(hdSrvCatModel);
                }

                List<HelpdeskService> hdServices = _helpdeskServices.FindByCategory(hdReqDetailInfo.SelectedHdSrvCatId);
                List<HelpdeskServiceModel> hdReqModels = new List<HelpdeskServiceModel>();
                HelpdeskServiceModel hdSrvModel = null;
                foreach (var hdSrv in hdServices)
                {
                    hdSrvModel = new HelpdeskServiceModel();
                    hdSrvModel.Id = hdSrv.Id;
                    hdSrvModel.Name = hdSrv.Name;
                    hdSrvModel.Price = hdSrv.Price.Value;
                    hdSrvModel.Description = hdSrv.Description;
                    hdReqModels.Add(hdSrvModel);
                }

                HelpdeskRequestModel hdReqModel = new HelpdeskRequestModel();
                hdReqModel.HdReqTitle = hdRequest.Title;
                hdReqModel.HdReqUserDesc = hdRequest.Description;

                hdReqDetailInfo.HdSrvCategories = hdSrvCatModels;
                hdReqDetailInfo.ListHdSrvBySelectedCat = hdReqModels;
                hdReqDetailInfo.HdReqInfoDetail = hdReqModel;

                response.Data = hdReqDetailInfo;
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Not found !";
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Home/HelpdeskRequest/AdminView")]
        public ActionResult ViewRequestingHdRequestToManager()
        {

            return View("ViewRequestingHdRequestsOfManager");
        }
    }
}