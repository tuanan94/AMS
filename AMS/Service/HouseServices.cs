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
        public House FindByBlockFloorHouseName(string block, string floor, string houseName)
        {
            IEnumerable<House> houses = _houseRepository.List.Where(h => h.Block.BlockName.ToLower().Contains(block.ToLower())
                                                            && h.Floor.ToLower().Contains(floor.ToLower())
                                                            && h.HouseName.ToLower().Contains(houseName.ToLower()));
            return houses.Count() == 0 ? null : houses.First();
        }
        public List<House> GetFloorInBlock(int blockId)
        {
            return _houseRepository.List.Where(h => h.Block.Id == blockId && h.OwnerID != null).
                OrderBy(h => h.Floor).GroupBy(h => h.Floor).Select(h => h.First()).ToList();
        }
        public List<House> GetRoomsInFloor(int blockId, string floorName)
        {
            return _houseRepository.List.Where(h => h.Floor.Contains(floorName) && h.Block.Id == blockId && h.OwnerID != null).
                OrderBy(h => h.HouseName).ToList();
        }

        public List<House> GetAllOwnedHousesThisMonth()
        {
            return _houseRepository.List.Where(h => h.OwnerID != null).OrderBy(h => h.HouseName).ToList();
        }
    }

    public class BlockServices
    {
        GenericRepository<Block> _blockRepository = new GenericRepository<Block>();

        public Block FindById(int id)
        {
            return _blockRepository.FindById(id);
        }

        public List<Block> GetAllBlocks()
        {
            return _blockRepository.List.OrderBy(b => b.BlockName).ToList();
        }
    }

    public class HouseCategoryServices
    {
        GenericRepository<HouseCategory> _houseCatRepository = new GenericRepository<HouseCategory>();

        public List<HouseCategory> GetAll()
        {
            return _houseCatRepository.List.OrderBy(b => b.Name).ToList();
        }
        public HouseCategory FindById(int id)
        {
            return _houseCatRepository.FindById(id);
        }
    }

    public class UtilServiceForHouseCatServices
    {
        GenericRepository<UtilServiceForHouseCat> _utilServiceForHouseCat = new GenericRepository<UtilServiceForHouseCat>();


        public UtilServiceForHouseCat FindById(int id)
        {
            return _utilServiceForHouseCat.FindById(id);
        }

        public void Delete(UtilServiceForHouseCat item)
        {
            _utilServiceForHouseCat.Delete(item);
        }

        public void Add(UtilServiceForHouseCat u)
        {
            _utilServiceForHouseCat.Add(u);
        }
        public void Update(UtilServiceForHouseCat u)
        {
            _utilServiceForHouseCat.Update(u);
        }

        public List<UtilServiceForHouseCat> GetAllGroupByUtilServiceId()
        {
            return _utilServiceForHouseCat.List.Where(utilSrv => utilSrv.Status != SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_REMOVED).GroupBy(utilSrv => utilSrv.UtilServiceId)
                                   .Select(utilSrv => utilSrv.First()).ToList();
        }

        public List<UtilServiceForHouseCat> GetFixActiveUtilService()
        {
            return _utilServiceForHouseCat.List.Where(
                        utilSrv => utilSrv.Status == SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_ENABLE)
                                   .GroupBy(utilSrv => utilSrv.UtilServiceId)
                                   .Select(utilSrv => utilSrv.First()).ToList();
        }
        public List<UtilServiceForHouseCat> GetActiveUtilServiceOfHouseCategory(int houseCatId)
        {
            return _utilServiceForHouseCat.List.Where(
                        utilSrv => utilSrv.Status == SLIM_CONFIG.UTILITY_SERVICE_OF_HOUSE_CAT_ENABLE && utilSrv.HouseCatId == houseCatId).
                        GroupBy(utilSrv => utilSrv.UtilServiceId).Select(utilSrv => utilSrv.First()).ToList();
        }

        public List<UtilServiceForHouseCat> GetByUtilServiceid(int utilSrvId)
        {
            return _utilServiceForHouseCat.List.Where(utiSrv => utiSrv.UtilServiceId == utilSrvId).ToList();
        }
    }
}