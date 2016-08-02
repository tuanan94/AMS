using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Tên đăng nhập")]
        public String username { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
        public String password { get; set; }
    }
}