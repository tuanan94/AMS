using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.ViewModel
{
    public class AddMemberViewModel
    {
        [Required(AllowEmptyStrings = false,ErrorMessage = "Tên đăng nhập không được để trống")]
        [Display(Name = "Tên đăng nhập")]
        [DataType(DataType.Text)]
        public String Username { get; set; }
        [Required(ErrorMessage ="Mật khẩu không được để trống")]
        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public String Password { get; set; }
        
        [Display(Name = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "Mật khẩu không trùng khớp")]
        public String ConfirmPassword { get; set; }

        [Required(ErrorMessage ="Họ và tên không được để trống")]
        [Display(Name = "Họ và tên")]
        [DataType(DataType.Text)]
        public String Fullname { get; set; }

        [Required]
        [Display(Name = "Giới tính")]
        public int Gender { get; set; }

        [Required]
        [Display(Name = "Ngày sinh")]
        public DateTime DateOfBirth { get; set; }
        
        [Display(Name = "Số CMND")]
        public String IDNumber { get; set; }
        
        [Required(AllowEmptyStrings =false,ErrorMessage ="Bạn chưa chọn ảnh đại diện cho người này")]
        public String ImageURL { get; set; }
    }
}