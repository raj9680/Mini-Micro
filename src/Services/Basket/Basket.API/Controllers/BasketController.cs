using Basket.API.Extensions;
using Basket.API.GrpcServices;
using Basket.API.Modals;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]/[action]")]
    public class BasketController : ControllerBase
    {
        private readonly NpgsqlConnection _cnn;
        private readonly IDistributedCache _cache;
        private readonly DiscountGrpcService _discountGrpcService;

        public BasketController(IDistributedCache cache, DiscountGrpcService discountDrpcService)
        {
            _cnn = new NpgsqlConnection("Server=localhost; Database=BasketDb; User Id=admin; Password=admin1234;");
            _cache = cache;
            _discountGrpcService = discountDrpcService;
        }



        [HttpGet("{username}")]
        public async Task<BasketModelList> GetBasket(string username)
        {
            // Hit Redis First
            string recordKey = username;
            var loadFromCache = await _cache.GetRecordAsync<BasketModelList>(recordKey);
            if(loadFromCache != null)
            {
                Console.WriteLine("Loaded from cache");
                return loadFromCache;
            }

            // Hit Database
            List<BasketModel> basketItems = new List<BasketModel>();
            NpgsqlCommand cmd = new NpgsqlCommand($"SELECT * FROM Basket WHERE UserName = '{username}'", _cnn);
            _cnn.Open();
            NpgsqlDataReader rdr = cmd.ExecuteReader();
 
            while (rdr.Read())
            {
                var dataList = new BasketModel();
                dataList.Id = int.Parse(rdr["Id"].ToString());
                dataList.CourseName = rdr.GetString(2);
                dataList.CourseDescription = rdr.GetString(3);
                dataList.Price = int.Parse(rdr["Price"].ToString());
                dataList.Quantity = int.Parse(rdr["Quantity"].ToString());


                basketItems.Add(dataList);
            }

            var TotalPrice = 0;
            foreach (var item in basketItems)
            {
                // Discount
                var coupon = await _discountGrpcService.GetDiscount(item.CourseName);
                TotalPrice += (item.Price - coupon.Price) * item.Quantity;
                // End
            }

            BasketModelList basketModel = new BasketModelList();
            basketModel.UserName = username;
            basketModel.basket = basketItems;
            basketModel.TotalPrice = TotalPrice;

            await _cache.SetRecordAsync(recordKey, basketModel);

            return basketModel;
        }



        [HttpDelete("{userName}")]
        public async Task<ActionResult> DeleteBasket(string userName)
        {
            NpgsqlCommand cmd = new NpgsqlCommand($"DELETE FROM Basket Where UserName = '{userName}'", _cnn);
            _cnn.Open();
            cmd.ExecuteNonQuery();
            _cnn.Close();
            return NoContent();
        }

    }
}
