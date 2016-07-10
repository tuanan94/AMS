using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using AMS.Constant;
using AMS.Models;
using AMS.Service;
using Microsoft.AspNet.Identity;
using Microsoft.SqlServer.Server;

namespace AMS.Controllers
{
    public class AroundServiceController : Controller
    {
        AroundProviderService _aroundProviderService = new AroundProviderService();
        AroundProviderProductService _aroundProviderProductService = new AroundProviderProductService();
        AroundProviderCategoryService _aroundProviderCategoryService = new AroundProviderCategoryService();

        //        // GET: ServiceAround
        //        public ActionResult Index()
        //        {
        //            return View();
        //        }



        [Route("Home/AroundService/ListView")]
        [AutoRedirect.MandatorySurveyRedirect]
        public ActionResult ViewAroundProvider(String cat)
        {
            ViewBag.AllProviders = _aroundProviderService.GetTopTenproviderOrderByView();
            return View();
        }
        [AutoRedirect.MandatorySurveyRedirect]
        [Route("Home/AroundService/All")]
        public ActionResult ViewGetAllAroundProvider(String cat)
        {
            ViewBag.AllProviders = _aroundProviderService.GetAllProviderWithCat(cat);
            List<AroundProviderCategory> allCat = _aroundProviderCategoryService.GetAllOrderByProviderClickCount();
            ViewBag.AllCategorys = allCat;
            if (allCat.Count != 0)
            {
                ViewBag.highestProCat = allCat.First();
            }
            ViewBag.activeCat = cat;
            return View("ViewAroundProviderDetail");
        }
        [AutoRedirect.MandatorySurveyRedirect]
        [Route("Home/AroundService/SingleProviderDetail")]
        public ActionResult SingleProviderDetail(int id)
        {
            List<AroundProviderProduct> products = _aroundProviderProductService.GetAroundProviderProduct(id);

            AroundProvider curProvider = _aroundProviderService.GetProvider(id);
            if (curProvider != null)
            {
                curProvider.ClickCount++;
                curProvider.LastModified = DateTime.Now;
                _aroundProviderService.Update(curProvider);
            }

            ViewBag.Products = products;
            ViewBag.CurProvider = curProvider;

            return View();
        }

        [HttpGet]
        [Authorize]
        [Route("Management/AroundService/Manage")]
        public ActionResult ViewListAroundService()
        {
            return View("ManageAroundService");
        }

        [HttpGet]
        [Authorize]
        [Route("Management/AroundService/Update")]
        public ActionResult UpdateAroundServiceProvider(int providerId)
        {
            AroundProvider provider = _aroundProviderService.FindById(providerId);
            ViewBag.provider = provider;

            //            string address = provider.Address;
            //            var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false",
            //                                                Uri.EscapeDataString(address));
            //            var request = WebRequest.Create(requestUri);
            //            var response = request.GetResponse();
            //            var xdoc = XDocument.Load(response.GetResponseStream());
            //
            //            var result = xdoc.Element("GeocodeResponse").Element("result");
            //            var locationElement = result.Element("geometry").Element("location");
            //            var lat = locationElement.Element("lat");
            //            var lng = locationElement.Element("lng");
            //            //Double.Parse(lng.Value);
            //            provider.Longitude = Double.Parse(lng.Value).ToString();
            //            provider.Latitude = Double.Parse(lat.Value).ToString();

            return View("UpdateAroundServiceProvider");
        }

        [HttpGet]
        [Route("Management/AroundService/GetAllProviderList")]
        public ActionResult GetAllProviderList()
        {
            List<AroundProvider> aroundProviders = _aroundProviderService.GetAllProviderWithCat(null);
            AroundServiceModel aroundService = null;
            List<AroundServiceModel> aroundServices = new List<AroundServiceModel>();
            foreach (var provider in aroundProviders)
            {
                aroundService = new AroundServiceModel();
                aroundService.SrvProvId = provider.Id;
                aroundService.SrvProvName = provider.Name;
                aroundService.SrvProvCatName = provider.AroundProviderCategory.Name;
                aroundService.SrvProvCreateDate = provider.CreateDate.Value.ToString(AmsConstants.DateFormat);
                aroundService.SrvProvCreateDateLong = provider.CreateDate.Value.Ticks;
                aroundService.SrvProvView = provider.ClickCount.Value;
                aroundService.DT_RowId = new StringBuilder("provider_").Append(provider.Id).ToString();
                aroundServices.Add(aroundService);
            }
            return Json(aroundServices, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/AroundService/GetAllProductOfProvider")]
        public ActionResult GetProductByProviderId(int providerId)
        {
            AroundProvider provider = _aroundProviderService.FindById(providerId);
            List<AroundProviderProduct> products = provider.AroundProviderProducts.ToList();
            List<ProductModel> productModels = new List<ProductModel>();
            if (null != provider)
            {
                ProductModel proModel = null;
                foreach (var pro in products)
                {
                    proModel = new ProductModel();
                    proModel.Id = pro.Id;
                    proModel.Name = pro.Name;
                    proModel.Price = pro.Price.Value;
                    proModel.ImageUrl = pro.ImgUrl;
                    proModel.CreateDate = pro.CreateDate.Value.ToString(AmsConstants.DateFormat);
                    proModel.CreateDateLong = pro.CreateDate.Value.Ticks;
                    proModel.DT_RowId = new StringBuilder("pro_").Append(pro.Id).ToString();
                    productModels.Add(proModel);
                }
            }
            return Json(productModels, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Management/AroundService/AddNewProduct")]
        public ActionResult AddNewProduct(ProductModel productModel)
        {
            MessageViewModels response = new MessageViewModels();
            AroundProvider provider = _aroundProviderService.FindById(productModel.SrvProvId);
            if (null != provider)
            {
                AroundProviderProduct product = new AroundProviderProduct();
                product.Name = productModel.Name;
                product.ImgUrl = productModel.ImageUrl;
                product.Price = productModel.Price;
                product.AroundProviderId = provider.Id;
                product.CreateDate = DateTime.Now;
                product.LastModified = DateTime.Now;
                _aroundProviderProductService.Add(product);
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/AroundService/UpdateProduct")]
        public ActionResult UpdateProduct(ProductModel productModel)
        {
            MessageViewModels response = new MessageViewModels();
            AroundProvider provider = _aroundProviderService.FindById(productModel.SrvProvId);
            if (null != provider)
            {
                AroundProviderProduct product = _aroundProviderProductService.FindById(productModel.Id);
                if (null != product)
                {
                    product.Name = productModel.Name;
                    product.ImgUrl = productModel.ImageUrl;
                    product.Price = productModel.Price;
                    product.LastModified = DateTime.Now;
                    _aroundProviderProductService.Update(product);
                }
                else
                {
                    response.StatusCode = -1;
                }
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/AroundService/DeleteProducts")]
        public ActionResult DeleteProducts(List<int> deleteItem)
        {
            MessageViewModels response = new MessageViewModels();
            try
            {
                foreach (var id in deleteItem)
                {
                    AroundProviderProduct product = _aroundProviderProductService.FindById(id);
                    if (null != product)
                    {
                        _aroundProviderProductService.Delete(product);
                    }
                }
            }
            catch (Exception)
            {
                response.StatusCode = -1;
                return Json(response);
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/AroundService/DeleteProvider")]
        public ActionResult DeleteProvider(List<int> deleteItem)
        {
            MessageViewModels response = new MessageViewModels();
            try
            {
                foreach (var id in deleteItem)
                {
                    AroundProvider provider = _aroundProviderService.FindById(id);
                    if (null != provider)
                    {
                        List<AroundProviderProduct> products = provider.AroundProviderProducts.ToList();
                        provider.AroundProviderProducts = null;
                        _aroundProviderService.Delete(provider);
                        foreach (var p in products)
                        {
                            AroundProviderProduct product = _aroundProviderProductService.FindById(p.Id);
                            if (null != product)
                            {
                                _aroundProviderProductService.Delete(product);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                response.StatusCode = -1;
                return Json(response);
            }
            return Json(response);
        }

        [HttpGet]
        [Route("Management/AroundService/GetProductDetail")]
        public ActionResult GetProductDetail(int productId)
        {
            MessageViewModels response = new MessageViewModels();
            AroundProviderProduct product = _aroundProviderProductService.FindById(productId);
            if (null != product)
            {
                ProductModel proModel = null;
                proModel = new ProductModel();
                proModel.Id = product.Id;
                proModel.Name = product.Name;
                proModel.Price = product.Price.Value;
                proModel.ImageUrl = product.ImgUrl;
                proModel.CreateDate = product.CreateDate.Value.ToString(AmsConstants.DateFormat);
                proModel.CreateDateLong = product.CreateDate.Value.Ticks;
                proModel.SrvProvId = product.AroundProviderId.Value;
                response.Data = proModel;
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/AroundService/GetDetailServiceProvider")]
        public ActionResult GetDetailServiceProvider(int providerId)
        {
            MessageViewModels response = new MessageViewModels();
            AroundProvider provider = _aroundProviderService.FindById(providerId);
            if (null != provider)
            {
                AroundServiceModel aroundService = new AroundServiceModel();
                aroundService.SrvProvCatId = provider.AroundProviderCategoryId.Value;
                aroundService.SrvProvCatName = provider.AroundProviderCategory.Name;
                aroundService.SrvProvName = provider.Name;
                aroundService.SrvProvAddress = provider.Address;
                aroundService.SrvProvTel = provider.Tel;
                aroundService.SrvProvDesc = provider.Description;

                List<AroundProviderCategory> providerCats = _aroundProviderCategoryService.GetAll();
                AroundServiceCatModel proCatModel = null;
                List<AroundServiceCatModel> proCatModelList = new List<AroundServiceCatModel>();
                foreach (var proCat in providerCats)
                {
                    proCatModel = new AroundServiceCatModel();
                    proCatModel.Id = proCat.Id;
                    proCatModel.Name = proCat.Name;
                    proCatModelList.Add(proCatModel);
                }
                object obj = new
                {
                    provider = aroundService,
                    providerCat = proCatModelList
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
        [Route("Management/AroundService/AddServiceProvider")]
        public ActionResult AddServiceProvider(AroundServiceModel serviceProvider)
        {
            MessageViewModels msgResponse = new MessageViewModels();
            try
            {
                AroundProvider provider = new AroundProvider();
                provider.Name = serviceProvider.SrvProvName;
                provider.AroundProviderCategoryId = serviceProvider.SrvProvCatId;
                provider.Tel = serviceProvider.SrvProvTel;
                provider.Description = serviceProvider.SrvProvDesc;
                provider.ImageUrl = AmsConstants.DefaultStoreImg;
                provider.ClickCount = 0;
                try
                {
                    var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false",
                                              Uri.EscapeDataString(serviceProvider.SrvProvAddress.Trim()));
                    var request = WebRequest.Create(requestUri);
                    var response = request.GetResponse();
                    var xdoc = XDocument.Load(response.GetResponseStream());

                    var result = xdoc.Element("GeocodeResponse").Element("result");
                    var locationElement = result.Element("geometry").Element("location");
                    var lat = locationElement.Element("lat");
                    var lng = locationElement.Element("lng");

                    //Double.Parse(lng.Value);
                    provider.Address = serviceProvider.SrvProvAddress;
                    provider.Latitude = Double.Parse(lat.Value).ToString();
                    provider.Longitude = Double.Parse(lng.Value).ToString();  
                }
                catch (Exception)
                {
                    msgResponse.StatusCode = 2;
                    return Json(msgResponse);
                    throw;
                }
                provider.LastModified = DateTime.Now;
                provider.CreateDate = DateTime.Now;
                _aroundProviderService.Add(provider);
            }
            catch (Exception)
            {
                msgResponse.StatusCode = -1;
                return Json(msgResponse);
            }
            return Json(msgResponse);
        }

        [HttpPost]
        [Route("Management/AroundService/UpdateServiceProviderInfo")]
        public ActionResult UpdateServiceProviderInfo(AroundServiceModel serviceProvider)
        {
            MessageViewModels msgResponse = new MessageViewModels();
            AroundProvider provider = _aroundProviderService.FindById(serviceProvider.SrvProvId);
            if (null != provider)
            {
                provider.Name = serviceProvider.SrvProvName;
                provider.AroundProviderCategoryId = serviceProvider.SrvProvCatId;
                provider.Tel = serviceProvider.SrvProvTel;
                provider.Description = serviceProvider.SrvProvDesc;


                if (!serviceProvider.SrvProvAddress.Trim().Equals(provider.Address))
                {
                    try
                    {
                        var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false",
                                                Uri.EscapeDataString(serviceProvider.SrvProvAddress.Trim()));
                        var request = WebRequest.Create(requestUri);
                        var response = request.GetResponse();
                        var xdoc = XDocument.Load(response.GetResponseStream());

                        var result = xdoc.Element("GeocodeResponse").Element("result");
                        var locationElement = result.Element("geometry").Element("location");
                        var lat = locationElement.Element("lat");
                        var lng = locationElement.Element("lng");

                        //Double.Parse(lng.Value);
                        provider.Address = serviceProvider.SrvProvAddress;
                        provider.Latitude = Double.Parse(lat.Value).ToString();
                        provider.Longitude = Double.Parse(lng.Value).ToString();
                    }
                    catch (Exception)
                    {
                        msgResponse.StatusCode = 2;
                        return Json(msgResponse);
                    }
                }
                provider.LastModified = DateTime.Now;
                _aroundProviderService.Update(provider);
            }
            else
            {
                msgResponse.StatusCode = -1;
            }
            return Json(msgResponse);
        }

        [HttpGet]
        [Route("Management/AroundService/GetServiceProviderCat")]
        public ActionResult GetServiceProviderCat()
        {
            List<AroundServiceCatModel> listCatModel = new List<AroundServiceCatModel>();
            AroundServiceCatModel catModel = null;
            List<AroundProviderCategory> listCatE = _aroundProviderCategoryService.GetAll();
            foreach (var eCat in listCatE)
            {
                catModel = new AroundServiceCatModel();
                catModel.Id = eCat.Id;
                catModel.Name = eCat.Name;
                listCatModel.Add(catModel);
            }
            return Json(listCatModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/AroundService/ManageCategory")]
        public ActionResult ManageAroundProviderCategory()
        {
            return View("ManageAroundServiceCategory");
        }

        [HttpGet]
        [Route("Management/AroundService/GetAroundServiceCatList")]
        public ActionResult GetAroundServiceCatList()
        {
            List<AroundProviderCategory> providerCat = _aroundProviderCategoryService.GetAll();
            List<AroundServiceCatModel> providerCatModelList = new List<AroundServiceCatModel>();
            AroundServiceCatModel providerCatModel = null;
            foreach (var proCat in providerCat)
            {
                providerCatModel = new AroundServiceCatModel();
                providerCatModel.Id = proCat.Id;
                providerCatModel.DT_RowId = new StringBuilder("pro_cat_").Append(proCat.Id).ToString();
                providerCatModel.Name = proCat.Name;
                providerCatModel.Count = proCat.AroundProviders.Sum(pro => pro.ClickCount).Value;
                providerCatModelList.Add(providerCatModel);
            }
            return Json(providerCatModelList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Management/AroundService/GetAroundServiceCatDetail")]
        public ActionResult GetAroundServiceCatDetail(int catId)
        {
            AroundServiceCatModel providerCatModel = null;

            MessageViewModels response = new MessageViewModels();
            AroundProviderCategory provCat = _aroundProviderCategoryService.FindById(catId);
            if (provCat != null)
            {
                providerCatModel = new AroundServiceCatModel();
                providerCatModel.Name = provCat.Name;
                providerCatModel.Id = provCat.Id;
                response.Data = providerCatModel;
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("Management/AroundService/AddServiceProviderCat")]
        public ActionResult AddServiceProviderCat(string name)
        {
            MessageViewModels repsonse = new MessageViewModels();
            if (name != null)
            {
                if (_aroundProviderCategoryService.CheckNameIsExited(name))
                {
                    repsonse.StatusCode = 2;
                }
                else
                {
                    AroundProviderCategory proCat = new AroundProviderCategory();
                    proCat.Name = name;
                    _aroundProviderCategoryService.Add(proCat);
                }
            }
            else
            {
                repsonse.StatusCode = -1;
            }
            return Json(repsonse);
        }

        [HttpPost]
        [Route("Management/AroundService/UpdateServiceProviderCat")]
        public ActionResult UpdateServiceProviderCat(int proCatId, String name)
        {
            MessageViewModels repsonse = new MessageViewModels();
            AroundProviderCategory proCat = _aroundProviderCategoryService.FindById(proCatId);
            if (proCat != null && name != null)
            {
                if (_aroundProviderCategoryService.CheckNewCatNameIsExited(proCat.Id, name))
                {
                    repsonse.StatusCode = 2;
                }
                else
                {
                    proCat.Name = name;
                    _aroundProviderCategoryService.Update(proCat);
                }
            }
            else
            {
                repsonse.StatusCode = -1;
            }
            return Json(repsonse);
        }

        [HttpPost]
        [Route("Management/AroundService/DeleteServiceProviderCat")]
        public ActionResult DeleteServiceProviderCat(List<int> deleteItem)
        {
            MessageViewModels response = new MessageViewModels();
            try
            {
                AroundProviderCategory proCat = null;
                List<AroundProvider> proServices = null;
                List<AroundProviderProduct> proProduct = null;
                foreach (var id in deleteItem)
                {
                    proCat = _aroundProviderCategoryService.FindById(id);
                    if (null != proCat)
                    {
                        proServices = proCat.AroundProviders.ToList();
                        proCat.AroundProviders = null;
                        _aroundProviderCategoryService.Delete(proCat);
                        foreach (var serProvider in proServices)
                        {
                            var serviceProvider = _aroundProviderService.FindById(serProvider.Id);
                            proProduct = serviceProvider.AroundProviderProducts.ToList();
                            _aroundProviderService.Delete(serviceProvider);
                            foreach (var pro in proProduct)
                            {
                                _aroundProviderProductService.DeleteById(pro.Id);
                            }
                            
                        }
                    }
                }
            }
            catch (Exception en)
            {
                response.StatusCode = -1;
                return Json(response);
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/AroundService/ChangeServiceProviderAvatar")]
        public ActionResult ChangeServiceProviderAvatar(int providerId, string imgUrl)
        {
            MessageViewModels response = new MessageViewModels();
            AroundProvider provider = _aroundProviderService.FindById(providerId);
            if (null != provider)
            {
                provider.ImageUrl = imgUrl;
                provider.LastModified = DateTime.Now;
                _aroundProviderService.Update(provider);
            }
            else
            {
                response.StatusCode = -1;
            }
            return Json(response);
        }

        [HttpPost]
        [Route("Management/Image/Upload")]
        public ActionResult UploadImage()
        {
            MessageViewModels response = new MessageViewModels();
            string fileName = "";
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["image"];
                int width = Int32.Parse(Request.Form["width"]);
                int height = Int32.Parse(Request.Form["height"]);
                if (pic != null)
                {
                    string newPath = Server.MapPath(AmsConstants.ImageFilePath);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }

                    Rectangle cropRect = new Rectangle();
                    cropRect.Width = width;
                    cropRect.Height = height;

                    System.Drawing.Image target = CommonUtil.FixedSize(System.Drawing.Image.FromStream(pic.InputStream), cropRect.Width, cropRect.Height, true);

                    fileName =
                       new StringBuilder().Append("img_").Append(DateTime.Now.Ticks).Append(".").Append(pic.ContentType.Replace(@"image/", ""))
                           .ToString();
                    target.Save(new StringBuilder(newPath).Append(fileName).ToString());
                    response.Data = new StringBuilder(AmsConstants.ImageFilePathDownload).Append(fileName).ToString();
                }
                else
                {
                    response.StatusCode = -1;
                    response.Msg = "Tải lên tập tin  thất bại";
                }
            }
            else
            {
                response.StatusCode = -1;
                response.Msg = "Tải lên tập tin  thất bại";
            }
            return Json(response);
        }

    }
}