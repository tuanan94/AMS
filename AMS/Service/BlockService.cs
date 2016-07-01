using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class BlockService
    {
        GenericRepository<Block> blockRepository = new GenericRepository<Block>();

        public List<Block> GetListBlock()
        {
            return blockRepository.List.ToList();
        }
        public Block FindBlockByName(string name)
        {
            //            return blockRepository.List.FirstOrDefault(t=>t.BlockName == name);// Code ngu vãi
            return blockRepository.List.FirstOrDefault(t => t.BlockName.Equals(name));
        }
        public bool CheckBlockNameIsExisted(string name)
        {
            return blockRepository.List.Where(t => t.BlockName.Equals(name)).ToList().Count == 0 ? false: true;
        }
        public Block FindBlockById(int blockId)
        {
            return blockRepository.List.FirstOrDefault(t => t.Id == blockId);
        }

    }
}