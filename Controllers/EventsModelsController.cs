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
using Microsoft.Extensions.Logging;
using System.Text;

namespace College.Controllers
{
    public class EventsModelsController : Controller
    {
        private readonly CollegeContext _context;
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly HttpClient _client;

        public EventsModelsController(CollegeContext context, UserManager<AspNetUsers> user)
        {
            _context = context;
            _userManager = user;
            _client = new HttpClient();
        }

        // GET: EventsModels
        public async Task<IActionResult> Index()
        {
            List<EventsModel> events = new List<EventsModel>();

            HttpResponseMessage response = _client.GetAsync("https://localhost:7241/api/EventsModels").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                events = JsonConvert.DeserializeObject<List<EventsModel>>(data);
            }
            return View(events);
        }

        // GET: EventsModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.events == null)
            {
                return NotFound();
            }

            var eventsModel = await _context.events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (eventsModel == null)
            {
                return NotFound();
            }

            return View(eventsModel);
        }

        //joining an event (adding to the db)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser = await _userManager.FindByIdAsync(userId);


            var student = _context.students.Where(s => s.user_id.Id == CurrUser.Id).FirstOrDefault();
            var events = _context.events.Where(e => e.EventId == id).FirstOrDefault();

            JoinedEventsModel joinedEvent = new JoinedEventsModel();
            if(student == null)
            {
                ViewBag.Reason = "Enroll yourself as a student before continuing";
                return View("Error");
            }
            joinedEvent.reg_no = student.RegNo;
            joinedEvent.event_id = events.EventId;
            joinedEvent.Status = "NotApproved";


            var json = JsonConvert.SerializeObject(joinedEvent);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage joinedEventsResponse = await _client.GetAsync("https://localhost:7241/api/JoinedEventsModels");
            string data = joinedEventsResponse.Content.ReadAsStringAsync().Result;
            var joinedEvents = JsonConvert.DeserializeObject<List<JoinedEventsModel>>(data);

            var isAlreadyRegistered = joinedEvents.Where(s => s.reg_no == student.RegNo && s.event_id == id).FirstOrDefault();

            if (isAlreadyRegistered == null)
            {
                Console.WriteLine("NUllllllllllllllllllllllllllllllllllll=========================================================================");
                HttpResponseMessage response = await _client.PostAsync("https://localhost:7241/api/JoinedEventsModels", content);
              
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("MyEvents");
                }
            }
            else
            {
                Console.WriteLine("not NUllllllllllllllllllllllllllllllllllll=========================================================================");
                return View("AlreadyRegistered");

            }
            return RedirectToAction("MyEvents");


        }

        public async Task<IActionResult> AlreadyRegistered()
        {
            return View();
        }



        public async Task<IActionResult> MyEvents()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser = await _userManager.FindByIdAsync(userId);

            var student = _context.students.Where(s => s.user_id.Id == CurrUser.Id).FirstOrDefault();
            List<JoinedEventsModel> joinedEvents = new List<JoinedEventsModel>();
            List<MyEventsViewModel> myEvents = new List<MyEventsViewModel>();

            HttpResponseMessage response = _client.GetAsync($"https://localhost:7241/api/JoinedEventsModels/").Result;
            

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                joinedEvents = JsonConvert.DeserializeObject<List<JoinedEventsModel>>(data);
                if(student == null)
                {
                    ViewBag.Reason = "Enroll yourself as a student before continuing";
                    return View("Error");
                }

                var listOfMyEvents = joinedEvents.Where(s=>s.reg_no == student.RegNo).ToList();
                
                foreach (var item in listOfMyEvents)
                { 

                    MyEventsViewModel myEventViewModel = new MyEventsViewModel();
                    var eventId = _context.events.Where(e => e.EventId == item.event_id).Select(s => s.EventId).FirstOrDefault();
                    myEventViewModel.event_id = eventId;
                    var eventName = _context.events.Where(e => e.EventId == item.event_id).Select(s => s.EventName).FirstOrDefault();
                    myEventViewModel.event_name = eventName;
                    var eventType = _context.events.Where(e => e.EventId == item.event_id).Select(s => s.EventType).FirstOrDefault();
                    myEventViewModel.event_type = eventType;
                    var eventVenue = _context.events.Where(e => e.EventId == item.event_id).Select(s => s.venue).FirstOrDefault();
                    myEventViewModel.event_venue = eventVenue;
                    var eventDescription = _context.events.Where(e => e.EventId == item.event_id).Select(s => s.EventDescription).FirstOrDefault();
                    myEventViewModel.event_description = eventDescription;
                    var eventDate = _context.events.Where(e => e.EventId == item.event_id).Select(s => s.EventDate).FirstOrDefault();
                    myEventViewModel.event_date = eventDate;
                    var StudentRegNo = _context.students.Where(e => e.RegNo == item.reg_no).Select(s => s.RegNo).FirstOrDefault();
                    myEventViewModel.reg_no = StudentRegNo;

                    myEvents.Add(myEventViewModel);
                }
            }

            return View(myEvents);
        }

        // GET: EventsModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EventsModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,EventType,EventDescription,EventDate,venue")] EventsModel eventsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eventsModel);
        }

        // GET: EventsModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.events == null)
            {
                return NotFound();
            }

            var eventsModel = await _context.events.FindAsync(id);
            if (eventsModel == null)
            {
                return NotFound();
            }
            return View(eventsModel);
        }

        // POST: EventsModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,EventType,EventDescription,EventDate,venue")] EventsModel eventsModel)
        {
            if (id != eventsModel.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventsModelExists(eventsModel.EventId))
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
            return View(eventsModel);
        }

        // GET: EventsModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.events == null)
            {
                return NotFound();
            }

            var eventsModel = await _context.events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (eventsModel == null)
            {
                return NotFound();
            }

            return View(eventsModel);
        }

        // POST: EventsModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.events == null)
            {
                return Problem("Entity set 'CollegeContext.events'  is null.");
            }
            var eventsModel = await _context.events.FindAsync(id);
            if (eventsModel != null)
            {
                _context.events.Remove(eventsModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventsModelExists(int id)
        {
          return (_context.events?.Any(e => e.EventId == id)).GetValueOrDefault();
        }
    }
}
