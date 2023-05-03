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
    public class AdminJoinedEventsModelsController : Controller
    {
        private readonly CollegeContext _context;
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly HttpClient _client;

            public AdminJoinedEventsModelsController(CollegeContext context, UserManager<AspNetUsers> user)
            {
                _context = context;
                _userManager = user;
                _client = new HttpClient();
            }

        // GET: AdminJoinedEventsModels
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser = await _userManager.FindByIdAsync(userId);

            var student = _context.students.Where(s => s.user_id.Id == CurrUser.Id).FirstOrDefault();
            List<JoinedEventsModel> joinedEvents = new List<JoinedEventsModel>();
            List<MyEventsViewModel> myEvents = new List<MyEventsViewModel>();

            
            HttpResponseMessage response = _client.GetAsync("https://localhost:7241/api/JoinedEventsModels").Result;


            if (response.IsSuccessStatusCode)
            {

                string data = response.Content.ReadAsStringAsync().Result;

                joinedEvents = JsonConvert.DeserializeObject<List<JoinedEventsModel>>(data);

                var x = joinedEvents.Where(s => s.Status != "Approved").ToList();

                foreach (var item in x)
                {
                   Console.WriteLine(item.JoinedEventsId +" "+" " +item.event_id+"======================================================================================");


                    MyEventsViewModel myEventViewModel = new MyEventsViewModel();
                    var joinedEventsId = _context.joinedEvents.Where(e => e.JoinedEventsId == item.JoinedEventsId).Select(s => s.JoinedEventsId).FirstOrDefault();
                    myEventViewModel.joined_events_id = joinedEventsId;
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
                    var StudentName = _context.students.Where(e => e.RegNo == item.reg_no).Select(s => s.Name).FirstOrDefault();
                    myEventViewModel.student_name = StudentName;

                    myEvents.Add(myEventViewModel);
                }
            }

            return View(myEvents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id)
        {
            Console.WriteLine("==================hello"+id);
            //var eventRow = new JoinedEventsModel();
            HttpResponseMessage response = _client.GetAsync($"https://localhost:7241/api/JoinedEventsModels/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var eventRow = JsonConvert.DeserializeObject<JoinedEventsModel>(data);
                eventRow.Status = "Approved";
                Console.WriteLine(eventRow.JoinedEventsId +" "+" " +eventRow.Status+"======================================================================================");

                //update
                var json = JsonConvert.SerializeObject(eventRow);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var PostResponse = await _client.PutAsync($"https://localhost:7241/api/JoinedEventsModels/{id}", content);

                if (PostResponse.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                return View("Error");

            }
            else
            {
                Console.WriteLine("Get failed=======================");
            }
            return View("Error");


        }


        // GET: AdminJoinedEventsModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null ||id==0 || _context.joinedEvents == null)
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

        // GET: AdminJoinedEventsModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminJoinedEventsModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JoinedEventsId,reg_no,event_id,Status")] JoinedEventsModel joinedEventsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(joinedEventsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(joinedEventsModel);
        }

        // GET: AdminJoinedEventsModels/Edit/5
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

        // POST: AdminJoinedEventsModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JoinedEventsId,reg_no,event_id,Status")] JoinedEventsModel joinedEventsModel)
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

        // GET: AdminJoinedEventsModels/Delete/5
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

        // POST: AdminJoinedEventsModels/Delete/5
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
