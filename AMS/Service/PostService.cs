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
        public void CreatePosts(Post post)
        {
            

            postRepository.Add(post);
        }
        //public void createPost(string Title, int PostId)
        //{
        //    Post h = new Post();
        //    h.Title = Title;
        //    h.PostId = PostId;

        //    postRepository.Add(h);
        //}
        //public IEnumerable<Post> getAllPost()
        //{
        //    return postRepository.List.OrderByDescending(t=>t.Id).ToList();
        //}
        //public IEnumerable<Post> getAllPostNotDe()
        //{
        //    return postRepository.List.ToList();
        //}
        //public IEnumerable<Post> getAllCommentBelongPost(long id)
        //{
        //    return postRepository.List.ToList().Where(t=>t.PostId == id);
        //}
        //public IEnumerable<Post> getCommentBelongPost(int id)
        //{
        //     return postRepository.List.ToList().Where(t => t.PostId == id);
           
        //}
        //public IEnumerable<Post> getCommentPostIdNotNull()
        //{
        //   return postRepository.List.ToList().Where(t => t.PostId.HasValue);
           
        //}
        //public Post getRowPostByPostId(int id)
        //{
        //    return postRepository.List.ToList().FirstOrDefault(t => t.PostId.HasValue);

        //}
        //public int GetPostIdByPost(int id)
        //{
        //    int postId =0;
        //    if (postRepository.FindById(id).PostId.HasValue)
        //    {
        //        postId = postRepository.FindById(id).PostId.Value;
        //    }
        //    return postId ;
        //}

        //public int CountComment(int id)
        //{
        //    int count= postRepository.List.Count(t=>t.PostId==id);
        //    return count;
        //}
    }
}