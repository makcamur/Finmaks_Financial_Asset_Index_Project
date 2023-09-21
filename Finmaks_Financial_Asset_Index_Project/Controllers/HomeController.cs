using Finmaks_Financial_Asset_Index_Project.DataAccess.Data.DTOs;
using Finmaks_Financial_Asset_Index_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json;
using System.Xml.Linq;

namespace Finmaks_Financial_Asset_Index_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("Home/Get")]
        public IActionResult GetPartial(string jsonData)
        {
            var model = JsonConvert.DeserializeObject<AssetIndexExchangeFinalTableDTO>(jsonData);
            return PartialView("_GetPartial", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}