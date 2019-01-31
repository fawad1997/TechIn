using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tech_In.Data;
using Tech_In.Models.Database;

namespace Tech_In.Controllers
{
    public class JobController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Job
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Job.Include(j => j.City).Include(j => j.Company);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Job/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Job/Create
        public IActionResult Create()
        {
            ViewData["Location"] = new SelectList(_context.City, "CityId", "CityId");
            ViewData["PostedBy"] = new SelectList(_context.Company, "Id", "About");
            return View();
        }

        // POST: Job/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,vacancies,Description,MinSalary,MaxSalary,Status,MinExpereince,MaxExpereince,Qualification,PostDate,JobType,JobShift,EndDate,Location,PostedBy")] Job job)
        {
            if (ModelState.IsValid)
            {
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Location"] = new SelectList(_context.City, "CityId", "CityId", job.Location);
            ViewData["PostedBy"] = new SelectList(_context.Company, "Id", "About", job.PostedBy);
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
            ViewData["Location"] = new SelectList(_context.City, "CityId", "CityId", job.Location);
            ViewData["PostedBy"] = new SelectList(_context.Company, "Id", "About", job.PostedBy);
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
            ViewData["Location"] = new SelectList(_context.City, "CityId", "CityId", job.Location);
            ViewData["PostedBy"] = new SelectList(_context.Company, "Id", "About", job.PostedBy);
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
    }
}
