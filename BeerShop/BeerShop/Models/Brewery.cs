﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeerShop.Models
{
    public class Brewery
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public List<Beer> Beers { get; set; }
        [ConcurrencyCheck]
        public int? ResourceVersion { get; set; }
    }
}
