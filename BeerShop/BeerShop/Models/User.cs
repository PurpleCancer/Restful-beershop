using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeerShop.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public long CartId { get; set; }
        public Cart Cart { get; set; }
    }
}
