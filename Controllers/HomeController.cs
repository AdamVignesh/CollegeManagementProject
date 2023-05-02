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
using Razorpay.Api;
using System.Diagnostics;
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
        public IActionResult attendance()
        {
            ViewBag.isAttendanceMarked = true;
            return View("Index");
        }

        //marking the attendance to the db
        public async Task<IActionResult> AddAttendance()
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser = await _userManager.FindByIdAsync(userId);

            StudentsModel student = _context.students.FirstOrDefault(u => u.user_id == CurrUser);
            if(student == null)
            {
                return View("Error");
            }
            Console.WriteLine(student.LatestAttendanceMArkedTime);
            DateTime? latestDateTime = student.LatestAttendanceMArkedTime;
            TimeSpan? TimeDiff =  DateTime.Now - latestDateTime ;
            
            TimeSpan? helperTime = new TimeSpan(1, 0, 0, 0);
            if (TimeDiff < helperTime )
            {
                ViewBag.isAttendanceMarked = false;
                ViewBag.isAttendanceAdded = false;
                return View("Index");
            }
            student.Attendance = student.Attendance + 1;
            student.LatestAttendanceMArkedTime = DateTime.Now;
            ViewBag.isAttendanceAdded = true;
            ViewBag.isAttendanceMarked = false;
            _context.SaveChanges();
            return View("Index");
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

            // do this for all values and add to viewbag   CurrStudent.isFeePaid

            return View(student);
        }

        public async Task<IActionResult> PayFees()
        {
            // string apiUrl = "https://api.razorpay.com/v1/orders";
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
        public IActionResult Privacy()
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