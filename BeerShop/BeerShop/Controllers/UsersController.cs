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
    public class UsersController : ControllerBase
    {
        private readonly BeerContext _context;

        public UsersController(BeerContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users;

            var response = users.Select(u => new
            {
                u.Id,
                u.Name,
            });

            return Ok(response);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);

            var response = new
            {
                user.Id,
                user.Name,
            };

            if (user == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        // GET: api/Users/5/cart
        [HttpGet("{id}/cart")]
        public async Task<IActionResult> GetUserCart([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users
                .Include(u => u.Cart)
                .Include(u => u.Cart.CartItems)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var response = user.Cart.CartItems.Select(c => new
            {
                c.BeerId,
                c.Count,
            });

            if (user == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] long id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (String.IsNullOrEmpty(user.Name))
                return BadRequest();

            var _user = _context.Users.Find(id);
            _user.Name = user.Name;

            _context.Entry(_user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (String.IsNullOrEmpty(user.Name))
                return BadRequest();

            var cart = new Cart();
            _context.Carts.Add(cart);

            var newUser = new User { Name = user.Name, CartId = cart.Id };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var response = new
            {
                newUser.Id,
                newUser.Name,
            };

            return CreatedAtAction("GetUser", new { id = newUser.Id }, response);
        }

        // POST: api/Users/5/cart
        [HttpPost("{id}/cart")]
        public async Task<IActionResult> PostCartItem([FromRoute] long id, [FromBody] CartItem cartItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!cartItem.BeerId.HasValue || !cartItem.Count.HasValue)
                return BadRequest();

            var user = await _context.Users
                .Include(u => u.Cart)
                .Include(u => u.Cart.CartItems)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
                return BadRequest();

            var beer = await _context.Beers.FindAsync(cartItem.BeerId);
            if (beer.Stock < cartItem.Count)
                return BadRequest();

            var cItem = user.Cart.CartItems.FirstOrDefault(c => c.BeerId == cartItem.BeerId);
            if (cItem == null)
            {
                var newCartItem = new CartItem
                {
                    CartId = user.CartId,
                    BeerId = cartItem.BeerId,
                    Count = cartItem.Count,
                };
                _context.CartItems.Add(newCartItem);
            }
            else
            {
                cItem.Count = cartItem.Count;
                _context.CartItems.Update(cItem);
            }

            await _context.SaveChangesAsync();

            var response = new
            {
                cartItem.BeerId,
                cartItem.Count
            };

            return CreatedAtAction("GetUserCart", new { id = user.Id }, response);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var response = new
            {
                user.Id,
                user.Name,
            };

            var cart = await _context.Carts.FindAsync(user.CartId);
            if (cart != null)
                _context.Carts.Remove(cart);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(response);
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}