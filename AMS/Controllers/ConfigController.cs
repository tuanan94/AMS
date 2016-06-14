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

        UtilityCategoryServices _utilityCategoryServices = new UtilityCategoryServices();
        UtilityServiceServices _utilityServiceServices = new UtilityServiceServices();
        UtilityServiceRangePriceServices _rangePriceServices = new UtilityServiceRangePriceServices();
        [HttpGet]
        [Route("Management/Config/UtilityService/View")]
        public ActionResult UtilityServiceView()
        {
            return View("UtilityService");
        }

        [HttpPost]
        [Route("Management/Config/UtilityService/AddUtilServicePrices")]
        public ActionResult AddUtilServicePrices(UtilityServiceModel utilSrvModel)
        {
            UtilityService utilityService = new UtilityService();
            utilityService.Name = utilSrvModel.Name;
            utilityService.CreateDate = DateTime.Now;
            utilityService.LastModified = DateTime.Now;
            MessageViewModels response = new MessageViewModels();

            UtilityServiceCategory utilCat = _utilityCategoryServices.FindByType(utilSrvModel.Type);
            if (utilCat == null)
            {
                utilCat = new UtilityServiceCategory();
                utilCat.Type = utilSrvModel.Type;
                string catName = "";
                if (utilSrvModel.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_ELECTRICITY)
                {
                    catName = AmsConstants.UtilityServiceElectricity;
                }
                else if (utilSrvModel.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_WATER)
                {
                    catName = AmsConstants.UtilityServiceWater;
                }
                else if (utilSrvModel.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_HOUSE_RENT)
                {
                    catName = AmsConstants.UtilityServiceHouseRent;
                }
                else if (utilSrvModel.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_HD_REQUEST)
                {
                    catName = AmsConstants.UtilityServiceHelpdeskRequest;
                }
                else if (utilSrvModel.Type == SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST)
                {
                    catName = AmsConstants.UtilityServiceFixedCost;
                }
                utilCat.Name = catName;
                utilCat.CreateDate = DateTime.Now;
                utilCat.LastModified = DateTime.Now;
                _utilityCategoryServices.Add(utilCat);
                utilityService.CategoryId = utilCat.Id;
            }
            else
            {
                utilityService.CategoryId = utilCat.Id;
            }
            _utilityServiceServices.Add(utilityService);

            if (utilSrvModel.UtilityServicePriceRanges != null)
            {
                UtilityServiceRangePrice rangePrice = null;
                int count = 0;
                foreach ( var rp in utilSrvModel.UtilityServicePriceRanges)
                {
                    rangePrice = new UtilityServiceRangePrice();
                    rangePrice.Name = rp.Name;
                    try
                    {
                        rangePrice.FromAmount = Double.Parse(rp.FromAmount);
                        count++;
                        if (count == utilSrvModel.UtilityServicePriceRanges.Count && rp.ToAmount.Equals("*"))
                        {
                            rangePrice.ToAmount = 99999999999999;
                        }
                        else
                        {
                            rangePrice.ToAmount = Double.Parse(rp.ToAmount);
                        }
                        rangePrice.Price = rp.Price;
                        rangePrice.ServiceId = utilityService.Id;

                        _rangePriceServices.Add(rangePrice);
                    }
                    catch (Exception)
                    {
                        response.StatusCode = -1;
                        response.Msg = "Các mức giá tiền không đúng";
                        return Json(response);
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
        [Route("Management/Config/UtilityService/AddFixedCost")]
        public ActionResult AddFixedCost(FixedCostModel fixedCostModel)
        {
            UtilityService utilityService = new UtilityService();
            utilityService.Name = fixedCostModel.Name;
            utilityService.CreateDate = DateTime.Now;
            utilityService.LastModified = DateTime.Now;
            MessageViewModels response = new MessageViewModels();

            UtilityServiceCategory utilCat = _utilityCategoryServices.FindByType(SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST);
            if (utilCat == null)
            {
                utilCat = new UtilityServiceCategory();
                utilCat.Type = SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST;
                utilCat.Name = AmsConstants.UtilityServiceFixedCost;
                utilCat.CreateDate = DateTime.Now;
                utilCat.LastModified = DateTime.Now;
                _utilityCategoryServices.Add(utilCat);
                utilityService.CategoryId = utilCat.Id;
            }
            else
            {
                utilityService.CategoryId = utilCat.Id;
            }
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
    }

}