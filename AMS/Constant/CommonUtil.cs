using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Twilio;

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
            toNumber = new StringBuilder("+84").Append(toNumber.Substring(1, toNumber.Length-1)).ToString();
            var twilio = new TwilioRestClient(accountSid, authToken);
            var message = twilio.SendMessage(
                "+12057198424", // From (Replace with your Twilio number)
                toNumber, // To (Replace with your phone number)
                messContent
                );
//            Console.WriteLine(message.Sid);
//            Console.Write("Press any key to continue.");
//            Console.ReadKey();
        }
    }
}