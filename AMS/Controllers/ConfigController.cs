using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using AMS.Constant;
using AMS.Filter;
using AMS.Models;
using AMS.Service;
using Microsoft.Ajax.Utilities;

namespace AMS.Controllers
{
    public class ConfigController : Controller
    {

        private UserServices _userServices = new UserServices();
        private UtilityServiceServices _utilityServiceServices = new UtilityServiceServices();
        private UtilityServiceRangePriceServices _rangePriceServices = new UtilityServiceRangePriceServices();
        private HouseCategoryServices _houseCategoryServices = new HouseCategoryServices();
        private UtilServiceForHouseCatServices _utilServiceForHouseCatServices = new UtilServiceForHouseCatServices();
        private ReceiptDetailServices _receiptDetailServices = new ReceiptDetailServices();
        private TransactionService _transactionService = new TransactionService();
        private UtilityServiceCateoryService _utilityServiceCateoryService = new UtilityServiceCateoryService();
        private BlockServices _blockServices = new BlockServices();
        private HouseServices _houseServices = new HouseServices();

        [HttpGet]
        [AdminAuthorize]
        [Route("Management/Config/UtilityService/Create")]
        public ActionResult CreateUtilityServiceView()
        {
            List<HouseCategory> houseCat = _houseCategoryServices.GetAll();
            ViewBag.houseCat = houseCat;
            return View("CreateUtilityService");
        }

        [HttpGet]
        [AdminAuthorize]
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
                model.UtilSrvCatName = houseCat.UtilityService.UtilServiceCategory.Name;
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
                model.UtilSrvCatName = fixedCost.UtilityService.UtilServiceCategory.Name;
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
                // Check  waterTransaction category and fixedCost category is existed
                List<UtilServiceCategory> allTransCategory = _utilityServiceCateoryService.GetAllMandatory();
                UtilServiceCategory waterUtilSrv = null;
                foreach (var transCat in allTransCategory)
                {
                    if (transCat.Name.Trim().Equals(AmsConstants.UtilityServiceWater))
                    {
                        waterUtilSrv = transCat;
                    }
                }
                if (waterUtilSrv == null)
                {
                    waterUtilSrv = new UtilServiceCategory();
                    waterUtilSrv.Name = AmsConstants.UtilityServiceWater;
                    waterUtilSrv.Status = SLIM_CONFIG.TRANS_CAT_STATUS_DENY_REMOVE;
                    waterUtilSrv.CreateDate = DateTime.Now;
                    waterUtilSrv.LastModified = DateTime.Now;
                    _utilityServiceCateoryService.Add(waterUtilSrv);
                }

                UtilityService utilityService = new UtilityService();
                utilityService.Name = utilSrvModel.Name;
                utilityService.CreateDate = DateTime.Now;
                utilityService.LastModified = DateTime.Now;
                utilityService.Type = utilSrvModel.Type;
                utilityService.UtilSrvCatId = waterUtilSrv.Id;
                _utilityServiceServices.Add(utilityService);

                if (utilSrvModel.WaterUtilServiceRangePrices != null)
                {
                    if (
                        !ParseData(utilSrvModel.WaterUtilServiceRangePrices, SLIM_CONFIG.RESIDENT_IN_REGISTRATION_BOOK,
                            utilityService.Id))
                    {
                        response.StatusCode = -1;
                        response.Msg = "Đã có lỗi xảy ra!";
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
                response.Msg = "Không tìm thấy dạng cư trú";
            }

            return Json(response);
        }

        [HttpGet]
        [AdminAuthorize]
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
                                receiptDetail.Total = ReceiptController.CalculateWaterUtiServiceVersion1(receiptDetail.Quantity.Value,
                                    utilService.UtilityServiceRangePrices.ToList());
                                receiptDetail.LastModified = DateTime.Now;
                                if (receiptDetail.Quantity.Value != 0)
                                {
                                    receiptDetail.UnitPrice = receiptDetail.Total / receiptDetail.Quantity.Value;
                                }
                                else
                                {
                                    receiptDetail.UnitPrice = 0;
                                }
                                var transactions = receiptDetail.Transactions.Where(tr => tr.BalanceSheet.Id == receiptDetail.Receipt.BlsId).ToList();
                                _receiptDetailServices.Update(receiptDetail);

                                //Update total amount of transaction when user receipt is unpublish
                                UpdateTotalAmountInTransaction(transactions, receiptDetail.Total.Value);
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
            // Check  waterTransaction category and fixedCost category is existed
            List<UtilServiceCategory> allTransCategory = _utilityServiceCateoryService.GetAllMandatory();
            UtilServiceCategory hdRequestTransCat = null;
            MessageViewModels response = new MessageViewModels();
            foreach (var transCat in allTransCategory)
            {
                if (transCat.Name.Trim().Equals(AmsConstants.UtilityServiceFixedCost))
                {
                    hdRequestTransCat = transCat;
                }
            }
            if (hdRequestTransCat == null)
            {
                hdRequestTransCat = new UtilServiceCategory();
                hdRequestTransCat.Name = AmsConstants.UtilityServiceFixedCost;
                hdRequestTransCat.Status = SLIM_CONFIG.TRANS_CAT_STATUS_DENY_REMOVE;
                hdRequestTransCat.CreateDate = DateTime.Now;
                hdRequestTransCat.LastModified = DateTime.Now;
                _utilityServiceCateoryService.Add(hdRequestTransCat);
            }

            UtilityService utilityService = new UtilityService();
            utilityService.Name = fixedCostModel.Name;
            utilityService.CreateDate = DateTime.Now;
            utilityService.LastModified = DateTime.Now;
            utilityService.Type = SLIM_CONFIG.UTILITY_SERVICE_TYPE_FIXED_COST;
            utilityService.UtilSrvCatId = hdRequestTransCat.Id;
            _utilityServiceServices.Add(utilityService);

            if (fixedCostModel.FixedCost != null)
            {
                FixedCostPriceModel fc = fixedCostModel.FixedCost;
                UtilityServiceRangePrice rangePrice = new UtilityServiceRangePrice();
                rangePrice.Price = fc.Price;
                rangePrice.Name = fc.Name;
                rangePrice.ServiceId = utilityService.Id;
                rangePrice.CreateDate = DateTime.Now;
                rangePrice.LastModified = DateTime.Now;
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

                        receiptDetail.Total = rangePrice.Price * rd.Receipt.House.Area;
                        receiptDetail.LastModified = DateTime.Now;
                        receiptDetail.Quantity = (int)rd.Receipt.House.Area.Value;
                        if (receiptDetail.Quantity.Value != 0)
                        {
                            receiptDetail.UnitPrice = receiptDetail.Total / receiptDetail.Quantity.Value;
                        }
                        var transactions = receiptDetail.Transactions.Where(tr => tr.BalanceSheet.Id == receiptDetail.Receipt.BlsId).ToList();
                        _receiptDetailServices.Update(receiptDetail);

                        //Update total amount of transaction when user receipt is unpublish
                        UpdateTotalAmountInTransaction(transactions, receiptDetail.Total.Value);
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

                                receiptDetail.Total =
                                    ReceiptController.CalculateWaterUtiServiceVersion1(receiptDetail.Quantity.Value,
                                        rangePrices);

                                /*Reassign new item*/
                                receiptDetail.UtilityServiceId = utilServiceForHouseCat.UtilServiceId;

                                receiptDetail.LastModified = DateTime.Now;

                                if (receiptDetail.Quantity.Value != 0)
                                {
                                    receiptDetail.UnitPrice = receiptDetail.Total / receiptDetail.Quantity.Value;
                                }
                                else
                                {
                                    receiptDetail.UnitPrice = 0;
                                }
                                var transactions = receiptDetail.Transactions.Where(tr => tr.BalanceSheet.Id == receiptDetail.Receipt.BlsId).ToList();
                                _receiptDetailServices.Update(receiptDetail);

                                //Update total amount of transaction when user receipt is unpublish
                                UpdateTotalAmountInTransaction(transactions, receiptDetail.Total.Value);
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
                                    receiptDetail.Total = price * rd.Receipt.House.Area;
                                    receiptDetail.LastModified = DateTime.Now;
                                    receiptDetail.Quantity = (int)rd.Receipt.House.Area.Value;
                                    receiptDetail.UtilityServiceId = utilServiceForHouseCat.UtilServiceId.Value;
                                    if (receiptDetail.Quantity.Value != 0)
                                    {
                                        receiptDetail.UnitPrice = receiptDetail.Total / receiptDetail.Quantity.Value;
                                    }
                                    var transactions = receiptDetail.Transactions.Where(tr => tr.BalanceSheet.Id == receiptDetail.Receipt.BlsId).ToList();
                                    _receiptDetailServices.Update(receiptDetail);

                                    //Update total amount of transaction when user receipt is unpublish
                                    UpdateTotalAmountInTransaction(transactions, receiptDetail.Total.Value);

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

        [HttpGet]
        [AdminAuthorize]
        [Route("Management/Config/UtilityService/ViewManageHouseBlock")]
        public ActionResult ViewManageHouseBlock()
        {
            return View("ManageHouseBlock");
        }

        [HttpGet]
        [AdminAuthorize]
        [Route("Management/Config/UtilityService/ViewCreateHouseBlock")]
        public ActionResult ViewCreateHouseBlock()
        {
            return View("CreateHouseBlock");
        }

        [HttpGet]
        [AdminAuthorize]
        [Route("Management/Config/UtilityService/ViewHousesInBlock")]
        public ActionResult ViewHousesInBlock(int blockId)
        {
            Block block = _blockServices.FindById(blockId);
            if (null != block)
            {
                block.NoFloor = block.Houses.GroupBy(h => h.Floor).Count();
                bool canRemoveBlock = true;
                if (block.Houses.Any(house => house.Status == SLIM_CONFIG.HOUSE_STATUS_ENABLE && house.OwnerID != null))
                {
                    canRemoveBlock = false;
                }
                ViewBag.block = block;
                ViewBag.canRemoveBlock = canRemoveBlock;
            }
            return View("ManageHouseInBlock");
        }

        [HttpPost]
        [Route("Management/Config/UtilityService/CreateHouseInBlock")]
        public ActionResult CreateHouseInBlock(BlockViewModel housesInBlock)
        {
            MessageViewModels response = new MessageViewModels();
            if (null != housesInBlock.Name)
            {
                try
                {
                    housesInBlock.Name = housesInBlock.Name.Trim();
                    if (!_blockServices.CheckBlockNameIsExisted(housesInBlock.Name))
                    {
                        Block block = new Block();
                        block.BlockName = housesInBlock.Name;
                        block.NoFloor = housesInBlock.NoOfFloor;
                        block.NoFloor = housesInBlock.NoRoomPerFloor;
                        _blockServices.Add(block);

                        if (null != housesInBlock.Houses || housesInBlock.NoOfFloor == 0)
                        {
                            House eHouse = null;
                            for (int i = 0; i < housesInBlock.NoOfFloor; i++)
                            {
                                foreach (var house in housesInBlock.Houses)
                                {
                                    eHouse = new House();
                                    eHouse.Area = house.Area;
                                    eHouse.Floor = i.ToString();
                                    if (i == 0)
                                    {
                                        eHouse.Floor = "G";
                                    }
                                    eHouse.HouseName =
                                        new StringBuilder(block.BlockName).Append("-")
                                            .Append(eHouse.Floor.Trim())
                                            .Append("-")
                                            .Append(house.Name.Trim())
                                            .ToString();
                                    eHouse.BlockId = block.Id;
                                    eHouse.Status = SLIM_CONFIG.HOUSE_STATUS_ENABLE;
                                    _houseServices.Add(eHouse);
                                }
                            }
                            response.Data = block.Id;
                        }
                        else
                        {
                            response.StatusCode = -1;
                            return Json(response);
                        }
                    }
                    else
                    {
                        response.StatusCode = 2;
                        return Json(response);
                    }
                }
                catch (Exception)
                {
                    response.StatusCode = -1;
                    return Json(response);
                }
            }
            else
            {
                response.StatusCode = -1;
                return Json(response);
            }
            return Json(response);
        }

        [HttpGet]
        [Route("Management/Config/UtilityService/GetHouseInBlock")]
        public ActionResult GetHouseInBlock(int blockId)
        {
            List<HouseViewModel> houses = new List<HouseViewModel>();
            Block block = _blockServices.FindById(blockId);
            if (null != block)
            {
                List<House> listHouse = block.Houses.Where(h => h.Status == SLIM_CONFIG.HOUSE_STATUS_ENABLE).ToList();
                HouseViewModel house = null;
                foreach (var h in listHouse)
                {
                    house = new HouseViewModel();
                    house.Id = h.Id;
                    house.Name = h.HouseName;
                    house.FloorName = h.Floor;
                    house.Area = h.Area.Value;
                    house.Status = h.OwnerID == null
                        ? SLIM_CONFIG.HOUSE_HAS_N0_RESIDENT
                        : SLIM_CONFIG.HOUSE_HAS_RESIDENT;
                    houses.Add(house);
                    house.DT_RowId = new StringBuilder("house_").Append(house.Id).ToString();
                }
            }
            return Json(houses, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/Config/UtilityService/GetHouseInfoDetail")]
        public ActionResult GetHouseInfoDetail(int houseId)
        {
            MessageViewModels response = new MessageViewModels();
            House house = _houseServices.FindById(houseId);
            if (null != house && house.Status != SLIM_CONFIG.HOUSE_STATUS_DISABLE)
            {
                HouseViewModel houseModel = new HouseViewModel();
                houseModel.Id = house.Id;
                houseModel.Name = house.HouseName.Split('-')[2];
                houseModel.FloorName = house.Floor;
                houseModel.BlockName = house.Block.BlockName;
                houseModel.Area = house.Area.Value;
                houseModel.TypeName = house.HouseCategory == null ? "Chưa thiết lập" : house.HouseCategory.Name;
                houseModel.Type = house.TypeId == null ? -1 : house.TypeId.Value;
                if (house.Status.Value == SLIM_CONFIG.HOUSE_STATUS_ENABLE && house.OwnerID != null)
                {
                    houseModel.HouseOwner =
                        house.Users.Where(u => u.RoleId == SLIM_CONFIG.USER_ROLE_HOUSEHOLDER).ToList().First().Fullname;
                    houseModel.Status = SLIM_CONFIG.HOUSE_HAS_RESIDENT;
                }
                else if (house.Status.Value == SLIM_CONFIG.HOUSE_STATUS_ENABLE && house.OwnerID == null)
                {
                    houseModel.Status = SLIM_CONFIG.HOUSE_HAS_N0_RESIDENT;
                }

                List<HouseCategoryModel> houseTypeList = new List<HouseCategoryModel>();
                HouseCategoryModel houseTypeModel = null;
                List<HouseCategory> listHouseCategory = _houseCategoryServices.GetAll();
                foreach (var houseCat in listHouseCategory)
                {
                    houseTypeModel = new HouseCategoryModel();
                    houseTypeModel.Id = houseCat.Id;
                    houseTypeModel.Name = houseCat.Name;
                    houseTypeList.Add(houseTypeModel);
                }
                object obj = new
                {
                    houseInfo = houseModel,
                    houseTypeList = houseTypeList
                };
                response.Data = obj;
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Management/Config/UtilityService/AddNewHouse")]
        public ActionResult AddNewHouse(HouseViewModel houseInfo)
        {
            MessageViewModels response = new MessageViewModels();
            Block block = _blockServices.FindById(houseInfo.BlockId);
            if (null != block)
            {
                string floorName = "";
                string houseNameModel = houseInfo.Name.Trim();
                if (houseInfo.AddFloor == SLIM_CONFIG.HOUSE_ADD_NEW_FLOOR)
                {
                    floorName = houseInfo.FloorNameNew.Trim();
                }
                else
                {
                    floorName = houseInfo.FloorName.Trim();
                }
                string houseName = new StringBuilder(block.BlockName).Append("-").Append(floorName).Append("-").Append(houseNameModel).ToString();
                if (_houseServices.CheckFloorIsExist(block.Id, floorName))
                {
                    if (_houseServices.CheckHouseNameIsExistInFloor(block.Id, floorName, houseName))
                    {
                        response.StatusCode = 2;
                        return Json(response);
                    }
                }
                House house = new House();
                house.Floor = floorName;
                house.BlockId = block.Id;
                if (houseInfo.Status == 0)
                {
                    houseInfo.Status = SLIM_CONFIG.HOUSE_STATUS_DISABLE;
                }
                house.Status = houseInfo.Status;
                house.Area = houseInfo.Area;
                HouseCategory houseCat = _houseCategoryServices.FindById(houseInfo.Type);
                if (null != houseCat)
                {
                    house.TypeId = houseInfo.Type;
                }
                house.HouseName = houseName;
                _houseServices.Add(house);
            }
            else
            {
                response.StatusCode = -1;
            }

            return Json(response);
        }

        [HttpPost]
        [Route("Management/Config/UtilityService/UpdateHouseInfo")]
        public ActionResult UpdateHouseInfo(HouseViewModel houseInfo)
        {
            MessageViewModels response = new MessageViewModels();
            House house = _houseServices.FindById(houseInfo.Id);
            if (null != house)
            {
                if (houseInfo.Status != 0)
                {
                    house.Status = houseInfo.Status;
                    if (houseInfo.Status == SLIM_CONFIG.HOUSE_STATUS_DISABLE)
                    {
                        house.OwnerID = null;
                        if (house.Users.Count != 0)
                        {
                            response.StatusCode = 5;
                        }

                        //                        foreach (var usrInHouse in house.Users)
                        //                        {
                        //                            User u = _userServices.FindById(usrInHouse.Id);
                        //                            u.HouseId = null;
                        //                            u.Status = SLIM_CONFIG.USER_STATUS_DISABLE;
                        //                            u.LastModified = DateTime.Now;
                        //                            u.RoleId = SLIM_CONFIG.USER_ROLE_RESIDENT;
                        //                            _userServices.Update(u);
                        //                        }
                    }
                }
                house.Area = houseInfo.Area;
                HouseCategory houseCat = _houseCategoryServices.FindById(houseInfo.Type);
                if (null != houseCat)
                {
                    house.TypeId = houseInfo.Type;
                }

                string houseName = new StringBuilder(house.Block.BlockName).Append("-").Append(house.Floor.Trim()).Append("-").Append(houseInfo.Name.Trim()).ToString();
                if (_houseServices.CheckHouseNameIsExist(house.Id, houseName.Trim()))
                {
                    response.StatusCode = 4;
                    return Json(response);
                }
                house.HouseName = houseName;
                _houseServices.Update(house);
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response);
        }

        [HttpGet]
        [Route("Management/Config/UtilityService/CheckRoomName")]
        public ActionResult CheckRoomName(int houseId, string roomName)
        {
            MessageViewModels response = new MessageViewModels();
            House house = _houseServices.FindById(houseId);
            if (null != house)
            {
                string houseName = new StringBuilder(house.Block.BlockName).Append("-").Append(house.Floor).Append("-").Append(roomName).ToString();
                if (_houseServices.CheckHouseNameIsExist(house.Id, houseName))
                {
                    response.StatusCode = 2;
                }
                else
                {
                    response.StatusCode = 1;
                }
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/Config/UtilityService/CheckFloorOrRoom")]
        public ActionResult CheckRoomName(int mode, int blockId, string floorName, string roomName)
        {
            MessageViewModels response = new MessageViewModels();
            // 1 is check floor    
            if (mode == 1)
            {
                if (_houseServices.CheckFloorIsExist(blockId, floorName))
                {
                    response.StatusCode = 2;
                }
                else
                {
                    response.StatusCode = 1;
                }
            }
            else if (mode == 2)
            {
                Block block = _blockServices.FindById(blockId);
                if (block != null)
                {
                    string houseName =
                        new StringBuilder(block.BlockName).Append("-").Append(floorName).Append("-").Append(roomName).ToString();
                    if (_houseServices.CheckHouseNameIsExistInFloor(blockId, floorName, houseName))
                    {
                        response.StatusCode = 2;
                    }
                    else
                    {
                        response.StatusCode = 1;
                    }
                }
                else
                {
                    response.StatusCode = -1;
                }

            }// mode 2 check room is exist in floor of the block
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Management/Config/UtilityService/DeleteHouse")]
        public ActionResult DeleteRoom(List<int> houseIdList)
        {
            MessageViewModels response = new MessageViewModels();
            if (null != houseIdList)
            {
                foreach (var houseId in houseIdList)
                {
                    House house = _houseServices.FindById(houseId);
                    if (house != null)
                    {
                        if (house.OwnerID != null || house.Users.Count != 0)
                        {
                            response.StatusCode = 2;
                            response.Data = house.HouseName;
                            return Json(response);
                        }
                    }
                }
                foreach (var houseId in houseIdList)
                {
                    House house = _houseServices.FindById(houseId);
                    if (null != house)
                    {
                        if (house.OwnerID == null || house.Users.Count == 0)
                        {
                            house.OwnerID = null;
                            house.Status = SLIM_CONFIG.HOUSE_STATUS_DISABLE;
                            _houseServices.Update(house);
                        }
                        else
                        {
                            response.StatusCode = 2;
                            response.Data = house.HouseName;
                            return Json(response);
                        }
                    }
                }
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/Config/UtilityService/DeleteBlock")]
        public ActionResult DeleteBlock(int blockId)
        {
            MessageViewModels response = new MessageViewModels();
            Block block = _blockServices.FindById(blockId);
            if (null != block)
            {
                if (block.Houses.Any(house => house.Status == SLIM_CONFIG.HOUSE_STATUS_ENABLE && house.OwnerID != null))
                {
                    response.StatusCode = 2;// Tòa nhà vẫn còn tồn tại cư dân dang ở
                    return Json(response);
                }
                List<House> houses = block.Houses.ToList();
                block.Houses = null;
                _blockServices.Delete(block);
                foreach (var house in houses)
                {
                    _houseServices.DeleteById(house.Id);
                }
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response);
        }

        [HttpGet]
        [Route("Management/Config/UtilityService/GetBlockInfo")]
        public ActionResult GetBlockInfo(int blockId)
        {
            MessageViewModels response = new MessageViewModels();
            Block block = _blockServices.FindById(blockId);
            List<FloorModel> listFloorModel = new List<FloorModel>();
            FloorModel floorModel = null;
            if (null != block)
            {
                List<House> listFloor = _houseServices.GetAllFloorInBlock(block.Id);
                foreach (var floor in listFloor)
                {
                    floorModel = new FloorModel();
                    floorModel.Name = floor.Floor;
                    floorModel.Id = floor.Floor;
                    listFloorModel.Add(floorModel);
                }
                object obj = new
                {
                    blockName = block.BlockName,
                    floor = listFloorModel
                };
                response.Data = obj;
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/Config/UtilityService/GetFloorInBlock")]
        public ActionResult GetFloorInBlock(int blockId)
        {
            MessageViewModels response = new MessageViewModels();
            List<FloorModel> floorList = new List<FloorModel>();
            FloorModel floorModel = null;
            Block block = _blockServices.FindById(blockId);
            if (null != block)
            {
                List<House> listFloor = _houseServices.GetAllFloorInBlock(blockId);
                foreach (var floor in listFloor)
                {
                    floorModel = new FloorModel();
                    floorModel.Id = floor.Floor;
                    floorModel.Name = floor.Floor;
                    floorList.Add(floorModel);
                }
                response.Data = floorList;
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/Config/UtilityService/GetBlockList")]
        public ActionResult GetBlockList()
        {
            List<BlockViewModel> blockList = new List<BlockViewModel>();
            BlockViewModel block = null;
            List<Block> blocks = _blockServices.GetAllBlocks();
            foreach (var b in blocks)
            {
                block = new BlockViewModel();
                block.Name = b.BlockName;
                block.Id = b.Id;
                block.NoOfFloor = b.Houses.GroupBy(h => h.Floor).ToList().Count();
                block.TotalRoom = b.Houses.ToList().Count;
                block.TotalActiveRoom = b.Houses.Where(h => h.Status != null && h.Status == SLIM_CONFIG.HOUSE_STATUS_ENABLE).ToList().Count();
                blockList.Add(block);
            }
            return Json(blockList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/Config/UtilityService/GetBlockname")]
        public ActionResult GetBlockname(int blockId)
        {
            MessageViewModels response = new MessageViewModels();
            Block block = _blockServices.FindById(blockId);
            if (null != block)
            {
                response.Data = block.BlockName;
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Management/Config/UtilityService/ChangeFloorName")]
        public ActionResult ChangeFloorName(int blockId, string oldFloorName, string newFloorName)
        {
            MessageViewModels response = new MessageViewModels();
            Block block = _blockServices.FindById(blockId);
            if (null != block && oldFloorName != null && newFloorName != null)
            {
                oldFloorName = oldFloorName.Trim();
                newFloorName = newFloorName.Trim();
                if (_houseServices.CheckFloorIsExist(block.Id, newFloorName))
                {
                    response.StatusCode = 2;
                }
                else
                {
                    List<House> getHouseInFloor = _houseServices.GetActiveRoomsInFloor(block.Id, oldFloorName);
                    string name = "";
                    string houseName = "";
                    foreach (var h in getHouseInFloor)
                    {
                        name = h.HouseName.Split('-')[2];
                        houseName = new StringBuilder(block.BlockName).Append("-").Append(newFloorName).Append("-").Append(name).ToString();
                        h.Floor = newFloorName;
                        h.HouseName = houseName;
                        _houseServices.Update(h);
                    }
                }
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/Config/UtilityService/UpdateBlockName")]
        public ActionResult UpdateBlockName(int blockId, string blockName)
        {
            MessageViewModels response = new MessageViewModels();
            Block block = _blockServices.FindById(blockId);
            if (null != block && blockName != null)
            {
                blockName = blockName.Trim();
                if (_blockServices.CheckBlockNameIsExisted(block.Id, blockName))
                {
                    response.StatusCode = 2;
                }
                else
                {
                    string name = "";
                    string houseName = "";
                    block.BlockName = blockName;
                    _blockServices.Update(block);
                    foreach (var h in block.Houses)
                    {
                        House eHouse = _houseServices.FindById(h.Id);
                        name = h.HouseName.Split('-')[2];
                        houseName = new StringBuilder(blockName).Append("-").Append(h.Floor).Append("-").Append(name).ToString();
                        eHouse.HouseName = houseName;
                        _houseServices.Update(eHouse);
                    }
                }
            }
            else
            {
                response.StatusCode = -1;
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

        public void UpdateTotalAmountInTransaction(List<Transaction> listTrans, double totalAmount)
        {
            //Update total amount of transaction when user receipt is unpublish
            if (listTrans.ToList().Count != 0)
            {
                var eTrans = _transactionService.FindById(listTrans.First().Id);
                eTrans.TotalAmount = totalAmount;
                eTrans.PaidAmount = 0;
                eTrans.LastModified = DateTime.Now;
                _transactionService.Update(eTrans);
            }
        }
    }

}