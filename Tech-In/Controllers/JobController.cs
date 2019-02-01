using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tech_In.Data;
using Tech_In.Models;
using Tech_In.Models.Database;
using Tech_In.Services;

namespace Tech_In.Controllers
{
    public class JobController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Job
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Job.Include(j => j.City).Include(j => j.Company);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Job/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.Job
                .Include(j => j.City)
                .Include(j => j.Company)
                .SingleOrDefaultAsync(m => m.Id == id);
            var isSaved = _context.SavedJob.Where(x => x.Id.Equals(ToGuid(id))).Any();
            ViewBag.IsSaved = isSaved;
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        public static Guid ToGuid(int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }
        // GET: Job/Create
        public IActionResult Create()
        {
            ViewData["Location"] = new SelectList(_context.City, "CityId", "CityName");
            ViewData["PostedBy"] = new SelectList(_context.Company, "Id", "Title");
            return View();
        }

        // POST: Job/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,vacancies,Description,MinSalary,MaxSalary,Status,MinExpereince,MaxExpereince,Qualification,JobType,JobShift,EndDate,Location,PostedBy")] Job job)
        {
            //var user = await _userManager.GetCurrentUser(HttpContext);
            job.PostDate = DateTime.Now;
            job.Status = JobStatus.Active;
            if (ModelState.IsValid)
            {
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Location"] = new SelectList(_context.City, "CityId", "CityName", job.Location);
            ViewData["PostedBy"] = new SelectList(_context.Company, "Id", "Title", job.PostedBy);
            return View(job);
        }

        // GET: Job/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job.SingleOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }
            ViewData["Location"] = new SelectList(_context.City, "CityId", "CityName", job.Location);
            ViewData["PostedBy"] = new SelectList(_context.Company, "Id", "Title", job.PostedBy);
            return View(job);
        }

        // POST: Job/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,vacancies,Description,MinSalary,MaxSalary,Status,MinExpereince,MaxExpereince,Qualification,PostDate,JobType,JobShift,EndDate,Location,PostedBy")] Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
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
            ViewData["Location"] = new SelectList(_context.City, "CityId", "CityName", job.Location);
            ViewData["PostedBy"] = new SelectList(_context.Company, "Id", "Title", job.PostedBy);
            return View(job);
        }

        // GET: Job/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job
                .Include(j => j.City)
                .Include(j => j.Company)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Job/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = await _context.Job.SingleOrDefaultAsync(m => m.Id == id);
            _context.Job.Remove(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(int id)
        {
            return _context.Job.Any(e => e.Id == id);
        }

        [Authorize]
        public async Task<IActionResult> ApplyForJob(int id)
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            Applicant applicant = new Applicant
            {
                JobId = id,
                AppliedBy = user.Id
            };
            _context.Applicant.Add(applicant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> SaveJob(int id)
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            SavedJob saved = new SavedJob
            {
                Job = id,
                User = user.Id
            };
            _context.SavedJob.Add(saved);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize]
        public async Task<IActionResult> UnSaveJob(int id)
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            var x = _context.SavedJob.Where(a => a.Job == id && a.User == user.Id).FirstOrDefault();
            _context.SavedJob.Remove(x);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> GetUserJobsAsync(int id)
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            ViewBag.jobs = _context.Job.Where(m => m.PostedBy == id).ToList();
            return View();
        }
    }
}
