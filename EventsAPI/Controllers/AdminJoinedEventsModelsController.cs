using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using College.Areas.Identity.Data;
using College.Data;

namespace EventsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminJoinedEventsModelsController : ControllerBase
    {
        private readonly CollegeContext _context;

        public AdminJoinedEventsModelsController(CollegeContext context)
        {
            _context = context;
        }

        // GET: api/AdminJoinedEventsModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JoinedEventsModel>>> GetjoinedEvents()
        {
          if (_context.joinedEvents == null)
          {
              return NotFound();
          }
            return await _context.joinedEvents.ToListAsync();
        }

        // GET: api/AdminJoinedEventsModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JoinedEventsModel>> GetJoinedEventsModel(int id)
        {
          if (_context.joinedEvents == null)
          {
              return NotFound();
          }
            var joinedEventsModel = await _context.joinedEvents.FindAsync(id);

            if (joinedEventsModel == null)
            {
                return NotFound();
            }

            return joinedEventsModel;
        }

        // PUT: api/AdminJoinedEventsModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJoinedEventsModel(int id, JoinedEventsModel joinedEventsModel)
        {
            if (id != joinedEventsModel.JoinedEventsId)
            {
                return BadRequest();
            }

            _context.Entry(joinedEventsModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JoinedEventsModelExists(id))
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

        // POST: api/AdminJoinedEventsModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<JoinedEventsModel>> PostJoinedEventsModel(JoinedEventsModel joinedEventsModel)
        {
          if (_context.joinedEvents == null)
          {
              return Problem("Entity set 'CollegeContext.joinedEvents'  is null.");
          }
            _context.joinedEvents.Add(joinedEventsModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJoinedEventsModel", new { id = joinedEventsModel.JoinedEventsId }, joinedEventsModel);
        }

        // DELETE: api/AdminJoinedEventsModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJoinedEventsModel(int id)
        {
            if (_context.joinedEvents == null)
            {
                return NotFound();
            }
            var joinedEventsModel = await _context.joinedEvents.FindAsync(id);
            if (joinedEventsModel == null)
            {
                return NotFound();
            }

            _context.joinedEvents.Remove(joinedEventsModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JoinedEventsModelExists(int id)
        {
            return (_context.joinedEvents?.Any(e => e.JoinedEventsId == id)).GetValueOrDefault();
        }
    }
}
