using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderedItemsController : ControllerBase
    {
        private readonly DeliverySystemDbContext _context;

        public OrderedItemsController(DeliverySystemDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderedItem>>> GetOrderedItems()
        {
            return await _context.OrderedItems.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderedItem>> GetOrderedItem(int id)
        {
            var orderedItem = await _context.OrderedItems.FindAsync(id);

            if (orderedItem == null)
            {
                return NotFound();
            }

            return orderedItem;
        }

        [HttpPost]
        public async Task<ActionResult<OrderedItem>> PostOrderedItem(OrderedItem orderedItem)
        {
            _context.OrderedItems.Add(orderedItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderedItem), new { id = orderedItem.OrderedItemId }, orderedItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderedItem(int id, OrderedItem orderedItem)
        {
            if (id != orderedItem.OrderedItemId)
            {
                return BadRequest();
            }

            _context.Entry(orderedItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderedItemExists(id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderedItem(int id)
        {
            var orderedItem = await _context.OrderedItems.FindAsync(id);
            if (orderedItem == null)
            {
                return NotFound();
            }

            _context.OrderedItems.Remove(orderedItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderedItemExists(int id)
        {
            return _context.OrderedItems.Any(e => e.OrderedItemId == id);
        }
    }
}
