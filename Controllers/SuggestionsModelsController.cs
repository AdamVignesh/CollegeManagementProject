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

namespace College.Controllers
{
    public class SuggestionsModelsController : Controller
    {
        private readonly CollegeContext _context;
        private readonly UserManager<AspNetUsers> _userManager;
        
        public SuggestionsModelsController(CollegeContext context, UserManager<AspNetUsers> user)
        {
            _context = context;
            _userManager = user;

        }
         
        

        // GET: SuggestionsModels
        public async Task<IActionResult> Index()
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser = await _userManager.FindByIdAsync(userId);

            var CurrUserSuggestion = _context.suggestions.Include(student => student.Students).Where(User => User.Students.user_id.Id == CurrUser.Id && (User.Status == "Processing" ||User.Status == "Acting on it")).ToList() ;

            if(CurrUserSuggestion.Count == 0)
            {
                return RedirectToAction("Create");
            }
            return View(CurrUserSuggestion);

        }

        public async Task<IActionResult>ResolvedSuggestions()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser = await _userManager.FindByIdAsync(userId);

            var CurrUserSuggestion = _context.suggestions.Include(student => student.Students).Where(User => User.Students.user_id.Id == CurrUser.Id && User.Status == "Resolved").ToList();

            if (CurrUserSuggestion.Count == 0)
            {
                return RedirectToAction("Create");
            }
            return View(CurrUserSuggestion);
        }

        // GET: SuggestionsModels/Details/5
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

        // GET: SuggestionsModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SuggestionsModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SuggestionId,SuggestionMessage")] SuggestionsModel suggestionsModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser = await _userManager.FindByIdAsync(userId);
            StudentsModel student = _context.students.FirstOrDefault(u => u.user_id == CurrUser);
            suggestionsModel.Status = "Processing";
            suggestionsModel.Students = student;
            _context.Add(suggestionsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            return View(suggestionsModel);
        }

        // GET: SuggestionsModels/Edit/5
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

        // POST: SuggestionsModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SuggestionId,SuggestionMessage")] SuggestionsModel suggestionsModel)
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

        // GET: SuggestionsModels/Delete/5
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

        // POST: SuggestionsModels/Delete/5
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
