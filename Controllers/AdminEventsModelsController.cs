using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using College.Areas.Identity.Data;
using College.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace College.Controllers
{
    public class AdminEventsModelsController : Controller
    {
        private readonly CollegeContext _context;
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly HttpClient _client;

        public AdminEventsModelsController(CollegeContext context, UserManager<AspNetUsers> user)
        {
            _context = context;
            _userManager = user;
            _client = new HttpClient();
        }

        // GET: AdminEventsModels
        public async Task<IActionResult> Index()
        {
            List<EventsModel> events = new List<EventsModel>();

            HttpResponseMessage response = _client.GetAsync("https://localhost:7241/api/EventsModels").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                events = JsonConvert.DeserializeObject<List<EventsModel>>(data);

                //Console.WriteLine("=============" + events);
            }
            return View(events);
        }

        // GET: AdminEventsModels/Details/5
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

        // GET: AdminEventsModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminEventsModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,EventType,EventDescription,EventDate,venue")] EventsModel eventsModel)
        {
            //seriailzing the value from bind and converting it to a http content to post it through api

            var json = JsonConvert.SerializeObject(eventsModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("https://localhost:7241/api/EventsModels", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index",eventsModel);
            }
            return View();
            
        }

        // GET: AdminEventsModels/Edit/5
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

        // POST: AdminEventsModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,EventType,EventDescription,EventDate,venue")] EventsModel eventsModel)
        {
            var json = JsonConvert.SerializeObject(eventsModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PutAsync($"https://localhost:7241/api/EventsModels/{id}", content);


            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", eventsModel);
            }
            else
            {
                Console.WriteLine("Edit Unsuccessful");
            }
                return View();
            
        }

        // GET: AdminEventsModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            EventsModel EventToDelete = new EventsModel();

            HttpResponseMessage response =  _client.GetAsync($"https://localhost:7241/api/EventsModels/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                EventToDelete = JsonConvert.DeserializeObject<EventsModel>(data);
            }
            else
            {
            }
                return View(EventToDelete);
        }

        // POST: AdminEventsModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"https://localhost:7241/api/EventsModels/{id}");
            return RedirectToAction("Index");
        }

        private bool EventsModelExists(int id)
        {
          return (_context.events?.Any(e => e.EventId == id)).GetValueOrDefault();
        }
    }
}
