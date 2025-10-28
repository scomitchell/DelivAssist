using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GigBoardBackend.Data;
using GigBoardBackend.Models;

namespace GigBoardBackend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DeliveryController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public DeliveryController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> PutDelivery(int id, Delivery delivery)
		{
			if (id != delivery.Id)
			{
				return BadRequest();
			}

			_context.Entry(delivery).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!DeliveryExists(id))
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


		private bool DeliveryExists(int id)
		{
			return _context.Deliveries.Any(d => d.Id == id);
		}
	}
}