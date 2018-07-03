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
        private IUrlHelper _urlHelper;

        public BeersController(BeerContext context, IUrlHelper helper)
        {
            _context = context;
            _urlHelper = helper;
        }

        // GET: api/Beers
        [HttpGet(Name = "GetBeers")]
        public async Task<IActionResult> GetBeer(int page = 1, int pageSize = 5)
        {
            var beers = await _context.Beers
                .ToArrayAsync();

            if (beers.Count() < pageSize * (page - 1))
                return NotFound();

            var pagedStyles = beers.Skip((page - 1) * pageSize).Take(pageSize);

            var totalPages = Math.Ceiling(((float)beers.Count()) / pageSize);

            var paging = new
            {
                TotalItems = beers.Count(),
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
            };

            var links = new List<Link>
            {
                new Link
                {
                    Href = _urlHelper.Link("GetBeers", new { page, pageSize }),
                    Rel = "self",
                    Method = "GET",
                }
            };
            if (page > 1)
                links.Add(new Link
                {
                    Href = _urlHelper.Link("GetBeers", new { page = page - 1, pageSize }),
                    Rel = "prevPage",
                    Method = "GET",
                });

            if (page < totalPages)
                links.Add(new Link
                {
                    Href = _urlHelper.Link("GetBeers", new { page = page + 1, pageSize }),
                    Rel = "nextPage",
                    Method = "GET",
                });

            var items = pagedStyles.Select(b => new
            {
                b.Id,
                b.StyleId,
                b.BreweryId,
                b.Name,
                b.Stock,
            });

            var result = new
            {
                Paging = paging,
                Links = links,
                Items = items,
            };

            return Ok(result);
        }

        // GET: api/Beers/5
        [HttpGet("{id}", Name = "GetBeer")]
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
                beer.ResourceVersion,
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

            if (String.IsNullOrEmpty(beer.Name)
                || !beer.StyleId.HasValue
                || !beer.BreweryId.HasValue
                || !beer.Stock.HasValue
                || !beer.ResourceVersion.HasValue)
            {
                return BadRequest();
            }

            if (id != beer.Id)
            {
                beer.Id = id;
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
                    return Redirect(_urlHelper.Link("GetBeer", new { id }));
                }
            }

            beer.ResourceVersion++;
            _context.Entry(beer).State = EntityState.Modified;
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

        // PATCH: api/Beers/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchBeer([FromRoute] long id, [FromBody] Beer beer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != beer.Id)
            {
                beer.Id = id;
            }

            var _beer = _context.Beers.Find(id);

            if (_beer == null)
                return NotFound();

            _beer.Name = !String.IsNullOrEmpty(beer.Name) ? beer.Name : _beer.Name;
            _beer.StyleId = beer.StyleId ?? _beer.StyleId;
            _beer.BreweryId = beer.BreweryId ?? _beer.BreweryId;
            _beer.Picture = !String.IsNullOrEmpty(beer.Picture) ? beer.Picture : _beer.Picture;
            _beer.Stock = beer.Stock ?? _beer.Stock;

            _context.Entry(_beer).State = EntityState.Modified;

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

            beer.ResourceVersion = 0;
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