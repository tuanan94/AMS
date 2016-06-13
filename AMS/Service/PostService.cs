using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;
using AMS.ViewModel;
using AMS.ObjectMapping;

namespace AMS.Service
{
    public class PostService
    {
        GenericRepository<Post> postRepository = new GenericRepository<Post>();
        GenericRepository<Comment> commentRepository = new GenericRepository<Comment>();
        public void createPost(PostViewModel post)
        {
           
          Post h = new Post();
            h.ImgUrl = post.ImgUrl;
            h.Title = post.Title;
            h.CreateDate = post.CreateDate;
            postRepository.Add(h);
        }
        public void createPost(ListPostViewModel post)
        {
            Post h = new Post();
            h.ImgUrl = post.ImgUrl;
            h.Title = post.Title;

            postRepository.Add(h);
        }
        public int addPost(Post p)
        {
            postRepository.Add(p);
            return p.Id;
        }
        
        public void CreatePosts(Post post)
        {
            

            postRepository.Add(post);
        }
        public void createPost(string Title, int PostId)
        {
            Post h = new Post();
            h.Title = Title;
          //  h.PostId = PostId;

            postRepository.Add(h);
        }
        public Post findPostById(int id)
        {
            var result = postRepository.FindById(id);
            return result;
        }
        public List<Post> getAllPost()
        {
            var result =  postRepository.List.OrderByDescending(t=>t.Id).ToList();
            return result;
        }
        public List<Post> getAllPostById(int id)
        {
            var result = postRepository.List.Where(t => t.Id==id).ToList();
            return result;
        }
        public List<Post> getAllPost(int? tokenId, int? houseId)
        {
            List<Post> result = new List<Post>();
            List<Post> allPost = getAllPost();
            if (houseId != null)
            {
                foreach (Post p in allPost)
                {
                    if (p.User.HouseId == houseId)
                    {
                        result.Add(p);
                    }
                }
            }else
            {
                result = allPost;
            }
            return result;
        }

        public IEnumerable<Post> getAllPostNotDe()
        {
            return postRepository.List.ToList();
        }
      
        public IEnumerable<Post> getCommentBelongPost(int id)
        {
            //   return postRepository.List.ToList().Where(t => t.PostId == id);
            return null;
        }
        public IEnumerable<Post> getCommentPostIdNotNull()
        {
            //   return postRepository.List.ToList().Where(t => t.PostId.HasValue);
            return null;
        }
        public Post getRowPostByPostId(int id)
        {
            //  return postRepository.List.ToList().FirstOrDefault(t => t.PostId.HasValue);
            return null;
        }
        public int GetPostIdByPost(int id)
        {
            return -1;
        }
        public List<CommentMapping> comments(int postid)
        {
            List<Comment> allComment = commentRepository.List.ToList();
            List<CommentMapping> result = new List<CommentMapping>();
            foreach(Comment c in allComment)
            {
                if(postid == c.postId)
                {
                    CommentMapping cMapping = new CommentMapping();
                    cMapping.id = c.id;
                    cMapping.detail = c.detail;
                    cMapping.createdDate = c.createdDate.GetValueOrDefault();
                    cMapping.username = c.User.Username;
                    cMapping.userProfile = c.User.ProfileImage;
                    cMapping.userId = c.userId.GetValueOrDefault();
                    result.Add(cMapping);
                }
            }
            return result;
        }
        public Comment findCommentById(int commentId)
        {
            return commentRepository.FindById(commentId);
        }
        public void addComment(Comment c)
        {
            commentRepository.Add(c);
        }
       
    }

}