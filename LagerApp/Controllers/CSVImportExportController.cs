using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using CsvHelper;
using LagerApp.Util.csv;
using Microsoft.Extensions.FileProviders;

namespace LagerApp.Controllers
{

    [Route("api/CSV")]
    public class CSVImportExportController : Controller
    {
        private readonly IHostingEnvironment _environment;
        private readonly IFileProvider _fileProvider;

        public CSVImportExportController(IHostingEnvironment hostingEnvironment, IFileProvider fileProvider)
        {
            _environment = hostingEnvironment;
            _fileProvider = fileProvider;
        }

        [Route("new")]
        [HttpPost]
        public async Task<IActionResult> PickListe(IFormFile file)
        {
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");

            if (file.Length > 0)
            {
                StreamReader streamReader = new StreamReader(file.OpenReadStream());
                using (var csv = new CsvReader(streamReader))
                {


                    csv.Configuration.RegisterClassMap<GLSMapper>();
                    csv.Configuration.HasHeaderRecord = false;

                    IEnumerable<GLSFile> glsList = csv.GetRecords<GLSFile>().ToList(); ;

                    

                    using (var f = new FileStream(Path.Combine(uploads, "GLS.csv"), FileMode.Create))
                    {
                    

                        StreamWriter writer = new StreamWriter(f);
                        CsvWriter glsoutput = new CsvWriter(writer);
                        glsoutput.Configuration.RegisterClassMap<GLSMapper>();
                        glsoutput.Configuration.HasHeaderRecord = false;

                        glsoutput.WriteRecords(glsList);

                    }
                }

            }





            return View();
        }


    }
}