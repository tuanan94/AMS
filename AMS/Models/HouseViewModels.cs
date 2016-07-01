using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class HouseViewModels
    {
    }

    public class HouseTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    
    public class FloorModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class HouseViewModel
    {
        public int Id { get; set; }
        public string FloorName { get; set; }
        public string FloorNameNew { get; set; }
        public string Name { get; set; }
        public int BlockId { get; set; }
        public string BlockName { get; set; }
        public int Status { get; set; }
        public double Area { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public string HouseOwner { get; set; }
        public int AddFloor { get; set; }
        public string DT_RowId { get; set; }
    }

    public class BlockViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int NoOfFloor { get; set; }
        public int TotalRoom { get; set; }
        public int TotalActiveRoom { get; set; }
        public int NoRoomPerFloor { get; set; }
        public List<HouseViewModel> Houses { get; set; }
        public List<FloorModel> Floor { get; set; }
    }
}