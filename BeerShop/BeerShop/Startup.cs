using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeerShop.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
                
                var ipa = new Style { Name = "IPA" };
                context.Styles.Add(ipa);

                var pinta = new Brewery { Name = "Pinta" };
                context.Breweries.Add(pinta);

                var atakChmielu = new Beer { StyleId = ipa.Id, BreweryId = pinta.Id, Name = "Atak chmielu", Stock = 4 };
                context.Beers.Add(atakChmielu);

                context.SaveChanges();
            }

            app.UseMvc();
        }
    }
}
