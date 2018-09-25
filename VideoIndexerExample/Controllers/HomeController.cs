using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VideoIndexerExample.Models;

namespace VideoIndexerExample.Controllers
{
    public class HomeController : Controller
    {
        internal VIServiceWrapper vi = new VIServiceWrapper("accountID Guid","apiKey", "location, ie. westus2","location subdomain, ie. wus2", "https://api.videoindexer.ai/");
        public IActionResult Index()
        {
            var videos = AsyncHelper.RunSync(()=> vi.GetAllVideos(500));
            return View(videos);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Edit(string id)
        {
            ViewBag.PlayerUrl = AsyncHelper.RunSync(() => vi.GetPlayerEmbedUrl(id));
            ViewBag.InsightsUrl = AsyncHelper.RunSync(() => vi.GetInsightsEmbedUrl(id));
            ViewBag.TitleFunction = "GetAccessToken()";
            return View();
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
