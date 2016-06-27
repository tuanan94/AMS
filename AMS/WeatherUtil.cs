using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using static AMS.SLIM_CONFIG;

namespace AMS
{
    public class WeatherUtil
    {
        public static WeatherResult getWeather()
        {


            return WeatherResult.NoneDefined;
        }
        public static String getJsonResult()
        {
            String requestContent = "";
            requestContent += "https://api.worldweatheronline.com/premium/v1/weather.ashx?num_of_days=3&q=";
            requestContent += "Ho+Chi+Minh+City"; //location
            requestContent += "&key=";
            requestContent += SLIM_CONFIG.WeatherAPI_KEY;
            requestContent += "&format=json";
            var request = (HttpWebRequest)WebRequest.Create(requestContent);

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var jsonobject = jss.Deserialize<dynamic>(responseString);
            var data = jsonobject["data"];
            var current_condition = data["current_condition"];
            return responseString;
        }
    }

    public class weatherObject
    {
        public String data { get; set; }
    }
    public class data
    {
        current_condition c;
    }
    public class current_condition
    {
        String weatherCode;
        String temp_C;

    }
}