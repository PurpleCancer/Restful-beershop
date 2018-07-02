using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeerShop.Models;

namespace BeerShop.Models
{
    public class BeerContext : DbContext
    {
        public BeerContext(DbContextOptions<BeerContext> options)
            : base(options)
        { }

        public DbSet<Style> Styles { get; set; }

        public DbSet<Brewery> Breweries { get; set; }

        public DbSet<Beer> Beers { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
    }
}
