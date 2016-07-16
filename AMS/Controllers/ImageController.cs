using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMS.Constant;

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

            if (SLIM_CONFIG.dirProfileImage.Equals(dir))
            {
                folderDir = "ProfileImages";
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
                    System.Drawing.Image target = CommonUtil.ScaleImage(System.Drawing.Image.FromStream(pic.InputStream), 480, 480);
                    long currentTime = DateTime.Now.Ticks;
                    target.Save(newPath + "/" + currentTime + "_"  +pic.FileName );
                    filePath = "/Images/" + folderDir + "/" + currentTime + "_" +  pic.FileName ;
                }
            }
            return filePath;
        }
    }
}