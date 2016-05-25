using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Reposiroty;
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
            postRepository.Add(h);
        }
        public List<Post> getAllPost()
        {
            return postRepository.List.ToList();
        }
    }
}