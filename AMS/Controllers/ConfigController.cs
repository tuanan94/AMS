using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AMS.Constant;
using AMS.Models;
using AMS.Service;
using Microsoft.Ajax.Utilities;

namespace AMS.Controllers
{
    public class ConfigController : Controller
    {

        UtilityServiceServices _utilityServiceServices = new UtilityServiceServices();
        UtilityServiceRangePriceServices _rangePriceServices = new UtilityServiceRangePriceServices();
        HouseCategoryServices _houseCategoryServices = new HouseCategoryServices();
        UtilServiceForHouseCatServices _utilServiceForHouseCatServices = new UtilServiceForHouseCatServices();
        private ReceiptDetailServices _receiptDetailServices = new ReceiptDetailServices();

        [HttpGet]
        [Route("Management/Config/UtilityService/Create")]
        public ActionResult CreateUtilityServiceView()
        {
            List<HouseCategory> houseCat = _houseCategoryServices.GetAll();
            ViewBag.houseCat = houseCat;
            return View("CreateUtilityService");
        }

        [HttpGet]
        [Route("Management/Config/UtilityService/View")]
        public ActionResult UtilityServiceView()
        {
            return View("ManageUtilityService");
        }

        [HttpGet]
        [Route("Management/Config/UtilityService/GetUtilityServices")]
        public ActionResult GetUtilityService()
        {
            List<UtilServiceForHouseCategoryModel> listmodel = new List<UtilServiceForHouseCategoryModel>();
            UtilServiceForHouseCategoryModel model = null;
            List<UtilServiceForHouseCat> fixCostList = null;
            List<UtilServiceForHouseCat> utilSrvForHouseCat = _utilServiceForHouseCatServices.GetAllGroupByUtilServiceId();

            foreach (var houseCat in utilSrvForHouseCat.Where(utilSrv => utilSrv.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER).ToList())
            {

                model = new UtilServiceForHouseCategoryModel();
                model.Id = houseCat.Id;
                model.Name = houseCat.UtilityService.Name;
                model.CreateDate = houseCat.CreateDate.Value.ToString(AmsConstants.DateFormat);
                model.HouseCat = houseCat.HouseCategory.Name;
                model.Status = houseCat.Status.Value;
                model.Type = SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER;
                model.DT_RowId = new StringBuilder("h_cat_util_srv_").Append(houseCat.Id).ToString();
                listmodel.Add(model);
            }
            foreach (var fixedCost in utilSrvForHouseCat.Where(utilSrv => utilSrv.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST).ToList())
            {
                model = new UtilServiceForHouseCategoryModel();
                model.Id = fixedCost.Id;
                model.Name = fixedCost.UtilityService.Name;
                model.CreateDate = fixedCost.CreateDate.Value.ToString(AmsConstants.DateFormat);
                model.HouseCat = "*";
                model.Status = fixedCost.Status.Value;
                model.Type = SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST;
                model.DT_RowId = new StringBuilder("h_cat_util_srv_").Append(fixedCost.Id).ToString();
                listmodel.Add(model);
            }

            return Json(listmodel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Management/Config/UtilityService/AddWaterServicePrices")]
        public ActionResult AddUtilServicePrices(UtilityServiceModel utilSrvModel)
        {
            HouseCategory houseCat = _houseCategoryServices.FindById(utilSrvModel.HouseCatId);
            MessageViewModels response = new MessageViewModels();
            if (houseCat != null)
            {
                UtilityService utilityService = new UtilityService();
                utilityService.Name = utilSrvModel.Name;
                utilityService.CreateDate = DateTime.Now;
                utilityService.LastModified = DateTime.Now;
                utilityService.Type = utilSrvModel.Type;

                _utilityServiceServices.Add(utilityService);

                if (utilSrvModel.WaterUtilServiceRangePrices != null)
                {
                    if (
                        !ParseData(utilSrvModel.WaterUtilServiceRangePrices, SLIM_CONFIG.RESIDENT_IN_REGISTRATION_BOOK,
                            utilityService.Id))
                    {
                        response.StatusCode = -1;
                        response.Msg = "Đã có lỗi xãy ra!";
                        return Json(response);
                    }
                    UtilServiceForHouseCat utilSrvForHouseCat = new UtilServiceForHouseCat();
                    utilSrvForHouseCat.CreateDate = DateTime.Now;
                    utilSrvForHouseCat.HouseCatId = houseCat.Id;
                    utilSrvForHouseCat.UtilServiceId = utilityService.Id;
                    utilSrvForHouseCat.Status = SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_DISABLE;
                    _utilServiceForHouseCatServices.Add(utilSrvForHouseCat);
                }

            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không tìm thấy loại nhà";
            }

            return Json(response);
        }

        [HttpGet]
        [Route("Management/Config/UtilityService/EditUtilServiceView")]
        public ActionResult EditUtilServiceView(int id)
        {
            List<HouseCategory> houseCategories = _houseCategoryServices.GetAll();
            UtilServiceForHouseCat utilServiceForHouseCat = _utilServiceForHouseCatServices.FindById(id);
            if (utilServiceForHouseCat != null)
            {
                ViewBag.utilServiceForHouseCat = utilServiceForHouseCat;
                ViewBag.houseCategories = houseCategories;
            }
            else
            {

            }

            return View("UpdateUtilityService");
        }

        [HttpPost]
        [Route("Management/Config/UtilityService/UpdateUtilServiceForHouseCat")]
        public ActionResult UpdateUtilServiceForHouseCat(UtilityServiceModel utilSrvModel)
        {
            MessageViewModels response = new MessageViewModels();
            UtilServiceForHouseCat utilServiceForHouseCat = _utilServiceForHouseCatServices.FindById(utilSrvModel.UtilServiceForHouseCatId);
            if (utilServiceForHouseCat != null)
            {
                UtilityService utilService = _utilityServiceServices.FindById(utilServiceForHouseCat.UtilServiceId.Value);
                if (null != utilService)
                {
                    utilService.Name = utilSrvModel.Name;
                    utilService.LastModified = DateTime.Now;
                    _utilityServiceServices.Update(utilService);

                    if (utilSrvModel.WaterUtilServiceRangePrices != null)
                    {
                        int count = 0;
                        UtilityServiceRangePrice rp = null;
                        foreach (var rangePrice in utilSrvModel.WaterUtilServiceRangePrices)
                        {
                            count++;
                            if (count == utilSrvModel.WaterUtilServiceRangePrices.Count && rangePrice.ToAmount.Equals("*"))
                            {
                                rangePrice.ToAmount = SLIM_CONFIG.MAX_NUMBER.ToString();
                            }
                            if (rangePrice.Id == 0)
                            {
                                rp = new UtilityServiceRangePrice();
                                rp.Name = rangePrice.Name;
                                rp.FromAmount = double.Parse(rangePrice.FromAmount);
                                rp.ToAmount = double.Parse(rangePrice.ToAmount);
                                rp.Price = rangePrice.Price;
                                rp.CreateDate = DateTime.Now;
                                rp.LastModified = DateTime.Now;
                                rp.ServiceId = utilService.Id;
                                _rangePriceServices.Add(rp);
                            }
                            else
                            {
                                rp = _rangePriceServices.FindById(rangePrice.Id);
                                if (rp != null)
                                {
                                    rp.Name = rangePrice.Name;
                                    rp.FromAmount = double.Parse(rangePrice.FromAmount);
                                    rp.ToAmount = double.Parse(rangePrice.ToAmount);
                                    rp.Price = rangePrice.Price;
                                    rp.LastModified = DateTime.Now;
                                    _rangePriceServices.Update(rp);
                                }
                            }
                        }
                        if (null != utilSrvModel.DeletedRangePrices)
                        {
                            foreach (var deletedRangePrice in utilSrvModel.DeletedRangePrices)
                            {
                                _rangePriceServices.DeleteById(deletedRangePrice);
                            }
                        }

                        if (utilServiceForHouseCat.Status == SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_ENABLE)
                        {
                            List<ReceiptDetail> receiptDetails = utilService.ReceiptDetails.Where(
                                rd => rd.Receipt.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED &&
                                    rd.Receipt.IsAutomation == SLIM_CONFIG.RECEIPT_TYPE_AUTOMATION).ToList();
                            foreach (var rd in receiptDetails)
                            {
                                ReceiptDetail receiptDetail = _receiptDetailServices.FindById(rd.Id);
                                receiptDetail.TotalBill = ReceiptController.CalculateWaterUtiServiceVersion1(receiptDetail.Quantity.Value,
                                    utilService.UtilityServiceRangePrices.ToList());
                                receiptDetail.LastModified = DateTime.Now;
                                if (receiptDetail.Quantity.Value != 0)
                                {
                                    receiptDetail.UnitPrice = receiptDetail.TotalBill / receiptDetail.Quantity.Value;
                                }
                                else
                                {
                                    receiptDetail.UnitPrice = 0;
                                }
                                _receiptDetailServices.Update(receiptDetail);
                            }
                        }// if this water utility service is applied in this month. It must be update all order.
                    }
                }
                else
                {
                    response.StatusCode = -1;
                    response.Msg = "Không tìm thấy";
                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không tìm thấy";
            }

            return Json(response);
        }


        [HttpPost]
        [Route("Management/Config/UtilityService/AddFixedCost")]
        public ActionResult AddFixedCost(FixedCostModel fixedCostModel)
        {
            UtilityService utilityService = new UtilityService();
            utilityService.Name = fixedCostModel.Name;
            utilityService.CreateDate = DateTime.Now;
            utilityService.LastModified = DateTime.Now;
            utilityService.Type = SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST;
            MessageViewModels response = new MessageViewModels();
            _utilityServiceServices.Add(utilityService);

            if (fixedCostModel.FixedCost != null)
            {
                FixedCostPriceModel fc = fixedCostModel.FixedCost;
                UtilityServiceRangePrice rangePrice = new UtilityServiceRangePrice();
                rangePrice.Price = fc.Price;
                rangePrice.Name = fc.Name;
                rangePrice.ServiceId = utilityService.Id;
                _rangePriceServices.Add(rangePrice);

                foreach (var houseCat in _houseCategoryServices.GetAll())
                {
                    UtilServiceForHouseCat utilSrvForHouseCat = new UtilServiceForHouseCat();
                    utilSrvForHouseCat.CreateDate = DateTime.Now;
                    utilSrvForHouseCat.HouseCatId = houseCat.Id;
                    utilSrvForHouseCat.UtilServiceId = utilityService.Id;
                    utilSrvForHouseCat.Status = SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_DISABLE;
                    _utilServiceForHouseCatServices.Add(utilSrvForHouseCat);
                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không tìm thấy các mức giá sinh hoạt";
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/Config/UtilityService/DeleteUtilServices")]
        public ActionResult DeleteUtilServices(List<int> deleteItems)
        {
            MessageViewModels response = new MessageViewModels();
            if (deleteItems != null)
            {
                foreach (var item in deleteItems)
                {
                    UtilServiceForHouseCat utilServiceForHouseCat = _utilServiceForHouseCatServices.FindById(item);
                    if (null != utilServiceForHouseCat)
                    {
                        List<UtilServiceForHouseCat> serviceForHouseCats =
                            _utilServiceForHouseCatServices.GetByUtilServiceid(
                                utilServiceForHouseCat.UtilServiceId.Value);
                        foreach (var s in serviceForHouseCats)
                        {
                            s.Status = SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_REMOVED;
                            _utilServiceForHouseCatServices.Update(s);
                        }
//                        if (utilSrv != null)
//                        {
//                            foreach (var range in utilSrv.UtilityServiceRangePrices)
//                            {
//                                _rangePriceServices.DeleteById(range.Id);
//                            }
//                            _utilityServiceServices.Delete(_utilityServiceServices.FindById(utilSrv.Id));
//                        }
                    }
                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không tìm thấy các mức giá sinh hoạt";
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/Config/UtilityService/UpdateFixedCost")]
        public ActionResult UpdateFixedCost(FixedCostModel fixedCostModel)
        {
            MessageViewModels response = new MessageViewModels();
            UtilityService fixedCost = _utilityServiceServices.FindById(fixedCostModel.Id);
            if (fixedCost != null && null != fixedCostModel.FixedCost)
            {
                fixedCost.Name = fixedCostModel.Name;
                fixedCost.LastModified = DateTime.Now;
                _utilityServiceServices.Update(fixedCost);

                UtilityServiceRangePrice rangePrice =
                    _rangePriceServices.FindById(fixedCost.UtilityServiceRangePrices.First().Id);
                rangePrice.LastModified = DateTime.Now;
                rangePrice.Name = fixedCostModel.FixedCost.Name;
                rangePrice.Price = fixedCostModel.FixedCost.Price;
                _rangePriceServices.Update(rangePrice);

                List<UtilServiceForHouseCat> fixCostForHouseCats = fixedCost.UtilServiceForHouseCats.GroupBy(fc => fc.UtilServiceId).Select(fc => fc.First()).ToList();
                if (fixCostForHouseCats.First().Status == SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_ENABLE)
                {
                    List<ReceiptDetail> receiptDetails = fixedCost.ReceiptDetails.Where(
                        rd => rd.Receipt.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED &&
                            rd.Receipt.IsAutomation == SLIM_CONFIG.RECEIPT_TYPE_AUTOMATION).ToList();
                    foreach (var rd in receiptDetails)
                    {
                        ReceiptDetail receiptDetail = _receiptDetailServices.FindById(rd.Id);

                        receiptDetail.TotalBill = rangePrice.Price * rd.Receipt.House.Area;
                        receiptDetail.LastModified = DateTime.Now;
                        receiptDetail.Quantity = (int)rd.Receipt.House.Area.Value;
                        if (receiptDetail.Quantity.Value != 0)
                        {
                            receiptDetail.UnitPrice = receiptDetail.TotalBill / receiptDetail.Quantity.Value;
                        }
                        _receiptDetailServices.Update(receiptDetail);
                    }
                }// if this water utility service is applied in this month. It must be update all order.

            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không tìm thấy các mức giá sinh hoạt";
            }
            return Json(response);
        }


        [HttpGet]
        [Route("Management/Config/UtilityService/GetListHouseType")]
        public ActionResult GetListHouseType()
        {
            List<HouseCategory> houseCat = _houseCategoryServices.GetAll();
            HouseCategoryModel houseCatModel = null;
            List<HouseCategoryModel> houseCatModelList = new List<HouseCategoryModel>();
            foreach (var cat in houseCat)
            {
                houseCatModel = new HouseCategoryModel();
                houseCatModel.Name = cat.Name;
                houseCatModel.Id = cat.Id;
                houseCatModelList.Add(houseCatModel);
            }
            return Json(houseCatModelList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Management/Config/UtilityService/ActivateUtilServices")]
        public ActionResult ActivateUtilServices(int id)
        {
            MessageViewModels response = new MessageViewModels();
            UtilServiceForHouseCat utilServiceForHouseCat = _utilServiceForHouseCatServices.FindById(id);
            if (null != utilServiceForHouseCat)
            {
                if (utilServiceForHouseCat.Status != SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_ENABLE)
                {

                    /*Deactiveate last util service*/
                    List<UtilServiceForHouseCat> curentUtilServices =
                    _utilServiceForHouseCatServices.GetActiveUtilServiceOfHouseCategory(utilServiceForHouseCat.HouseCatId.Value);
                    UtilServiceForHouseCat curUtilSrv = null;
                    List<ReceiptDetail> receiptDetails = null;

                    if (utilServiceForHouseCat.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER)
                    {
                        /*Activeate this util service*/
                        utilServiceForHouseCat.Status = SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_ENABLE;
                        _utilServiceForHouseCatServices.Update(utilServiceForHouseCat);


                        if (curentUtilServices.Count != 0)
                        {
                            /*Deactiveate last util service*/
                            curUtilSrv = curentUtilServices.Where(utilSrv => utilSrv.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER).ToList().First();
                            curUtilSrv.Status = SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_DISABLE;
                            _utilServiceForHouseCatServices.Update(curUtilSrv);

                            /*Update if exist order that applied this util service*/
                            receiptDetails = curUtilSrv.UtilityService.ReceiptDetails.Where(
                                rd => rd.Receipt.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED &&
                                    rd.Receipt.IsAutomation == SLIM_CONFIG.RECEIPT_TYPE_AUTOMATION).ToList();
                            foreach (var rd in receiptDetails)
                            {
                                ReceiptDetail receiptDetail = _receiptDetailServices.FindById(rd.Id);

                                /*Get rangePrices of previous util Item*/
                                List<UtilityServiceRangePrice> rangePrices =
                                    utilServiceForHouseCat.UtilityService.UtilityServiceRangePrices.ToList();

                                receiptDetail.TotalBill =
                                    ReceiptController.CalculateWaterUtiServiceVersion1(receiptDetail.Quantity.Value,
                                        rangePrices);

                                /*Reassign new item*/
                                receiptDetail.UtilityServiceId = utilServiceForHouseCat.UtilServiceId;

                                receiptDetail.LastModified = DateTime.Now;

                                if (receiptDetail.Quantity.Value != 0)
                                {
                                    receiptDetail.UnitPrice = receiptDetail.TotalBill / receiptDetail.Quantity.Value;
                                }
                                else
                                {
                                    receiptDetail.UnitPrice = 0;
                                }
                                _receiptDetailServices.Update(receiptDetail);
                            }
                        }// Update current active to deactive. And get order detail of current active util service


                    }
                    else
                    {
                        List<UtilServiceForHouseCat> fixCostList = _utilServiceForHouseCatServices.GetByUtilServiceid(
                                utilServiceForHouseCat.UtilServiceId.Value);
                        foreach (var fixedCost in fixCostList)
                        {
                            fixedCost.Status = SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_ENABLE;
                            _utilServiceForHouseCatServices.Update(fixedCost);
                        }
                        if (curentUtilServices.Count != 0)
                        {
                            foreach (var curentSrv in curentUtilServices.Where(utilSrv => utilSrv.UtilityService.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST).ToList())
                            {
                                List<UtilServiceForHouseCat> lastFixCostList = _utilServiceForHouseCatServices.GetByUtilServiceid(
                                curentSrv.UtilServiceId.Value);
                                foreach (var fixedCost in lastFixCostList)
                                {
                                    fixedCost.Status = SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_DISABLE;
                                    _utilServiceForHouseCatServices.Update(fixedCost);
                                }

                                receiptDetails = curentSrv.UtilityService.ReceiptDetails.Where(
                                    rd => rd.Receipt.Status == SLIM_CONFIG.RECEIPT_STATUS_UNPUBLISHED &&
                                    rd.Receipt.IsAutomation == SLIM_CONFIG.RECEIPT_TYPE_AUTOMATION).ToList();
                                foreach (var rd in receiptDetails)
                                {
                                    ReceiptDetail receiptDetail = _receiptDetailServices.FindById(rd.Id);
                                    double price =
                                        utilServiceForHouseCat.UtilityService.UtilityServiceRangePrices.First().Price.Value;
                                    receiptDetail.TotalBill = price * rd.Receipt.House.Area;
                                    receiptDetail.LastModified = DateTime.Now;
                                    receiptDetail.Quantity = (int)rd.Receipt.House.Area.Value;
                                    receiptDetail.UtilityServiceId = utilServiceForHouseCat.UtilServiceId.Value;
                                    if (receiptDetail.Quantity.Value != 0)
                                    {
                                        receiptDetail.UnitPrice = receiptDetail.TotalBill / receiptDetail.Quantity.Value;
                                    }
                                    _receiptDetailServices.Update(receiptDetail);
                                }

                            }
                        }




                    }

                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không tìm thấy dịch vụ tiện ích";
            }

            return Json(response);
        }


        private bool ParseData(List<UtilityServiceRangePriceModel> priceList, int type, int utilServiceId)
        {
            UtilityServiceRangePrice rangePrice = null;
            int count = 0;
            foreach (var rp in priceList)
            {
                rangePrice = new UtilityServiceRangePrice();
                rangePrice.Name = rp.Name;
                try
                {
                    rangePrice.FromAmount = Double.Parse(rp.FromAmount);
                    count++;
                    if (count == priceList.Count && rp.ToAmount.Equals("*"))
                    {
                        rangePrice.ToAmount = SLIM_CONFIG.MAX_NUMBER;
                    }
                    else
                    {
                        rangePrice.ToAmount = Double.Parse(rp.ToAmount);
                    }
                    rangePrice.Price = rp.Price;
                    rangePrice.ServiceId = utilServiceId;
                    rangePrice.Type = type;
                    _rangePriceServices.Add(rangePrice);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }
    }

}