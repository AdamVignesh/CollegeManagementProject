using College.Areas.Identity.Data;
using College.Data;
using College.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
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
        public IActionResult attendance()
        {
            ViewBag.isAttendanceMarked = true;
            return View("Index");
        }
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
        public IActionResult Index()
        {
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