using AMS.ObjectMapping;
using AMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Service
{
    public class NotificationService
    {
        GenericRepository<NotificationObject> notificationObjectRepository = new GenericRepository<NotificationObject>();
        GenericRepository<NotificationChange> notificationChangeRepository = new GenericRepository<NotificationChange>();

        NotificationObject findNotificationObject(int UserId, String ObjectBeingChanged,int? targetObjecID)
        {
            if (targetObjecID == null)
            {
                return notificationObjectRepository.List.Where(s => s.UserID == UserId && s.TargetObject.Equals(ObjectBeingChanged)).FirstOrDefault();
            }
            else
            {
                
                return notificationObjectRepository.List.Where(s => s.UserID == UserId && s.TargetObject.Equals(ObjectBeingChanged)&&s.TargetObjectID.HasValue&&s.TargetObjectID==targetObjecID).FirstOrDefault();
            }
        }
        NotificationChange findNotificationChange(int id)
        {
            return notificationChangeRepository.FindById(id);
        }
        private void addNotificationObject(NotificationObject nObject)
        {
            notificationObjectRepository.Add(nObject);
        }
        private void addNotificationChange(NotificationChange nChange)
        {
            notificationChangeRepository.Add(nChange);
        }
        /// <summary>
        /// A notification is about something (object = event, friendship..) being changed (verb = added, requested..) by someone (actor) and reported to the user (subject).
        /// </summary>
        /// <param name="ObjectBeingChanged">Post,Comment,HelpdeskRequest,Add member request</param>
        /// <param name="userID">To user</param>
        /// <param name="verb">Change,Like,Comment,Approve</param>
        /// <param name="actor">By User</param>
        public void addNotification(String ObjectBeingChanged, int userID,String verb,int actor,int? targetObjectID)
        {
            NotificationObject nObject = findNotificationObject(userID, ObjectBeingChanged,targetObjectID);
            if (nObject == null)
            {
                nObject = new NotificationObject();
                nObject.UserID = userID;
                nObject.TargetObject = ObjectBeingChanged;
                nObject.TargetObjectID = targetObjectID;
                addNotificationObject(nObject);
            }
            NotificationChange nChange = new NotificationChange();
            nChange.NotificationObjectID = nObject.ID;
            nChange.Actor = actor;
            nChange.Verb = verb;

            nChange.CreatedDate = DateTime.Now;
            
            notificationChangeRepository.Add(nChange);
        }
        public List<NotificationChange> getAllNotificationChange(int UserId)
        {
            return notificationChangeRepository.List.Where(l => l.NotificationObject.UserID == UserId).ToList();
        }
        public void deleteNoticByNchangeID(int nChangeID)
        {
            NotificationChange nChange = findNotificationChange(nChangeID);
            if (nChange != null)
            {
                notificationChangeRepository.Delete(nChange);
            }
        }


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
                    n.notification = "Ban quản lý vừa đăng một bài viết";
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