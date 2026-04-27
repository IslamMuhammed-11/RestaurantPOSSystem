using DataAccessLayer.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IPaymentRepo
    {
        Task<List<PaymentEntity>> GetAllPaymentsAsync();

        Task<PaymentEntity?> GetPaymentByPaymentIdAsync(int id);

        Task<PaymentEntity?> GetPaymentByOrderIdAsync(int orderId);

        Task<bool> IsPaid(int orderId);

        Task<int?> CreateNewPaymentAsync(PaymentEntity entity);

    }
}
