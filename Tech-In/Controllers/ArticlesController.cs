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

        public IActionResult ArticleSingle()
        {
            return View();
        }
        [HttpGet("article/{id}/{title}", Name = "Display")]
        public IActionResult Display(int id,string title)
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
                return this.RedirectToRoutePermanent("Display", new { id = id, title = friendlyTitle });
            }
            var articleVM = _mapper.Map<NewArticleVM>(article);
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
                return RedirectToAction($"Display", new { id = article.Id,title = article.Title});
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
    }
}