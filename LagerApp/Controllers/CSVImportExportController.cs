using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace LagerApp.Controllers
{
    
    [Route("api/CSV")]
    public class CSVImportExportController : Controller
    {
        private readonly IHostingEnvironment _environment;

        public CSVImportExportController(IHostingEnvironment hostingEnvironment)
        {
            _environment = hostingEnvironment;
        }

        [Route("new")]
        [HttpPost]
        public async Task<IActionResult> PickListe(IFormFile file)
        {
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            
                if (file.Length > 0)
                {
                    using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            
            return View();
        }


    }
}