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
        internal VIServiceWrapper vi = new VIServiceWrapper("apiKey", "location", "accountId", "https://api.videoindexer.ai/");
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

        public IActionResult IFrameToken(string id)
        {
            ViewBag.AccessToken = AsyncHelper.RunSync(() => vi.GetAccessToken());
            ViewBag.AccountId = vi.AccountId;
            ViewBag.Id = id;
            ViewBag.TitleFunction = "GetAccessToken()";
            return View();
        }

        public IActionResult IFrameVideoToken(string id)
        {
            ViewBag.AccessToken = AsyncHelper.RunSync(() => vi.GetVideoAccessToken(id));
            ViewBag.AccountId = vi.AccountId;
            ViewBag.Id = id;
            ViewBag.TitleFunction = "GetVideoAccessToken()";
            return View("IFrameToken");
        }
        public IActionResult IFrameUserToken(string id)
        {
            ViewBag.AccessToken = AsyncHelper.RunSync(() => vi.GetUserAccessToken());
            ViewBag.AccountId = vi.AccountId;
            ViewBag.Id = id;
            ViewBag.TitleFunction = "GetUserAccessToken()";
            return View("IFrameToken");
        }
        public IActionResult APIIframe(string id)
        {
            ViewBag.PlayerHtml = System.Web.HttpUtility.HtmlEncode(AsyncHelper.RunSync(() => vi.GetPlayerHtml(id)));
            ViewBag.InsightsHtml = System.Web.HttpUtility.HtmlEncode(AsyncHelper.RunSync(() => vi.GetInsightsHtml(id, true)));
            ViewBag.Id = id;
            return View();
        }

        public IActionResult PlayerFrame(string id)
        {
            ViewBag.Html = AsyncHelper.RunSync(() => vi.GetPlayerHtml(id));
            return View("FrameSrc");
        }

        public IActionResult InsightsFrame(string id)
        {
            ViewBag.Html = AsyncHelper.RunSync(() => vi.GetInsightsHtml(id, true));
            return View("FrameSrc");
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
