
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AMS.Constant;
using AMS.Models;
using AMS.Service;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;

namespace AMS.Controllers
{
    public class ManagementController : Controller
    {
        HelpdeskServicesService _helpdeskServicesService = new HelpdeskServicesService();
        HelpdeskServiceCatService _helpdeskServiceCatService = new HelpdeskServiceCatService();
        UserServices _userServices = new UserServices();
        AroundProviderService _aroundProviderService = new AroundProviderService();
        NotificationService notificationService = new NotificationService();
        readonly string parternTime = "dd-MM-yyyy HH:mm";
        [Authorize]
        public ActionResult Index()
        {
            User curUser = _userServices.FindById(int.Parse(User.Identity.GetUserId()));
            ViewBag.curUser = curUser;
            return View();
        }

        // GET: /Management/
        public ActionResult AdminBoard()
        {
            return View();
        }
        public ActionResult ManageHousehold()
        {
            return View();
        }

        public ActionResult ManageRequest()
        {
            String action = this.Request.QueryString["action"];
            if (null != action)
            {
                String url = "";
                if (action.Equals("loadHdSrvCat"))
                {
                    // Start load all service category
                    List<HelpdeskServiceCategory> hdSrvCategories = _helpdeskServiceCatService.GetAll();
                    List<HelpdeskServiceCatModel> hdSrvCats = new List<HelpdeskServiceCatModel>();
                    HelpdeskServiceCatModel hdSrvCat = null;
                    for (var i = 0; i < hdSrvCategories.Capacity; i++)
                    {
                        hdSrvCat = new HelpdeskServiceCatModel();
                        hdSrvCat.Id = hdSrvCategories[i].Id;
                        hdSrvCat.Name = hdSrvCategories[i].Name;
                        hdSrvCats.Add(hdSrvCat);
                    }
                    HelpdeskSerivceCatListModel hdSrvCatListModel = new HelpdeskSerivceCatListModel(hdSrvCats);
                    // Start load all service category

                    return Json(hdSrvCatListModel, JsonRequestBehavior.AllowGet);
                }
                else if (action.Equals("loadHdSrvTable"))
                {
                    // Start load all service category
                    ViewBag.helpdeskServices = _helpdeskServicesService.GetHelpdeskServices();
                    // Start load all service category
                    url = "_helpdeskSeviceTablePartial";
                    return PartialView(url);
                }
                else if (action.Equals("hdSrvDetail"))
                {
                    NameValueCollection nvc = this.Request.QueryString;
                    String idStr = nvc["id"];
                    try
                    {
                        int id = -1;
                        id = Int32.Parse(idStr);
                        HelpdeskService hdService = _helpdeskServicesService.FindById(id);

                        if (null != hdService)
                        {
                            HelpdeskServiceModel hdServiceModel = new HelpdeskServiceModel();
                            hdServiceModel.Id = hdService.Id;
                            hdServiceModel.Name = hdService.Name;
                            hdServiceModel.Description = hdService.Description;
                            hdServiceModel.Price = hdService.Price.Value;
                            hdServiceModel.Status = hdService.Status.Value;
                            hdServiceModel.HelpdeskServiceCategoryId = hdService.HelpdeskServiceCategoryId.Value;
                            hdServiceModel.HelpdeskServiceCategoryName = hdService.HelpdeskServiceCategory.Name;

                            // Start load all service category
                            List<HelpdeskServiceCategory> hdSrvCategories = _helpdeskServiceCatService.GetAll();
                            List<HelpdeskServiceCatModel> hdSrvCats = new List<HelpdeskServiceCatModel>();
                            HelpdeskServiceCatModel hdSrvCat = null;
                            for (var i = 0; i < hdSrvCategories.Capacity; i++)
                            {
                                hdSrvCat = new HelpdeskServiceCatModel();
                                hdSrvCat.Id = hdSrvCategories[i].Id;
                                hdSrvCat.Name = hdSrvCategories[i].Name;
                                hdSrvCats.Add(hdSrvCat);
                            }
                            // Start load all service category

                            hdServiceModel.HdSrvCategories = hdSrvCats;
                            return Json(hdServiceModel, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            MessageViewModels response = new MessageViewModels();
                            var message = new List<object>();
                            message.Add(new { txt_error = "Id phải là số !" });
                            response.Data = message;
                            response.StatusCode = -1;
                            return Json(response);
                        }
                    }
                    catch (Exception)
                    {
                        MessageViewModels response = new MessageViewModels();
                        var message = new List<object>();
                        message.Add(new { txt_error = "Id phải là số !" });
                        response.Data = message;
                        response.StatusCode = -1;
                        return Json(response);
                    }
                }
                else if (action.Equals("addHelpdeskSrv"))
                {
                    NameValueCollection nvc = this.Request.Form;
                    String name = nvc["hdSrvName"];
                    String typeId = nvc["hdSrvCat"];
                    String price = nvc["hdSrvPrice"];
                    String status = nvc["hdSrvStatus"];
                    String desc = nvc["hdSrvDesc"];

                    HelpdeskService hdService = new HelpdeskService();
                    hdService.Name = name;
                    hdService.HelpdeskServiceCategoryId = Int32.Parse(typeId);
                    hdService.Price = Double.Parse(price);
                    hdService.Status = Int32.Parse(status);
                    hdService.Description = desc;
                    _helpdeskServicesService.Add(hdService);

                    MessageViewModels response = new MessageViewModels();

                    return Json(response);
                }
                else if (action.Equals("updateHelpdeskSrv"))
                {
                    NameValueCollection nvc = this.Request.Form;
                    String name = nvc["hdSrvName"];
                    String idStr = nvc["hdSrvId"];
                    String typeId = nvc["hdSrvCat"];
                    String price = nvc["hdSrvPrice"];
                    String status = nvc["hdSrvStatus"];
                    String desc = nvc["hdSrvDesc"];
                    try
                    {
                        MessageViewModels response = new MessageViewModels();

                        int id = Int32.Parse(idStr);
                        HelpdeskService hdService = _helpdeskServicesService.FindById(id);
                        if (null != hdService)
                        {
                            hdService.Name = name;
                            hdService.HelpdeskServiceCategoryId = Int32.Parse(typeId);
                            hdService.Price = Double.Parse(price);
                            hdService.Status = Int32.Parse(status);
                            hdService.Description = desc;
                            _helpdeskServicesService.Update(hdService);

                        }
                        else
                        {
                            var message = new List<object>();
                            message.Add(new { txt_error = "Không tìm thấy hổ trợ dịch vụ !" });
                            response.Data = message;
                            response.StatusCode = 4;
                        }
                        return Json(response);
                    }
                    catch (Exception e)
                    {
                        MessageViewModels response = new MessageViewModels();
                        //                        var message = new List<object>();
                        //                        response.Data = message;
                        response.Msg = "Cập nhật thất bại !";
                        response.StatusCode = -1;
                        return Json(response);
                    }
                }
                else if (action.Equals("delHelpdeskSrv"))
                {
                    MessageViewModels response = new MessageViewModels();
                    NameValueCollection nvc = this.Request.Form;
                    String desc = nvc["hdSrvDeletedList"];
                    List<string> ids = desc.Split(',').ToList();
                    if (ids.Count != 0)
                    {
                        foreach (var id in ids)
                        {
                            try
                            {
                                HelpdeskService s = _helpdeskServicesService.FindById(Int32.Parse(id));
                                if (null != s)
                                {
                                    _helpdeskServicesService.Delete(s);
                                }
                            }
                            catch (Exception exception)
                            {
                                response.StatusCode = -1;
                                return Json(response);
                            }
                        }
                        response.Msg = "OK !";
                    }
                    else
                    {
                        response.StatusCode = -1;
                    }
                    return Json(response);
                }
                else if (action.Equals("searchHdSrv"))
                {
                    MessageViewModels response = new MessageViewModels();
                    NameValueCollection nvc = this.Request.Form;
                    String searchStr = nvc["searchStr"];
                    String modeStr = nvc["filterMode"];
                    String filterValue = nvc["filterValue"];
                    try
                    {
                        int mode = Int32.Parse(modeStr);
                        int value = -1;
                        if (mode != 0)
                        {
                            value = Int32.Parse(filterValue);
                        }
                        List<HelpdeskService> helpdeskServices = null;
                        if (mode == 1)
                        {
                            helpdeskServices = _helpdeskServicesService.FindByCategoryAndName(searchStr, value);
                        }
                        else if (mode == 2)
                        {
                            helpdeskServices = _helpdeskServicesService.FindByStatusAndName(searchStr, value);
                        }
                        else
                        {
                            helpdeskServices = _helpdeskServicesService.FindByName(searchStr);
                        }
                        ViewBag.helpdeskServices = helpdeskServices;
                        url = "_helpdeskSeviceTablePartial";
                        return PartialView(url);
                    }
                    catch (Exception)
                    {
                        response.StatusCode = -1;
                        return Json(response);
                    }
                }

                else if (action.Equals("hdSrvCatDetail"))
                {
                    NameValueCollection nvc = this.Request.QueryString;
                    String idStr = nvc["id"];
                    MessageViewModels response = new MessageViewModels();
                    try
                    {
                        int id = Int32.Parse(idStr);
                        HelpdeskServiceCategory hdSrvCategory = _helpdeskServiceCatService.FindById(id);
                        if (null != hdSrvCategory)
                        {
                            HelpdeskServiceCatModel catModel = new HelpdeskServiceCatModel();
                            catModel.Id = hdSrvCategory.Id;
                            catModel.Name = hdSrvCategory.Name;
                            response.Data = catModel;
                        }
                        else
                        {
                            response.StatusCode = 4;
                            response.Msg = "Không tìm thấy nhóm dịch vụ hổ trợ.";
                        }
                        return Json(response, JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception)
                    {
                        response.StatusCode = -1;
                        return Json(response);
                    }
                }
                else if (action.Equals("loadHdSrvCatTable"))
                {
                    url = "_helpdeskServiceCategoryTablePartial";
                    ViewBag.helpdeskServiceCategories = _helpdeskServiceCatService.GetAll();
                }
                else if (action.Equals("searchHdSrvCategory"))
                {
                    url = "_helpdeskServiceCategoryTablePartial";
                    NameValueCollection nvc = this.Request.Form;
                    string name = nvc["searchStrHdSrvCat"];
                    ViewBag.helpdeskServiceCategories = _helpdeskServiceCatService.FindByName(name);
                }
                else if (action.Equals("addHdSrvCategory"))
                {
                    NameValueCollection nvc = this.Request.Form;
                    String name = nvc["hdSrvCatName"];
                    HelpdeskServiceCategory hdSrvCategory = new HelpdeskServiceCategory();
                    hdSrvCategory.Name = name;
                    _helpdeskServiceCatService.Add(hdSrvCategory);
                    MessageViewModels response = new MessageViewModels();
                    return Json(response);
                }
                else if (action.Equals("updateHdSrvCategory"))
                {
                    NameValueCollection nvc = this.Request.Form;
                    String name = nvc["hdSrvCatName"];
                    String idStr = nvc["hdSrvCatId"];
                    MessageViewModels response = new MessageViewModels();
                    try
                    {
                        int id = Int32.Parse(idStr);
                        HelpdeskServiceCategory hdSrvCategory = _helpdeskServiceCatService.FindById(id);
                        if (hdSrvCategory != null)
                        {
                            hdSrvCategory.Name = name;
                            _helpdeskServiceCatService.Update(hdSrvCategory);
                        }
                        else
                        {
                            response.StatusCode = 4;
                            response.Msg = "Không tìm thấy nhóm dịch vụ hổ trợ.";
                        }
                        return Json(response);
                    }
                    catch (Exception)
                    {
                        response.StatusCode = -1;
                        response.Msg = "Lỗi cập nhật nhóm dịch vụ hổ trợ.";
                        return Json(response);
                    }
                }
                else if (action.Equals("delHdSrvCategory"))
                {
                    MessageViewModels response = new MessageViewModels();
                    NameValueCollection nvc = this.Request.Form;
                    string idStrList = nvc["hdSrvCatDeletedList"];
                    List<string> ids = idStrList.Split(',').ToList();
                    if (ids.Count != 0)
                    {
                        foreach (var id in ids)
                        {
                            try
                            {
                                HelpdeskServiceCategory s = _helpdeskServiceCatService.FindById(Int32.Parse(id));
                                if (null != s)
                                {
                                    _helpdeskServiceCatService.Delete(s);
                                }
                            }
                            catch (Exception exception)
                            {
                                response.StatusCode = -1;
                                return Json(response);
                            }
                        }
                        response.Msg = "OK !";
                    }
                    else
                    {
                        response.StatusCode = -1;
                    }
                    return Json(response);
                }
                else if (action.Equals("getHdServiceByCatId"))
                {
                    MessageViewModels response = new MessageViewModels();
                    NameValueCollection nvc = this.Request.QueryString;
                    string idStr = nvc["hdSrvCatId"];
                    try
                    {
                        int id = Int32.Parse(idStr);
                        List<HelpdeskService> hdSrvsList = _helpdeskServicesService.FindByCategoryAndEnable(id);
                        List<HelpdeskServiceModel> hdServiceModels = new List<HelpdeskServiceModel>();


                        if (hdSrvsList != null && hdSrvsList.Count != 0)
                        {
                            HelpdeskServiceModel model = null;
                            foreach (var srv in hdSrvsList)
                            {
                                model = new HelpdeskServiceModel();
                                model.Name = srv.Name;
                                model.Description = srv.Description;
                                model.Id = srv.Id;
                                model.HelpdeskServiceCategoryId = srv.HelpdeskServiceCategoryId.Value;
                                model.HelpdeskServiceCategoryName = srv.HelpdeskServiceCategory.Name;
                                model.Price = srv.Price.Value;
                                hdServiceModels.Add(model);
                            }
                            response.Data = hdServiceModels;
                        }
                        else
                        {
                            response.StatusCode = 4;
                            response.Msg = "Không tìm thấy nhóm dịch vụ hổ trợ.";
                        }
                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception)
                    {
                        response.StatusCode = -1;
                        response.Msg = "Lỗi cập nhật nhóm dịch vụ hổ trợ.";
                        return Json(response);
                    }
                }
                return PartialView(url);
            }
            ViewBag.helpdeskServices = _helpdeskServicesService.GetHelpdeskServices();
            return View("ManageHelpdeskService");
        }

        [HttpGet]
        [Route("Management/ManageRequestView/{userId}")]
        public ActionResult ManageRequestView(int userId)
        {
            ViewBag.helpdeskServices = _helpdeskServicesService.GetHelpdeskServices();
            ViewBag.userId = userId;
            return View("ManageHelpdeskService");
        }

        [HttpGet]
        [Route("Management/ViewHelpdeskServiceCategory/{userId}")]
        public ActionResult ViewHelpdeskServiceCategory(int userId)
        {
            ViewBag.helpdeskServiceCategories = _helpdeskServiceCatService.GetAll();
            ViewBag.userId = userId;
            return View("ManageHelpdeskServiceCategory");
        }

        public ActionResult ManageIncome()
        {
            return View();
        }
        public ActionResult ManageReport()
        {
            return View();
        }
        public ActionResult ManageSurvey()
        {
            return View();
        }


        [HttpGet]
        [Route("Management/ResidentApprovement/{userId}")]
        public ActionResult ViewResidentApprovement(int userId)
        {
            ViewBag.userId = userId;
            return View("ResidentApprovement");
        }

        [HttpGet]
        [Route("Management/ResidentApprovement/GetResidentApprovementList")]
        public ActionResult GetResidentApprovementList(int userId)
        {
            List<User> listUser = _userServices.GetAllUnapproveUsers();
            List<UnapprovedResident> unapprovedResident = new List<UnapprovedResident>();
            UnapprovedResident uResdident = null;
            foreach (var u in listUser)
            {
                uResdident = new UnapprovedResident();
                uResdident.UserId = u.Id;
                uResdident.FullName = u.Fullname;
                uResdident.HouseId = u.HouseId.Value;
                uResdident.HouseName = u.House.HouseName;
                uResdident.HouseHolderName = u.House.Users.First().Fullname;
                uResdident.CreateDate = u.CreateDate.Value.ToString(parternTime);
                unapprovedResident.Add(uResdident);
            }
            return Json(unapprovedResident, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/ResidentApprovement/GetResidentInfo")]
        public ActionResult GetResidentInfo(int userId)
        {
            User res = _userServices.FindById(userId);
            MessageViewModels response = new MessageViewModels();
            if (res != null)
            {
                UnapprovedResidentDetail resdident = new UnapprovedResidentDetail();
                resdident.UserId = res.Id;
                resdident.FullName = res.Fullname;
                resdident.HouseId = res.HouseId.Value;
                resdident.HouseName = res.House.HouseName;
                resdident.HouseHolderName = res.House.Users.First().Fullname;
                resdident.CreateDate = res.CreateDate.Value.ToString(parternTime);
                response.Data = resdident;
            }
            else
            {
                response.Msg = "Không tìm thấy cư dân!";
                response.StatusCode = -1;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Management/ResidentApprovement/AcceptResidentApprovement")]
        public ActionResult AcceptResidentApprovement(int resId, int fromUserId, int mode)
        {
            User res = _userServices.FindById(resId);
            User fromUser = _userServices.FindById(fromUserId);
            MessageViewModels response = new MessageViewModels();
            if (res != null && fromUser != null && fromUser.RoleId == SLIM_CONFIG.USER_ROLE_MANAGER)
            {
                bool isUpdated = true;
                if (mode == SLIM_CONFIG.USER_STATUS_ENABLE)
                {
                    res.Status = SLIM_CONFIG.USER_STATUS_ENABLE;
                }
                else if (mode == SLIM_CONFIG.USER_APPROVE_REJECT)
                {
                    res.Status = SLIM_CONFIG.USER_APPROVE_REJECT;
                }
                else
                {
                    isUpdated = false;
                }
                if (isUpdated)
                {
                    res.LastModified = DateTime.Now;
                    _userServices.Update(res);
                    if (res.Status.Value == SLIM_CONFIG.USER_STATUS_ENABLE)
                    {
                        StringBuilder message = new StringBuilder();
                        message.Append("Chung cu AMS. Tai khoan duoc kich hoat thanh cong! Ten đang nhap: ")
                                .Append(res.Username)
                                .Append(". Mat khau: ")
                                .Append(res.Password);
                        

                        CommonUtil.SentSms(res.SendPasswordTo, message.ToString());

                        //Thông báo đến tất cả những người trong nhà
                        List<User> members = res.House.Users.ToList();
                        foreach(User u in members)
                        {
                            notificationService.addNotification(SLIM_CONFIG.NOTIC_TARGET_OBJECT_ADD_MEMBER_REQUEST,u.Id,SLIM_CONFIG.NOTIC_VERB_APPROVE,fromUser.Id,null);
                        }
                    }
                }
            }
            else
            {
                response.Msg = "Không tìm thấy cư dân hoặc người quản lý!";
                response.StatusCode = -1;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateReceipt()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ManageAroundProvider()
        {
            ViewBag.AllProviders = _aroundProviderService.GetAllProviders();
            return View();
        }

        public ActionResult DetailProvider(int id)
        {
            AroundProvider curProvider = _aroundProviderService.FindById(id);
            AroundProviderDetailModel providerDetail = new AroundProviderDetailModel();
            providerDetail.Id = curProvider.Id;
            providerDetail.Name = curProvider.Name;
            providerDetail.Address = curProvider.Address;
            providerDetail.Description = curProvider.Description;
            providerDetail.ImageUrl = curProvider.ImageUrl;
            providerDetail.Tel = curProvider.Tel;
            //providerDetail.ClickCount = curProvider.ClickCount;

            return Json(providerDetail);
            //ViewBag.providerDetail = providerDetail;
            //return View("View");
        }

        [HttpPost]
        public ActionResult UpdateProvider(AroundProviderDetailModel aroundProviderDetailModel)
        {
            AroundProvider curProvider = _aroundProviderService.FindById(aroundProviderDetailModel.Id);
            curProvider.Name = aroundProviderDetailModel.Name;
            curProvider.Description = aroundProviderDetailModel.Description;
            curProvider.Address = aroundProviderDetailModel.Address;
            curProvider.Tel = aroundProviderDetailModel.Tel;
            _aroundProviderService.Update(curProvider);
            //try
            //{
            //    _aroundProviderService.Update(curProvider);
            //}
            //catch (Exception)
            //{
            //    string message = "Cannot update!";
            //    return Json(message);
            //}
            return RedirectToAction("ManageAroundProvider");
            //return RedirectToAction("ManageAroundProvider");
        }

        public ActionResult DeleteProvider(int id)
        {
            AroundProvider curProvider = _aroundProviderService.FindById(id);
            _aroundProviderService.Delete(curProvider);
            return Redirect("~/Management/ManageAroundProvider");
        }

        [HttpPost]
        public ActionResult CreateAroundProvider(AroundProviderDetailModel aroundProviderDetailModel)
        {
            MessageViewModels response = new MessageViewModels();
            try
            {
                AroundProvider provider = new AroundProvider();
                provider.Name = aroundProviderDetailModel.Name;
                provider.Description = aroundProviderDetailModel.Description;
                provider.Address = aroundProviderDetailModel.Address;
                provider.Tel = aroundProviderDetailModel.Tel;
                provider.ImageUrl = aroundProviderDetailModel.ImageUrl;
                _aroundProviderService.Add(provider);
            }
            catch (Exception)
            {
                response.StatusCode = -1;
                response.Msg = "Thêm mới thất bại";
            }

            return Json(response);
        }
        public ActionResult AddAroundProvider()
        {
            return View();
        }
    }


}