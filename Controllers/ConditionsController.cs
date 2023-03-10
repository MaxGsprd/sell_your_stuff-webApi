using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellYourStuffWebApi.Data;
using SellYourStuffWebApi.Models;

namespace SellYourStuffWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConditionsController : ControllerBase
    {
        private readonly SellYourStuffWebApiContext _context;

        public ConditionsController(SellYourStuffWebApiContext context)
        {
            _context = context;
        }

        // GET: api/Condition
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Condition>>> GetCondition()
        {
            return await _context.Condition.ToListAsync();
        }

        // GET: api/Condition/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Condition>> GetCondition(int id)
        {
            var condition = await _context.Condition.FindAsync(id);

            if (condition == null)
            {
                return NotFound();
            }

            return condition;
        }

        // PUT: api/Condition/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCondition(int id, Condition condition)
        {
            if (id != condition.Id)
            {
                return BadRequest();
            }

            _context.Entry(condition).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConditionExists(id))
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

        // POST: api/Condition
        [HttpPost]
        public async Task<ActionResult<Condition>> PostCondition(Condition condition)
        {
            _context.Condition.Add(condition);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCondition", new { id = condition.Id }, condition);
        }

        // DELETE: api/Condition/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCondition(int id)
        {
            var condition = await _context.Condition.FindAsync(id);
            if (condition == null)
            {
                return NotFound();
            }

            _context.Condition.Remove(condition);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ConditionExists(int id)
        {
            return _context.Condition.Any(e => e.Id == id);
        }
    }
}
