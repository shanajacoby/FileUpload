using FileUpload.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FileUpload.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using FileUpload.Web.Models;
using Newtonsoft.Json;

namespace FileUpload.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = "Data Source=.\\sqlexpress;Initial Catalog=FileUpload;Integrated Security=True";
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly string ImageIdsSession = "ImageIdsAndPassword";
        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Upload(Image image, IFormFile imagefile)
        {
            string fileName = $"{Guid.NewGuid()}-{imagefile.FileName}";
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(filePath, FileMode.CreateNew);
            imagefile.CopyTo(fs);
            var db = new FileUploadRepository(_connectionString);
            image.ImagePath = filePath;
            image.Id=db.Add(image);
            
            return View(image);
        }
        public IActionResult EnterPassword(int id)
        {
            var imageIds = HttpContext.Session.Get<List<int>>("imageIdsAndPwd");
            if (imageIds != null && imageIds.Contains(id))
            {
                return Redirect($"home/viewImage?id={id}");
            }
            return View(new PasswordViewModel
            {
                Id = id,
                Message = (string)TempData["IncorrectPassword"]
            }); 

        }
        public IActionResult ViewImage(int id)
        {
            var imageIds = HttpContext.Session.Get<List<int>>("imageIdsAndPwd");
            if(imageIds== null || !imageIds.Contains(id))
            {
                return Redirect($"/home/enterpassword?id={id}");
            }
            var db = new FileUploadRepository(_connectionString);
            Image image = db.GetImageById(id);
            db.UpdateImages(id);
            return View(image);
        }
        [HttpPost]
        public IActionResult ViewImage(int id, string password)
        {
            var db = new FileUploadRepository(_connectionString);
            string correctPassword = db.GetPassword(id);
            var imageIds = HttpContext.Session.Get<List<int>>("imageIdsandPwd");
            if(imageIds== null)
            {
                imageIds = new List<int>();
            }
            if (password == correctPassword)
            {
                imageIds.Add(id);
                HttpContext.Session.Set("imageIdsAndPwd", imageIds);
                return Redirect($"/home/viewimage?id={id}");
            }
            else
            {
                TempData["Message"] = "Incorrect Password! Please try again";
                return Redirect($"/home/enterpassword?id={id}");
            }
        }


    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
}
