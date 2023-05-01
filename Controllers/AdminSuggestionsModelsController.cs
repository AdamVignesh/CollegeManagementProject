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
    public class AdminSuggestionsModelsController : Controller
    {
        private readonly CollegeContext _context;

        public AdminSuggestionsModelsController(CollegeContext context)
        {
            _context = context;
        }

        // GET: AdminSuggestionsModels
        public async Task<IActionResult> Index()
        {
              return _context.suggestions != null ? 
                          View(await _context.suggestions.ToListAsync()) :
                          Problem("Entity set 'CollegeContext.suggestions'  is null.");
        }

        // GET: AdminSuggestionsModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.suggestions == null)
            {
                return NotFound();
            }

            var suggestionsModel = await _context.suggestions
                .FirstOrDefaultAsync(m => m.SuggestionId == id);
            if (suggestionsModel == null)
            {
                return NotFound();
            }

            return View(suggestionsModel);
        }

        // GET: AdminSuggestionsModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminSuggestionsModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SuggestionId,SuggestionMessage,Status")] SuggestionsModel suggestionsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(suggestionsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(suggestionsModel);
        }

        // GET: AdminSuggestionsModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.suggestions == null)
            {
                return NotFound();
            }

            var suggestionsModel = await _context.suggestions.FindAsync(id);
            if (suggestionsModel == null)
            {
                return NotFound();
            }
            return View(suggestionsModel);
        }

        // POST: AdminSuggestionsModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SuggestionId,SuggestionMessage,Status")] SuggestionsModel suggestionsModel)
        {
            if (id != suggestionsModel.SuggestionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(suggestionsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SuggestionsModelExists(suggestionsModel.SuggestionId))
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
            return View(suggestionsModel);
        }

        // GET: AdminSuggestionsModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.suggestions == null)
            {
                return NotFound();
            }

            var suggestionsModel = await _context.suggestions
                .FirstOrDefaultAsync(m => m.SuggestionId == id);
            if (suggestionsModel == null)
            {
                return NotFound();
            }

            return View(suggestionsModel);
        }

        // POST: AdminSuggestionsModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.suggestions == null)
            {
                return Problem("Entity set 'CollegeContext.suggestions'  is null.");
            }
            var suggestionsModel = await _context.suggestions.FindAsync(id);
            if (suggestionsModel != null)
            {
                _context.suggestions.Remove(suggestionsModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SuggestionsModelExists(int id)
        {
          return (_context.suggestions?.Any(e => e.SuggestionId == id)).GetValueOrDefault();
        }
    }
}
