using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using Tech_In.Data;
using Tech_In.Extensions;
using Tech_In.Models;
using Tech_In.Models.Database;
using Tech_In.Models.ViewModels.ArticleViewModels;
using Tech_In.Services;

namespace Tech_In.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private IHttpContextAccessor _accessor;
        private IHostingEnvironment _environment;
        private readonly IMapper _mapper;
        public ArticlesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,IMapper mapper, IHttpContextAccessor accessor,IHostingEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _accessor = accessor;
            _environment = environment;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet("Article/{id}/{title}", Name = "ArticleSingle")]
        public IActionResult ArticleSingle(int id,string title)
        {
            var article = _context.Article.Where(x => x.Id == id).SingleOrDefault();
            if (article == null)
            {
                return NotFound();
            }
            string friendlyTitle = FriendlyUrlHelper.GetFriendlyTitle(article.Title);
            if (!string.Equals(friendlyTitle, title, StringComparison.Ordinal))
            {
                // If the title is null, empty or does not match the friendly title, return a 301 Permanent
                // Redirect to the correct friendly URL.
                return this.RedirectToRoutePermanent("ArticleSingle", new { id = id, title = friendlyTitle });
            }
            var articleVM = _mapper.Map<SingleArticleVM>(article);
            //Get Article Tags Ids
            var arttags = _context.ArticleTag.Where(tg => tg.ArticleId == article.Id).ToList();
            articleVM.Tags = new List<SkillTag>();
            foreach(var arttag in arttags)
            {
                var singleTag = _context.SkillTag.Where(sktag => sktag.SkillTagId == arttag.TagId).SingleOrDefault();
                articleVM.Tags.Add(singleTag);
            }
            //Author
            var author = _context.UserPersonalDetail.Where(usr => usr.UserId == article.UserId).SingleOrDefault();
            articleVM.AuthorId = article.UserId;
            articleVM.AuthorImg = author.ProfileImage;
            articleVM.AuthorName = author.FirstName + " " + author.LastName;
            articleVM.AuthorSummary = author.Summary;
            articleVM.CommentsCount = _context.ArticleComment.Where(cmt => cmt.ArticleId == article.Id).Count();
            articleVM.VisitorsCount = _context.ArticleVisitor.Where(av => av.ArticleId == article.Id).Count();
            return View("ArticleSingle",articleVM);
        }
        [Authorize]
        public IActionResult New()
        {
            List<Category> categories=  _context.Category.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Title");
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostArticle(NewArticleVM vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetCurrentUser(HttpContext);
                var article = _mapper.Map<Article>(vm);
                article.CreateTime = DateTime.Now;
                article.Status = "active";
                article.OriginalId = 0;
                article.UserId = user.Id;
                _context.Article.Add(article);
                _context.SaveChanges();
                string[] tagArray = vm.Tags.Split(',');
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
                        ArticleTag articleTag = new ArticleTag
                        {
                            TagId = sktag.SkillTagId,
                            ArticleId = article.Id
                        };
                        _context.ArticleTag.Add(articleTag);
                        _context.SaveChanges();
                    }
                    else
                    {
                        ArticleTag articleTag = new ArticleTag
                        {
                            TagId = tagFromDB.SkillTagId,
                            ArticleId = article.Id
                        };
                        _context.ArticleTag.Add(articleTag);
                        _context.SaveChanges();

                    }
                }//Forwach for tags
                ArticleCategory articleCategory = new ArticleCategory
                {
                    ArticleId = article.Id,
                    CategoryId = vm.CategoryId
                };
                _context.ArticleCategory.Add(articleCategory);
                article.OriginalId = article.Id;
                _context.SaveChanges();
                return RedirectToAction($"ArticleSingle", new { id = article.Id,title = article.Title});
            }
            return View(vm);
        }
        //Check Valid Image and Upload to Server
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
                        if (img.Height > 400)
                        {
                            if (img.Width > 700)
                            {
                                if (vm.Size < 10000)
                                {
                                    return Json(new { success = true, response = vm });
                                }
                                else
                                    errorMsg = "Image should be less than 10 MB";
                            }
                            else
                                errorMsg = "Image Width should be greater than 700px";
                        }
                        else
                            errorMsg = "Image height should be greater than 400px";
                        
                    if (errorMsg!=null)
                    {
                        string fullPath = path +"/" + filename;
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

        [Authorize]
        public async Task<IActionResult> AddComment(AddCommentVM vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetCurrentUser(HttpContext);
                var comment = new ArticleComment {
                    ArticleId = vm.ArticleId,
                    Comment = vm.Comment,
                    CreateTime = DateTime.Now,
                    Status = "active",
                    OriginalId = 0,
                    UserId = user.Id
                };
                _context.ArticleComment.Add(comment);
                _context.SaveChanges();
            }
            return View("ArticleSingle");
        }
        [HttpPost]
        public IActionResult Comments(int articleId)
        {
            var comments = _context.ArticleComment.Where(art => art.ArticleId == articleId).Select(x => new CommentVM { Comment = x.Comment, CreateTime = x.CreateTime, Id = x.Id, Status = x.Status, UserId = x.UserId }).ToList();
            foreach(var cmt in comments)
            {
                var commentAuthor = _context.UserPersonalDetail.Where(y => y.UserId == cmt.UserId).SingleOrDefault();
                cmt.UserImg = commentAuthor.ProfileImage;
                cmt.UserName = commentAuthor.FirstName + " " + commentAuthor.LastName;
            }
            //return Json(new { success = true, response = comments });
            return View("_CommentsPartial",comments);
        }
        public async Task<IActionResult> ArticleViews(int articleId)
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            if (user != null)
            {
                var userPersonalRow = _context.UserPersonalDetail.Where(a => a.UserId == user.Id).SingleOrDefault();
                if (userPersonalRow == null)
                {
                    return RedirectToAction("CompleteProfile", "Home");
                }
                //If Registered Visitor Counter
                ArticleVisitor isVisited = _context.ArticleVisitor.Where(qv => qv.UserId == user.Id && qv.ArticleId == articleId).SingleOrDefault();
                if (isVisited == null)
                {
                    _context.ArticleVisitor.Add(new ArticleVisitor
                    {
                        ArticleId = articleId,
                        UserId = user.Id,
                        IsLoggedIn = true,
                        UserIp = null
                    });
                    _context.SaveChanges();
                }
                var articleTags = _context.ArticleTag.Where(at => at.ArticleId == articleId).ToList();
                foreach(var tag in articleTags)
                {
                    var userInterestedTag = _context.AIUserInterest.Where(uit => uit.TagId == tag.TagId).SingleOrDefault();
                    if (userInterestedTag == null)
                    {
                        AIUserInterest userLikeTag = new AIUserInterest
                        {
                            TagId = tag.TagId,
                            UserId = user.Id,
                            Count = 1
                        };
                        _context.AIUserInterest.Add(userLikeTag);
                    }
                    else
                    {
                        userInterestedTag.Count = userInterestedTag.Count + 1;
                    }
                    _context.SaveChanges();
                }
            }
            else
            {//If Anonomus Visitor Counter
                string currentUserIp = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                ArticleVisitor isVisited = _context.ArticleVisitor.Where(qv => qv.UserIp == currentUserIp && qv.ArticleId == articleId).SingleOrDefault();
                if (isVisited == null)
                {
                    _context.ArticleVisitor.Add(new ArticleVisitor
                    {
                        ArticleId = articleId,
                        UserId = null,
                        IsLoggedIn = false,
                        UserIp = currentUserIp
                    });
                    _context.SaveChanges();
                }
            }//If Anonomus Visitor Ends Here
            return Ok();
        }

        public IActionResult TopCategories()
        {
            var topCatg = _context.ArticleCategory.GroupBy(o => new { o.CategoryId }).Select(g => new TopCategoryVM
            {
                CategoryId = g.Key.CategoryId,
                Count = g.Count()
            }).OrderByDescending(o=>o.Count).Take(5).ToList();
            foreach(var category in topCatg)
            {
                category.CategoryName = _context.Category.Where(x => x.Id == category.CategoryId).Select(y=>y.Title).SingleOrDefault();
            }
            return View("_TopCategory",topCatg);
        }

        public IActionResult PopularPosts()
        {
            var topPosts = _context.ArticleVisitor.GroupBy(o => new { o.ArticleId }).Select(m => new PopularPostVM
            {
                ArticleId = m.Key.ArticleId,
                VisitorCount = m.Count()
            }).OrderByDescending(o => o.VisitorCount).Take(3).ToList();
            foreach(var post in topPosts)
            {
                var article = _context.Article.Where(x => x.Id == post.ArticleId).SingleOrDefault();
                post.ArticleImage = article.ArticleImg;
                post.ArticleTitle = article.Title;
                post.CreateTime = article.CreateTime;
            }
            return View("_PopularPosts",topPosts);
        }

        public IActionResult TopTags()
        {
            var topTags = _context.ArticleTag.GroupBy(o => new { o.TagId }).Select(m => new TopTagVM
            {
                Id = m.Key.TagId,
                Count = m.Count()
            }).OrderByDescending(o => o.Count).Take(5).ToList();
            foreach(var tagCountID in topTags)
            {
                var tag = _context.SkillTag.Where(x => x.SkillTagId == tagCountID.Id).SingleOrDefault();
                tagCountID.TagName = tag.SkillName;
            }
            return View("_TopTags",topTags);
        }
    }
}