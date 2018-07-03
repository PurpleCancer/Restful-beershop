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
        private IUrlHelper _urlHelper;

        public UsersController(BeerContext context, IUrlHelper helper)
        {
            _context = context;
            _urlHelper = helper;
        }

        // GET: api/Users
        [HttpGet(Name = "GetUsers")]
        public IActionResult GetUsers(int page = 1, int pageSize = 5)
        {
            var users = _context.Users;

            if (users.Count() < pageSize * (page - 1))
                return NotFound();

            var pagedStyles = users.Skip((page - 1) * pageSize).Take(pageSize);

            var totalPages = Math.Ceiling(((float)users.Count()) / pageSize);

            var paging = new
            {
                TotalItems = users.Count(),
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
            };

            var links = new List<Link>
            {
                new Link
                {
                    Href = _urlHelper.Link("GetUsers", new { page, pageSize }),
                    Rel = "self",
                    Method = "GET",
                }
            };
            if (page > 1)
                links.Add(new Link
                {
                    Href = _urlHelper.Link("GetUsers", new { page = page - 1, pageSize }),
                    Rel = "prevPage",
                    Method = "GET",
                });

            if (page < totalPages)
                links.Add(new Link
                {
                    Href = _urlHelper.Link("GetUsers", new { page = page + 1, pageSize }),
                    Rel = "nextPage",
                    Method = "GET",
                });

            var items = pagedStyles.Select(u => new
            {
                u.Id,
                u.Name,
            });

            var result = new
            {
                Paging = paging,
                Links = links,
                Items = items,
            };

            return Ok(result);
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

        // DELETE: api/Users/5/cart
        [HttpDelete("{id}/cart/{beerId}")]
        public async Task<IActionResult> DeleteUserCartItem([FromRoute] long id, [FromRoute] long beerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users
                .Include(u => u.Cart)
                .Include(u => u.Cart.CartItems)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
                return BadRequest();

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(m => m.Id == beerId);

            if (cartItem == null)
                return NotFound();

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            var response = new
            {
                cartItem.BeerId,
                cartItem.Count,
            };

            return Ok(response);
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}