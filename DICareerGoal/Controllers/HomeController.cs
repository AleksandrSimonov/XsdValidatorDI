using DICareerGoal.Models;
using DICareerGoal.Validator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMessageValidator _messageValidator;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, IMessageValidator messageValidator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _messageValidator = messageValidator ?? throw new ArgumentNullException(nameof(messageValidator));
        }

        [HttpGet]
        public ActionResult Index() => View(new UploadViewModel());



        [HttpPost]
        public ActionResult UploadFile(IEnumerable<IFormFile> files)
        {
            try
            {
                string fileName = string.Empty;
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
                        string dirPath = Path.Combine(_webHostEnvironment.WebRootPath, "App_Data");
                        Directory.CreateDirectory(dirPath);
                        if (file.Length > 0)
                        {
                            using (var stream = new FileStream(Path.Combine(dirPath, fileName), FileMode.Create))
                        {
                                file.CopyTo(stream);
                            }
                        }
                    }
                }

                return Json(new { isSuccess = true, fileName = fileName });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, ex = ex});
            }
        }

        [HttpPost]
        public ActionResult RemoveFile(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    string physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, "~/App_Data", fileName);

                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }

                }
                return Json(new { isSuccess = true, fileName = fileName }, "text/plain");
            }
            catch (Exception)
            {
                return Json(new { isSuccess = false });
            }
        }

        [HttpPost]
        public ActionResult ValidateXmlByXsd(string fileName)
        {
            string fileFullName = Path.Combine(_webHostEnvironment.WebRootPath, "App_Data", fileName);

            ValidationResult result = _messageValidator.ValidateFile(fileFullName);

            return Json(
                new
                {
                    isSuccess = result.IsValid,
                    message = result.Message
                });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
