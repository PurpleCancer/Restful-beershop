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
    public class BreweriesController : ControllerBase
    {
        private readonly BeerContext _context;

        public BreweriesController(BeerContext context)
        {
            _context = context;
        }

        // GET: api/Breweries
        [HttpGet]
        public async Task<IActionResult> GetBrewery()
        {
            var breweries = await _context.Breweries
                .ToArrayAsync();

            var response = breweries.Select(b => new
            {
                b.Id,
                b.Name,
            });

            return Ok(Response);
        }

        // GET: api/Breweries/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrewery([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var brewery = await _context.Breweries
                .Include(b => b.Beers)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (brewery == null)
            {
                return NotFound();
            }

            var response = new
            {
                brewery.Id,
                brewery.Name,
                Beers = brewery.Beers.Select(b => b.Name),
            };

            return Ok(response);
        }

        // PUT: api/Breweries/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBrewery([FromRoute] long id, [FromBody] Brewery brewery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (String.IsNullOrEmpty(brewery.Name))
            {
                return BadRequest();
            }

            if (id != brewery.Id)
            {
                brewery.Id = id;
            }

            _context.Entry(brewery).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BreweryExists(id))
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

        // POST: api/Breweries
        [HttpPost]
        public async Task<IActionResult> PostBrewery([FromBody] Brewery brewery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Breweries.Add(brewery);
            await _context.SaveChangesAsync();

            var response = new
            {
                brewery.Id,
                brewery.Name,
            };

            return CreatedAtAction("GetBrewery", new { id = brewery.Id }, response);
        }

        // DELETE: api/Breweries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrewery([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var brewery = await _context.Breweries.FindAsync(id);
            if (brewery == null)
            {
                return NotFound();
            }

            _context.Breweries.Remove(brewery);
            await _context.SaveChangesAsync();

            var response = new
            {
                brewery.Id,
                brewery.Name,
            };

            return Ok(response);
        }

        private bool BreweryExists(long id)
        {
            return _context.Breweries.Any(e => e.Id == id);
        }
    }
}