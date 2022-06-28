using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Modals
{
    public class BasketModel
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }

    public class BasketModelList
    {
        public string UserName { get; set; }
        public IEnumerable<BasketModel> basket { get; set; }
        public int TotalPrice { get; set; }
    }
}
