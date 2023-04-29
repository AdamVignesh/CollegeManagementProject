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
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;

namespace College.Controllers
{
    public class ClubsModelsController : Controller
    {
        private readonly CollegeContext _context;
        private readonly UserManager<AspNetUsers> _userManager;

        public ClubsModelsController(CollegeContext context, UserManager<AspNetUsers> user)
        {
            _context = context;
            _userManager = user;
        }

        // GET: ClubsModels
        public async Task<IActionResult> Index()
        {
              return _context.clubs != null ? 
                          View(await _context.clubs.ToListAsync()) :
                          Problem("Entity set 'CollegeContext.clubs'  is null.");
        }

        // GET: ClubsModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.clubs == null)
            {
                return NotFound();
            }

            var clubsModel = await _context.clubs
                .FirstOrDefaultAsync(m => m.ClubId == id);
            if (clubsModel == null)
            {
                return NotFound();
            }

            return View(clubsModel);
        }

        public async Task<IActionResult> Club(int ClubId)
        {
            return View();
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Club(int ClubId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser = await _userManager.FindByIdAsync(userId);
            StudentsModel student = _context.students.FirstOrDefault(user => user.user_id == CurrUser);
            Console.WriteLine("CLubId " + ClubId + "studentID " + student.RegNo);
            return View("Index");
        }
        // GET: ClubsModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClubsModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClubId,ClubName,ClubDescription")] ClubsModel clubsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clubsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clubsModel);
        }

        // GET: ClubsModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.clubs == null)
            {
                return NotFound();
            }

            var clubsModel = await _context.clubs.FindAsync(id);
            if (clubsModel == null)
            {
                return NotFound();
            }
            return View(clubsModel);
        }

        // POST: ClubsModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClubId,ClubName,ClubDescription")] ClubsModel clubsModel)
        {
            if (id != clubsModel.ClubId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clubsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClubsModelExists(clubsModel.ClubId))
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
            return View(clubsModel);
        }

        // GET: ClubsModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.clubs == null)
            {
                return NotFound();
            }

            var clubsModel = await _context.clubs
                .FirstOrDefaultAsync(m => m.ClubId == id);
            if (clubsModel == null)
            {
                return NotFound();
            }

            return View(clubsModel);
        }

        // POST: ClubsModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.clubs == null)
            {
                return Problem("Entity set 'CollegeContext.clubs'  is null.");
            }
            var clubsModel = await _context.clubs.FindAsync(id);
            if (clubsModel != null)
            {
                _context.clubs.Remove(clubsModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClubsModelExists(int id)
        {
          return (_context.clubs?.Any(e => e.ClubId == id)).GetValueOrDefault();
        }
    }
}
