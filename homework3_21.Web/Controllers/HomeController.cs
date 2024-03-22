using homework3_21.Data;
using homework3_21.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;


namespace homework3_21.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _connectionString;

        public HomeController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
            _environment = environment;
        }
        public IActionResult Index()
        {
            ImageRepository repo = new ImageRepository(_connectionString);
            HomeViewModel vm = new()
            {
                Images = repo.GetAll()
            };
            return View(vm);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile imageFile, string title)
        {
            var fileName = $"{Guid.NewGuid()}-{imageFile.FileName}";
            var fullImagePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

            using FileStream fs = new FileStream(fullImagePath, FileMode.Create);
            imageFile.CopyTo(fs);

            var repo = new ImageRepository(_connectionString);
            repo.Add(title, fileName);
            return Redirect("/home/index");
        }

        public IActionResult ViewImage(int id)
        {
            ImageRepository repo = new ImageRepository(_connectionString);
            Data.Image i = repo.GetById(id);
            ImageViewModel vm = new();
            vm.Image = i;
            vm.Ids = GetIdsFromSession();
            return View(vm);
        }

        [HttpPost]
        public void IncrementLikes(int id)
        {
            ImageRepository repo = new ImageRepository (_connectionString);
            repo.IncrementLikes(id);
            List<int> sessionIds = GetIdsFromSession();
            sessionIds.Add(id);
            HttpContext.Session.Set("ids", sessionIds);
        }

        public IActionResult GetLikesById(int id)
        {
            ImageRepository repository = new ImageRepository(_connectionString);
            int likes = repository.GetLikes(id);
            return Json(likes);
        }

        public List<int> GetIdsFromSession()
        {
            List<int> sessionIds = HttpContext.Session.Get<List<int>>("ids");
            if (sessionIds == null)
            {
                return new List<int>();
            }
            return sessionIds;
        }
    }
}
public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T Get<T>(this ISession session, string key)
    {
        string value = session.GetString(key);

        return value == null ? default(T) :
            JsonSerializer.Deserialize<T>(value);
    }
}