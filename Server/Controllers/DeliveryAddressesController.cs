using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryAddressesController : ControllerBase
    {
        private readonly DeliverySystemDbContext _context;

        public DeliveryAddressesController(DeliverySystemDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliveryAddress>>> GetDeliveryAddresses()
        {
            return await _context.DeliveryAddresses.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryAddress>> GetDeliveryAddress(int id)
        {
            var deliveryAddress = await _context.DeliveryAddresses.FindAsync(id);

            if (deliveryAddress == null)
            {
                return NotFound();
            }

            return deliveryAddress;
        }

        [HttpPost]
        public async Task<ActionResult<DeliveryAddress>> PostDeliveryAddress(DeliveryAddress deliveryAddress)
        {
            _context.DeliveryAddresses.Add(deliveryAddress);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDeliveryAddress), new { id = deliveryAddress.AddressId }, deliveryAddress);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeliveryAddress(int id, DeliveryAddress deliveryAddress)
        {
            if (id != deliveryAddress.AddressId)
            {
                return BadRequest();
            }

            _context.Entry(deliveryAddress).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeliveryAddressExists(id))
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
        public async Task<IActionResult> DeleteDeliveryAddress(int id)
        {
            var deliveryAddress = await _context.DeliveryAddresses.FindAsync(id);
            if (deliveryAddress == null)
            {
                return NotFound();
            }

            _context.DeliveryAddresses.Remove(deliveryAddress);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeliveryAddressExists(int id)
        {
            return _context.DeliveryAddresses.Any(e => e.AddressId == id);
        }
    }
}
