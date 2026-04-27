using DataAccessLayer.Entites;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IOrderRepo
    {
        Task<OrderAndItemsEntity?> GetOrderAndItemsByOrderIDAsync(int id);

        Task<OrderEntity?> GetOrderByIDAsync(int id);

        Task<List<OrderEntity>> GetAllOrdersAsync();

        Task<int?> CreateOrderAsync(OrderEntity order, DataTable OrderItems);

        Task<bool> UpdateOrderAsync(OrderEntity order);

        Task<bool> ChangeOrderStatus(int id, OrderEntity.enOrderStatus orderStatus);

        Task<bool> ChangeTable(int id, int TableID);

        //Task<bool> DeleteUserAsync(int UserID);
    }
}