using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AMS.ViewModel
{
    public class UpdateHouseViewModel
    {
        public int Id { get; set; }
        public String Block { get; set; }
      
        public String Floor { get; set; }
    
        public float Area { get; set; }
       
        [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} must be at least {2} characters long.")]
        [Display(Name = "HouseName")]
        [DataType(DataType.Text)]
        public String HouseName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "The {0} must be at least {2} characters long.")]
        [Display(Name = "Discription")]
        [DataType(DataType.Text)]
        public String Description { get; set; }

    }
}