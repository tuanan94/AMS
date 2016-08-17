using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Twilio;
using Encoder = System.Text.Encoder;

namespace AMS.Constant
{
    public class CommonUtil
    {
        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public static void SentSms(string toNumber, string messContent)
        {
            var accountSid = "AC10ae7ed64035004a9f1ed772747b94dc"; // Your Account SID from www.twilio.com/console
            var authToken = "c867c6dadb271752b1fa0bb988f1c284";  // Your Auth Token from www.twilio.com/console
            toNumber = new StringBuilder("+84").Append(toNumber.Substring(1, toNumber.Length - 1)).ToString();
            var twilio = new TwilioRestClient(accountSid, authToken);
            var message = twilio.SendMessage(
                "+12057198424", // From (Replace with your Twilio number)
                toNumber, // To (Replace with your phone number)
                messContent
                );
            
//            Console.WriteLine(message.Status);
//            Console.WriteLine(message.Sid);
//            Console.Write("Press any key to continue.");
//            Console.ReadKey();
        }

        /*http://stackoverflow.com/questions/6501797/resize-image-proportionally-with-maxheight-and-maxwidth-constraints*/
        public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);
            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);
            var newImage = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        /*http://stackoverflow.com/questions/6501797/resize-image-proportionally-with-maxheight-and-maxwidth-constraints*/
        public static System.Drawing.Image ResizeImageImage(System.Drawing.Image image, int width, int height, int x, int y)
        {
            Rectangle crop = new Rectangle(x, y, width, height);
            var newImage = new Bitmap(crop.Width, crop.Height);
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.Clear(Color.White);
                graphics.DrawImage(image, new Rectangle(0, 0, crop.Width, crop.Height), crop, GraphicsUnit.Pixel);
            }

            return newImage;
        }


        /*http://stackoverflow.com/questions/10323633/resize-image-in-c-sharp-with-aspect-ratio-and-crop-central-image-so-there-are-no*/
        public static System.Drawing.Image FixedSize(System.Drawing.Image image, int Width, int Height, bool needToFill)
        {
            #region много арифметики
            int sourceWidth = image.Width;
            int sourceHeight = image.Height;
            int sourceX = 0;
            int sourceY = 0;
            double destX = 0;
            double destY = 0;

            double nScale = 0;
            double nScaleW = 0;
            double nScaleH = 0;

            nScaleW = ((double)Width / (double)sourceWidth);
            nScaleH = ((double)Height / (double)sourceHeight);
            if (!needToFill)
            {
                nScale = Math.Min(nScaleH, nScaleW);
            }
            else
            {
                nScale = Math.Max(nScaleH, nScaleW);
                destY = (Height - sourceHeight * nScale) / 2;
                destX = (Width - sourceWidth * nScale) / 2;
            }

            if (nScale > 1)
                nScale = 1;

            int destWidth = (int)Math.Round(sourceWidth * nScale);
            int destHeight = (int)Math.Round(sourceHeight * nScale);
            #endregion

            System.Drawing.Bitmap bmPhoto = null;
            try
            {
                bmPhoto = new System.Drawing.Bitmap(destWidth + (int)Math.Round(2 * destX), destHeight + (int)Math.Round(2 * destY));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("destWidth:{0}, destX:{1}, destHeight:{2}, desxtY:{3}, Width:{4}, Height:{5}",
                    destWidth, destX, destHeight, destY, Width, Height), ex);
            }
            using (System.Drawing.Graphics grPhoto = System.Drawing.Graphics.FromImage(bmPhoto))
            {
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                grPhoto.CompositingQuality = CompositingQuality.HighQuality;
                grPhoto.SmoothingMode = SmoothingMode.HighQuality;

                Rectangle to = new System.Drawing.Rectangle((int)Math.Round(destX), (int)Math.Round(destY), destWidth, destHeight);
                Rectangle from = new System.Drawing.Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);
                //Console.WriteLine("From: " + from.ToString());
                //Console.WriteLine("To: " + to.ToString());
                grPhoto.DrawImage(image, to, from, System.Drawing.GraphicsUnit.Pixel);

                return bmPhoto;
            }
        }

        public static System.Drawing.Image ResizeImageNewForThumbnail(System.Drawing.Image image,
            /* note changed names */
                     int canvasWidth, int canvasHeight
            /* new */
                     )
        {


            int originalWidth = image.Width;
            int originalHeight = image.Height;

            System.Drawing.Image thumbnail =
                new Bitmap(canvasWidth, canvasHeight); // changed parm names
            System.Drawing.Graphics graphic =
                         System.Drawing.Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            /* ------------------ new code --------------- */

            // Figure out the ratio
            double ratioX = (double)canvasWidth / (double)originalWidth;
            double ratioY = (double)canvasHeight / (double)originalHeight;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
            int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

            //            graphic.Clear(Color.White); // white padding
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);

            return thumbnail;
            /* ------------- end new code ---------------- */

            //            System.Drawing.Imaging.ImageCodecInfo[] info =
            //                             ImageCodecInfo.GetImageEncoders();
            //            EncoderParameters encoderParameters;
            //            encoderParameters = new EncoderParameters(1);
            //            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality,
            //                             100L);
            //            thumbnail.Save(path + newWidth + "." + originalFilename, info[1],
            //                             encoderParameters);
        }

        /*http://stackoverflow.com/questions/991587/how-can-i-crop-scale-user-images-so-i-can-display-fixed-sized-thumbnails-without*/
        public static System.Drawing.Image ScaleImageFixHeight(System.Drawing.Image oldImage, int canvasWidth, int canvasHeight)
        {
            double resizeFactor = 1;

            if (oldImage.Width > canvasWidth || oldImage.Height > canvasHeight)
            {
                double widthFactor = Convert.ToDouble(oldImage.Width) / canvasWidth;
                double heightFactor = Convert.ToDouble(oldImage.Height) / canvasWidth;
                resizeFactor = Math.Max(widthFactor, heightFactor);
            }
            int width = Convert.ToInt32(oldImage.Width / resizeFactor);
            int height = Convert.ToInt32(oldImage.Height / resizeFactor);
            Bitmap newImage = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(newImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(oldImage, 0, 0, newImage.Width, newImage.Height);
            return newImage;
        }



        /*http://stackoverflow.com/questions/3566830/what-method-in-the-string-class-returns-only-the-first-n-characters*/
        public static string TruncateLongString(string str, int maxLength)
        {
            if (str != null)
            {
                return str.Substring(0, Math.Min(str.Length, maxLength));
            } return "";
        }
        /*http://stackoverflow.com/questions/3566830/what-method-in-the-string-class-returns-only-the-first-n-characters*/
        public static int CalculateAge(DateTime dob)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - dob.Year;

            if (dob > today.AddYears(-age))
                age--;
            return age;
        }

        public static string GetImageExt(System.Drawing.Image img)
        {
            string ext = "";
            if (ImageFormat.Jpeg.Equals(img.RawFormat))
            {
                ext = "jpg";
            }
            else if (ImageFormat.Png.Equals(img.RawFormat))
            {
                // PNG
                ext = "png";
            }
            else if (ImageFormat.Gif.Equals(img.RawFormat))
            {
                // GIF
                ext = "gif";
            }
            return ext;
        }
    }
}