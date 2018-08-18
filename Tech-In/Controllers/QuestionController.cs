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
using Microsoft.EntityFrameworkCore;
using Tech_In.Extensions;

namespace Tech_In.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private IHttpContextAccessor _accessor;
        public QuestionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor accessor)
        {
            _context = context;
            _userManager = userManager;
            _accessor = accessor;
        }
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            //Check User Profile is complete or not 
            var user = await _userManager.GetCurrentUser(HttpContext);
            var userPersonalRow = _context.UserPersonalDetail.Where(a => a.UserId == user.Id).SingleOrDefault();
            var questions = from q in _context.UserQuestion.OrderByDescending(x => x.UserQuestionId) select q;
            var Question = (from o in _context.SkillTag
                            join c in _context.QuestionSkill on o.SkillTagId equals c.SkillTagId
                            join p in _context.UserQuestion on c.UserQuestionId equals p.UserQuestionId
                            where o.SkillName == c.SkillTag.SkillName
                            select new
                            {
                                c.SkillTag.SkillName
                            });
            
            //var asd= (from w in _context.SkillTag 
            //         join d in _context.QuestionSkill on w.SkillTagId equals d.SkillTagId 
            //         join n in _context.UserQuestion on d.UserQuestionId equals n.UserQuestionId
            //         select w.SkillName).ToList();
           
            if (!String.IsNullOrEmpty(searchString))
            {
                questions = questions.Where(s => s.Description.Contains(searchString)
                                || s.Title.Contains(searchString));

            }
            if (userPersonalRow == null)
            {
                return RedirectToAction("CompleteProfile", "Home");
            }
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            int pageSize = 4;
            return View(await PaginatedList<UserQuestion>.CreateAsync(questions.AsNoTracking(), page ?? 1, pageSize));


            //return View(questionList);
        }

        public async Task<IActionResult> QuestionDetail(int id)
        {
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            ViewBag.QuestionId = id;
            var user = await _userManager.GetCurrentUser(HttpContext);
            var QuestionList = _context.UserQuestion.Where(x => x.UserQuestionId == id)
                     .Select(c => new NewQuestionVM
                     {
                         Title = c.Title,
                         Description = HttpUtility.HtmlDecode(c.Description),
                         //Tags=_context.QuestionSkill.Where(qs=> qs.QuestionSkillId==id).Select(d=> new QuestionTagViewModel { SkillName = d.SkillTag.SkillName }).ToList(),
                         PostedBy = _context.UserPersonalDetail.Where(aa => aa.UserId == c.UserId).Select(z => z.FirstName).SingleOrDefault(),
                         UserPic = _context.UserPersonalDetail.Where(aa => aa.UserId == c.UserId).Select(z => z.ProfileImage).SingleOrDefault(),
                         PostTime = c.PostTime,
                         Visitors = _context.QuestionVisitor.Where(cont=> cont.QuestionId == id).Count(),
                         HasVerifiedAns = c.HasVerifiedAns,
                         Tags = c.Tag.Select(t => new QuestionTagViewModel
                         {
                             SkillName = t.SkillTag.SkillName
                         }).ToList(),
                         Answers = c.UserQAnswer.Select(x => new QAnswerViewModel
                         {
                             Description = HttpUtility.HtmlDecode(x.Description),
                             UserQAnswerId = x.UserQAnswerId,
                             Date = x.PostTime,
                             IsVerified = x.IsVerified,
                             Votes = _context.UserQAVoting.Where(y => y.UserAnswerId == x.UserQAnswerId).Sum(z => z.Value),
                             User = _context.UserPersonalDetail.Where(y => y.UserId == x.ApplicationUser.Id).Select(z => z.FirstName).SingleOrDefault()
                         }).OrderByDescending(ord=>ord.IsVerified).ThenBy(ordasc =>ordasc.UserQAnswerId).ToList(),
                         Comment = c.UserQAComment.Select(z => new QACommentsViewModel
                         {
                             UserQuestionId = z.UserQuestionId,
                             UserQAnswerId = z.UserQAnswerId,
                             Description = z.Description,
                             IsAnswer = z.IsAnswer,
                             Visibility = z.Visibility,
                             UserQACommentID = z.UserQACommentID,
                             PostedBy = _context.UserPersonalDetail.Where(aa => aa.UserId == z.UserId).Select(aaa => aaa.FirstName).SingleOrDefault(),

                             UserId = z.ApplicationUser.Id,
                         }).ToList(),
                         Voting = c.UserQAVoting.Sum(x => x.Value)

                     }).SingleOrDefault();
            if (QuestionList != null)
            {
                if (user != null)
                {
                    var userPersonalRow = _context.UserPersonalDetail.Where(a => a.UserId == user.Id).SingleOrDefault();
                    if (userPersonalRow == null)
                    {
                        return RedirectToAction("CompleteProfile", "Home");
                    }
                    //If Registered Visitor Counter
                    QuestionVisitor isVisited = _context.QuestionVisitor.Where(qv => qv.UserId == user.Id && qv.QuestionId == id).SingleOrDefault();
                    if (isVisited == null)
                    {
                        _context.QuestionVisitor.Add(new QuestionVisitor
                        {
                            QuestionId = id,
                            UserId = user.Id,
                            IsLoggedIn = true,
                            UserIp = null
                        });
                        _context.SaveChanges();
                    }
                    if (!QuestionList.HasVerifiedAns)
                    {
                        var isUserPostedQuestion = _context.UserQuestion.Where(a => a.UserId == user.Id && a.UserQuestionId == id).SingleOrDefault();
                        if (isUserPostedQuestion != null)
                        {
                            ViewBag.ShowTick = true;
                        }
                        else
                        {
                            ViewBag.ShowTick = false;
                        }
                    }else
                        ViewBag.ShowTick = false;
                }
                else
                {//If Anonomus Visitor Counter
                    string currentUserIp = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    QuestionVisitor isVisited = _context.QuestionVisitor.Where(qv => qv.UserIp == currentUserIp && qv.QuestionId == id).SingleOrDefault();
                    if(isVisited == null)
                    {
                        _context.QuestionVisitor.Add(new QuestionVisitor
                        {
                            QuestionId = id,
                            UserId = null,
                            IsLoggedIn = false,
                            UserIp = currentUserIp
                        });
                        _context.SaveChanges();
                    }
                }
                ViewBag.QuestionList = QuestionList;
                if (TempData["Msg"] != null)
                {
                    ViewBag.VoteMsg = TempData["Msg"];
                    ViewBag.Color = TempData["Color"];
                }
                return View("Detail");
            }
            else
                return NotFound();
        }


        public async Task<IActionResult> SearchDetails(string text)
        {
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            text = text.ToLower();
            //Check User Profile is complete or not
            var user = await _userManager.GetCurrentUser(HttpContext);
            var userPersonalRow = _context.UserPersonalDetail.Where(a => a.UserId == user.Id).SingleOrDefault();
            if (userPersonalRow != null)
            {
                var QuestionList = _context.UserQuestion.Where(x => x.Description.ToLower().Contains(text) || x.Title.ToLower().Contains(text)).Select(c => new UserQuestion { ApplicationUser = c.ApplicationUser, PostTime = c.PostTime, Tag = c.Tag, UserQuestionId = c.UserQuestionId, UserId = c.UserId, Title = c.Title, Description = HttpUtility.HtmlDecode(c.Description) }).ToList();
                return View("Index", QuestionList);
                
            }
            else
            {
                return RedirectToAction("CompleteProfile", "Home");
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

        
        [HttpPost]
        public async Task<IActionResult> PostQuestion(NewQuestionVM vm,string tags)
        {
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetCurrentUser(HttpContext); 
                UserQuestion userQuestion = new UserQuestion();
                userQuestion.Title = vm.Title;
                userQuestion.PostTime = DateTime.Now;
                userQuestion.Description = HttpUtility.HtmlEncode(vm.Description);
                userQuestion.UserId = user.Id;
                _context.UserQuestion.Add(userQuestion);
                _context.SaveChanges();
                string[] tagArray = tags.Split(',');
                foreach (string tag in tagArray)
                {
                    var tagFromDB = _context.SkillTag.Where(x => x.SkillName == tag.ToLower()).SingleOrDefault();
                    if (tagFromDB == null)
                    {
                        SkillTag sktag = new SkillTag
                        {
                            ApprovedStatus = false,
                            SkillName = tag.ToLower(),
                            TimeApproved = DateTime.Now,
                            UserId = user.Id
                        };
                        _context.SkillTag.Add(sktag);
                        _context.SaveChanges();
                        QuestionSkill qs = new QuestionSkill
                        {
                            SkillTagId = sktag.SkillTagId,
                            UserQuestionId = userQuestion.UserQuestionId
                        };
                        _context.QuestionSkill.Add(qs);
                        _context.SaveChanges();
                    }
                    else
                    {
                        QuestionSkill qs = new QuestionSkill
                        {
                            SkillTagId = tagFromDB.SkillTagId,
                            UserQuestionId = userQuestion.UserQuestionId
                        };
                        _context.QuestionSkill.Add(qs);
                        _context.SaveChanges();
                    }
                }

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
        public async Task<IActionResult> UpVote(int id,Boolean isQuestion,int ans)
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            if (isQuestion)
            {
                var isAlreadyVoted = _context.UserQAVoting.FirstOrDefault(x => x.UserQuestionId == id && x.UserId == user.Id);
                var isVotingOwnQuestion = _context.UserQuestion.FirstOrDefault(x => x.UserQuestionId == id && x.UserId == user.Id);
                if(isVotingOwnQuestion != null)
                {
                    TempData["Msg"] = "You can't vote your own question!";
                    TempData["Color"] = "error";
                }
                else if(isAlreadyVoted == null)
                {
                    var question = _context.UserQuestion.FirstOrDefault(c => c.UserQuestionId == id);
                    _context.UserQAVoting.Add(new UserQAVoting()
                    {
                        Value = +1,
                        IsAnswer = false,
                        UserQuestionId = question.UserQuestionId,
                        UserAnswerId = null,
                        Visibility = true,
                        UserId = user.Id
                    });
                    TempData["Msg"] = "Thank you for your feedback!";
                    TempData["Color"] = "success";
                }
                else
                {
                    TempData["Msg"] = "You have already casted your vote!";
                    TempData["Color"] = "error";
                }
            }
            else
            {//If Answer
                var isAlreadyVoted = _context.UserQAVoting.FirstOrDefault(x => x.UserAnswerId == ans && x.UserId == user.Id);
                var isVotingOwnAnswer = _context.UserQAnswer.FirstOrDefault(x => x.UserQAnswerId == ans && x.UserId == user.Id);
                if (isVotingOwnAnswer != null)
                {
                    TempData["Msg"] = "You can't vote your own answer!";
                    TempData["Color"] = "error";
                }
                else if (isAlreadyVoted == null)
                {
                    var answer = _context.UserQAnswer.FirstOrDefault(c => c.UserQAnswerId == ans);
                    _context.UserQAVoting.Add(new UserQAVoting()
                    {
                        Value = +1,
                        IsAnswer = true,
                        UserQuestionId = null,
                        UserAnswerId = answer.UserQAnswerId,
                        Visibility = true,
                        UserId = user.Id
                    });
                    TempData["Msg"] = "Thank you for your feedback!";
                    TempData["Color"] = "success";
                }
                else
                {
                    TempData["Msg"] = "You have already casted your vote!";
                    TempData["Color"] = "error";
                }
            }
            _context.SaveChanges();
            return RedirectToAction($"QuestionDetail", new { id = id });
        }
        public async Task<IActionResult> DownVote(int id, Boolean isQuestion, int ans)
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            if (isQuestion)
            {
                var isAlreadyVoted = _context.UserQAVoting.FirstOrDefault(x => x.UserQuestionId == id && x.UserId == user.Id);
                var isVotingOwnQuestion = _context.UserQuestion.FirstOrDefault(x => x.UserQuestionId == id && x.UserId == user.Id);
                if (isVotingOwnQuestion != null)
                {
                    TempData["Msg"] = "You can't vote your own question!";
                    TempData["Color"] = "error";
                }
                else if (isAlreadyVoted == null)
                {
                    var question = _context.UserQuestion.FirstOrDefault(c => c.UserQuestionId == id);
                    _context.UserQAVoting.Add(new UserQAVoting()
                    {
                        Value = -1,
                        IsAnswer = false,
                        UserQuestionId = question.UserQuestionId,
                        UserAnswerId = null,
                        Visibility = true,
                        UserId = user.Id
                    });
                    TempData["Msg"] = "Thank you for your feedback!";
                    TempData["Color"] = "success";
                }
                else
                {
                    TempData["Msg"] = "You have already casted your vote!";
                    TempData["Color"] = "error";
                }
            }
            else
            {//If Answer
                var isAlreadyVoted = _context.UserQAVoting.FirstOrDefault(x => x.UserAnswerId == ans && x.UserId == user.Id);
                var isVotingOwnAnswer = _context.UserQAnswer.FirstOrDefault(x => x.UserQAnswerId == ans && x.UserId == user.Id);
                if (isVotingOwnAnswer != null)
                {
                    TempData["Msg"] = "You can't vote your own answer!";
                    TempData["Color"] = "error";
                }
                else if (isAlreadyVoted == null)
                {
                    var answer = _context.UserQAnswer.FirstOrDefault(c => c.UserQAnswerId == ans);
                    _context.UserQAVoting.Add(new UserQAVoting()
                    {
                        Value = -1,
                        IsAnswer = true,
                        UserQuestionId = null,
                        UserAnswerId = answer.UserQAnswerId,
                        Visibility = true,
                        UserId = user.Id
                    });
                    TempData["Msg"] = "Thank you for your feedback!";
                    TempData["Color"] = "success";
                }
                else
                {
                    TempData["Msg"] = "You have already casted your vote!";
                    TempData["Color"] = "error";
                }
            }
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
            return NotFound();
        }

        public async Task<IActionResult> VerifyAnswer(int ansId,int questionId)
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            UserQuestion question= _context.UserQuestion.Where(x => x.UserQuestionId == questionId && x.UserId == user.Id).SingleOrDefault();
            UserQAnswer answer = _context.UserQAnswer.Where(y => y.UserQAnswerId == ansId).SingleOrDefault();
            if(question!=null && answer != null)
            {
                question.HasVerifiedAns = true;
                answer.IsVerified = true;
                _context.SaveChanges();
                TempData["Msg"] = "Answer has been verified!";
                TempData["Color"] = "success";
            }
            return RedirectToAction($"QuestionDetail", new { id = questionId });
        }

        //[HttpPost]
        //public async Task<IActionResult> SearchTag(string text)
        //{
        //    @ViewBag.UName = HttpContext.Session.GetString("Name");
           
        //    //Check User Profile is complete or not
        //    var user = await _userManager.GetCurrentUser(HttpContext);
        //    var userPersonalRow = _context.UserPersonalDetail.Where(a => a.UserId == user.Id).SingleOrDefault();
        //    if (userPersonalRow != null)
        //    {
        //        //var QuestionList = _context.UserQuestion.Where(x => x.Description.ToLower().Contains(text) || x.Title.ToLower().Contains(text)).Select(c => new UserQuestion { ApplicationUser = c.ApplicationUser, PostTime = c.PostTime, Tag = c.Tag, UserQuestionId = c.UserQuestionId, UserId = c.UserId, Title = c.Title, Description = HttpUtility.HtmlDecode(c.Description) }).ToList();
        //        //var result = _context.QuestionSkill.Where(a => a.SkillTag.SkillName.Contains(text)).Select(f => new QuestionSkill { SkillTag = f.SkillTag, QuestionSkillId = f.QuestionSkillId, SkillTagId = f.SkillTagId, UserQuestionId = f.UserQuestionId ,UserQuestion=f.UserQuestion });
        //        var res = _context.UserQuestion.Where(q => q.Tag.ToString().Contains(text)).Select(f => new UserQuestion { Title = f.Title, Tag = f.Tag, Description = HttpUtility.HtmlDecode(f.Description), ApplicationUser=f.ApplicationUser,PostTime=f.PostTime, }).ToList();
        //        return View("Index", res);

        //    }
        //    else
        //    {
        //        return RedirectToAction("CompleteProfile", "Home");
        //    }
        //}


    }
}