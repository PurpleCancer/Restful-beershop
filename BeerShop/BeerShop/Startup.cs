﻿using System;
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

                var pinta = new Brewery { Name = "Pinta", Country = "Poland", ResourceVersion = 0 };
                context.Breweries.Add(pinta);

                var atakChmielu = new Beer { StyleId = ipa.Id, BreweryId = pinta.Id, Name = "Atak chmielu", Stock = 4, ResourceVersion = 0 };
                context.Beers.Add(atakChmielu);
                
                var user1Cart = new Cart();
                context.Carts.Add(user1Cart);
                var cartItem1 = new CartItem { CartId = user1Cart.Id, BeerId = atakChmielu.Id, Count = 2 };
                context.CartItems.Add(cartItem1);
                var user1 = new User { Name = "Jan", CartId = user1Cart.Id,  ResourceVersion = 0 };
                context.Users.Add(user1);

                context.SaveChanges();
            }

            app.UseMvc();
        }
    }
}
