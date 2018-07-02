using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeerShop.Models
{
    public class Beer
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public long? StyleId { get; set; }
        public Style Style { get; set; }
        public long? BreweryId { get; set; }
        public Brewery Brewery { get; set; }

        public string Picture { get; set; }
        public int? Stock { get; set; }
    }
}
