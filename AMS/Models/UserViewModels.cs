using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Models
{
    public class UserViewModels
    {
    }

    public class ResidentGroupByTypeViewModel
    {
        public User User { get; set; }
        public int Count { get; set; }

        public ResidentGroupByTypeViewModel(User user, int count)
        {
            User = user;
            Count = count;
        }
    }
}