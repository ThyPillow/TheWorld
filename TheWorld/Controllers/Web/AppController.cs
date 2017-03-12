using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Web
{
    public class AppController : Controller
    {
        private IMailService _mailService;
        private IConfigurationRoot _config;
        private IWorldRepository _worldRepository;
        private ILogger<AppController> _logger;

        public AppController(IMailService mailService, IConfigurationRoot config, IWorldRepository worldRepository, ILogger<AppController> logger)
        {
            _mailService = mailService;
            _config = config;
            _worldRepository = worldRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
             return View();
        }

        [Authorize]
        public IActionResult Trips()
        {
            return View();
        }


        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if (model.Email.Contains("aol.com"))
            {
                ModelState.AddModelError("Email", "Property We don't support AOL addresses");
                ModelState.AddModelError(String.Empty, "Model We don't support AOL addresses");
            }
            if (ModelState.IsValid)
            {
                _mailService.SendMail(_config["MailSettings:ToAddress"], model.Email, "From The World", model.Message);
                ModelState.Clear();
                ViewBag.UserMessage = "Message Sent !";
            }
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
