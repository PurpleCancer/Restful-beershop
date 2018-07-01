using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeerShop.Models
{
    public class Brewery
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<Beer> Beers { get; set; }
    }
}
