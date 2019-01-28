using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tech_In.Data;
using Tech_In.Extensions;
using Tech_In.Models;
using Tech_In.Models.Database;
using Tech_In.Models.Model;
using Tech_In.Models.ViewModels.ProfileViewModels;
using Tech_In.Models.ViewModels.QuestionViewModels;
using Tech_In.Services;

namespace Tech_In.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _hostingEnvironment;
        //private IHttpContextAccessor _accessor;
        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,IMapper mapper, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
            //_accessor = accessor;
        }
        public async Task<IActionResult> Index(string currentFilter, string search, int? page)
        {
            string userloggedId = await OnGetSesstion();
            ViewData["CurrentFilter"] = search;
            if (search != null)
            {
                page = 1;
            }
            else
            {
                search = currentFilter;
            }
            var users = (from u in _context.UserPersonalDetail
                         join us in _userManager.Users on u.UserId equals us.Id
                         orderby u.FirstName
                         select new SingleUserVM
                         {
                             FirstName = u.FirstName,
                             LastName = u.LastName,
                             ProfileImage = u.ProfileImage,
                             UserName = us.UserName,
                             IsFriend = _context.UserNetwork.Where(a => (a.User1 == us.Id || a.User1 == userloggedId) && (a.User2 == us.Id || a.User2 == userloggedId) && a.AreFriend==true).Any(),
                             IsFriendReqSent = _context.UserNetwork.Where(a => a.User1 == userloggedId && a.User2 == us.Id).Any(),
                             IsFriendReqRecieved = _context.UserNetwork.Where(a => a.User1 == us.Id && a.User2 == userloggedId).Any()
                         }).Take(10);
            if (!String.IsNullOrEmpty(search))
            {
                users = users.Where(s => s.FirstName.Contains(search) || s.LastName.Contains(search));
            }
            //var users = _context.UserPersonalDetail.Select(x=> new UserListVM { FirstName = x.FirstName,LastName=x.LastName,ProfileImage = x.ProfileImage, UserName = _userManager.Users.Where(y=>y.Id==x.UserId).Select(y=>y.UserName).FirstOrDefault() }).ToList();
            return View(new UserListVM { User = await PaginatedList<SingleUserVM>.CreateAsync(users.AsQueryable(), page ?? 1, 10) });
        }

        [HttpGet("u/{username}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(string username)
        {
            string userloggedId = await OnGetSesstion();
            //Check User Profile is complete or not
            var user = _userManager.Users.Where(x => x.UserName.Equals(username)).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            var userPersonalRow = _context.UserPersonalDetail.Where(a => a.UserId == user.Id).SingleOrDefault();
            if (userPersonalRow == null)
            {
                return RedirectToAction("CompleteProfile", "Home");
            }
            ProfileViewModal PVM = new ProfileViewModal();
            if (user.Id == userloggedId)
                PVM.IsCurrentUser = true;
            else
            {
                PVM.IsCurrentUser = false;
                var AreFrndList = _context.UserNetwork.Where(a => (a.User1 == user.Id || a.User1 == userloggedId) && (a.User2 == user.Id || a.User2 == userloggedId)).ToList();
                if (AreFrndList.Any())
                {
                    PVM.AreFriends = AreFrndList.Where(a => (a.User1 == user.Id || a.User1 == userloggedId) && (a.User2 == user.Id || a.User2 == userloggedId) && a.AreFriend == true).Any();
                    PVM.IsFriendReqRecieved = AreFrndList.Where(a => (a.User1 == user.Id) && (a.User2 == userloggedId) && a.AreFriend == false).Any();
                    PVM.IsFriendReqSent = AreFrndList.Where(a => (a.User1 == userloggedId) && (a.User2 == user.Id) && a.AreFriend == false).Any();
                }
            }
            var wallPosts = (from pst in _context.UserPost
                             join us in _userManager.Users on pst.UserId equals us.Id
                             join u in  _context.UserPersonalDetail on pst.UserId equals u.UserId
                             orderby pst.OriginalId descending
                select new UserPostVM
                {
                    ProfilePic = u.ProfileImage,
                    Name = u.FirstName+" "+u.LastName,
                    UserName = us.UserName,
                    UserPostId = pst.UserPostId,
                    OriginalId = pst.OriginalId,
                    Status = pst.Status,
                    Summary = pst.Summary,
                    Image = pst.Image,
                    CreateTime = pst.CreateTime,
                    IsLiked = _context.PostLikes.Where(y=>y.PostId == pst.UserPostId && y.UserId == userloggedId).Any(),
                    TotalLikes = _context.PostLikes.Where(z=>z.PostId == pst.UserPostId).Count()
                }
            ).Take(10);
            PVM.UserPosts = wallPosts;
            PVM.UserPersonalVM = _context.UserPersonalDetail.Where(m => m.UserId == user.Id).Select(x => new UserPersonalViewModel { FirstName = x.FirstName, LastName = x.LastName, Summary = x.Summary, ProfileImage = x.ProfileImage, DOB = x.DOB, UserPersonalDetailID = x.UserPersonalDetailId, Gender = x.Gender, CityName = x.City.CityName, CountryName = x.City.Country.CountryName }).SingleOrDefault();
            PVM.UserName = username;
            PVM.UserPersonalVM.PhoneNo = user.PhoneNumber;
            PVM.UserPersonalVM.Email = user.Email;



            List<ExperienceVM> userExperienceList = _context.UserExperience.Where(x => x.UserId == user.Id).Select(c => new ExperienceVM { Title = c.Title, UserExperienceId = c.UserExperienceId, CityId = c.CityID, CityName = c.City.CityName, CountryName = c.City.Country.CountryName, CompanyName = c.CompanyName, CurrentWorkCheck = c.CurrentWorkCheck, Description = c.Description, StartDate = c.StartDate, EndDate = c.EndDate }).ToList();
            ViewBag.UserExperienceList = userExperienceList;

            List<EducationVM> userEducationList = _context.UserEducation.Where(x => x.UserId == user.Id).Select(c => new EducationVM { Title = c.Title, Details = c.Details, SchoolName = c.SchoolName, StartDate = c.StartDate, EndDate = c.EndDate, CurrentStatusCheck = c.CurrentStatusCheck, CityId = c.CityId, CityName = c.City.CityName, CountryName = c.City.Country.CountryName, UserEducationID = c.UserEducationId }).ToList();
            ViewBag.UserEducationList = userEducationList;

            List<CertificationVM> userCertificationList = _context.UserCertification.Where(x => x.UserId == user.Id).Select(c => new CertificationVM { Name = c.Name, URL = c.URL, UserCertificationId = c.UserCertificationId, LiscenceNo = c.LiscenceNo, CertificationDate = c.CertificationDate, ExpirationDate = c.ExpirationDate }).ToList();
            ViewBag.UserCertificationList = userCertificationList;

            PVM.AchievVMList = _context.UserAcheivement.Where(x => x.UserId == user.Id).Select(c => new AchievmentVM { Description = c.Description, UserAchievementId = c.UserAchievementId });

            PVM.HobbyVMList = _context.UserHobby.Where(x => x.UserId == user.Id).Select(c => new HobbyVM { UserHobbyId = c.UserHobbyId, HobbyOrIntrest = c.HobbyOrIntrest });

            PVM.LanguageSkillVMList = _context.UserLanguageSkill.Where(x => x.UserId == user.Id).Select(c => new LanguageSkillVM { LanguageSkillId = c.LanguageSkillId, SkillName = c.SkillName });

            PVM.PublicationVMListJP = _context.UserPublication.Where(x => x.UserId == user.Id && x.ConferenceOrJournal == false).Select(c => new PublicationVM { Title = c.Title, PublishYear = c.PublishYear, Description = c.Description, ConferenceOrJournal = c.ConferenceOrJournal, UserPublicationId = c.UserPublicationId });
            PVM.PublicationVMListCP = _context.UserPublication.Where(x => x.UserId == user.Id && x.ConferenceOrJournal == true).Select(c => new PublicationVM { Title = c.Title, PublishYear = c.PublishYear, Description = c.Description, ConferenceOrJournal = c.ConferenceOrJournal, UserPublicationId = c.UserPublicationId });

            //Question
            var questionList = _context.UserQuestion.Where(u => u.UserId == user.Id).Select(c => new NewQuestionVM
            {
                UserQuestionId = c.UserQuestionId,
                Title = c.Title,
                PostedBy = _context.UserPersonalDetail.Where(aa => aa.UserId == c.UserId).Select(z => z.FirstName).SingleOrDefault(),
                UserPic = _context.UserPersonalDetail.Where(aa => aa.UserId == c.UserId).Select(z => z.ProfileImage).SingleOrDefault(),
                PostTime = c.PostTime,
                HasVerifiedAns = c.HasVerifiedAns,
                Visitors = _context.QuestionVisitor.Where(f => f.QuestionId == c.UserQuestionId).Count(),
                Tags = c.Tag.Select(t => new QuestionTagViewModel { SkillName = t.SkillTag.SkillName }).ToList(),
                Voting = c.UserQAVoting.Sum(x => x.Value)
            }).ToList();

            //comment
            var comments = _context.UserQAComment.Where(q => q.UserId == user.Id).Select(s => new QACommentsViewModel
            {
                Description = s.Description,
                UserQACommentID = s.UserQACommentID,
                UserQuestionId = s.UserQuestionId,
                IsAnswer = s.IsAnswer,
                PostedBy = _context.UserPersonalDetail.Where(aa => aa.UserId == s.UserId).Select(z => z.FirstName).SingleOrDefault(),
                UserId = user.Id,
            }).ToList();

            //answer
            var answers = _context.UserQAnswer.Where(f => f.UserId == user.Id).Select(k => new QAnswerViewModel
            {
                Description = HttpUtility.HtmlDecode(k.Description),
                Date = k.PostTime,
                IsVerified = k.IsVerified,
                User = user.Id,
                UserQAnswerId = k.UserQAnswerId,
                UserQuestion = _context.UserQuestion.Where(g => g.UserQuestionId == k.UserQuestionId).Select(j => new NewQuestionVM
                {
                    UserQuestionId = j.UserQuestionId,
                    Title = j.Title,
                    PostedBy = _context.UserPersonalDetail.Where(aa => aa.UserId == j.UserId).Select(z => z.FirstName).SingleOrDefault(),
                    UserPic = _context.UserPersonalDetail.Where(aa => aa.UserId == j.UserId).Select(z => z.ProfileImage).SingleOrDefault(),
                    PostTime = j.PostTime,
                    HasVerifiedAns = j.HasVerifiedAns,
                    Visitors = _context.QuestionVisitor.Where(f => f.QuestionId == j.UserQuestionId).Count(),
                    Tags = j.Tag.Select(t => new QuestionTagViewModel { SkillName = t.SkillTag.SkillName }).ToList()
                }).ToList(),
            }).ToList();

            ViewBag.answer = answers;
            ViewBag.comment = comments;
            ViewBag.question = questionList;
            return View(PVM);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddPost(AddPostVM post)
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            if (post.PostImg == null && post.PostDescription.Length < 3)
            {
                ModelState.AddModelError("PostDescription", "Post length should be greater then 3 characters");
                return null;
            }
            UserPost p = new UserPost
            {
                OriginalId = 0,
                Status = "Active",
                CreateTime = DateTime.Now,
                Summary = post.PostDescription,
                Image = null,
                UserId = user.Id
            };
            if (post.PostImg != null)
            {
                if ((post.PostImg.Length / 1000) > 7000)
                {
                    ViewBag.ProfilePic = "Image can't exceed 7Mb size";
                    return View();
                }
                string path = Path.Combine(_hostingEnvironment.WebRootPath, "images/posts");
                string extension = Path.GetExtension(post.PostImg.FileName).Substring(1);
                if (!(extension.ToLower() == "png" || extension.ToLower() == "jpg" || extension.ToLower() == "jpeg"))
                {
                    ViewBag.ProfilePic = "Only png, jpg & jpeg are allowed";
                    return View();
                }
                string fileNam = user.Id.Substring(24) + "p." + extension;
                using (var vs = new FileStream(Path.Combine(path, fileNam), FileMode.CreateNew))
                {
                    await post.PostImg.CopyToAsync(vs);
                }
                using (var img = SixLabors.ImageSharp.Image.Load(Path.Combine(path, fileNam)))
                {
                    p.Image = $"/images/posts/{fileNam}";
                }
            }
            else
            {
                p.Image = null;
            }
            _context.UserPost.Add(p);
            await _context.SaveChangesAsync();
            p.OriginalId = p.UserPostId;
            _context.SaveChanges();
            return Redirect("/u/"+ user.UserName);
        }

        public async Task<IActionResult> DeletePost(int Id)
        {
            string userloggedId = await OnGetSesstion();
            var likes = _context.PostLikes.Where(aa => aa.PostId == Id);
            foreach(var like in likes)
            {
                _context.PostLikes.Remove(like);
            }
            _context.SaveChanges();
            var userpost = _context.UserPost.Where(x => x.UserPostId == Id).FirstOrDefault();
            _context.Remove(userpost);
            _context.SaveChanges();
            return Redirect("/u/" + HttpContext.Session.GetString("_UserName"));
        }

        public IActionResult ViewLikes(int Id)
        {
            var users = (from usr in _context.UserPersonalDetail
                         join p in _context.PostLikes on usr.UserId equals p.UserId
                         join us in _userManager.Users on usr.UserId equals us.Id
                         where p.PostId == Id
                         select new LikesViewVM
                         {
                             Name = usr.FirstName + " " + usr.LastName,
                             Image = usr.ProfileImage,
                             UserName = us.UserName
                         }
                  ).ToList();
            return View("_ViewLikes",users);
        }
        //Personal Details
        public async Task<IActionResult> UpdatePersonalDetail(int Id)
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            ViewBag.CountryList = new SelectList(GetCountryList(), "CountryId", "CountryName");
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            UserPersonalViewModel vm = new UserPersonalViewModel();
            if (Id > 0)
            {
                UserPersonalDetail personalDetail = _context.UserPersonalDetail.SingleOrDefault(x => x.UserPersonalDetailId == Id);
                vm.UserPersonalDetailID = personalDetail.UserPersonalDetailId;
                vm.DOB = personalDetail.DOB;
                vm.FirstName = personalDetail.FirstName;
                vm.LastName = personalDetail.LastName;
                vm.PhoneNo = user.PhoneNumber;
                vm.Email = user.Email;
                vm.CityId = personalDetail.CityId;
                if (personalDetail.Gender == Gender.Male)
                {
                    vm.Gender = Gender.Male;
                }
                else
                {
                    vm.Gender = Gender.Female;
                }
            }
            else
            {
                return NotFound();
            }
            return PartialView(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePersonalDetails(UserPersonalViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetCurrentUser(HttpContext);
                var pd = _context.UserPersonalDetail.Where(p => p.UserId == user.Id).FirstOrDefault();
                pd.FirstName = vm.FirstName;
                pd.LastName = vm.LastName;
                pd.DOB = vm.DOB;
                pd.CityId = vm.CityId;
                if (vm.Gender == 0)
                {
                    pd.Gender = Gender.Male;
                }
                else
                {
                    pd.Gender = Gender.Female;
                }
                user.PhoneNumber = vm.PhoneNo;
                _context.SaveChanges();
            }
            else
            {
                vm.Email = HttpContext.Session.GetString("Email");
                ViewBag.CountryList = new SelectList(GetCountryList(), "CountryId", "CountryName");
                return PartialView("UpdatePersonalDetail", vm);
            }
            return Json(new { success = true });
        }

        public async Task<string> SetUserId()
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            HttpContext.Session.SetString("UserId", user.Id);
            return user.Id;
        }

        public async Task<IActionResult> AddFriend(string UserName)
        {
            string currentUserId = await OnGetSesstion();
            var userToAdd = _context.Users.Where(x => x.UserName == UserName).FirstOrDefault();
            if (userToAdd == null || userToAdd.Id == currentUserId)
                return Json(new { success = false, msg = "Sorry, Unable to send Friend Request :(" });
            int countrecieverfrnds = _context.UserNetwork.Where(y => y.User1 == userToAdd.Id || y.User2 == userToAdd.Id && y.AreFriend == true).Count();
            int countsenderfrnds = _context.UserNetwork.Where(y => y.User1 == currentUserId || y.User2 == currentUserId && y.AreFriend == true).Count();
            if (countrecieverfrnds >= 5000)
            {
                return Json(new { success = false, msg = "Sorry, Reciever has too many friends :(" });
            }
            else
            {
                DateTime lastOneDayReq = DateTime.Now.AddDays(-1);
                int lastDayReqCount = _context.UserNetwork.Where(z => z.User1 == currentUserId && z.RecordTime > lastOneDayReq).Count();
                if(lastDayReqCount>=20)
                    return Json(new { success = false, msg = "Sorry, you can only send 20 requests per day :(" });
                if (countsenderfrnds >= 5000)
                    return Json(new { success = false, msg = "Sorry, you have reached your 5000 friends limit :(" });
            }
            UserNetwork uN = new UserNetwork
            {
                User1 = currentUserId,
                User2 = userToAdd.Id,
                RecordTime = DateTime.Now,
                AreFriend = false
            };
            _context.UserNetwork.Add(uN);
            _context.SaveChanges();
            return Json(new { success = true, msg = "Friend Request Sent!" });
        }
        public async Task<IActionResult> AcceptFriend(string UserName)
        {
            string currentUserId = await OnGetSesstion();
            var userToAdd = _context.Users.Where(x => x.UserName == UserName).FirstOrDefault();
            if (userToAdd == null || userToAdd.Id == currentUserId)
                return Json(new { success = false, msg = "Unable to accept :(" });
            int countrecieverfrnds = _context.UserNetwork.Where(y => y.User1 == currentUserId || y.User2 == currentUserId && y.AreFriend == true).Count();
            if (countrecieverfrnds >= 5000)
            {
                return Json(new { success = false, msg = "Sorry, You already have 5000 friends :(" });
            }
            UserNetwork frnd = _context.UserNetwork.Where(a => a.User1 == userToAdd.Id && a.User2 == currentUserId && a.AreFriend == false).FirstOrDefault();
            frnd.AreFriend = true;
            _context.SaveChanges();
            return Json(new { success = true, msg = "You and "+UserName+" are now Friends!" });
        }
        public async Task<IActionResult> AcceptFriendR(string UserName)
        {
            string currentUserId = await OnGetSesstion();
            var userToAdd = _context.Users.Where(x => x.UserName == UserName).FirstOrDefault();
            if (userToAdd == null || userToAdd.Id == currentUserId)
                return Json(new { success = false, msg = "Unable to accept :(" });
            UserNetwork frnd = _context.UserNetwork.Where(a => a.User1 == userToAdd.Id && a.User2 == currentUserId && a.AreFriend == false).FirstOrDefault();
            frnd.AreFriend = true;
            _context.SaveChanges();
            return Json(new { success = true });
        }
        public async Task<IActionResult> RejectFriend(string UserName)
        {
            string currentUserId = await OnGetSesstion();
            var userToAdd = _context.Users.Where(x => x.UserName == UserName).FirstOrDefault();
            if (userToAdd == null || userToAdd.Id == currentUserId)
                return Json(new { success = false, msg = "Unable to accept :(" });
            UserNetwork frnd = _context.UserNetwork.Where(a => a.User1 == userToAdd.Id && a.User2 == currentUserId && a.AreFriend == false).FirstOrDefault();
            _context.UserNetwork.Remove(frnd);
            _context.SaveChanges();
            return Json(new { success = true, msg = "Friend Request Rejected!" });
        }

        public async Task<IActionResult> CancelFriend(string UserName)
        {
            string currentUserId = await OnGetSesstion();
            var userToCancel = _context.Users.Where(x => x.UserName == UserName).FirstOrDefault();
            var alreadyEsists = _context.UserNetwork.Where(a => (a.User1 == currentUserId || a.User1 == userToCancel.Id) && (a.User2 == currentUserId || a.User2 == userToCancel.Id)).FirstOrDefault();
            if (alreadyEsists == null)
                return Json(new { success = false, msg = "Unable to cancel request!" });
            _context.UserNetwork.Remove(alreadyEsists);
            _context.SaveChanges();
            return Json(new { success = true, msg = "Friend Request Canceled!" });
        }
        public async Task<IActionResult> CountFriendReq(string UserName)
        {
            string currentUserId = await OnGetSesstion();
            int countFriendReq = _context.UserNetwork.Where(x => x.User2 == currentUserId && x.AreFriend == false).Count();
            return Json(new { count = countFriendReq });
        }

        public async Task<IActionResult> FriendRequests()
        {
            string currentUserId = await OnGetSesstion();
            var friendR = (from un in _context.UserNetwork
                           join up in _context.UserPersonalDetail on un.User1 equals up.UserId
                           where un.User2 == currentUserId && un.AreFriend == false
                           select new FriendsVM
                           {
                               Name = up.FirstName + " " + up.LastName,
                               UserName = un.ApplicationUser1.UserName,
                               ProfilePic = up.ProfileImage,
                               ReqTime = un.RecordTime
                           }).ToList();
            return View("_FriendReq", friendR);
        }
        public async Task<IActionResult> MyFriendList()
        {

            string currentUserId = await OnGetSesstion();
            var friendsR = (from un in _context.UserNetwork
                           join up in _context.UserPersonalDetail on un.User1 equals up.UserId
                           where un.User2 == currentUserId && un.AreFriend == true
                           select new FriendsVM
                           {
                               Name = up.FirstName + " " + up.LastName,
                               UserName = un.ApplicationUser1.UserName,
                               ProfilePic = up.ProfileImage,
                               ReqTime = un.RecordTime
                           }).ToList();
            var friendsR2 = (from un in _context.UserNetwork
                            join up in _context.UserPersonalDetail on un.User2 equals up.UserId
                            where un.User1 == currentUserId && un.AreFriend == true
                            select new FriendsVM
                            {
                                Name = up.FirstName + " " + up.LastName,
                                UserName = un.ApplicationUser2.UserName,
                                ProfilePic = up.ProfileImage,
                                ReqTime = un.RecordTime
                            }).ToList();
            friendsR.AddRange(friendsR2);
            return View("_FriendsList",friendsR);
        }
        
        public async Task<IActionResult> ToggleLike(int Id)
        {
            string userloggedId = await OnGetSesstion();
            PostLikes ifLiked = _context.PostLikes.Where(x => x.PostId == Id && x.UserId == userloggedId).FirstOrDefault();
            if (ifLiked == null)
            {
                PostLikes pLike = new PostLikes
                {
                    PostId = Id,
                    UserId = userloggedId
                };
                _context.PostLikes.Add(pLike);
                _context.SaveChanges();
                return Json(new { like = true, unlike = false });
            }
            else
            {
                _context.PostLikes.Remove(ifLiked);
                _context.SaveChanges();
                return Json(new { like = false, unlike = true });
            }
        }

        //User Experience
        public async Task<IActionResult> AddEditUserExperience(int Id)
        {
            ViewBag.CountryList = new SelectList(GetCountryList(), "CountryId", "CountryName");
            ExperienceVM vm = new ExperienceVM();
            if (Id > 0)
            {
                string userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    userId = await SetUserId();
                }
                UserExperience exp = _context.UserExperience.SingleOrDefault(x => x.UserExperienceId == Id && x.UserId == userId);
                vm.Title = exp.Title;
                vm.CompanyName = exp.CompanyName;
                vm.CityId = exp.CityID;
                vm.CurrentWorkCheck = exp.CurrentWorkCheck;
                vm.Description = exp.Description;
                vm.StartDate = exp.StartDate;
                vm.EndDate = exp.EndDate;
                vm.UserExperienceId = Id;
            }
            return PartialView(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateExperience(ExperienceVM vm)
        {
            //var user = await _userManager.GetCurrentUser(HttpContext);
            if (ModelState.IsValid)
            {
                string userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    userId = await SetUserId();
                }
                if (vm.UserExperienceId > 0)
                {
                    UserExperience exp = _context.UserExperience.SingleOrDefault(x => x.UserExperienceId == vm.UserExperienceId && x.UserId == userId);
                    exp.Title = vm.Title;
                    exp.CompanyName = vm.CompanyName;
                    exp.CityID = vm.CityId;
                    exp.CurrentWorkCheck = vm.CurrentWorkCheck;
                    exp.Description = vm.Description;
                    exp.StartDate = vm.StartDate;
                    exp.EndDate = vm.EndDate;
                }
                else
                {
                    UserExperience exp = new UserExperience();
                    exp.Title = vm.Title;
                    exp.CompanyName = vm.CompanyName;
                    exp.CityID = vm.CityId;
                    exp.CurrentWorkCheck = vm.CurrentWorkCheck;
                    exp.Description = vm.Description;
                    exp.StartDate = vm.StartDate;
                    exp.EndDate = vm.EndDate;
                    exp.UserId = userId;
                    _context.UserExperience.Add(exp);
                }
                _context.SaveChanges();
            }
            else
            {
                ViewBag.CountryList = new SelectList(GetCountryList(), "CountryId", "CountryName");
                return PartialView("AddEditUserExperience", vm);
            }
            return Json(new { success = true });
        }

        public async Task<JsonResult> DeleteUserExperience(int Id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            bool result = false;
            UserExperience exp = _context.UserExperience.SingleOrDefault(x => x.UserExperienceId == Id && x.UserId==userId);
            if (exp != null)
            {
                _context.Remove(exp);
                _context.SaveChanges();
                result = true;
            }
            
            return Json(result);
        }

        //User Education
        public async Task<IActionResult> AddEditUserEducation(int Id)
        {
            ViewBag.CountryList = new SelectList(GetCountryList(), "CountryId", "CountryName");
            EducationVM vm = new EducationVM();
            if (Id > 0)
            {
                string userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    userId = await SetUserId();
                }
                UserEducation edu = _context.UserEducation.SingleOrDefault(x => x.UserEducationId == Id && x.UserId == userId);
                vm.Title = edu.Title;
                vm.SchoolName = edu.SchoolName;
                vm.Details = edu.Details;
                vm.StartDate = edu.StartDate;
                vm.EndDate = edu.EndDate;
                vm.CurrentStatusCheck = edu.CurrentStatusCheck;
                vm.CityId = edu.CityId;
                vm.UserEducationID = edu.UserEducationId;
            }
            return PartialView(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEducation(EducationVM vm)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            if (vm.UserEducationID > 0)
            {
                UserEducation edu = _context.UserEducation.SingleOrDefault(x => x.UserEducationId == vm.UserEducationID);
                //int eduId = edu.UserEducationId; 
                //edu = _mapper.Map<UserEducation>(vm);
                //edu.UserEducationId = eduId;
                edu.Title = vm.Title;
                edu.SchoolName = vm.SchoolName;
                edu.CityId = vm.CityId;
                edu.CurrentStatusCheck = vm.CurrentStatusCheck;
                edu.Details = vm.Details;
                edu.StartDate = vm.StartDate;
                edu.EndDate = vm.EndDate;
            }
            else
            {
                var edu = _mapper.Map<UserEducation>(vm);
                edu.UserId = userId;
                _context.UserEducation.Add(edu);
            }
            _context.SaveChanges();
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            return RedirectToAction("Index");
        }

        public async Task<JsonResult> DeleteUserEducation(int Id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            bool result = false;
            UserEducation edu = _context.UserEducation.SingleOrDefault(x => x.UserEducationId == Id && x.UserId == userId);
            if (edu != null)
            {
                _context.Remove(edu);
                _context.SaveChanges();
                result = true;
            }

            return Json(result);
        }

        //Certification
        public async Task<IActionResult> AddEditUserCertification(int Id)
        {
            CertificationVM vm = new CertificationVM();
            if (Id > 0)
            {
                string userId = HttpContext.Session.GetString("UserId");
                if (userId == null)
                {
                    userId = await SetUserId();
                }
                UserCertification cert = _context.UserCertification.SingleOrDefault(x => x.UserCertificationId == Id && x.UserId == userId);
                vm.Name = cert.Name;
                vm.CertificationDate = cert.CertificationDate;
                vm.ExpirationDate = cert.ExpirationDate;
                vm.LiscenceNo = cert.LiscenceNo;
                vm.URL = cert.URL;
                vm.UserCertificationId = cert.UserCertificationId;
            }
            return PartialView(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCertification(CertificationVM vm)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            if (vm.UserCertificationId > 0)
            {
                UserCertification cert = _context.UserCertification.SingleOrDefault(x => x.UserCertificationId == vm.UserCertificationId);
                cert.Name = vm.Name;
                cert.CertificationDate = vm.CertificationDate;
                cert.ExpirationDate = vm.ExpirationDate;
                cert.LiscenceNo = vm.LiscenceNo;
                cert.URL = vm.URL;
            }
            else
            {
                UserCertification cert = new UserCertification();
                cert.Name = vm.Name;
                cert.CertificationDate = vm.CertificationDate;
                cert.ExpirationDate = vm.ExpirationDate;
                cert.LiscenceNo = vm.LiscenceNo;
                cert.URL = vm.URL;
                cert.UserId = userId;
                _context.UserCertification.Add(cert);
            }
            _context.SaveChanges();
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            return RedirectToAction("Index");
            //return View("Index");
        }

        public async Task<JsonResult> DeleteUserCertification(int Id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            bool result = false;
            UserCertification cert = _context.UserCertification.SingleOrDefault(x => x.UserCertificationId == Id && x.UserId == userId);
            if (cert != null)
            {
                _context.Remove(cert);
                _context.SaveChanges();
                result = true;
            }

            return Json(result);
        }

        //Achievments
        public async Task<IActionResult> AddEditUserAchievement(int Id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            AchievmentVM vm = new AchievmentVM();
            if (Id > 0)
            {
                UserAcheivement achiev = _context.UserAcheivement.SingleOrDefault(x => x.UserAchievementId == Id && x.UserId == userId);
                vm.UserAchievementId = achiev.UserAchievementId;
                vm.Description = achiev.Description;
            }
            return PartialView(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAchievment(AchievmentVM vm)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            if (vm.UserAchievementId > 0)
            {
                UserAcheivement achiev = _context.UserAcheivement.SingleOrDefault(x => x.UserAchievementId == vm.UserAchievementId && x.UserId==userId);
                if (achiev != null)
                {
                    achiev.Description = vm.Description;
                }
            }
            else
            {
                UserAcheivement achiev = new UserAcheivement();
                achiev.Description = vm.Description;
                achiev.UserId = userId;
                _context.UserAcheivement.Add(achiev);
            }
            _context.SaveChanges();
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            return RedirectToAction("Index");
            //return View("Index");
        }

        public async Task<JsonResult> DeleteUserAchievment(int Id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            bool result = false;
            UserAcheivement acheivement = _context.UserAcheivement.SingleOrDefault(x => x.UserAchievementId == Id && x.UserId == userId);
            if (acheivement != null)
            {
                _context.Remove(acheivement);
                _context.SaveChanges();
                result = true;
            }

            return Json(result);
        }

        //Hobby
        public async Task<IActionResult> AddEditUserHobby(int Id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            HobbyVM vm = new HobbyVM();
            if (Id > 0)
            {
                UserHobby hobby = _context.UserHobby.SingleOrDefault(x => x.UserHobbyId == Id && x.UserId == userId);
                vm.UserHobbyId = hobby.UserHobbyId;
                vm.HobbyOrIntrest = hobby.HobbyOrIntrest;
            }
            return PartialView(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateHobby(HobbyVM vm)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            if (vm.UserHobbyId > 0)
            {
                UserHobby hobby = _context.UserHobby.SingleOrDefault(x => x.UserHobbyId == vm.UserHobbyId && x.UserId == userId);
                if (hobby != null)
                {
                    hobby.HobbyOrIntrest = vm.HobbyOrIntrest;
                }
            }
            else
            {
                UserHobby hobby = new UserHobby();
                hobby.HobbyOrIntrest = vm.HobbyOrIntrest;
                hobby.UserId = userId;
                _context.UserHobby.Add(hobby);
            }
            _context.SaveChanges();
            var hobbiess = _context.UserHobby.ToList();
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            //return Json(new { hobbies = hobbiess });
            return RedirectToAction("Index");
            //return View("Index");
        }

        public async Task<JsonResult> DeleteUserHobby(int Id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            bool result = false;
            UserHobby hobby = _context.UserHobby.SingleOrDefault(x => x.UserHobbyId == Id && x.UserId == userId);
            if (hobby != null)
            {
                _context.Remove(hobby);
                _context.SaveChanges();
                result = true;
            }

            return Json(result);
        }

        //User Language Skills
        public async Task<IActionResult> AddEditLanguageSkill(int Id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            LanguageSkillVM vm = new LanguageSkillVM();
            if (Id > 0)
            {
                UserLanguageSkill ls = _context.UserLanguageSkill.SingleOrDefault(x => x.LanguageSkillId == Id && x.UserId == userId);
                vm.SkillName = ls.SkillName;
                vm.LanguageSkillId = ls.LanguageSkillId;
            }
            return PartialView(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLS(LanguageSkillVM vm)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            if (vm.LanguageSkillId > 0)
            {
                UserLanguageSkill ls = _context.UserLanguageSkill.SingleOrDefault(x => x.LanguageSkillId == vm.LanguageSkillId && x.UserId == userId);
                if (ls != null)
                {
                    ls.SkillName = vm.SkillName;
                }
            }
            else
            {
                UserLanguageSkill ls = new UserLanguageSkill();
                ls.SkillName = vm.SkillName;
                ls.UserId = userId;
                _context.UserLanguageSkill.Add(ls);
            }
            _context.SaveChanges();
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            return RedirectToAction("Index");
            //return View("Index");
        }

        public async Task<JsonResult> DeleteLanguageSkill(int Id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            bool result = false;
            UserLanguageSkill ls = _context.UserLanguageSkill.SingleOrDefault(x => x.LanguageSkillId == Id && x.UserId == userId);
            if (ls != null)
            {
                _context.Remove(ls);
                _context.SaveChanges();
                result = true;
            }

            return Json(result);
        }


        //User Publication
        public async Task<IActionResult> AddEditPublication(int Id,bool IsJournal)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            PublicationVM vm = new PublicationVM();
            if (Id > 0)
            {
                UserPublication publication = _context.UserPublication.SingleOrDefault(x => x.UserPublicationId == Id && x.UserId == userId);
                vm.Title = publication.Title;
                vm.PublishYear = publication.PublishYear;
                vm.Description = publication.Description;
                vm.UserPublicationId = publication.UserPublicationId;
            }
            vm.ConferenceOrJournal = IsJournal;
            return PartialView(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePublication(PublicationVM vm)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            if (vm.UserPublicationId > 0)
            {
                UserPublication publication = _context.UserPublication.SingleOrDefault(x => x.UserPublicationId == vm.UserPublicationId && x.UserId == userId);
                if (publication != null)
                {
                    publication.Title = vm.Title;
                    publication.PublishYear = vm.PublishYear;
                    publication.Description = vm.Description;
                    publication.ConferenceOrJournal = vm.ConferenceOrJournal;
                }
            }
            else
            {
                UserPublication publication = new UserPublication();
                publication.Title = vm.Title;
                publication.PublishYear = vm.PublishYear;
                publication.Description = vm.Description;
                publication.ConferenceOrJournal = vm.ConferenceOrJournal;
                publication.UserId = userId;
                _context.UserPublication.Add(publication);
            }
            _context.SaveChanges();
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            return RedirectToAction("Index");
            //return View("Index");
        }

        public async Task<JsonResult> DeletePublication(int Id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            bool result = false;
            UserPublication publication = _context.UserPublication.SingleOrDefault(x => x.UserPublicationId == Id && x.UserId == userId);
            if (publication != null)
            {
                _context.Remove(publication);
                _context.SaveChanges();
                result = true;
            }

            return Json(result);
        }

        public async Task<IActionResult> Resume()
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            ProfileViewModal pVM = new ProfileViewModal();
            //Personal Details
            pVM.UserPersonalVM = _context.UserPersonalDetail.Where(m => m.UserId == user.Id).Select(x => new UserPersonalViewModel { FirstName = x.FirstName, LastName = x.LastName, Summary = x.Summary, DOB = x.DOB, UserPersonalDetailID = x.UserPersonalDetailId, Gender = x.Gender, CityName = x.City.CityName, CountryName = x.City.Country.CountryName }).SingleOrDefault();
            pVM.UserPersonalVM.PhoneNo = user.PhoneNumber;
            pVM.UserPersonalVM.Email = user.Email;

            //Education
            pVM.EduVMList = _context.UserEducation.Where(x => x.UserId == user.Id).Select(c => new EducationVM { Title = c.Title, Details = c.Details, SchoolName = c.SchoolName, StartDate = c.StartDate, EndDate = c.EndDate, CurrentStatusCheck = c.CurrentStatusCheck, CityId = c.CityId, CityName = c.City.CityName, CountryName = c.City.Country.CountryName, UserEducationID = c.UserEducationId }).ToList();
            pVM.ExpVMList = _context.UserExperience.Where(x => x.UserId == user.Id).Select(c => new ExperienceVM { Title = c.Title, UserExperienceId = c.UserExperienceId, CityId = c.CityID, CityName = c.City.CityName, CountryName = c.City.Country.CountryName, CompanyName = c.CompanyName, CurrentWorkCheck = c.CurrentWorkCheck, Description = c.Description, StartDate = c.StartDate, EndDate = c.EndDate }).ToList();

            pVM.CertificationVMList = _context.UserCertification.Where(x => x.UserId == user.Id).Select(c => new CertificationVM { Name = c.Name, URL = c.URL, UserCertificationId = c.UserCertificationId, LiscenceNo = c.LiscenceNo, CertificationDate = c.CertificationDate, ExpirationDate = c.ExpirationDate }).ToList();

            pVM.AchievVMList = _context.UserAcheivement.Where(x => x.UserId == user.Id).Select(c => new AchievmentVM { Description = c.Description, UserAchievementId = c.UserAchievementId });

            pVM.HobbyVMList = _context.UserHobby.Where(x => x.UserId == user.Id).Select(c => new HobbyVM { UserHobbyId = c.UserHobbyId, HobbyOrIntrest = c.HobbyOrIntrest });

            pVM.LanguageSkillVMList = _context.UserLanguageSkill.Where(x => x.UserId == user.Id).Select(c => new LanguageSkillVM { LanguageSkillId = c.LanguageSkillId, SkillName = c.SkillName });

            pVM.PublicationVMListJP = _context.UserPublication.Where(x => x.UserId == user.Id && x.ConferenceOrJournal == false).Select(c => new PublicationVM { Title = c.Title, PublishYear = c.PublishYear, Description = c.Description, ConferenceOrJournal = c.ConferenceOrJournal, UserPublicationId = c.UserPublicationId });
            pVM.PublicationVMListCP = _context.UserPublication.Where(x => x.UserId == user.Id && x.ConferenceOrJournal == true).Select(c => new PublicationVM { Title = c.Title, PublishYear = c.PublishYear, Description = c.Description, ConferenceOrJournal = c.ConferenceOrJournal, UserPublicationId = c.UserPublicationId });

            //Get Object from parameter and generate Resume
            //university = new University();
            //university.Name = "CUST";
            //university.Chancler = "Amir";
            //university.PublishedDate = new DateTime(1990, 08, 08);
            //university.City = "Islamabad";
            //university.Country = "Pakistan";
            //university.Students = GetStudents();
            //university.Address = "Kahota Road";

            PDFGenerator userPDF = new PDFGenerator(pVM);
            byte[] abytes = userPDF.PrepareReport();
            return File(abytes, "application/pdf");
        }

        public List<Country> GetCountryList()
        {
            List<Country> countries = _context.Country.OrderBy(x=> x.CountryName).ToList();
            return countries;
        }
        
        public IActionResult GetCitiesList(int CountryId)
        {
            List<City> cities = _context.City.Where(x => x.CountryId == CountryId).OrderBy(x=> x.CityName).ToList();
            ViewBag.CitiesList = new SelectList(cities, "CityId", "CityName");
            return PartialView("CitiesPartial");
        }


        public async Task<JsonResult> DeleteQuestion(int Id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            bool result = false;
            List<UserQAComment> com = _context.UserQAComment.Where(c => c.UserQuestionId == Id).ToList();
            List<UserQAnswer> ans = _context.UserQAnswer.Where(a => a.UserQuestionId == Id).ToList();
            List<UserQAVoting> vot = _context.UserQAVoting.Where(v => v.UserQuestionId == Id).ToList();
            UserQuestion que = _context.UserQuestion.Where(x => x.UserQuestionId == Id).SingleOrDefault();
                
            if (que != null)
            {
                _context.Remove(que);
                _context.SaveChanges();
                result = true;
            }

            return Json(result);
        }

        public async Task<JsonResult> DeleteComment(int Id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            bool result = false;
            UserQAComment com = _context.UserQAComment.Where(c => c.UserQACommentID == Id).SingleOrDefault();
            
            if (com != null)
            {
                _context.Remove(com);
                _context.SaveChanges();
                result = true;
            }

            return Json(result);
        }

        public async Task<JsonResult> DeleteAnswer(int Id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            bool result = false;
            UserQAnswer ans = _context.UserQAnswer.Where(a => a.UserQAnswerId == Id).SingleOrDefault();
            if (ans != null)
            {
                _context.Remove(ans);
                _context.SaveChanges();
                result = true;
            }

            return Json(result);
        }
        public async Task<JsonResult> CheckUserName(string userName)
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            var exists = _userManager.Users.ToList().Find(aa => aa.UserName.Equals(userName.ToLower()));
            if(exists!=null)
                return Json(new { success = false });
            return Json(new { success = true });
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