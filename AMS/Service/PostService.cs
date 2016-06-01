using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;
using AMS.ViewModel;

namespace AMS.Service
{
    public class PostService
    {
        GenericRepository<Post> postRepository = new GenericRepository<Post>();
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
          //  h.PostId = PostId;

         //   postRepository.Add(h);
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
       
       
    }

}