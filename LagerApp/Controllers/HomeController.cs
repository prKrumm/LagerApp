using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LagerApp.Model;
using System.Net.Http;
using Microsoft.AspNetCore.NodeServices;

namespace LagerApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            LagerContext con=HttpContext.RequestServices.GetService(typeof(LagerApp.Model.LagerContext)) as LagerContext;
            ViewData["AnzArtikel"]=con.GetNumberOfArticles();
            ViewData["AnzBoxen"] = con.GetNumberOfBoxes();
            return View(con.GetAllArtikelLimit());
        }

        [HttpGet]
        public async Task<IActionResult> About([FromServices] INodeServices nodeServices)
        {
            HttpClient hc = new HttpClient();
            var htmlContent = await hc.GetStringAsync($"http://{Request.Host}/uploads/report.html");

            var result = await nodeServices.InvokeAsync<byte[]>("./pdf", htmlContent);

            HttpContext.Response.ContentType = "application/pdf";

            HttpContext.Response.Headers.Add("x-filename", "report.pdf");
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "x-filename");
            HttpContext.Response.Body.Write(result, 0, result.Length);

            return View();
        }

        public IActionResult Scan()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult PickListe()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }


        public IActionResult Error()
        {
            return View();
        }
    }
}
