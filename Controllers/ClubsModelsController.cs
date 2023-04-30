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
            var user = _userManager.GetUserId(this.User);
            if(user==null)
            {
                return View("Error");
            }
            if (user.Equals("13236b44-f440-4b18-97b4-c33788227fd4"))
            {
                return View("Error");
            }
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
        public async Task<IActionResult> Create([Bind("ClubId,ClubName,ClubDescription,ClubImageURL")] ClubsModel clubsModel)
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
        public async Task<IActionResult> Edit(int id, [Bind("ClubId,ClubName,ClubDescription,ClubImageURL")] ClubsModel clubsModel)
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int id)
        {
            Console.WriteLine("In delete Post--------- "+id);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser = await _userManager.FindByIdAsync(userId);
            JoinedClubsModel joinedClub = new JoinedClubsModel();
            if (CurrUser != null)
            {
                var ClubsJoinedByCurrUser = _context.joinedClubs.Include(clubs => clubs.club_id).Include(student => student.reg_no).Where(s => s.reg_no.user_id.Id == CurrUser.Id).ToList();
                foreach(var item in ClubsJoinedByCurrUser)
                {
                    if(item.club_id.ClubId == id)
                    {
                        return View("AlreadyAMember");
                    }
                }

                ClubsModel club = _context.clubs.Where(club => club.ClubId == id).FirstOrDefault();
                StudentsModel student = _context.students.Where(student => student.user_id.Id == CurrUser.Id).FirstOrDefault();
                joinedClub.club_id = club;
                joinedClub.reg_no = student;

                _context.joinedClubs.Add(joinedClub);
                _context.SaveChanges();
            }

            return RedirectToAction("MyClubs");
        }

        public async Task<IActionResult> AlreadyAMember()
        {
            return View();
        }
        public async Task<IActionResult> MyClubs()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrUser = await _userManager.FindByIdAsync(userId);

            var ClubsJoinedByCurrUser = _context.joinedClubs.Include(clubs=>clubs.club_id).Include(student=>student.reg_no).Where(s => s.reg_no.user_id.Id == CurrUser.Id).ToList();
            return View(ClubsJoinedByCurrUser);
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
