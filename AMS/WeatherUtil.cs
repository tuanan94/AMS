using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace AMS
{
    public class WeatherUtil
    {
        
        public static weatherResult getWeatherResult() // return weatherCode
        {
            weatherResult weatherResult = new weatherResult();
            String requestContent = "";
            requestContent += "https://api.worldweatheronline.com/premium/v1/weather.ashx?num_of_days=3&q=";
            requestContent += "Ho+Chi+Minh+City"; //location
            requestContent += "&key=";
            requestContent += SLIM_CONFIG.WeatherAPI_KEY;
            requestContent += "&format=json";
            var request = (HttpWebRequest)WebRequest.Create(requestContent);

            WebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch
            {
                //do nothing
            }
            if (response != null)
            {
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var jsonobject = jss.Deserialize<dynamic>(responseString);
                var data = jsonobject["data"];
                var current_condition = data["current_condition"];
                string weatherCode = current_condition[0]["weatherCode"];
                string weatherDes = current_condition[0]["weatherDesc"][0]["value"];
                string weatherIconUrl = current_condition[0]["weatherIconUrl"][0]["value"];
                string temp_C = current_condition[0]["temp_C"];
                weatherResult.code = weatherCode;
                weatherResult.desc = weatherDes;
                weatherResult.weatherIcon = weatherResult.getWeatherIcon(weatherCode, weatherIconUrl);
                weatherResult.tempC = temp_C;
            }
            else
            {
                weatherResult.code = "116";
                weatherResult.desc = "Partly Cloudy";
                weatherResult.weatherIcon = weatherResult.getWeatherIcon("116", "http://cdn.worldweatheronline.net/images/wsymbols01_png_64/wsymbol_0002_sunny_intervals.png");
                weatherResult.tempC = "30";
            }
         
            return weatherResult;
        }
    }

}
public class weatherResult
{
    public static string getWeatherIcon(string code, string defaultIcon)
    {
        string result = "";
        switch (code)
        {
            case "113":
                    result = "/Content/Images/WeatherIcon/Sunny.gif";
                    break;
            case "116":
                    result = "/Content/Images/WeatherIcon/Partlycloudy.gif";
                    break;
            case "296":
                result = "/Content/Images/WeatherIcon/Raining.gif";
                break;
            default:
                result = defaultIcon;
                break;
                
        }

        return result; 
    }

    public weatherResult()
    {
        code = "000";
        desc = "No information";
        tempC = "No information";
        maxTemp = "No information";
        minTemp = "No information";
        weatherIcon = "";
        
    }
    public string code { get; set; }
    public string desc { get; set; }
    public string tempC { get; set; }
    public string maxTemp { get; set; }
    public string minTemp { get; set; }
    public string weatherIcon { get; set; }

}