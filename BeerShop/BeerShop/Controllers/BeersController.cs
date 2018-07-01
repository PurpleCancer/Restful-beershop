using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeerShop.Models;

namespace BeerShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeersController : ControllerBase
    {
        private readonly BeerContext _context;

        public BeersController(BeerContext context)
        {
            _context = context;
        }

        // GET: api/Beers
        [HttpGet]
        public async Task<IActionResult> GetBeer()
        {
            var beers = await _context.Beers
                .ToArrayAsync();

            var response = beers.Select(b => new
            {
                b.Id,
                b.StyleId,
                b.BreweryId,
                b.Name,
                b.Stock,
            });

            return Ok(response);
        }

        // GET: api/Beers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBeer([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var beer = await _context.Beers
                .Include(b => b.Style)
                .Include(b => b.Brewery)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (beer == null)
            {
                return NotFound();
            }

            var response = new
            {
                beer.Id,
                beer.Name,
                Style = new
                {
                    beer.Style.Id,
                    beer.Style.Name,
                },
                Brewery = new
                {
                    beer.Brewery.Id,
                    beer.Brewery.Name,
                },
                beer.Picture,
                beer.Stock,
            };

            return Ok(response);
        }

        // PUT: api/Beers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBeer([FromRoute] long id, [FromBody] Beer beer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != beer.Id)
            {
                return BadRequest();
            }

            _context.Entry(beer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BeerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Beers
        [HttpPost]
        public async Task<IActionResult> PostBeer([FromBody] Beer beer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Beers.Add(beer);
            await _context.SaveChangesAsync();

            var response = new
            {
                beer.Id,
                beer.StyleId,
                beer.BreweryId,
                beer.Picture,
                beer.Stock,
            };

            return CreatedAtAction("GetBeer", new { id = beer.Id }, response);
        }

        // DELETE: api/Beers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBeer([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var beer = await _context.Beers.FindAsync(id);

            if (beer == null)
            {
                return NotFound();
            }

            _context.Beers.Remove(beer);
            await _context.SaveChangesAsync();

            var response = new
            {
                beer.Id,
                beer.StyleId,
                beer.BreweryId,
                beer.Picture,
                beer.Stock,
            };

            return Ok(response);
        }

        private bool BeerExists(long id)
        {
            return _context.Beers.Any(e => e.Id == id);
        }
    }
}