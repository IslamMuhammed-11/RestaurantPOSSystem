using Contracts.DTOs.OrderItemsDTOs;
using DataAccessLayer.Entites;

namespace BusinessLogicLayer.Mapping
{
    public class ItemMap
    {
        public static ReadItemDTO ToReadDTO(ItemsEntity entity)
        {
            if (entity == null) return null!;
            return new ReadItemDTO
            {
                ItemID = entity.ItemID,
                ProductID = entity.ProductID,
                OrderID = entity.OrderID,
                Quantity = entity.Quantity,
                Price = entity.Price
            };
        }

        public static ItemsEntity ToEntity(AddItemDTO dto, int orderId)
        {
            if (dto == null) return null!;
            return new ItemsEntity
            {
                ProductID = dto.ProductID,
                OrderID = orderId,
                Quantity = dto.Quantity,
                Price = 0 // Price will be set by DB or other logic if needed
            };
        }

        public static bool ToEntity(UpdateItemDTO dto, ItemsEntity existing)
        {
            if (dto == null || existing == null) return false;
            if (dto.ProductID.HasValue)
                existing.ProductID = dto.ProductID.Value;
            if (dto.OrderID.HasValue)
                existing.OrderID = dto.OrderID.Value;
            if (dto.Quantity.HasValue)
                existing.Quantity = dto.Quantity.Value;
            if (dto.Price.HasValue)
                existing.Price = dto.Price.Value;
            return true;
        }
    }
}
