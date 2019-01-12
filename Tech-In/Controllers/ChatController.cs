using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PusherServer;
using Tech_In.Data;
using Tech_In.Models;
using Tech_In.Models.Database;
using Tech_In.Models.Model;
using Tech_In.Models.ViewModels.ChatViewModels;
using Tech_In.Models.ViewModels.ProfileViewModels;
using Tech_In.Services;

namespace Tech_In.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private Pusher pusher;
        //class constructor
        public ChatController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper)
        {

            _mapper = mapper;
            _context = context;
            _userManager = userManager;

            var options = new PusherOptions();
            options.Cluster = "PUSHER_APP_CLUSTER";
            pusher = new Pusher(
               "PUSHER_APP_ID",
               "PUSHER_APP_KEY",
               "PUSHER_APP_SECRET",
               options
           );
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            ViewBag.allUsers = _context.UserPersonalDetail.Where(a => a.UserId != user.Id).Select(c => new UserPersonalViewModel
            {
                UserID = c.UserId,
                ProfileImage = c.ProfileImage,
                FirstName = c.FirstName,
                LastName = c.LastName
            }).ToList();
            ViewBag.currentUser = user.Id;
            return View();
        }

        public async Task<IActionResult> Conversation(string contact)
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
           
            var conversations = _context.Conversation.Where(c => (c.RecieverId ==user.Id  && c.SenderId == contact) || (c.RecieverId == contact && c.SenderId == user.Id))
                                  .OrderBy(c => c.CreatedAt)
                                  .ToList();
            //return Json( new { status = "success", data = conversations }/*,JsonRequestBehavior.AllowGet*/);
            return View("Index", conversations);
        }

        [HttpPost]
        public async Task<JsonResult> SendMessage()
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            var userId = Convert.ToInt32(user.Id);
            string socket_id = Request.Form["socket_id"];
            Conversation convo = new Conversation
            {
                SenderId = user.Id,
                Message = Request.Form["message"],
                RecieverId = Request.Form["contact"]
              
            };
            _context.Conversation.Add(convo);
            _context.SaveChanges();
            
            var conversationChannel = GetConvoChannel(userId, Convert.ToInt32(Request.Form["contact"]));
            await pusher.TriggerAsync(
              conversationChannel,
              "new_message",
              convo,
              new TriggerOptions() { SocketId = socket_id });
            return Json(convo);
        }
      
        private String GetConvoChannel(int user_id, int contact_id)
        {
            if (user_id > contact_id)
            {
                return "private-chat-" + contact_id + "-" + user_id;
            }
            return "private-chat-" + user_id + "-" + contact_id;
        }
       
    }
}