using AMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Service
{
    public class TestService
    {
        GenericRepository<House> houseObj = new GenericRepository<House>();

        /*  @detail get all house
        *   @return list House object
        */
        public List<House> getAllHouse()
        {
            return houseObj.List.ToList();
        }

        /*  @Detail Add house
         *  @Param Block : building apartment
         *  @Param Floor
         *  @Param Name of house
         *  @return 
         */
        public void addHouse(String Block, String Floor, String HouseName)
        {
            House house = new House();
         //   house.Block = Block;
            house.Floor = Floor;
            house.HouseName = HouseName;
            houseObj.Add(house);    //Add to house object
        }
    }
}