using AMS.ObjectMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Service
{
    public class NotificationService
    {
        PostService postService = new PostService();
        public List<Notification> getNotification(int userId)
        {
            List<Notification> result = new List<Notification>();
            List<Notification> postFromManager = getNotificationTypeManagerPost(userId);
            foreach(Notification n in postFromManager)
            {
                result.Add(n);
            }
            return result;
        }
        public List<Notification> getNotificationTypeManagerPost(int userId)
        {
            List<Notification> result = new List<Notification>();
            List<Post> allManagerPost =  postService.getAllPostByRole(SLIM_CONFIG.USER_ROLE_MANAGER);
            foreach(Post p in allManagerPost)
            {
                if (p.CreateDate != null)
                {
                    Notification n = new Notification();
                    n.notification = "Ban quản lí vừa đăng một bài viết";
                    n.type = SLIM_CONFIG.NOTIFICATION_TYPE_MANAGERPOST;
                    n.source = "/Post/" + p.Id;
                    n.date = p.CreateDate.GetValueOrDefault();
                    result.Add(n);
                }
                
            }
          
           
            return result;
        }
    }
}