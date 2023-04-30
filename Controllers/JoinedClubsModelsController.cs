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
    public class JoinedClubsModelsController : Controller
    {
        private readonly CollegeContext _context;
        private readonly UserManager<AspNetUsers> _userManager;
      
        public JoinedClubsModelsController(CollegeContext context, UserManager<AspNetUsers> user)
        {
            _context = context;
            _userManager = user;

        }

        // GET: JoinedClubsModels
        public async Task<IActionResult> Index()
        {
              return _context.joinedClubs != null ? 
                          View(await _context.joinedClubs.ToListAsync()) :
                          Problem("Entity set 'CollegeContext.joinedClubs'  is null.");
        }


        //to add a student to his/her club
        public async Task<IActionResult> JoinClub(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser = await _userManager.FindByIdAsync(userId);
            JoinedClubsModel joinedClub = new JoinedClubsModel();
            if (CurrUser != null)
            {
                ClubsModel club = _context.clubs.Where(club => club.ClubId == id).FirstOrDefault();
                StudentsModel student = _context.students.Where(student => student.user_id.Id == CurrUser.Id).FirstOrDefault();
                joinedClub.club_id = club;
                joinedClub.reg_no = student;
                _context.joinedClubs.Add(joinedClub);
                _context.SaveChanges();
            }
            List<JoinedClubsModel> clubsJoined = _context.joinedClubs.Where(student => student.reg_no.user_id.Id == CurrUser.Id).ToList();
            foreach (var i in clubsJoined)
            {
                Console.WriteLine("clubs " + i.JoinedClubId);
                Console.WriteLine("student " + i.reg_no.Name);

            }
            ViewBag.joinedClubs = clubsJoined;
            //var joinedClub = _context.joinedClubs
            return View();
        }


        // GET: JoinedClubsModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.joinedClubs == null)
            {
                return NotFound();
            }

            var joinedClubsModel = await _context.joinedClubs
                .FirstOrDefaultAsync(m => m.JoinedClubId == id);
            if (joinedClubsModel == null)
            {
                return NotFound();
            }

            return View(joinedClubsModel);
        }

        // GET: JoinedClubsModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JoinedClubsModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JoinedClubId,CludHead")] JoinedClubsModel joinedClubsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(joinedClubsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(joinedClubsModel);
        }

        // GET: JoinedClubsModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.joinedClubs == null)
            {
                return NotFound();
            }

            var joinedClubsModel = await _context.joinedClubs.FindAsync(id);
            if (joinedClubsModel == null)
            {
                return NotFound();
            }
            return View(joinedClubsModel);
        }

        // POST: JoinedClubsModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JoinedClubId,CludHead")] JoinedClubsModel joinedClubsModel)
        {
            if (id != joinedClubsModel.JoinedClubId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(joinedClubsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JoinedClubsModelExists(joinedClubsModel.JoinedClubId))
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
            return View(joinedClubsModel);
        }

        // GET: JoinedClubsModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.joinedClubs == null)
            {
                return NotFound();
            }

            var joinedClubsModel = await _context.joinedClubs
                .FirstOrDefaultAsync(m => m.JoinedClubId == id);
            if (joinedClubsModel == null)
            {
                return NotFound();
            }

            return View(joinedClubsModel);
        }

        // POST: JoinedClubsModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.joinedClubs == null)
            {
                return Problem("Entity set 'CollegeContext.joinedClubs'  is null.");
            }
            var joinedClubsModel = await _context.joinedClubs.FindAsync(id);
            if (joinedClubsModel != null)
            {
                _context.joinedClubs.Remove(joinedClubsModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JoinedClubsModelExists(int id)
        {
          return (_context.joinedClubs?.Any(e => e.JoinedClubId == id)).GetValueOrDefault();
        }
    }
}
