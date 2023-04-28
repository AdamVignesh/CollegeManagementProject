using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using College.Areas.Identity.Data;
using College.Data;

namespace College.Controllers
{
    public class StudentsModelsController : Controller
    {
        private readonly CollegeContext _context;

        public StudentsModelsController(CollegeContext context)
        {
            _context = context;
        }

        // GET: StudentsModels
        public async Task<IActionResult> Index()
        {
              return _context.students != null ? 
                          View(await _context.students.ToListAsync()) :
                          Problem("Entity set 'CollegeContext.students'  is null.");
        }

        // GET: StudentsModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.students == null)
            {
                return NotFound();
            }

            var studentsModel = await _context.students
                .FirstOrDefaultAsync(m => m.RegNo == id);
            if (studentsModel == null)
            {
                return NotFound();
            }

            return View(studentsModel);
        }

        // GET: StudentsModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StudentsModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RegNo,Name,SchoolName,Percentage12th,Percentage10th,City,isFeePaid,YearOfStudy,Attendance,CGPA,Email,ImageURL")] StudentsModel studentsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(studentsModel);
        }

        // GET: StudentsModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.students == null)
            {
                return NotFound();
            }

            var studentsModel = await _context.students.FindAsync(id);
            if (studentsModel == null)
            {
                return NotFound();
            }
            return View(studentsModel);
        }

        // POST: StudentsModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegNo,Name,SchoolName,Percentage12th,Percentage10th,City,isFeePaid,YearOfStudy,Attendance,CGPA,Email,ImageURL")] StudentsModel studentsModel)
        {
            if (id != studentsModel.RegNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentsModelExists(studentsModel.RegNo))
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
            return View(studentsModel);
        }

        // GET: StudentsModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.students == null)
            {
                return NotFound();
            }

            var studentsModel = await _context.students
                .FirstOrDefaultAsync(m => m.RegNo == id);
            if (studentsModel == null)
            {
                return NotFound();
            }

            return View(studentsModel);
        }

        // POST: StudentsModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.students == null)
            {
                return Problem("Entity set 'CollegeContext.students'  is null.");
            }
            var studentsModel = await _context.students.FindAsync(id);
            if (studentsModel != null)
            {
                _context.students.Remove(studentsModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentsModelExists(int id)
        {
          return (_context.students?.Any(e => e.RegNo == id)).GetValueOrDefault();
        }
    }
}
