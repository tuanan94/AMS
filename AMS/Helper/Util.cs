using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace AMS.Helper
{
    public class Util
    {
        public static string GetWebRoot()
        {
            var request = HttpContext.Current.Request;
            if (request.ApplicationPath != null)
                return request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";
            // TODO default
            return "http://localhost:49280/";
        }
        public static long GetUnixTime()
        {
            return (long)GetUnixTimeWithMili();
        }
        public static double GetUnixTimeWithMili()
        {
            return (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        public static bool ConvertImageToJpg(string uploadDir, string fileName, long quality, string outputDir, string newFileName)
        {
            //Source file url
            string oldFile = HttpContext.Current.Server.MapPath(Path.Combine(uploadDir, fileName));
            //Destination file url
            string newFile = HttpContext.Current.Server.MapPath(Path.Combine(outputDir, newFileName));
            if (File.Exists(oldFile))
            {
                // Load the image.
                var image = Image.FromFile(oldFile);

                // Get encoder format
                var jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                // Create an Encoder object based on the GUID for the Quality parameter category.
                var myEncoder = Encoder.Quality;

                // Create an EncoderParameters object. An EncoderParameters object has an array of 
                // EncoderParameter objects. In this case, there is only one EncoderParameter object in the array.
                var myEncoderParameters = new EncoderParameters(1);

                // Save the bitmap as a JPG file with a quality level compression.
                var myEncoderParameter = new EncoderParameter(myEncoder, quality);
                myEncoderParameters.Param[0] = myEncoderParameter;
                image.Save(newFile, jpgEncoder, myEncoderParameters);

                //Delete source file
                image.Dispose();
                File.Delete(oldFile);
                return true;
            }
            return false;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        public static bool DeleteFile(string filePath)
        {
            // Delete a file by using File class static method...
            if (System.IO.File.Exists(filePath))
            {
                // Use a try block to catch IOExceptions, to
                // handle the case of the file already being
                // opened by another process.
                try
                {
                    System.IO.File.Delete(filePath);
                    return true;
                }
                catch (System.IO.IOException e)
                {
                    // TODO send it to logger
                    Console.WriteLine(e.Message);
                }
            }
            return false;
        }
    }
}