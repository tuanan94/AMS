using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AMS.Repository;

namespace AMS.Service
{
    public class BlockSurveyService
    {
        GenericRepository<BlockSurvey> blockRepository = new GenericRepository<BlockSurvey>();
        GenericRepository<House> houseRepository = new GenericRepository<House>();
        public void AddBlockSurvey(BlockSurvey obj)
        {
            blockRepository.Add(obj);
        }
        public List<BlockSurvey> FindBySurveyId(int surveyId)
        {
            return blockRepository.List.Where(t => t.SurveyId == surveyId).ToList();
        }
        public House FindBlockIdByHouseId(int houseId)
        {
            return houseRepository.List.FirstOrDefault(t => t.Id == houseId);
        }
        public void UpdateBlockSurvey(BlockSurvey obj)
        {
            blockRepository.Update(obj);
        }

        public void DeleteBlockPoll(BlockSurvey obj)
        {
            blockRepository.Delete(obj);
        }

        public BlockSurvey FIndBlockSurveyByBlockIdSurveyId(int blockId, int surveyId)
        {
            return blockRepository.List.FirstOrDefault(t => t.BlockId == blockId && t.SurveyId == surveyId);
        }

       
        public bool CheckBlock(int blockId, int surveyId)
        {
            var result = blockRepository.List.FirstOrDefault(t => t.BlockId == blockId && t.SurveyId == surveyId);
            if (result != null)
            {
                return true;
            }
            return false;
        }
    }
}