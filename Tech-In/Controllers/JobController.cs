
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tech_In.Controllers
{
    public class JobController : Controller
    {
        public IActionResult Index()
        {
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            return View();
        }

        public IActionResult JobSingle()
        {
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            return View();
        }

        public IActionResult PostJob()
        {
            @ViewBag.UName = HttpContext.Session.GetString("Name");
            return View();
        }

    }
}