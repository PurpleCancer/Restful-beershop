using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public List<Favorite> Favorites { get; set; }

        [ConcurrencyCheck]
        public int? ResourceVersion { get; set; }
    }
}
