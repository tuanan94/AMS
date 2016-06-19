using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Models;
using AMS.Repository;

namespace AMS.Service
{
    public class HouseServices
    {
        GenericRepository<House> _houseRepository = new GenericRepository<House>();

        public House FindById(int id)
        {
            return _houseRepository.FindById(id);
        }
        public House FindByHouseName(string houseName)
        {
            return _houseRepository.List.Where(h => h.HouseName.ToLower().Contains(houseName.ToLower())).First();
        }
        public List<House> GetAllBlock()
        {
            return _houseRepository.List.Where(h => h.OwnerID != null).
                OrderBy(h => h.Block).GroupBy(h => h.Block).Select(h => h.First()).ToList();
        }
        //public List<House> GetFloorInBlock(string blockName)
        //{
        //    return _houseRepository.List.Where(h => h.Block.Contains(blockName) &&  h.OwnerID != null).
        //        OrderBy(h => h.Floor).GroupBy(h => h.Floor).Select(h => h.First()).ToList();
        //}
        //public List<House> GetRoomsInFloor(string blockName, string floorName)
        //{
        //    return _houseRepository.List.Where(h => h.Floor.Contains(floorName) && h.Block.Contains(blockName) && h.OwnerID != null).
        //        OrderBy(h => h.HouseName).ToList();
        //}
        public void updateHouse(House h)
        {
            _houseRepository.Update(h);
        }
    }
}