using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Model;
using College.Areas.Identity.Data;
using College.Data;
using College.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Razorpay.Api;
using System.Diagnostics;
using System.Drawing.Text;
using System.Security.Claims;
using System.Text;
using static Amazon.S3.Util.S3EventNotification;

namespace College.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CollegeContext _context;
        private readonly UserManager<AspNetUsers> _userManager;

        public HomeController(CollegeContext context, ILogger<HomeController> logger, UserManager<AspNetUsers> user)
        {
            _context = context;
            _logger = logger;
            _userManager = user;

        }
        //checked attendance btn
        public async Task<IActionResult> attendance()
        {
            Console.WriteLine("In attendance");
            ViewBag.isAttendanceMarked = true;
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser = await _userManager.FindByIdAsync(userId);
            StudentsModel student = _context.students.FirstOrDefault(u => u.user_id == CurrUser);

            if (student == null)
            {
                ViewBag.isFeePaid = false;
                
            }
            else if (student.isFeePaid == true)
            {
                Console.WriteLine("In attendance ================================true");
                ViewBag.isFeePaid = true;
            }
            return View("Index",student);
        }

        //marking the attendance to the db
        public async Task<IActionResult> AddAttendance()
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser = await _userManager.FindByIdAsync(userId);

            StudentsModel student = _context.students.FirstOrDefault(u => u.user_id == CurrUser);
            if(student == null)
            {
                ViewBag.Reason = "Enroll as a student and the admin shall add your attendance first";
                return View("Error");
            }
            if (student ==null )
            {
                ViewBag.isFeePaid = false;
            }
            else if(student.isFeePaid == true)
            {
                ViewBag.isFeePaid = true;
            }
            Console.WriteLine(student.LatestAttendanceMArkedTime);
            DateTime? latestDateTime = student.LatestAttendanceMArkedTime;
            TimeSpan? TimeDiff =  DateTime.Now - latestDateTime ;
            
            TimeSpan? helperTime = new TimeSpan(1, 0, 0, 0);
            if (TimeDiff < helperTime )
            {
                ViewBag.isAttendanceMarked = false;
                ViewBag.isAttendanceAdded = false;
                return View("Index", student);
            }
            student.Attendance = student.Attendance + 1;
            student.LatestAttendanceMArkedTime = DateTime.Now;
            ViewBag.isAttendanceAdded = true;
            ViewBag.isAttendanceMarked = false;
            _context.SaveChanges();
            return View("Index",student);
        }


        public async Task<IActionResult>Index()
        {
            
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser = await _userManager.FindByIdAsync(userId);
            StudentsModel student = _context.students.FirstOrDefault(u => u.user_id == CurrUser);
            
            if(CurrUser==null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            else if(CurrUser.Email == "admin@gmail.com")
            {
                return RedirectToAction("Index","Admin");
            }
            if(student == null)
            {
                ViewBag.isFeePaid = false;
                return View();
            }
            ViewBag.isFeePaid = student.isFeePaid;

            // do this for all values and add to viewbag   CurrStudent.isFeePaid

            return View(student);
        }

        public async Task<IActionResult> PayFees()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser = await _userManager.FindByIdAsync(userId);
            StudentsModel student = _context.students.FirstOrDefault(u => u.user_id == CurrUser);
            
            if(student ==null)
            {
                ViewBag.Reason = "Enroll yourself as a student before continuing";
                return View("Error");
            }

            IConfiguration config = new ConfigurationBuilder()
     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
     .Build();

            string apiUrl = "https://api.razorpay.com/v1/orders";
         
            RazorpayClient client = new RazorpayClient(config["RazorPayKey"], config["RazorSecretKey"]);
            
            var orderOptions = new Dictionary<string, object>
            {
                { "amount", 100 },
                { "currency", "INR" },
                { "receipt", "order_rcptid_11" },
                { "payment_capture", 1 }
            };
            var order = client.Order.Create(orderOptions);
            string orderId = order["id"].ToString();
            ViewBag.OrderId = orderId;
            ViewBag.AccessKey = config["RazorPaykey"];
            ViewBag.amount = 100;
           
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayFees(string id)
        {

            Console.WriteLine("In fees post+"+id+"+++++++++++++++++++++++++++++++");
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser =  _userManager.Users.Where(s=>s.Id == userId).FirstOrDefault();
            StudentsModel student = _context.students.FirstOrDefault(u => u.user_id == CurrUser);
            student.isFeePaid = true;
            _context.SaveChanges();
            return RedirectToAction("Index",student);
        }
        public IActionResult Support()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}