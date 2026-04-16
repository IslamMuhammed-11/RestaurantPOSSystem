using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.DTOs.CategoryDTOs;
using DataAccessLayer.Entites;

namespace BusinessLogicLayer.Mapping
{
    public class CategoryMap
    {
        public static ReadCategoryDTO ToReadDTO(CategoryEntity entity)
        {
            return new ReadCategoryDTO
            {
                CategoryID = entity.CategoryID,
                Name = entity.Name
            };
        }

        public static CategoryEntity ToEntity(CreateCategoryDTO dto)
        {
            return new CategoryEntity
            {
                Name = dto.Name
            };
        }

        public static bool ToEntity(UpdateCategoryDTO dto, CategoryEntity category)
        {
            if (dto == null)
                return false;
            if (!string.IsNullOrEmpty(dto.Name))
                category.Name = dto.Name;
            return true;
        }

        public static List<ReadCategoryDTO> ToReadDTOList(List<CategoryEntity> entities)
        {
            if (entities == null || entities.Count == 0)
                return new List<ReadCategoryDTO>();
            return entities.Select(e => ToReadDTO(e)).ToList();
        }
    }
}
