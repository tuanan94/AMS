using AMS.Reposiroty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Service
{
    public class AdminService
    {
        GenericRepository<House> allHouseInfo = new GenericRepository<House>();
        // get house's info
        // return list house's info
        public List<House> getAllHouseInfo()
        {
            return allHouseInfo.List.ToList();
        }

        public void addHouse(String Block, String Floor, String HouseName, String Description, float Area)
        {
            House house = new House();
            house.Block = Block;
            house.Floor = Floor;
            house.HouseName = HouseName;
            house.Description = Description;
            house.Area = Area;

            allHouseInfo.Add(house);    //Add to house object
        }
        public void deleteHouse(int Id)
        {
            House deleteHouse = new House();
            deleteHouse.Id = Id;
            allHouseInfo.Delete(allHouseInfo.FindById(deleteHouse.Id));
        }
    }

    
}