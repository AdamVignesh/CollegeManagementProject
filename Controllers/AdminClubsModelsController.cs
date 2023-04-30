using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using College.Areas.Identity.Data;
using College.Data;
using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Amazon;

namespace College.Controllers
{
    public class AdminClubsModelsController : Controller
    {
        private readonly CollegeContext _context;
        private readonly UserManager<AspNetUsers> _userManager;
      

        public AdminClubsModelsController(CollegeContext context, UserManager<AspNetUsers> user)
        {
            _context = context;
            _userManager = user;

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
        public async Task<IActionResult> Create(IFormFile file,[Bind("ClubId,ClubName,ClubDescription,ClubImageURL")] ClubsModel clubsModel)
        {
            //Console.WriteLine("file "+file.FileName);

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.GetUserId(this.User);
            //Console.WriteLine("userid: " + userId);
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
                clubsModel.ClubImageURL = url;


            }
            Console.WriteLine("Image uploaded successfully!");
            _context.Add(clubsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
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
            ViewBag.imgSrc = clubsModel.ClubImageURL;
            return View(clubsModel);
        }

        //get
     

            // POST: AdminClubsModels/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile file,int id, [Bind("ClubId,ClubName,ClubDescription,ClubImageURL")] ClubsModel clubsModel)
        {
            
            if (id != clubsModel.ClubId)
            {
                return NotFound();
            }
            Console.WriteLine("File: " + file);
             try
                {
                if (file != null)
                {
                    Console.WriteLine("inside if da deiiiiiiiiiiiiiiiiiiiiiii");

                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    //DepartmentsModel departmentValues = _context.departments.Find(department);
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
                        clubsModel.ClubImageURL = url;
                    }
                    
                }
                _context.Update(clubsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
                //return View("Index");
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
                return View("Index");

                //return RedirectToAction(nameof(Index));
            
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
