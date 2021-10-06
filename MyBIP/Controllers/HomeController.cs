using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyBIP.Components.HtmlTemplate.Extra;
using MyBIP.Components.StoreProcedure;
using MyBIP.Components.StoreProcedure.Extra;
using MyBIP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyBIP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            //Read Template And Send Message
            UseTemplateAndSendMail useTemplateAndSendMail = new UseTemplateAndSendMail(_env);
            useTemplateAndSendMail.SendCreateUserTemplateEmail();
            return View();
        }

        public IActionResult GetProcedure()
        {
            //StoreProcedure Getting And Read Data
            long total;
            UserListSearchRequest userListSearchRequest = new UserListSearchRequest();
            userListSearchRequest.BrandId = 1;
            userListSearchRequest.PageNumber = 1;
            userListSearchRequest.PageSize = 20;
            var result = UserBusinessLogic.ListSearch(userListSearchRequest,out total).ResultValue;
            return View();
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
