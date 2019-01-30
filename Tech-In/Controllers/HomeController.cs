using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tech_In.Data;
using Tech_In.Models;
using Tech_In.Models.Model;
using Tech_In.Models.Database;
using Tech_In.Models.ViewModels.ProfileViewModels;
using Tech_In.Services;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Tech_In.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }


        public async Task<IActionResult> Index()
        {
            IQueryable<UserPostVM> wallPosts = null;
            string currentUserId = await OnGetSesstion();
            if(currentUserId == null)
            {
                return View();
            }
            else
            {
                var userPersonalRow = _context.UserPersonalDetail.Where(a => a.UserId == currentUserId).SingleOrDefault();
                if (userPersonalRow == null)
                {
                    return RedirectToAction("CompleteProfile", "Home");
                }
                wallPosts = (from pst in _context.UserPost
                    join us in _userManager.Users on pst.UserId equals us.Id
                             join u in _context.UserPersonalDetail on pst.UserId equals u.UserId
                             orderby pst.OriginalId descending
                    select new UserPostVM
                    {
                        ProfilePic = u.ProfileImage,
                        Name = u.FirstName + " " + u.LastName,
                        UserName = us.UserName,
                        UserPostId = pst.UserPostId,
                        OriginalId = pst.OriginalId,
                        Status = pst.Status,
                        Summary = pst.Summary,
                        Image = pst.Image,
                        CreateTime = pst.CreateTime,
                        IsLiked = _context.PostLikes.Where(y => y.PostId == pst.UserPostId && y.UserId == currentUserId).Any(),
                        TotalLikes = _context.PostLikes.Where(z => z.PostId == pst.UserPostId).Count()
                    }
                ).Take(10);
            }
            return View("Welcome",wallPosts);
        }


        [Authorize]
        public async Task<IActionResult> CompleteProfile()
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            if(_context.UserPersonalDetail.Where(x=>x.UserId == user.Id).Any())
            {
                return RedirectToAction("Index","Home");
            }
            CompleteProfileVM vm = new CompleteProfileVM();
            ViewBag.CountryList = new SelectList(GetCountryList(), "CountryId", "CountryName");
            return View(vm);
        }
        

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProfile(CompleteProfileVM vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetCurrentUser(HttpContext);
                if (_userManager.Users.ToList().Find(aa => aa.UserName.Equals(vm.UserName.ToLower()))!=null)
                {
                    return BadRequest();
                }
                user.UserName = vm.UserName;
                var file = vm.ProfileImage;
                UserPersonalDetail userPersonal = new UserPersonalDetail();
                if (file != null)
                {
                    if ((file.Length / 1000) > 5000)
                    {
                        ViewBag.ProfilePic = "Image can't exceed 5Mb size";
                        return View();
                    }
                    string path = Path.Combine(_hostingEnvironment.WebRootPath, "images/users");
                    string extension = Path.GetExtension(file.FileName).Substring(1);
                    if (!(extension.ToLower() == "png" || extension.ToLower() == "jpg" || extension.ToLower() == "jpeg"))
                    {
                        ViewBag.ProfilePic = "Only png, jpg & jpeg are allowed";
                        return View();
                    }
                    string fileNam = user.Id.Substring(24) + "p." + extension;
                    using (var vs = new FileStream(Path.Combine(path, fileNam), FileMode.CreateNew))
                    {
                        await file.CopyToAsync(vs);
                    }
                    using (var img = SixLabors.ImageSharp.Image.Load(Path.Combine(path, fileNam)))
                    {
                        userPersonal.ProfileImage = $"/images/users/{fileNam}";
                    }
                }
                else
                {
                    userPersonal.ProfileImage = "/images/user.png";
                }
                user.UserName = vm.UserName;
                userPersonal.CityId = vm.CityId;
                userPersonal.FirstName = vm.FirstName;
                userPersonal.LastName = vm.LastName;
                userPersonal.IsDOBPublic = false;
                userPersonal.DOB = vm.DOB;
                userPersonal.CoverImage = "/images/s.png";
                if (vm.Gender == 0)
                {
                    userPersonal.Gender = Gender.Male;
                }
                else
                {
                    userPersonal.Gender = Gender.Female;
                }
                userPersonal.UserId = user.Id;
                _context.UserPersonalDetail.Add(userPersonal);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("Name", userPersonal.FirstName);

                return RedirectToAction("Index", "User");
            }
            ViewBag.CountryList = new SelectList(GetCountryList(), "CountryId", "CountryName");
            return View("CompleteProfile");
        }

        public List<Country> GetCountryList()
        {
            List<Country> countries = _context.Country.ToList();
            return countries;
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult CountUsers()
        {
            int countUsers = _context.UserPersonalDetail.Count();
            return Json(new { count = countUsers });
        }
        public IActionResult CountQuestions()
        {
            int countQuestions = _context.UserQuestion.Count();
            return Json(new { count = countQuestions });
        }
        public IActionResult CountArticles()
        {
            int countArticles = _context.Article.Count();
            return Json(new { count = countArticles });
        }
        public IActionResult Policies()
        {
            return View();
        }
        public IActionResult Terms()
        {
            return View();
        }
        public IActionResult Faqs()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Developers()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Help()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Security()
        {
            return View();
        }
        public async Task<string> OnGetSesstion()
        {
            const string SessionKeyName = "_Name";
            const string SessionKeyPic = "_PPic";
            const string SessionKeyId = "_Id";
            const string SessionUserName = "_UserName";
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyName)))
            {
                var user = await _userManager.GetCurrentUser(HttpContext);
                if (user == null)
                    return null;
                HttpContext.Session.SetString(SessionKeyName, _context.UserPersonalDetail.Where(x => x.UserId == user.Id).Select(c => c.FirstName).FirstOrDefault());
                HttpContext.Session.SetString(SessionKeyPic, _context.UserPersonalDetail.Where(x => x.UserId == user.Id).Select(c => c.ProfileImage).FirstOrDefault());
                HttpContext.Session.SetString(SessionKeyId, user.Id);
                HttpContext.Session.SetString(SessionUserName, user.UserName);
            }
            @ViewBag.UName = HttpContext.Session.GetString(SessionKeyName);
            @ViewBag.UserName = HttpContext.Session.GetString(SessionUserName);
            @ViewBag.UserPic = HttpContext.Session.GetString(SessionKeyPic);
            return HttpContext.Session.GetString(SessionKeyId);
        }
    }
}
