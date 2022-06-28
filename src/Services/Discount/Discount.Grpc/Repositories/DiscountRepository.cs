using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        private readonly NpgsqlConnection _cnn;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _cnn = new NpgsqlConnection("Server=localhost; Database=CouponDb; User Id=admin; Password=admin1234;");
        }


        public async Task<CouponModel> CreateDiscount()
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteDiscount(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<CouponModel> GetDiscount(string courseName)
        {
            NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM Coupon WHERE CourseName = '{courseName}'", _cnn);
            _cnn.Open();

            var price = new CouponModel();
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                price.Id = int.Parse(rdr["Id"].ToString());
                price.CourseName = rdr.GetString(1);
                price.CourseDescription = rdr.GetString(2);
                price.Price = int.Parse(rdr["Price"].ToString());
            }
            return price;
        }


        public Task<int> UpdateDiscount(CouponModel model)
        {
            throw new NotImplementedException();
        }
    }
}
