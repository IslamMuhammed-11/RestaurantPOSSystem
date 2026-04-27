using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.DTOs.ProductDTOs;
using DataAccessLayer.Entites;

namespace BusinessLogicLayer.Mapping
{
    public class ProductMap
    {
        public static ProductResponse ToReadDTO(ProductEntity entity)
        {
            return new ProductResponse
            {
                ProductID = entity.ProductID,
                CategoryID = entity.CategoryID,
                Name = entity.Name,
                Price = entity.Price,
                IsAvailable = entity.IsAvailable
            };
        }

        public static ProductEntity ToEntity(CreateProductRequest dto)
        {
            return new ProductEntity
            {
                CategoryID = dto.CategoryID,
                Name = dto.Name,
                Price = dto.Price,
                IsAvailable = dto.IsAvailable
            };
        }

        public static bool ToEntity(UpdateProductRequest dto, ProductEntity product)
        {
            if (dto == null)
                return false;
            if (dto.CategoryID != null)
                product.CategoryID = dto.CategoryID.Value;
            if (!string.IsNullOrEmpty(dto.Name))
                product.Name = dto.Name;
            if (dto.Price != null)
                product.Price = dto.Price.Value;
            if (dto.IsAvailable != null)
                product.IsAvailable = dto.IsAvailable.Value;
            return true;
        }

        public static List<ProductResponse> ToReadDTOList(List<ProductEntity> entities)
        {
            if (entities == null || entities.Count == 0)
                return new List<ProductResponse>();
            return entities.Select(e => ToReadDTO(e)).ToList();
        }
    }
}