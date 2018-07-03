using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeerShop.Models
{
    public class Cart
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public List<CartItem> CartItems { get; set; }

        public Cart()
        {
            OrderId = 0;
        }
    }
}
