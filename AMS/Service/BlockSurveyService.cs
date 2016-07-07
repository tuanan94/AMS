using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class BlockPollService
    {
        GenericRepository<BlockPoll> blockRepository = new GenericRepository<BlockPoll>();
        GenericRepository<House> houseRepository = new GenericRepository<House>();
        public void AddBlockPoll(BlockPoll obj)
        {
            blockRepository.Add(obj);
        }
        public List<BlockPoll> FindByPollId(int PollId)
        {
            return blockRepository.List.Where(t => t.PollId == PollId).ToList();
        }
        public House FindBlockIdByHouseId(int houseId)
        {
            return houseRepository.List.FirstOrDefault(t => t.Id == houseId);
        }
        public void UpdateBlockPoll(BlockPoll obj)
        {
            blockRepository.Update(obj);
        }

        public void DeleteBlockPoll(BlockPoll obj)
        {
            blockRepository.Delete(obj);
        }

        public BlockPoll FIndBlockPollByBlockIdPollId(int blockId, int PollId)
        {
            return blockRepository.List.FirstOrDefault(t => t.BlockId == blockId && t.PollId == PollId);
        }

       
        public bool CheckBlock(int blockId, int PollId)
        {
            var result = blockRepository.List.FirstOrDefault(t => t.BlockId == blockId && t.PollId == PollId);
            if (result != null)
            {
                return true;
            }
            return false;
        }
    }
}