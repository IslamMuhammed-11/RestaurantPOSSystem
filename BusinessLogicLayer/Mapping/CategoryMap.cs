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
        public static CategoryResponse ToReadDTO(CategoryEntity entity)
        {
            return new CategoryResponse
            {
                CategoryID = entity.CategoryID,
                Name = entity.Name
            };
        }

        public static CategoryEntity ToEntity(CreateCategoryRequest dto)
        {
            return new CategoryEntity
            {
                Name = dto.Name
            };
        }

        public static bool ToEntity(UpdateCategoryRequest dto, CategoryEntity category)
        {
            if (dto == null)
                return false;
            if (!string.IsNullOrEmpty(dto.Name))
                category.Name = dto.Name;
            return true;
        }

        public static List<CategoryResponse> ToReadDTOList(List<CategoryEntity> entities)
        {
            if (entities == null || entities.Count == 0)
                return new List<CategoryResponse>();
            return entities.Select(e => ToReadDTO(e)).ToList();
        }
    }
}
