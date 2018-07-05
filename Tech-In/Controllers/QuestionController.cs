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
            var questionList = _context.UserQuestion.OrderByDescending(x => x.UserQuestionId).ToList();
            if (userPersonalRow == null)
            {
                return RedirectToAction("CompleteProfile", "Home");
            }
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            return View(questionList);
        }

        public async Task<IActionResult> Detail(int id)
        {
            //Check User Profile is complete or not
            var user = await _userManager.GetCurrentUser(HttpContext);
            var userPersonalRow = _context.UserPersonalDetail.Where(a => a.UserId == user.Id).SingleOrDefault();
            if (userPersonalRow != null)
            {
                var QuestionList = _context.UserQuestion.Where(x => x.ApplicationUser.Id == user.Id && x.UserQuestionId == id)
                                           .Select(c => new NewQuestionVM
                                           {
                                               Title = c.Title,
                                               Description = HttpUtility.HtmlDecode(c.Description),
                                               Answers = c.UserQAnswer.Select(x => new QAnswerViewModel
                                               {
                                                   Description = HttpUtility.HtmlDecode(x.Description),
                                                   UserQAnswerId = x.UserQAnswerId,
                                                   Date = x.PostTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                                   User = x.ApplicationUser.UserName
                                               }).ToList(),
                                               Comment = c.UserQAComment.Select(z => new QACommentsViewModel
                                               {
                                                   UserQuestionId = z.UserQuestionId,
                                                   UserQAnswerId = z.UserQAnswerId,
                                                   Description = z.Description,
                                                   IsAnswer = z.IsAnswer,
                                                   Visibility = z.Visibility,
                                                   UserQACommentID = z.UserQACommentID,

                                                   UserId = z.ApplicationUser.Id,
                                               }).ToList(),
                                           }).FirstOrDefault();
                ViewBag.QuestionList = QuestionList;
                return View();
            }
            else
            {
                return RedirectToAction("CompleteProfile", "Home");
            }
            @ViewBag.UName = HttpContext.Session.GetString("Name");
        }

        public async Task<IActionResult> QuestionDetail(int id)
        {
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            //Check User Profile is complete or not
            ViewBag.QuestionId = id;
            var user = await _userManager.GetCurrentUser(HttpContext);
            var userPersonalRow = _context.UserPersonalDetail.Where(a => a.UserId == user.Id).SingleOrDefault();
            if (userPersonalRow == null)
            {
                return RedirectToAction("CompleteProfile", "Home");
            }
            else
            {
                var QuestionList = _context.UserQuestion.Where(x => x.UserQuestionId == id)
                     .Select(c => new NewQuestionVM
                     {
                         Title = c.Title,
                         Description = HttpUtility.HtmlDecode(c.Description),
                         PostedBy= _context.UserPersonalDetail.Where(aa=> aa.UserId== c.UserId).Select(z=> z.FirstName).SingleOrDefault(),


                         Answers = c.UserQAnswer.Select(x => new QAnswerViewModel
                         {
                             Description = HttpUtility.HtmlDecode(x.Description),
                             UserQAnswerId = x.UserQAnswerId,
                             Date = x.PostTime.ToString("yyyy-MM-dd HH:mm:ss"),
                             User = _context.UserPersonalDetail.Where(y => y.UserId == x.ApplicationUser.Id).Select(z => z.FirstName).SingleOrDefault()
                         }).ToList(),
                         Comment = c.UserQAComment.Select(z => new QACommentsViewModel
                         {
                             UserQuestionId = z.UserQuestionId,
                             UserQAnswerId = z.UserQAnswerId,
                             Description = z.Description,
                             IsAnswer = z.IsAnswer,
                             Visibility = z.Visibility,
                             UserQACommentID = z.UserQACommentID,
                             PostedBy= _context.UserPersonalDetail.Where(aa=> aa.UserId == z.UserId).Select(aaa=> aaa.FirstName).SingleOrDefault(),
                             
                             UserId = z.ApplicationUser.Id,
                         }).ToList(),
                         Voting = c.UserQAVoting.Sum(x=> x.Value),
                         
                     }).SingleOrDefault();
                ViewBag.QuestionList = QuestionList;
                return View("Detail");
            }
        }


        public async Task<IActionResult> SearchDetails(string text)
        {
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            text = text.ToLower();
            //Check User Profile is complete or not
            var user = await _userManager.GetCurrentUser(HttpContext);
            var userPersonalRow = _context.UserPersonalDetail.Where(a => a.UserId == user.Id).SingleOrDefault();
            if (userPersonalRow == null)
            {
                return RedirectToAction("CompleteProfile", "Home");
            }
            else
            {
                var QuestionList = _context.UserQuestion.Where(x => x.Description.ToLower().Contains(text) || x.Title.ToLower().Contains(text)).Select(c => new UserQuestion { ApplicationUser = c.ApplicationUser, PostTime = c.PostTime, Tag = c.Tag, UserQuestionId = c.UserQuestionId, UserId = c.UserId, Title = c.Title, Description = HttpUtility.HtmlDecode(c.Description) }).ToList();
                return View("Index", QuestionList);
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
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetCurrentUser(HttpContext);
                //List<SkillTag> list = new List<SkillTag>();

                //foreach (var tag in vm.Tags)
                //{
                //    var _dbTag = _context.SkillTag.FirstOrDefault(c => c.SkillName.ToLower() == tag.SkillName.ToLower());
                //    if (_dbTag == null)
                //    {
                //        _context.SkillTag.Add(new SkillTag()
                //        {
                //            SkillName = tag.SkillName,
                //            ApplicationUser = user,
                //            ApprovedStatus = true,
                //            TimeApproved = DateTime.UtcNow,
                //        });
                //        _context.SaveChanges();
                //        _dbTag = _context.SkillTag.FirstOrDefault(c => c.SkillName.ToLower() == tag.SkillName.ToLower());

                //    }
                //    if(_dbTag != null)
                //        list.Add(_dbTag);
                //}
                UserQuestion userQuestion = new UserQuestion();
                userQuestion.Title = vm.Title;
                userQuestion.PostTime = DateTime.Now;
                userQuestion.Description = HttpUtility.HtmlEncode(vm.Description);
                userQuestion.UserId = user.Id;

                //userQuestion.Tag = list;
                _context.UserQuestion.Add(userQuestion);
                _context.SaveChanges();

                return RedirectToAction($"QuestionDetail", new { id = _context.UserQuestion.OrderByDescending(c => c.UserQuestionId).FirstOrDefault().UserQuestionId });
            }

            return View("New", vm);
        }
        public async Task<IActionResult> ViewQuestion()
        {
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            var user = await _userManager.GetCurrentUser(HttpContext);
            List<NewQuestionVM> QuestionList = _context.UserQuestion.Where(x => x.UserId == user.Id).Select(c => new NewQuestionVM { Title = c.Title, Description = c.Description }).ToList();
            ViewBag.QuestionList = QuestionList;
            return View("Detail");
        }
        [HttpPost]
        public async Task<IActionResult> PostAnswer(CommonViewModel vm)
        {
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetCurrentUser(HttpContext);
                UserQAnswer userQAnswer = new UserQAnswer();
                userQAnswer.Description = HttpUtility.HtmlEncode(vm.QAnswerViewModel.Description);
                userQAnswer.PostTime = DateTime.UtcNow;
                userQAnswer.UserId = user.Id;
                userQAnswer.UserQuestionId = vm.QAnswerViewModel.QuestionId;
                _context.UserQAnswer.Add(userQAnswer);
                _context.SaveChanges();
                return RedirectToAction($"QuestionDetail", new { id = vm.QAnswerViewModel.QuestionId });
            }

            return View("Detail", vm.NewQuestionVM);
        }
        public async Task<IActionResult> UpVote(int id)
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            var question = _context.UserQuestion.FirstOrDefault(c => c.UserQuestionId == id);
            _context.UserQAVoting.Add(new UserQAVoting()
            {
                Value = +1,
                IsAnswer = false,
                UserQuestionId = question.UserQuestionId,
                Visibility = true
            });
            _context.SaveChanges();
            return RedirectToAction($"QuestionDetail", new { id = id });
        }
        public async Task<IActionResult> DownVote(int id)
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            var question = _context.UserQuestion.FirstOrDefault(c => c.UserQuestionId == id);
            _context.UserQAVoting.Add(new UserQAVoting()
            {
                Value = -1,
                IsAnswer = false,
                UserQuestionId = question.UserQuestionId,
                Visibility = true,
                
            });
            _context.SaveChanges();
            return RedirectToAction($"QuestionDetail", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> PostComment(CommonViewModel vm)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetCurrentUser(HttpContext);
                UserQAComment userQAComment = new UserQAComment();
                userQAComment.Description = vm.QACommentsViewModel.Description;
                userQAComment.UserId = user.Id;
                userQAComment.UserQuestionId = vm.QACommentsViewModel.UserQuestionId;
                userQAComment.UserQAnswerId = vm.QACommentsViewModel.UserQAnswerId;
                userQAComment.Visibility = false;
                userQAComment.IsAnswer = false;
                _context.UserQAComment.Add(userQAComment);
                _context.SaveChanges();
                return RedirectToAction($"QuestionDetail", new { id = vm.QACommentsViewModel.UserQuestionId });
            }
            return View("Detail", vm.QAnswerViewModel);
        }

    }
}