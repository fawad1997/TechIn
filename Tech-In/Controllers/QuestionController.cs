using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tech_In.Data;
using Tech_In.Models;
using Tech_In.Models.Database;
using Tech_In.Models.ViewModels.QuestionViewModels;
using Tech_In.Services;

namespace Tech_In.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public QuestionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            //Check User Profile is complete or not
            var user = await _userManager.GetCurrentUser(HttpContext);
            var userPersonalRow = _context.UserPersonalDetail.Where(a => a.UserId == user.Id).SingleOrDefault();
            if (userPersonalRow == null)
            {
                return RedirectToAction("CompleteProfile", "Home");
            }
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            return View();
        }

        public async Task<IActionResult> Detail()
        {
            //Check User Profile is complete or not
            var user = await _userManager.GetCurrentUser(HttpContext);
            var userPersonalRow = _context.UserPersonalDetail.Where(a => a.UserId == user.Id).SingleOrDefault();
            if (userPersonalRow == null)
            {
                return RedirectToAction("CompleteProfile", "Home");
            }
            else
            {
                var QuestionList = _context.UserQuestion.Where(x => x.ApplicationUser.Id == user.Id).OrderByDescending(x=> x.UserQuestionId).Select(c => new NewQuestionVM { Title = c.Title, Description = HttpUtility.HtmlDecode(c.Description) }).FirstOrDefault();
                ViewBag.QuestionList = QuestionList;
                return View();
            }
        }

        public async Task<IActionResult> New()
        {
            //Check User Profile is complete or not
            var user = await _userManager.GetCurrentUser(HttpContext);
            var userPersonalRow = _context.UserPersonalDetail.Where(a => a.UserId == user.Id).SingleOrDefault();
            if (userPersonalRow == null)
            {
                return RedirectToAction("CompleteProfile", "Home");
            }
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            return View();
            
        }

        public IActionResult Xyz()
        {
            return View();
        }
        [HttpPost]
         public async Task<IActionResult> PostQuestion(NewQuestionVM vm)
        {
            
            if (ModelState.IsValid)
            {
                var user =  await _userManager.GetCurrentUser(HttpContext);
                UserQuestion userQuestion = new UserQuestion();
                userQuestion.Title = vm.Title;
                userQuestion.PostTime = DateTime.Now;
                userQuestion.Description = HttpUtility.HtmlEncode(vm.Description);
                userQuestion.UserId = user.Id;
                _context.UserQuestion.Add(userQuestion);
                _context.SaveChanges();
                return RedirectToAction("Detail");
            }

            return View("New", vm);
        }
        public async Task<IActionResult> ViewQuestion()
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            List<NewQuestionVM> QuestionList = _context.UserQuestion.Where(x => x.UserId == user.Id).Select(c => new NewQuestionVM { Title = c.Title, Description=c.Description}).ToList();
            ViewBag.QuestionList = QuestionList;
            return View("Detail");
        }
    }
}