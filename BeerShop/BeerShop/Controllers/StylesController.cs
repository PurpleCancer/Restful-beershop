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
    [Produces("application/json")]
    [Route("api/Styles")]
    public class StylesController : Controller
    {
        private readonly BeerContext _context;
        private IUrlHelper _urlHelper;

        public StylesController(BeerContext context, IUrlHelper helper)
        {
            _context = context;
            _urlHelper = helper;
        }

        // GET: api/Styles
        [HttpGet(Name = "GetStyles")]
        public async Task<IActionResult> GetStyles(int page = 1, int pageSize = 5)
        {
            var styles = await _context.Styles
                .Include(s => s.Beers)
                .ToArrayAsync();

            if (styles.Count() < pageSize * (page - 1))
                return NotFound();

            var pagedStyles = styles.Skip((page - 1) * pageSize).Take(pageSize);

            var totalPages = Math.Ceiling(((float)styles.Count()) / pageSize);

            var paging = new
            {
                TotalItems = styles.Count(),
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
            };

            var links = new List<Link>
            {
                new Link
                {
                    Href = _urlHelper.Link("GetStyles", new { page, pageSize }),
                    Rel = "self",
                    Method = "GET",
                }
            };
            if (page > 1)
                links.Add(new Link
                {
                    Href = _urlHelper.Link("GetStyles", new { page = page - 1, pageSize }),
                    Rel = "prevPage",
                    Method = "GET",
                });

            if (page < totalPages)
                links.Add(new Link
                {
                    Href = _urlHelper.Link("GetStyles", new { page = page + 1, pageSize }),
                    Rel = "nextPage",
                    Method = "GET",
                });

            var items = pagedStyles.Select(s => new
            {
                s.Id,
                s.Name,
                s.OptimalTemperature,
            });

            var result = new
            {
                Paging = paging,
                Links = links,
                Items = items,
            };

            return Ok(result);
        }

        // GET: api/Styles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStyleItem([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var styleItem = await _context.Styles
                .Include(s => s.Beers)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (styleItem == null)
            {
                return NotFound();
            }

            var response = new
            {
                styleItem.Id,
                styleItem.Name,
                styleItem.OptimalTemperature,
                Beers = styleItem.Beers.Select(b => b.Name),
            };

            return Ok(response);
        }

        // PUT: api/Styles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStyleItem([FromRoute] long id, [FromBody] Style styleItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (String.IsNullOrEmpty(styleItem.Name)
                || !styleItem.OptimalTemperature.HasValue)
            {
                return BadRequest();
            }

            if (id != styleItem.Id)
            {
                styleItem.Id = id;
            }

            _context.Entry(styleItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StyleItemExists(id))
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

        // PATCH: api/Styles/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchStyleItem([FromRoute] long id, [FromBody] Style styleItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var _style = _context.Styles.Find(id);

            if (_style == null)
                return NotFound();

            _style.Name = !String.IsNullOrEmpty(styleItem.Name) ? styleItem.Name : _style.Name;
            _style.OptimalTemperature = styleItem.OptimalTemperature ?? _style.OptimalTemperature;

            _context.Entry(_style).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StyleItemExists(id))
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

        // POST: api/Styles
        [HttpPost]
        public async Task<IActionResult> PostStyleItem([FromBody] Style styleItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Styles.Add(styleItem);
            await _context.SaveChangesAsync();

            var response = new
            {
                styleItem.Id,
                styleItem.Name,
                styleItem.OptimalTemperature,
            };

            return CreatedAtAction("GetStyleItem", new { id = styleItem.Id }, response);
        }

        // DELETE: api/Styles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStyleItem([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var styleItem = await _context.Styles.SingleOrDefaultAsync(m => m.Id == id);
            if (styleItem == null)
            {
                return NotFound();
            }

            _context.Styles.Remove(styleItem);
            await _context.SaveChangesAsync();

            var response = new
            {
                styleItem.Id,
                styleItem.Name,
                styleItem.OptimalTemperature,
            };

            return Ok(response);
        }

        private bool StyleItemExists(long id)
        {
            return _context.Styles.Any(e => e.Id == id);
        }
    }
}