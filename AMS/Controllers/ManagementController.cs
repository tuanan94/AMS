using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AMS.Models;
using AMS.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AMS.Controllers
{
    public class ManagementController : Controller
    {
        HelpdeskServicesService _helpdeskServicesService = new HelpdeskServicesService();
        HelpdeskServiceCatService _helpdeskServiceCatService = new HelpdeskServiceCatService();
        //
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
                if (action.Equals("helpdeskServiceManage"))
                {
                    url = "HelpdeskServiceTabPartial";
                    ViewBag.helpdeskServices = _helpdeskServicesService.GetHelpdeskServices();
                }
                else if (action.Equals("hdSrvCategoryManage"))
                {
                    url = "_helpdeskServiceCategoryPartial";
                    ViewBag.helpdeskServiceCategories = _helpdeskServiceCatService.GetAll();
                }
                else if (action.Equals("loadHdSrvCat"))
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
                            HelpdeskSerivceCatListModel hdSrvCatListModel = new HelpdeskSerivceCatListModel(hdSrvCats);
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

                    List<String> listError = new List<string>();
                    MessageViewModels response = new MessageViewModels();
                    //                    var message = new List<object>();
                    //                    message.Add(new { txt_error = "MSG" });
                    //                    message.Add(new { txt_error_2 = "MSG" });
                    //                    response.Data = message;

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

                            //                            List<String> listError = new List<string>();
                            //                            MessageViewModels response = new MessageViewModels();
                            //                            var message = new List<object>();
                            //                            message.Add(new {txt_error = "MSG"});
                            //                            message.Add(new {txt_error_2 = "MSG"});
                            //                            response.Data = message;
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
                }else if (action.Equals("searchHdSrvCategory"))
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
                    String idStrList = nvc["hdSrvCatDeletedList"];
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
                

                return PartialView(url);
            }
            else
            {
                return View();
            }
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
        public ActionResult ManageReceipt()
        {
            return View();
        }
        public ActionResult ViewReceipt()
        {
            return View();
        }
        public ActionResult CreateReceipt()
        {
            return View();
        }
    }


}