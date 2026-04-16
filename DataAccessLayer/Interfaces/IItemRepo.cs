using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entites;
namespace DataAccessLayer.Interfaces
{
    public interface IItemRepo
    {
        Task<ItemsEntity> GetItemByIdAsync(int Id);

        Task<int?> AddNewItemAsync(ItemsEntity item);

        Task<bool> UpdateItemsAsync(ItemsEntity item);

        Task<bool> UpdateQuantityAsync(ItemsEntity item);

        Task<bool> DeleteItemsAsync(int id);

    }
}
