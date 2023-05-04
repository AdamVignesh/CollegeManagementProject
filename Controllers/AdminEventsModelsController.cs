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
using System.Security.Claims;
using MimeKit;
using System.ComponentModel;

namespace College.Controllers
{
    public class AdminEventsModelsController : Controller
    {
        private readonly CollegeContext _context;
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public AdminEventsModelsController(CollegeContext context, UserManager<AspNetUsers> user, IConfiguration configuration)
        {
            _context = context;
            _userManager = user;
            _client = new HttpClient();
            _configuration = configuration;
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

                ////////////////////////////////////////////////////

                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var CurrUser = await _userManager.FindByIdAsync(userId);

                var student = _context.students.Where(s => s.user_id.Id == CurrUser.Id).FirstOrDefault();
                List<JoinedEventsModel> joinedEvents = new List<JoinedEventsModel>();
                List<MyEventsViewModel> myEvents = new List<MyEventsViewModel>();

                HttpResponseMessage JoinedEventsResponse = _client.GetAsync("https://localhost:7241/api/JoinedEventsModels").Result;
                Console.WriteLine(JoinedEventsResponse.IsSuccessStatusCode + "======successcode==============================================================================");

                if (JoinedEventsResponse.IsSuccessStatusCode)
                {

                    string JoinedEventsData = JoinedEventsResponse.Content.ReadAsStringAsync().Result;

                    joinedEvents = JsonConvert.DeserializeObject<List<JoinedEventsModel>>(JoinedEventsData);
                    Console.WriteLine(joinedEvents.Count + "======count==============================================================================");

                    foreach (var item in joinedEvents)
                    {
                    Console.WriteLine(item.Status+ "======item==============================================================================");

                    }

                    var x = joinedEvents.Where(s => s.Status == "NotApproved").ToList();


                    foreach (var item in x)
                    {
                        //   Console.WriteLine(item.JoinedEventsId + " " + " " + item.event_id + "======================================================================================");

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
                Console.WriteLine(myEvents.Count+"======from==============================================================================");
                ViewBag.NotificationCount = myEvents.Count;
                return View(events);

                ////////////////////////////////////////////////////
            }
            ViewBag.NotificationCount = 0;
            return View(events);
        }

        // GET: AdminEventsModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id == 0 || _context.events == null)
            {
                return RedirectToAction("Index");
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

        //method for sending email
        public void SendEmail(string toEmail, string Description, string EventName)
        {
            try
            {

                string fromPassword = _configuration["fromEmailPass"];
                Console.WriteLine("from password" + fromPassword);
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("testname1234554321@gmail.com"));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = "New Event ALERT" ;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = $"<div><h1>Event Name: {EventName}</h1><p>{Description}</p> <p>Check our college website for further details</p></div>",
                };

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                smtp.Authenticate("testname1234554321@gmail.com", fromPassword);

                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("eeeeeeeeeexxecption ========================" + ex.Message);
            }
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
                //calling send mail 
                SendEmail("vighu1610@gmail.com", eventsModel.EventDescription, eventsModel.EventName);

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
