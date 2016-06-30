using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class HouseViewModels
    {
    }

    public class HouseViewModel
    {
        public int Id { get; set; }
        public string FloorName { get; set; }
        public string Name { get; set; }
        public string BlockId { get; set; }
        public int Status { get; set; }
        public double Area { get; set; }
        public int Type { get; set; }
    }

    public class BlockViewModel
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public int NoOfFloor { get; set; }
        public int NoRoomPerFloor { get; set; }
        public List<HouseViewModel> Houses { get; set; }
    }
}