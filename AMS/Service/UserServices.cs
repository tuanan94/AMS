using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Constant;
using AMS.Repository;

namespace AMS.Service
{
    public class UserServices
    {
        GenericRepository<User> userRepository = new GenericRepository<User>();

        public User FindById(int id)
        {
            return userRepository.FindById(id);
        }
        public List<User> FindUserByRole(int id)
        {
            return userRepository.List.Where(u => u.RoleId == SLIM_CONFIG.USER_ROLE_SUPPORTER).ToList();
        }
        public List<User> GetAllUnapproveUsers()
        {
            return
                userRepository.List.Where(
                    u => u.RoleId == SLIM_CONFIG.USER_ROLE_RESIDENT 
                    && u.IsApproved != null && u.IsApproved == SLIM_CONFIG.USER_APPROVE_WAITING).ToList();
        }

        public void Update(User u)
        {
            userRepository.Update(u);
        }
    }
}