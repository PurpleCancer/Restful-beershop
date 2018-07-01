﻿using System;
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

        public StylesController(BeerContext context)
        {
            _context = context;
        }

        // GET: api/Styles
        [HttpGet]
        public async Task<IActionResult> GetStyles()
        {
            var styles = await _context.Styles
                .Include(s => s.Beers)
                .ToArrayAsync();

            var response = styles.Select(s => new
            {
                s.Id,
                s.Name,
            });

            return Ok(response);
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

            if (id != styleItem.Id)
            {
                return BadRequest();
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
            };

            return Ok(response);
        }

        private bool StyleItemExists(long id)
        {
            return _context.Styles.Any(e => e.Id == id);
        }
    }
}