using AMS.Reposiroty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Service
{
    public class TestService
    {
        GenericRepository<House> testRepository = new GenericRepository<House>();
        public List<House> getAllHouse()
        {
            return testRepository.List.ToList();
        }
    }
}