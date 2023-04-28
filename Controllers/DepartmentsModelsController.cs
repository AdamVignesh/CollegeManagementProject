  using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using College.Areas.Identity.Data;
using College.Data;
using Microsoft.AspNetCore.Authorization;

namespace College.Controllers
{
    public class DepartmentsModelsController : Controller
    {
        private readonly CollegeContext _context;

        public DepartmentsModelsController(CollegeContext context)
        {
            _context = context;
        }

        // GET: DepartmentsModels
        public async Task<IActionResult> Index()
        {
              return _context.departments != null ? 
                          View(await _context.departments.ToListAsync()) :
                          Problem("Entity set 'CollegeContext.departments'  is null.");
        }

        // GET: DepartmentsModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.departments == null)
            {
                return NotFound();
            }

            var departmentsModel = await _context.departments
                .FirstOrDefaultAsync(m => m.DeptId == id);
            if (departmentsModel == null)
            {
                return NotFound();
            }

            return View(departmentsModel);
        }

        // GET: DepartmentsModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DepartmentsModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DeptId,DeptName,DeptHead")] DepartmentsModel departmentsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(departmentsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(departmentsModel);
        }

        // GET: DepartmentsModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.departments == null)
            {
                return NotFound();
            }

            var departmentsModel = await _context.departments.FindAsync(id);
            if (departmentsModel == null)
            {
                return NotFound();
            }
            return View(departmentsModel);
        }

        // POST: DepartmentsModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DeptId,DeptName,DeptHead")] DepartmentsModel departmentsModel)
        {
            if (id != departmentsModel.DeptId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(departmentsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentsModelExists(departmentsModel.DeptId))
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
            return View(departmentsModel);
        }

        // GET: DepartmentsModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.departments == null)
            {
                return NotFound();
            }

            var departmentsModel = await _context.departments
                .FirstOrDefaultAsync(m => m.DeptId == id);
            if (departmentsModel == null)
            {
                return NotFound();
            }

            return View(departmentsModel);
        }

        // POST: DepartmentsModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.departments == null)
            {
                return Problem("Entity set 'CollegeContext.departments'  is null.");
            }
            var departmentsModel = await _context.departments.FindAsync(id);
            if (departmentsModel != null)
            {
                _context.departments.Remove(departmentsModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentsModelExists(int id)
        {
          return (_context.departments?.Any(e => e.DeptId == id)).GetValueOrDefault();
        }
    }
}
