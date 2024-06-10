using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryStatusesController : ControllerBase
    {
        private readonly DeliverySystemDbContext _context;

        public DeliveryStatusesController(DeliverySystemDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliveryStatus>>> GetDeliveryStatuses()
        {
            return await _context.DeliveryStatuses.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryStatus>> GetDeliveryStatus(int id)
        {
            var deliveryStatus = await _context.DeliveryStatuses.FindAsync(id);

            if (deliveryStatus == null)
            {
                return NotFound();
            }

            return deliveryStatus;
        }

        [HttpPost]
        public async Task<ActionResult<DeliveryStatus>> PostDeliveryStatus(DeliveryStatus deliveryStatus)
        {
            _context.DeliveryStatuses.Add(deliveryStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDeliveryStatus), new { id = deliveryStatus.StatusId }, deliveryStatus);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeliveryStatus(int id, DeliveryStatus deliveryStatus)
        {
            if (id != deliveryStatus.StatusId)
            {
                return BadRequest();
            }

            _context.Entry(deliveryStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeliveryStatusExists(id))
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
        public async Task<IActionResult> DeleteDeliveryStatus(int id)
        {
            var deliveryStatus = await _context.DeliveryStatuses.FindAsync(id);
            if (deliveryStatus == null)
            {
                return NotFound();
            }

            _context.DeliveryStatuses.Remove(deliveryStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeliveryStatusExists(int id)
        {
            return _context.DeliveryStatuses.Any(e => e.StatusId == id);
        }
    }
}
