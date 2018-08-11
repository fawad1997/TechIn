using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tech_In.Data;
using Tech_In.Models;
using Tech_In.Models.Database;
using Tech_In.Models.Model;
using Tech_In.Models.ViewModels.ProfileViewModels;
using Tech_In.Services;

namespace Tech_In.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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


            ProfileViewModal PVM = new ProfileViewModal();

            PVM.UserPersonalVM = _context.UserPersonalDetail.Where(m => m.UserId == user.Id).Select(x => new UserPersonalViewModel { FirstName = x.FirstName,LastName=x.LastName,Summary=x.Summary,ProfileImage = x.ProfileImage,DOB=x.DOB, UserPersonalDetailID = x.UserPersonalDetailId, Gender = x.Gender,CityName = x.City.CityName, CountryName = x.City.Country.CountryName }).SingleOrDefault();
            
            PVM.UserPersonalVM.PhoneNo = user.PhoneNumber;
            PVM.UserPersonalVM.Email = user.Email;



            List<ExperienceVM> userExperienceList = _context.UserExperience.Where(x => x.UserId == user.Id).Select(c => new ExperienceVM { Title = c.Title, UserExperienceId = c.UserExperienceId, CityId = c.CityID, CityName = c.City.CityName, CountryName=c.City.Country.CountryName,CompanyName = c.CompanyName, CurrentWorkCheck = c.CurrentWorkCheck, Description = c.Description, StartDate = c.StartDate, EndDate = c.EndDate }).ToList();
            ViewBag.UserExperienceList = userExperienceList;

            List<EducationVM> userEducationList = _context.UserEducation.Where(x => x.UserId == user.Id).Select(c => new EducationVM { Title = c.Title, Details = c.Details, SchoolName = c.SchoolName, StartDate = c.StartDate, EndDate = c.EndDate, CurrentStatusCheck = c.CurrentStatusCheck, CityId = c.CityId, CityName = c.City.CityName, CountryName = c.City.Country.CountryName, UserEducationID=c.UserEducationId }).ToList();
            ViewBag.UserEducationList = userEducationList;

            List<CertificationVM> userCertificationList = _context.UserCertification.Where(x => x.UserId == user.Id).Select(c => new CertificationVM { Name = c.Name, URL = c.URL, UserCertificationId = c.UserCertificationId, LiscenceNo = c.LiscenceNo, CertificationDate = c.CertificationDate, ExpirationDate = c.ExpirationDate }).ToList();
            ViewBag.UserCertificationList = userCertificationList;

            PVM.AchievVMList = _context.UserAcheivement.Where(x => x.UserId == user.Id).Select(c => new AchievmentVM { Description = c.Description, UserAchievementId = c.UserAchievementId });

            PVM.HobbyVMList = _context.UserHobby.Where(x => x.UserId == user.Id).Select(c => new HobbyVM { UserHobbyId = c.UserHobbyId, HobbyOrIntrest = c.HobbyOrIntrest });

            PVM.LanguageSkillVMList = _context.UserLanguageSkill.Where(x => x.UserId == user.Id).Select(c => new LanguageSkillVM { LanguageSkillId = c.LanguageSkillId, SkillName = c.SkillName });

            PVM.PublicationVMListJP = _context.UserPublication.Where(x => x.UserId == user.Id && x.ConferenceOrJournal==false).Select(c => new PublicationVM { Title = c.Title, PublishYear = c.PublishYear, Description = c.Description, ConferenceOrJournal = c.ConferenceOrJournal, UserPublicationId = c.UserPublicationId });
            PVM.PublicationVMListCP = _context.UserPublication.Where(x => x.UserId == user.Id && x.ConferenceOrJournal == true).Select(c => new PublicationVM { Title = c.Title, PublishYear = c.PublishYear, Description = c.Description, ConferenceOrJournal = c.ConferenceOrJournal, UserPublicationId = c.UserPublicationId });
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            return View(PVM);
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
            //@ViewBag.UName = HttpContext.Session.GetString("Name");
            //return RedirectToAction("Index");
            //return View("Index");
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
           // var user = await _userManager.GetCurrentUser(HttpContext);
            string userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                userId = await SetUserId();
            }
            if (vm.UserEducationID > 0)
            {
                UserEducation edu = _context.UserEducation.SingleOrDefault(x => x.UserEducationId == vm.UserEducationID);
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
                UserEducation edu = new UserEducation();
                edu.Title = vm.Title;
                edu.SchoolName = vm.SchoolName;
                edu.CityId = vm.CityId;
                edu.CurrentStatusCheck = vm.CurrentStatusCheck;
                edu.Details = vm.Details;
                edu.StartDate = vm.StartDate;
                edu.EndDate = vm.EndDate;
                edu.UserId = userId;
                _context.UserEducation.Add(edu);
            }
            _context.SaveChanges();
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            return RedirectToAction("Index");
            //return View("Index");
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
        
    }
}