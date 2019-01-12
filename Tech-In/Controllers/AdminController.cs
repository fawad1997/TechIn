using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tech_In.Data;
using Tech_In.Extensions;
using Tech_In.Models;
using Tech_In.Models.Database;
using Tech_In.Models.ViewModels.ArticleViewModels;
using Tech_In.Models.ViewModels.ProfileViewModels;
using Tech_In.Models.ViewModels.QuestionViewModels;
using Tech_In.Services;

namespace Tech_In.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private IHttpContextAccessor _accessor;
        private IHostingEnvironment _environment;
        private readonly IMapper _mapper;
        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, IHttpContextAccessor accessor, IHostingEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _accessor = accessor;
            _environment = environment;
            _mapper = mapper;
        }

      [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var usercount = _context.UserPersonalDetail.Count();
            ViewBag.userCount = usercount;
            var questioncount = _context.UserQuestion.Count();
            ViewBag.questionCount = questioncount;
            var articlescount = _context.Article.Count();
            ViewBag.articlesCount = articlescount;
            

            return View();
        }

        public async Task<IActionResult> UserAdmin()
        {
            

            var users = _context.UserPersonalDetail.Select(x => new UserPersonalViewModel { FirstName = x.FirstName, LastName = x.LastName, DOB = x.DOB, UserPersonalDetailID = x.UserPersonalDetailId, Gender = x.Gender }).ToList();
                
                return View(users);
        }

        public async Task<IActionResult> QuestionAdmin()
        {
            var questions = (from question in _context.UserQuestion
                                join personalInfo in _context.UserPersonalDetail on question.UserId equals personalInfo.UserId
                                select new NewQuestionVM
                                {
                                    UserQuestionId = question.UserQuestionId,
                                    Description = question.Description,
                                    Title = question.Title,
                                    Tags = question.Tag.Select(t => new QuestionTagViewModel
                                    {
                                        SkillName = t.SkillTag.SkillName,
                                        SkillTagId = t.SkillTagId,
                                    }).ToList(),
                                    Voting = _context.UserQAVoting.Sum(x => x.Value),
                                    Visitors = _context.QuestionVisitor.Where(av => av.QuestionId == question.UserQuestionId).Count(),
                                    PostedBy = personalInfo.FirstName + " " + personalInfo.LastName,
                                    UserPic = personalInfo.ProfileImage,
                                    PostTime = question.PostTime,
                                    HasVerifiedAns = question.HasVerifiedAns,
                                }).ToList();

            return View(questions);
        }

        public async Task<IActionResult> ArticleAdmin()
        {
            var artice = (from art in _context.Article
                           join u in _context.UserPersonalDetail on art.UserId equals u.UserId
                           where art.Status.Contains("active")
                           orderby art.Id descending
                           select new SingleArticleVM
                           {
                               Id = art.Id,
                               AuthorId = u.UserId,
                               Title = art.Title,
                               ArticleBody = art.ArticleBody.Substring(0, 40),
                               ArticleImg = art.ArticleImg,
                               CreateTime = art.CreateTime,
                               AuthorName = u.FirstName + " " + u.LastName,
                               VisitorsCount = _context.ArticleVisitor.Where(av => av.ArticleId == art.OriginalId).Count(),
                               CommentsCount = _context.ArticleComment.Where(cmt => cmt.ArticleId == art.OriginalId && cmt.Status == "active").Count()
                           }
                              ).ToList();
            return View(artice);
        }
    }
}