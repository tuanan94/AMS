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
        public void Add(House house)
        {
            _houseRepository.Add(house);
        }
        public void Delete(House house)
        {
            _houseRepository.Delete(house);
        }
        public void DeleteById(int houseId)
        {
            House house = _houseRepository.FindById(houseId);
            if (house != null)
            {
                _houseRepository.Delete(house);
            }
        }
        public House FindByHouseName(string houseName)
        {
            return _houseRepository.List.Where(h => h.HouseName.ToLower().Contains(houseName.ToLower())).First();
        }
        public House FindByBlockFloorHouseName(string block, string floor, string houseName)
        {
            IEnumerable<House> houses = _houseRepository.List.Where(h => h.Block.BlockName.ToLower().Contains(block.ToLower())
                                                            && h.Floor.ToLower().Contains(floor.ToLower())
                                                            && h.HouseName.ToLower().Contains(houseName.ToLower())
                                                            && h.Status != SLIM_CONFIG.HOUSE_STATUS_DELETE);
            return houses.Count() == 0 ? null : houses.First();
        }

        public List<House> GetAllFloorInBlock(int blockId)
        {
            return _houseRepository.List.Where(h => h.Block.Id == blockId).
                OrderBy(h => h.Floor).GroupBy(h => h.Floor).Select(h => h.First()).ToList();
            //            return _houseRepository.List.Where(h => h.Block.Id == blockId).
            //                OrderBy(h => h.Floor).GroupBy(h => h.Floor).Select(h => h.First()).ToList();
        }

        public List<House> GetFloorHasResidentInBlock(int blockId)
        {
            return _houseRepository.List.Where(h => h.Block.Id == blockId && h.Status == SLIM_CONFIG.HOUSE_STATUS_ENABLE).
                OrderBy(h => h.Floor).GroupBy(h => h.Floor).Select(h => h.First()).ToList();
            //            return _houseRepository.List.Where(h => h.Block.Id == blockId && h.OwnerID != null).
            //                OrderBy(h => h.Floor).GroupBy(h => h.Floor).Select(h => h.First()).ToList();
            //            return _houseRepository.List.Where(h => h.Block.Id == blockId).
            //                OrderBy(h => h.Floor).GroupBy(h => h.Floor).Select(h => h.First()).ToList();
        }
        public List<House> GetActiveRoomsInFloor(int blockId, string floorName)
        {
            return _houseRepository.List.Where(h => h.Floor.Contains(floorName) && h.Block.Id == blockId && h.Status == SLIM_CONFIG.HOUSE_STATUS_ENABLE).
                OrderBy(h => h.HouseName).ToList();
            //            return _houseRepository.List.Where(h => h.Floor.Contains(floorName) && h.Block.Id == blockId && h.OwnerID != null).
            //                OrderBy(h => h.HouseName).ToList();
        }
        public List<House> GetAlllRoomsInFloor(int blockId, string floorName)
        {
            return _houseRepository.List.Where(h => h.Floor.Contains(floorName) && h.Block.Id == blockId && h.Status !=  SLIM_CONFIG.HOUSE_STATUS_DELETE).
                OrderBy(h => h.HouseName).ToList();
        }
        public List<House> GetAllOwnedHousesThisMonth()
        {
            return _houseRepository.List.Where(h => h.OwnerID != null && h.Status == SLIM_CONFIG.HOUSE_STATUS_ENABLE).OrderBy(h => h.HouseName).ToList();
        }
        public void Update(House h)
        {
            _houseRepository.Update(h);
        }
        public bool CheckHouseNameIsExist(int houseId, string houseName)
        {
            return _houseRepository.List.Where(house => house.HouseName.Equals(houseName) && house.Id != houseId && house.Status != SLIM_CONFIG.HOUSE_STATUS_DELETE).ToList().Count == 0 ? false : true;
        }
        public bool CheckFloorIsExist(int blockId, string floorName)
        {
            return _houseRepository.List.Where(house => house.BlockId == blockId && house.Floor.Equals(floorName.Trim()) && house.Status != SLIM_CONFIG.HOUSE_STATUS_DELETE).ToList().Count == 0 ? false : true;
        }
        public bool CheckHouseNameIsExistInFloor(int blockId, string floorName, string houseName)
        {
            return _houseRepository.List.Where(house => house.BlockId == blockId && house.HouseName.Equals(houseName.Trim()) && house.Floor.Equals(floorName.Trim()) && house.Status != SLIM_CONFIG.HOUSE_STATUS_DELETE).ToList().Count == 0 ? false : true;
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
        public bool CheckBlockNameIsExisted(int blockId, string name)
        {
            return _blockRepository.List.Where(t => t.Id != blockId && t.BlockName.Equals(name)).ToList().Count == 0 ? false : true;
        }
        public bool CheckBlockNameIsExisted(string name)
        {
            return _blockRepository.List.Where(t => t.BlockName.Equals(name)).ToList().Count == 0 ? false : true;
        }
        public void Add(Block block)
        {
            _blockRepository.Add(block);
        }
        public void Update(Block block)
        {
            _blockRepository.Update(block);
        }
        public void Delete(Block block)
        {
            _blockRepository.Delete(block);
        }
        public void Delete(int blockId)
        {
            Block block = _blockRepository.FindById(blockId);
            if (block != null)
            {
                _blockRepository.Delete(block);
            }
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

        public List<UtilServiceForHouseCat> GetActiveUtilService()
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