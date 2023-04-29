using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using College.Areas.Identity.Data;
using College.Data;
using System.Security.Claims;
using System.Diagnostics;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Identity;


namespace College.Controllers
{
    public class StudentsModelsController : Controller
    {
        private readonly CollegeContext _context;
        private readonly UserManager<AspNetUsers> _userManager;
        public StudentsModelsController(CollegeContext context, UserManager<AspNetUsers> user)
        {
            _context = context;
            _userManager = user;
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
        public async Task<IActionResult> Create()
        { 
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<StudentsModel> s = _context.students.Include(r=>r.department_id).Where(s=>s.user_id.Id == userId).ToList();

            var CurrUser =  await _userManager.FindByIdAsync(userId);
            StudentsModel student = _context.students.FirstOrDefault(user=>user.user_id == CurrUser);
            if(student == null)
            {
                return View("Error");
            }

            if (s.Count==0)
            {
                var departments = _context.departments.Select(t => new DepartmentsModel
                {
                    DeptId = t.DeptId,
                    DeptName = t.DeptName,
                }).ToList();
                ViewBag.DepartmentNames = departments;
                foreach(DepartmentsModel d in departments)
                {
                    Console.WriteLine("deptId "+d.DeptId);
                }
                return View();
            }
            else
            {
                foreach(var si in s)
                {
                    Console.WriteLine("id "+si.department_id.DeptName);

                }
                //DepartmentsModel departmentValues = _context.departments.Find(s[0].department_id);
                //DepartmentsModel departmentValues = _context.departments.Find();

                return View("Index", s);
                
            }
            return View();
        }

        // POST: StudentsModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file,int department, [Bind("RegNo,Name,SchoolName,Percentage12th,Percentage10th,City,isFeePaid,YearOfStudy,Attendance,CGPA,Email,ImageURL")] StudentsModel studentsModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            DepartmentsModel departmentValues = _context.departments.Find(department);
            //DepartmentsModel departmentValues = _context.students.Include(s=>s.department_id).Where(id => id.department_id.DeptId == department).FirstOrDefault();
            var user = _userManager.GetUserId(this.User);
            Console.WriteLine("userid: " + userId);
            var CurrUser = await _userManager.FindByIdAsync(userId);

           // Console.WriteLine(department);
            //AWS configs

            IConfiguration config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

            string accessKey = config["AccessKey"];
            string secretKey = config["SecretKey"];
            string bucketName = config["BucketName"];

            /*string filePath = Path.GetFullPath(file.FileName);
            Console.WriteLine("path "+filePath);*/
            Console.WriteLine("nweAcc " + accessKey);
            string filePath = $"C:\\Users\\HP\\source\\College\\College\\images\\{file.FileName}";

            using (var client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.EUNorth1))
            {
                using (var transferUtility = new TransferUtility(client))
                {
                    var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        FilePath = filePath,
                        Key = $"images/{file.FileName}",
                        CannedACL = S3CannedACL.PublicRead
                    };

                    transferUtility.Upload(fileTransferUtilityRequest);
                }
                var url = client.GetPreSignedURL(new Amazon.S3.Model.GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = $"images/{file.FileName}",
                    Expires = DateTime.Now.AddDays(5) // Set the expiration date of the URL
                });

                // Print out the URL of the uploaded file
                url = url.Substring(0, url.IndexOf("?"));
               // Console.WriteLine("Uploaded file URL: " + url);
                ViewData["imgsrc"] = url;
                studentsModel.ImageURL = url;
                studentsModel.user_id = CurrUser;
                studentsModel.department_id= departmentValues;
            }
            Console.WriteLine("Image uploaded successfully!");

            _context.Add(studentsModel);
                await _context.SaveChangesAsync();
            string CurrUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<StudentsModel> studentValues = _context.students.Where(s => s.user_id.Id == CurrUserId).ToList();
            foreach(StudentsModel s in studentValues)
            {
                Console.WriteLine(s.Name +" -------------"+s.department_id.DeptName);
            }
            return View("Index", studentValues);
          //  return View(studentsModel);
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
        public async Task<IActionResult> Edit(int id ,bool feeStatus,[Bind("RegNo,Name,SchoolName,Percentage12th,Percentage10th,City,isFeePaid,YearOfStudy,Attendance,CGPA,Email,ImageURL")] StudentsModel studentsModel)
        {
            //Console.WriteLine(feeStatus);

            if (id != studentsModel.RegNo)
            {
                return NotFound();
            }
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
            studentsModel.isFeePaid = true;
                return RedirectToAction("ViewStudents", "Admin");
            
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
