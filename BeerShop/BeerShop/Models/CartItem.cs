using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeerShop.Models
{
    public class CartItem
    {
        public long Id { get; set; }
        public long CartId { get; set; }
        public Cart Cart { get; set; }
        public long BeerId { get; set; }
        public Beer Beer { get; set; }
        public int Count { get; set; }
    }
}
