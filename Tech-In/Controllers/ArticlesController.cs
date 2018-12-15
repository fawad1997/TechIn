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
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Index(string currentFilter, string search,int? page)
        {
            await OnGetSesstion();
            ViewData["CurrentFilter"] = search;
            if (search != null)
            {
                page = 1;
            }
            else
            {
                search = currentFilter;
            }
            var articel = (from art in _context.Article
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
                                  AuthorName = u.FirstName +" "+u.LastName,
                                  VisitorsCount = _context.ArticleVisitor.Where(av => av.ArticleId == art.OriginalId).Count(),
                                  CommentsCount = _context.ArticleComment.Where(cmt => cmt.ArticleId == art.OriginalId && cmt.Status=="active").Count()
                              }
                              ).Take(10);
            if (!String.IsNullOrEmpty(search))
            {
                articel = articel.Where(s => s.Title.Contains(search));
            }
            int pageSize = 10;
            return View(new ArticleListVM { Articles = await PaginatedList<SingleArticleVM>.CreateAsync(articel.AsQueryable(), page ?? 1, pageSize) });
        }
        
        [HttpGet("Article/{id}/{title}", Name = "ArticleSingle")]
        public async Task<IActionResult> ArticleSingle(int id,string title)
        {
            var article = _context.Article.Where(x => x.Id == id).SingleOrDefault();
            string currentUserId = await OnGetSesstion();
            if (article == null)
            {
                return NotFound();
            }
            if(article.Status == "history")
            {
                article = _context.Article.Where(re => re.OriginalId == article.OriginalId && re.Status == "active").SingleOrDefault();
            }
            string friendlyTitle = FriendlyUrlHelper.GetFriendlyTitle(article.Title);
            if (!string.Equals(friendlyTitle, title, StringComparison.Ordinal))
            {
                // If the title is null, empty or does not match the friendly title, return a 301 Permanent
                // Redirect to the correct friendly URL.
                return this.RedirectToRoutePermanent("ArticleSingle", new { id = id, title = friendlyTitle });
            }
            var articleVM = _mapper.Map<SingleArticleVM>(article);
            if (article.Id != article.OriginalId)
                articleVM.IsEdited = true;
            else
                articleVM.IsEdited = false;
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
            articleVM.CommentsCount = _context.ArticleComment.Where(cmt => cmt.ArticleId == article.OriginalId).Count();
            articleVM.VisitorsCount = _context.ArticleVisitor.Where(av => av.ArticleId == article.OriginalId).Count();
            if (currentUserId != null)
            {
                if (articleVM.AuthorId == currentUserId)
                    articleVM.IsArticleAuthor = true;
                else
                    articleVM.IsArticleAuthor = false;
            }
            else
                articleVM.IsArticleAuthor = false;
            return View("ArticleSingle",articleVM);
        }
        [Authorize]
        public async Task<IActionResult> New()
        {
            await OnGetSesstion();
            List<Category> categories=  _context.Category.OrderBy(x=>x.Title).ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Title");
            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostArticle(NewArticleVM vm)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = await OnGetSesstion();
                var article = _mapper.Map<Article>(vm);
                article.CreateTime = DateTime.Now;
                article.Status = "active";
                article.OriginalId = 0;
                article.UserId = currentUserId;
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
                            UserId = currentUserId
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
                }//Foreach for tags
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
        public async Task<IActionResult> Edit(int Id)
        {
            string currentUserId = await OnGetSesstion();
            var article = _context.Article.Where(x => x.Id == Id && x.UserId==currentUserId).SingleOrDefault();
            if (article == null)
            {
                return NotFound();
            }
            NewArticleVM articleVM = _mapper.Map<NewArticleVM>(article);
            List<Category> categories = _context.Category.ToList();
            var tags = _context.ArticleTag.Where(a => a.ArticleId == articleVM.Id).ToList();
            string tagName = null;
            foreach (var tag in tags)
            {
                tagName += _context.SkillTag.Where(b => b.SkillTagId == tag.TagId).Select(c => c.SkillName).SingleOrDefault();
                tagName += ", ";
            }
            ViewBag.Tag = tagName;
            ViewBag.Categories = new SelectList(categories, "Id", "Title");
            return View(articleVM);
        }
        [Authorize]
        public async Task<IActionResult> DeleteArticle(int Id)
        {
            string currentUserId = await OnGetSesstion();
            var articles = _context.Article.Where(x => x.OriginalId == Id && x.UserId==currentUserId);
            if (articles == null)
            {
                return NotFound();
            }
            var comments = _context.ArticleComment.Where(y => y.ArticleId == Id);
            var artViews = _context.ArticleVisitor.Where(z => z.ArticleId == Id);
            foreach(var article in articles)
            {
                var tags = _context.ArticleTag.Where(a => a.ArticleId == article.Id).ToList();
                var artCategories = _context.ArticleCategory.Where(b => b.ArticleId == article.Id).ToList();
                foreach(var tag in tags)
                {
                    _context.ArticleTag.Remove(tag);
                }
                foreach(var artCategory in artCategories)
                {
                    _context.ArticleCategory.Remove(artCategory);
                }
            }
            foreach(var comment in comments)
            {
                _context.ArticleComment.Remove(comment);
            }
            foreach (var visitor in artViews)
            {
                _context.ArticleVisitor.Remove(visitor);
            }
            foreach(var article in articles)
            {
                _context.Article.Remove(article);
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateArticle(NewArticleVM vm)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = await OnGetSesstion();
                var oldArticle = _context.Article.Where(olda => olda.Id == vm.Id).SingleOrDefault();
                if (currentUserId != oldArticle.UserId)
                    return BadRequest();
                oldArticle.Status = "history";
                vm.Id = 0;
                var article = _mapper.Map<Article>(vm);
                article.CreateTime = DateTime.Now;
                article.Status = "active";
                article.OriginalId = oldArticle.OriginalId;
                article.UserId = currentUserId;
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
                            UserId = currentUserId
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
                }//Foreach for tags
                ArticleCategory articleCategory = new ArticleCategory
                {
                    ArticleId = article.Id,
                    CategoryId = vm.CategoryId
                };
                _context.ArticleCategory.Add(articleCategory);
                _context.SaveChanges();
                return RedirectToAction($"ArticleSingle", new { id = article.Id, title = article.Title });
            }
            return View(vm);
        }

        public IActionResult ArticleEditHistory(int articleId)
        {
            var historyList = _context.Article.Where(x => x.OriginalId == articleId).ToList();
            var vm = _mapper.Map<List<Article>, List<SingleArticleVM>>(historyList);
            foreach(var singleEdit in vm)
            {
                //Get Article Tags Ids
                var arttags = _context.ArticleTag.Where(tg => tg.ArticleId == singleEdit.Id).ToList();
                singleEdit.Tags = new List<SkillTag>();
                foreach (var arttag in arttags)
                {
                    var singleTag = _context.SkillTag.Where(sktag => sktag.SkillTagId == arttag.TagId).SingleOrDefault();
                    singleEdit.Tags.Add(singleTag);
                }
            }
            return View("_ArticleEditHistory",vm);
        }

        public async Task<IActionResult> SearchByTag(int Id,string search,int? page)
        {
            await OnGetSesstion();
            ViewData["CurrentFilter"] = search;
            if (search != null)
            {
                page = 1;
            }
            var articel = (from tg in _context.ArticleTag
                           join art in _context.Article on tg.ArticleId equals art.Id
                           join u in _context.UserPersonalDetail on art.UserId equals u.UserId
                           where tg.TagId.Equals(Id) && art.Status.Contains("active")
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
                               CommentsCount = _context.ArticleComment.Where(cmt => cmt.ArticleId == art.OriginalId).Count()
                           }
                              ).Take(10);
            if (!String.IsNullOrEmpty(search))
            {
                articel = articel.Where(s => s.Title.Contains(search));
            }
            int pageSize = 10;
            return View("Index",new ArticleListVM { Articles = await PaginatedList<SingleArticleVM>.CreateAsync(articel.AsQueryable(), page ?? 1, pageSize) });
        }

        public async Task<IActionResult> SearchByCategory(int Id, string search, int? page)
        {
            await OnGetSesstion();
            ViewData["CurrentFilter"] = search;
            if (search != null)
            {
                page = 1;
            }
            var articel = (from ct in _context.ArticleCategory
                           join art in _context.Article on ct.ArticleId equals art.Id
                           join u in _context.UserPersonalDetail on art.UserId equals u.UserId
                           where ct.CategoryId.Equals(Id) && art.Status.Contains("active")
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
                               CommentsCount = _context.ArticleComment.Where(cmt => cmt.ArticleId == art.OriginalId).Count()
                           }
                              ).Take(10);
            if (!String.IsNullOrEmpty(search))
            {
                articel = articel.Where(s => s.Title.Contains(search));
            }
            int pageSize = 10;
            return View("Index", new ArticleListVM { Articles = await PaginatedList<SingleArticleVM>.CreateAsync(articel.AsQueryable(), page ?? 1, pageSize) });
        }

        [Authorize]
        public async Task<IActionResult> AddComment(AddCommentVM vm)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = await OnGetSesstion();
                var comment = new ArticleComment {
                    ArticleId = vm.ArticleId,
                    Comment = vm.Comment,
                    CreateTime = DateTime.Now,
                    Status = "active",
                    OriginalId = 0,
                    UserId = currentUserId
                };
                _context.ArticleComment.Add(comment);
                _context.SaveChanges();
                comment.OriginalId = comment.Id;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public async Task<IActionResult> Comments(int articleId)
        {
            string currentUserId = await OnGetSesstion();
            var comments = _context.ArticleComment.Where(art => art.ArticleId == articleId && art.Status=="active").Select(x => new CommentVM { Comment = x.Comment, CreateTime = x.CreateTime, Id = x.Id, Status = x.Status, UserId = x.UserId }).ToList();
            foreach(var cmt in comments)
            {
                var commentAuthor = _context.UserPersonalDetail.Where(y => y.UserId == cmt.UserId).SingleOrDefault();
                cmt.UserImg = commentAuthor.ProfileImage;
                cmt.UserName = commentAuthor.FirstName + " " + commentAuthor.LastName;
                cmt.IsCommentAuthor = false;
                if (currentUserId != null)
                {
                    if(currentUserId == commentAuthor.UserId)
                    {
                        cmt.IsCommentAuthor = true;
                    }
                }
            }
            //return Json(new { success = true, response = comments });
            return View("_CommentsPartial",comments);
        }
        public async Task<IActionResult> EditComment(int Id)
        {
            string currentUserId = await OnGetSesstion();
            var comment = _context.ArticleComment.Where(x => x.Id == Id && x.UserId == currentUserId).SingleOrDefault();
            if (comment == null)
            {
                return NotFound();
            }
            CommentVM cmt = _mapper.Map<CommentVM>(comment);
            return View("_EditComment",cmt);
        }
        public async Task<IActionResult> UpdateComment(CommentVM vm)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = await OnGetSesstion();
                var comment = _context.ArticleComment.Where(x => x.Id == vm.Id && x.UserId == currentUserId).SingleOrDefault();
                if (comment == null)
                {
                    return NotFound();
                }
                comment.Status = "history";
                ArticleComment updatedComment = new ArticleComment
                {
                    OriginalId = comment.Id,
                    ArticleId = comment.ArticleId,
                    Comment = vm.Comment,
                    CreateTime = DateTime.Now,
                    Status = "active",
                    UserId = currentUserId
                };
                _context.ArticleComment.Add(updatedComment);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        public async Task<IActionResult> DeleteComment(int id)
        {
            string currentUserId = await OnGetSesstion();
            var comments = _context.ArticleComment.Where(x => x.OriginalId == id && x.UserId == currentUserId).ToList();
            if (comments == null)
            {
                return NotFound();
            }
            foreach(var comment in comments)
            {
                _context.ArticleComment.Remove(comment);
            }
            _context.SaveChanges();
            return Json(new { success = true });
        }
        public async Task<IActionResult> ArticleViews(int articleId)
        {
            string currentUserId = await OnGetSesstion();
            if (currentUserId != null)
            {
                var userPersonalRow = _context.UserPersonalDetail.Where(a => a.UserId == currentUserId).SingleOrDefault();
                if (userPersonalRow == null)
                {
                    return RedirectToAction("CompleteProfile", "Home");
                }
                //If Registered Visitor Counter
                ArticleVisitor isVisited = _context.ArticleVisitor.Where(qv => qv.UserId == currentUserId && qv.ArticleId == articleId).SingleOrDefault();
                if (isVisited == null)
                {
                    _context.ArticleVisitor.Add(new ArticleVisitor
                    {
                        ArticleId = articleId,
                        UserId = currentUserId,
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
                            UserId = currentUserId,
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
                var article = _context.Article.Where(x => x.OriginalId == post.ArticleId).OrderByDescending(y=>y.Id).FirstOrDefault();
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
                var tag = _context.SkillTag.Where(x => x.SkillTagId == tagCountID.Id).FirstOrDefault();
                tagCountID.TagName = tag.SkillName;
            }
            return View("_TopTags",topTags);
        }
        public async Task<IActionResult> AIUserInterests()
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            if (user == null)
                return NotFound();
            try
            {
                var userInterestedTag = _context.AIUserInterest.Where(aa=>aa.UserId == user.Id).OrderByDescending(x => x.Count).FirstOrDefault();
                var articleID = _context.ArticleTag.Where(a => a.TagId == userInterestedTag.TagId).Take(3).ToList();
                List<SingleArticleVM> articles = new List<SingleArticleVM>();
                foreach (var art in articleID)
                {
                    var article = _context.Article.Where(b => b.Id == art.ArticleId && b.Status == "active").Select(c => new SingleArticleVM { ArticleImg = c.ArticleImg, Title = c.Title, Id = c.Id }).FirstOrDefault();
                    if(article!=null)
                        articles.Add(article);
                }
                return View("_AIUserInterestedArticles", articles);
            }
            catch(Exception e)
            {
                return NotFound();
            }
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