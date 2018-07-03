using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeerShop.Models
{
    public class Favorite
    {
        public long FavoriteId { get; set; }
        public long? UserId { get; set; }
        public User User { get; set; }
        public long? BeerId { get; set; }
        public Beer Beer { get; set; }
    }
}
