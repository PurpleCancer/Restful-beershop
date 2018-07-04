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
        private IUrlHelper _urlHelper;

        public BreweriesController(BeerContext context, IUrlHelper helper)
        {
            _context = context;
            _urlHelper = helper;
        }

        // GET: api/Breweries
        [HttpGet(Name = "GetBreweries")]
        public async Task<IActionResult> GetBrewery(int page = 1, int pageSize = 5)
        {
            var breweries = await _context.Breweries
                .ToArrayAsync();

            if (breweries.Count() < pageSize * (page - 1))
                return NotFound();

            var pagedStyles = breweries.Skip((page - 1) * pageSize).Take(pageSize);

            var totalPages = Math.Ceiling(((float)breweries.Count()) / pageSize);

            var paging = new
            {
                TotalItems = breweries.Count(),
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
            };

            var links = new List<Link>
            {
                new Link
                {
                    Href = _urlHelper.Link("GetBreweries", new { page, pageSize }),
                    Rel = "self",
                    Method = "GET",
                }
            };
            if (page > 1)
                links.Add(new Link
                {
                    Href = _urlHelper.Link("GetBreweries", new { page = page - 1, pageSize }),
                    Rel = "prevPage",
                    Method = "GET",
                });

            if (page < totalPages)
                links.Add(new Link
                {
                    Href = _urlHelper.Link("GetBreweries", new { page = page + 1, pageSize }),
                    Rel = "nextPage",
                    Method = "GET",
                });

            var items = pagedStyles.Select(b => new
            {
                b.Id,
                b.Name,
                b.Country,
            });

            var result = new
            {
                Paging = paging,
                Links = links,
                Items = items,
            };

            return Ok(result);
        }

        // GET: api/Breweries/5
        [HttpGet("{id}", Name = "GetBrewery")]
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
                brewery.Country,
                Beers = brewery.Beers.Select(b => b.Name),
                brewery.ResourceVersion,
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

            if (String.IsNullOrEmpty(brewery.Name)
                || String.IsNullOrEmpty(brewery.Country)
                || !brewery.ResourceVersion.HasValue)
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
                    return Conflict();
                }
            }

            brewery.ResourceVersion++;
            _context.Entry(brewery).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // PATCH: api/Breweries/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchBrewery([FromRoute] long id, [FromBody] Brewery brewery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var _brewery = _context.Breweries.Find(id);

            if (_brewery == null)
                return NotFound();

            _brewery.Name = !String.IsNullOrEmpty(brewery.Name) ? brewery.Name : _brewery.Name;
            _brewery.Country = !String.IsNullOrEmpty(brewery.Country) ? brewery.Country : _brewery.Country;

            _context.Entry(_brewery).State = EntityState.Modified;

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

            brewery.ResourceVersion = 0;
            _context.Breweries.Add(brewery);
            await _context.SaveChangesAsync();

            var response = new
            {
                brewery.Id,
                brewery.Name,
                brewery.Country,
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
                brewery.Country,
            };

            return Ok(response);
        }

        private bool BreweryExists(long id)
        {
            return _context.Breweries.Any(e => e.Id == id);
        }
    }
}