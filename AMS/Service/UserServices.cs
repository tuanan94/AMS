using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Constant;
using AMS.Reposiroty;

namespace AMS.Service
{
    public class UserServices
    {
        GenericRepository<User> useRepository = new GenericRepository<User>();

        public User FindById(int id)
        {
            return useRepository.FindById(id);
        }
        public List<User> FindUserByRole(int id)
        {
            return useRepository.List.Where(u => u.RoleId == (int) RoleEnum.Helpdesk).ToList();
        }
    }
}