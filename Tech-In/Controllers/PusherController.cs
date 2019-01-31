using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PusherServer;
using Tech_In.Data;
using Tech_In.Models;
using Tech_In.Services;

namespace Tech_In.Controllers
{
    public class PusherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> Auth(string channel_name, string socket_id)
        {
            var user = await _userManager.GetCurrentUser(HttpContext);
            var userId = Int32.Parse(user.Id);
            var options = new PusherOptions
            {
                Cluster = "us2",
                Encrypted = true
            };

            var pusher = new Pusher( "687183", "32faf5983beaa45f0d69","f63fefed03842ede52f9",options);
            if (channel_name.IndexOf(userId.ToString()) == -1)
            {
                return Json(
                  new { status = "error", message = "User cannot join channel" }
                );
            }
            var auth = pusher.Authenticate(channel_name, socket_id);
            return Json(auth);
        }
    }
}