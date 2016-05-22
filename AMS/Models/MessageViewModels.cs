using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace AMS.Models
{
    public class MessageViewModels
    {
        public string Msg { get; set; } = "Ok";

        public int StatusCode { get; set; } = 0;

        public Object Data { get; set; } = new Object();


    }
}