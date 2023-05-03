using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using College.Areas.Identity.Data;
using College.Data;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace College.Controllers
{
    public class JoinedEventsModelsController : Controller
    {
        private readonly CollegeContext _context;
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly HttpClient _client;

        public JoinedEventsModelsController(CollegeContext context, UserManager<AspNetUsers> user)
        {
            _context = context;
            _userManager = user;
            _client = new HttpClient();
        }

        // GET: JoinedEventsModels
        public async Task<IActionResult> Index()
        {
            List<EventsModel> EventJoinedByUSer = new List<EventsModel>();

            HttpResponseMessage response = _client.GetAsync("https://localhost:7241/api/EventsModels").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                EventJoinedByUSer = JsonConvert.DeserializeObject<List<EventsModel>>(data);

                Console.WriteLine("=============" + EventJoinedByUSer);
            }
            return View()   ;
          
        }

        

        // GET: JoinedEventsModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.joinedEvents == null)
            {
                return NotFound();
            }

            var joinedEventsModel = await _context.joinedEvents
                .FirstOrDefaultAsync(m => m.JoinedEventsId == id);
            if (joinedEventsModel == null)
            {
                return NotFound();
            }

            return View(joinedEventsModel);
        }

        // GET: JoinedEventsModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JoinedEventsModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JoinedEventsId")] JoinedEventsModel joinedEventsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(joinedEventsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(joinedEventsModel);
        }

        // GET: JoinedEventsModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.joinedEvents == null)
            {
                return NotFound();
            }

            var joinedEventsModel = await _context.joinedEvents.FindAsync(id);
            if (joinedEventsModel == null)
            {
                return NotFound();
            }
            return View(joinedEventsModel);
        }

        // POST: JoinedEventsModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JoinedEventsId")] JoinedEventsModel joinedEventsModel)
        {
            if (id != joinedEventsModel.JoinedEventsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(joinedEventsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JoinedEventsModelExists(joinedEventsModel.JoinedEventsId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(joinedEventsModel);
        }

        // GET: JoinedEventsModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.joinedEvents == null)
            {
                return NotFound();
            }

            var joinedEventsModel = await _context.joinedEvents
                .FirstOrDefaultAsync(m => m.JoinedEventsId == id);
            if (joinedEventsModel == null)
            {
                return NotFound();
            }

            return View(joinedEventsModel);
        }

        // POST: JoinedEventsModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.joinedEvents == null)
            {
                return Problem("Entity set 'CollegeContext.joinedEvents'  is null.");
            }
            var joinedEventsModel = await _context.joinedEvents.FindAsync(id);
            if (joinedEventsModel != null)
            {
                _context.joinedEvents.Remove(joinedEventsModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JoinedEventsModelExists(int id)
        {
          return (_context.joinedEvents?.Any(e => e.JoinedEventsId == id)).GetValueOrDefault();
        }
    }
}
