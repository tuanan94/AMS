using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    public class ImageController : Controller
    {
        [HttpPost]
        public String upload(String dir)
        {
            String folderDir = "ProfileImages";
            
            if (SLIM_CONFIG.dirPostImage.Equals(dir))
            {
                folderDir = "PostImages";
            }//AnTT

            if (SLIM_CONFIG.dirHouseProfileImage.Equals(dir))
            {
                folderDir = "HouseProfileImages";
            }
            String filePath = "";
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["HelpSectionImages"];
                if (pic != null && pic.FileName != null)
                {
                    string newPath = Server.MapPath(SLIM_CONFIG.imagePath + folderDir);
                    if (!Directory.Exists(newPath))
                    {
                        System.IO.Directory.CreateDirectory(newPath);
                    }
                    pic.SaveAs(newPath + "/" + pic.FileName);
                    filePath = "/Images/" + folderDir +"/" + pic.FileName;
                }
            }
            return filePath;
        }
    }
}