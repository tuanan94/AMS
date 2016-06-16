using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Constant;
using AMS.Models;
using AMS.Service;

namespace AMS.Controllers
{
    public class ConfigController : Controller
    {

        UtilityServiceServices _utilityServiceServices = new UtilityServiceServices();
        UtilityServiceRangePriceServices _rangePriceServices = new UtilityServiceRangePriceServices();
        [HttpGet]
        [Route("Management/Config/UtilityService/View")]
        public ActionResult UtilityServiceView()
        {
            return View("UtilityService");
        }

        [HttpPost]
        [Route("Management/Config/UtilityService/AddWaterServicePrices")]
        public ActionResult AddUtilServicePrices(UtilityServiceModel utilSrvModel)
        {
            UtilityService utilityService = new UtilityService();
            utilityService.Name = utilSrvModel.Name;
            utilityService.CreateDate = DateTime.Now;
            utilityService.LastModified = DateTime.Now;
            utilityService.Type = utilSrvModel.Type;
            MessageViewModels response = new MessageViewModels();
            _utilityServiceServices.Add(utilityService);

            if (utilSrvModel.ResInRegisBookPrices != null)
            {
                if (!ParseData(utilSrvModel.ResInRegisBookPrices, SLIM_CONFIG.RESIDENT_IN_REGISTRATION_BOOK, utilityService.Id))
                {
                    response.StatusCode = -1;
                    response.Msg = "Đã có lỗi xãy ra!";
                    return Json(response);
                }
            }
            if (utilSrvModel.ResHasKt3Prices != null)
            {
                if (!ParseData(utilSrvModel.ResHasKt3Prices, SLIM_CONFIG.RESIDENT_HAS_KT3, utilityService.Id))
                {
                    response.StatusCode = -1;
                    response.Msg = "Đã có lỗi xãy ra!";
                    return Json(response);
                }
            }
            if (utilSrvModel.ResOtherPrices != null)
            {
                if (!ParseData(utilSrvModel.ResOtherPrices, SLIM_CONFIG.RESIDENT_OTHER, utilityService.Id))
                {
                    response.StatusCode = -1;
                    response.Msg = "Đã có lỗi xãy ra!";
                    return Json(response);
                }
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
            if (fixedCostModel.FixedCosts != null)
            {
                UtilityServiceRangePrice rangePrice = null;
                foreach (var fc in fixedCostModel.FixedCosts)
                {
                    rangePrice = new UtilityServiceRangePrice();
                    rangePrice.Price = fc.Price;
                    rangePrice.Name = fc.Name;
                    rangePrice.ServiceId = utilityService.Id;
                    _rangePriceServices.Add(rangePrice);
                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Không tìm thấy các mức giá sinh hoạt";
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
                        rangePrice.ToAmount = 99999999999999;
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