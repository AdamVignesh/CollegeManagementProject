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
    public class AdminClubsModelsController : Controller
    {
        private readonly CollegeContext _context;

        public AdminClubsModelsController(CollegeContext context)
        {
            _context = context;
        }

        // GET: AdminClubsModels
        public async Task<IActionResult> Index()
        {
              return _context.clubs != null ? 
                          View(await _context.clubs.ToListAsync()) :
                          Problem("Entity set 'CollegeContext.clubs'  is null.");
        }

        // GET: AdminClubsModels/Details/5
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

        // GET: AdminClubsModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminClubsModels/Create
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

        // GET: AdminClubsModels/Edit/5
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

        // POST: AdminClubsModels/Edit/5
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

        // GET: AdminClubsModels/Delete/5
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

        // POST: AdminClubsModels/Delete/5
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
