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
    public class EventsModelsController : ControllerBase
    {
        private readonly CollegeContext _context;

        public EventsModelsController(CollegeContext context)
        {
            _context = context;
        }

        // GET: api/EventsModels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventsModel>>> Getevents()
        {
          if (_context.events == null)
          {
              return NotFound();
          }
            return await _context.events.ToListAsync();
        }

        // GET: api/EventsModels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EventsModel>> GetEventsModel(int id)
        {
          if (_context.events == null)
          {
              return NotFound();
          }
            var eventsModel = await _context.events.FindAsync(id);

            if (eventsModel == null)
            {
                return NotFound();
            }

            return eventsModel;
        }

        // PUT: api/EventsModels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEventsModel(int id, EventsModel eventsModel)
        {
            if (id != eventsModel.EventId)
            {
                return BadRequest();
            }

            _context.Entry(eventsModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventsModelExists(id))
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

        // POST: api/EventsModels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EventsModel>> PostEventsModel(EventsModel eventsModel)
        {
          if (_context.events == null)
          {
              return Problem("Entity set 'CollegeContext.events'  is null.");
          }
            _context.events.Add(eventsModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEventsModel", new { id = eventsModel.EventId }, eventsModel);
        }

        // DELETE: api/EventsModels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventsModel(int id)
        {
            if (_context.events == null)
            {
                return NotFound();
            }
            var eventsModel = await _context.events.FindAsync(id);
            if (eventsModel == null)
            {
                return NotFound();
            }

            _context.events.Remove(eventsModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventsModelExists(int id)
        {
            return (_context.events?.Any(e => e.EventId == id)).GetValueOrDefault();
        }
    }
}
