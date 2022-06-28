using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Repositories
{
    public interface IDiscountRepository
    {
        Task<CouponModel> CreateDiscount();
        Task<int> UpdateDiscount(CouponModel model);
        Task<int> DeleteDiscount(int Id);
        Task<CouponModel> GetDiscount(string courseName);
    }
}
