using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeerShop.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BeerShop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BeerContext>(opt =>
                opt.UseInMemoryDatabase("beers"));
            services.AddMvc();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x => {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<BeerContext>();

                var ipa = new Style { Name = "IPA", OptimalTemperature = 1.8, ResourceVersion = 0 };
                context.Styles.Add(ipa);
                var stout = new Style { Name = "Stout", OptimalTemperature = 3.2, ResourceVersion = 0 };
                context.Styles.Add(stout);
                var porter = new Style { Name = "IPA", OptimalTemperature = 4.2, ResourceVersion = 0 };
                context.Styles.Add(porter);
                var hefe = new Style { Name = "Hefeweizen", OptimalTemperature = 1.6, ResourceVersion = 0 };
                context.Styles.Add(hefe);
                var wheat = new Style { Name = "Wheat wine", OptimalTemperature = 1.2, ResourceVersion = 0 };
                context.Styles.Add(wheat);

                var pinta = new Brewery { Name = "Pinta", Country = "Poland", ResourceVersion = 0 };
                context.Breweries.Add(pinta);
                var szalpiw = new Brewery { Name = "Pinta", Country = "Poland", ResourceVersion = 0 };
                context.Breweries.Add(szalpiw);
                var nogne = new Brewery { Name = "Nogne", Country = "Norway", ResourceVersion = 0 };
                context.Breweries.Add(nogne);

                var atakChmielu = new Beer { StyleId = ipa.Id, BreweryId = pinta.Id, Name = "Atak Chmielu", Stock = 4, ResourceVersion = 0 };
                context.Beers.Add(atakChmielu);
                var warsaw = new Beer { StyleId = wheat.Id, BreweryId = pinta.Id, Name = "Warsaw Express", Stock = 7, ResourceVersion = 0 };
                context.Beers.Add(warsaw);
                var imperator = new Beer { StyleId = porter.Id, BreweryId = pinta.Id, Name = "Imperator Bałtycki", Stock = 1, ResourceVersion = 0 };
                context.Beers.Add(imperator);
                var bamber = new Beer { StyleId = hefe.Id, BreweryId = szalpiw.Id, Name = "Bamber", Stock = 10, ResourceVersion = 0 };
                context.Beers.Add(bamber);
                var _500 = new Beer { StyleId = ipa.Id, BreweryId = nogne.Id, Name = "#500", Stock = 2, ResourceVersion = 0 };
                context.Beers.Add(_500);
                var harvest = new Beer { StyleId = stout.Id, BreweryId = nogne.Id, Name = "Havrestout", Stock = 4, ResourceVersion = 0 };
                context.Beers.Add(harvest);
                var ris = new Beer { StyleId = stout.Id, BreweryId = pinta.Id, Name = "RISFACTOR", Stock = 4, ResourceVersion = 0 };
                context.Beers.Add(ris);

                var user1Cart = new Cart();
                context.Carts.Add(user1Cart);
                var cartItem1 = new CartItem { CartId = user1Cart.Id, BeerId = atakChmielu.Id, Count = 2 };
                context.CartItems.Add(cartItem1);
                var cartItem2 = new CartItem { CartId = user1Cart.Id, BeerId = _500.Id, Count = 1 };
                context.CartItems.Add(cartItem2);
                var user1 = new User { Name = "Jan", CartId = user1Cart.Id, ResourceVersion = 0 };
                context.Users.Add(user1);

                var user2Cart = new Cart();
                context.Carts.Add(user2Cart);
                var user2 = new User { Name = "Adam", CartId = user2Cart.Id, ResourceVersion = 0 };
                context.Users.Add(user2);

                context.SaveChanges();
            }

            app.UseMvc();
        }
    }
}
