using DataAccessLayer.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IPaymentMethodRepo
    {
        Task<int?> AddNewMethod(PaymentMethodEntity method);

        Task<List<PaymentMethodEntity>> GetAllMethods();

        Task<PaymentMethodEntity?> GetMethodByIdAsync(int id);

        Task<bool> DeleteMethod(int methodId);

        Task<bool> UpdateMethod(PaymentMethodEntity method);

    }
}
