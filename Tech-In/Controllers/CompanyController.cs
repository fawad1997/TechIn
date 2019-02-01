using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using Tech_In.Data;
using Tech_In.Models;
using Tech_In.Models.Database;
using Tech_In.Models.ViewModels.ArticleViewModels;
using Tech_In.Services;

namespace Tech_In.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _environment;

        public CompanyController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;

        }

        // GET: Company
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Company.Include(c => c.City);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Company/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .Include(c => c.City)
                .Include(c => c.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Company/Create
        public IActionResult Create()
        {
            ViewData["Location"] = new SelectList(_context.City, "CityId", "CityName");
            return View();
        }

        // POST: Company/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Company co)
        {
            if (co == null)
            {
                throw new ArgumentNullException(nameof(co));
            }

            var user = await _userManager.GetCurrentUser(HttpContext);
            Company company = new Company
            {
                UserId = user.Id,
                Title = co.Title,
                About = co.About,
                WebSite = co.WebSite,
                FoundedDate = co.FoundedDate,
                Industry = co.Industry,
                Type = co.Type,
                Size = co.Size,
                Location = co.Location,
                Logo = co.Logo,
                Speciality = co.Speciality
            };

            if (ModelState.IsValid)
            {
                company.UserId = user.Id;
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Location"] = new SelectList(_context.City, "CityId", "CityName", company.Location);
            return View(company);
        }

        public async Task<IActionResult> CheckImg(FileUploadVM vm)
        {
            var file = vm.File;
            if (file.Length > 0)
            {
                string errorMsg = null;

                String[] orgfileName = file.FileName.Split('.');
                if (orgfileName[1].ToLower() != "jpg" && orgfileName[1].ToLower() != "jpeg" && orgfileName[1].ToLower() != "png")
                {
                    errorMsg = "Only .jpg, .jpeg & .png are allowed";
                    return Json(new { success = false, response = errorMsg });
                }

                string path = Path.Combine(_environment.WebRootPath, "images/article");
                var filename = orgfileName[0] + DateTime.Now.ToString("ddMMyyhhmmsstt") + "." + orgfileName[1];
                using (var fs = new FileStream(Path.Combine(path, filename), FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }
                using (var img = Image.Load(Path.Combine(path, filename)))
                {
                    vm.Source = $"/images/article/{filename}";
                    vm.Extension = Path.GetExtension(filename).Substring(1);
                    vm.Width = img.Width;
                    vm.Height = img.Height;
                    vm.Size = file.Length / 1000;
                    if (img.Height == 400)
                    {
                        if (img.Width == 400)
                        {
                            if (vm.Size < 2000)
                            {
                                return Json(new { success = true, response = vm });
                            }
                            else
                                errorMsg = "Image should be less than 2 MB";
                        }
                        else
                            errorMsg = "Image Width should be equal to 400px";
                    }
                    else
                        errorMsg = "Image height should be equal to 400px";

                    if (errorMsg != null)
                    {
                        string fullPath = path + "/" + filename;
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        return Json(new { success = false, response = errorMsg });
                    }
                }
            }
            return BadRequest();
        }

        // GET: Company/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company.SingleOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }
            ViewData["Location"] = new SelectList(_context.City, "CityId", "CityId", company.Location);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", company.UserId);
            return View(company);
        }

        // POST: Company/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,About,WebSite,FoundedDate,Type,Location,Logo,UserId")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            ViewData["Location"] = new SelectList(_context.City, "CityId", "CityId", company.Location);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", company.UserId);
            return View(company);
        }

        // GET: Company/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .Include(c => c.City)
                .Include(c => c.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Company.SingleOrDefaultAsync(m => m.Id == id);
            _context.Company.Remove(company);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
            return _context.Company.Any(e => e.Id == id);
        }
    }
}
