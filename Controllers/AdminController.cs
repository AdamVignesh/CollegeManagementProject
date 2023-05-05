using College.Areas.Identity.Data;
using College.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace College.Controllers
{
    public class AdminController : Controller
    {

        private readonly CollegeContext _context;
        private readonly UserManager<AspNetUsers> _userManager;
        public AdminController(CollegeContext context, UserManager<AspNetUsers> user)
        {
            _context = context;
            _userManager = user;
        }

        // GET: AdminController
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ViewStudents()
        {
            List<StudentsModel> students = _context.students.Include(t=>t.department_id).Select(x=>x).ToList();
            ViewBag.StudentsList = students;
            //students[0].department_id
            foreach(var s in students)
            {
                Console.WriteLine("----- "+s.department_id);
            }
            return View();
        }
        // GET: AdminController/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int id)
        {
            var student = _context.students.Where(s => s.RegNo == id).FirstOrDefault();
            return View(student);
        }

        // GET: AdminController/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var student = _context.students.Where(s => s.RegNo == id).FirstOrDefault();
            return View(student);
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
                Console.WriteLine("==========delete post==================");
                Console.WriteLine("=========="+id+"==================");
            if (_context.students == null)
            {
                return  Problem("Entity set 'CollegeContext.students'  is null.");
            }
            var studentsModel = await _context.students.FindAsync(id);
            if (studentsModel != null)
            {
                Console.WriteLine("==========" + id + "===inside if===============");

                _context.students.Remove(studentsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewStudents");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
