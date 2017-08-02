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
using System.Text;
using LagerApp.Model;
using LagerApp.Util;
using System.Text.RegularExpressions;

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
            var uploads = Path.Combine(_environment.WebRootPath, "uploads/GLS");
            List<Artikel> list = new List<Artikel>();
            if (file!=null&&file.Length > 0)
            {
                StreamReader streamReader = new StreamReader(file.OpenReadStream(), Encoding.UTF7);
                using (var csv = new CsvReader(streamReader))
                {                    
                    csv.Configuration.RegisterClassMap<GLSMapper>();
                    csv.Configuration.HasHeaderRecord = false;
                    //csv.Configuration.Encoding = Encoding.UTF7;
                    IEnumerable<GLSFile> glsList = csv.GetRecords<GLSFile>().ToList(); ;
                    glsList = addArtikelNr(glsList);
                    list = printPickliste(glsList);

                    String fileName = "GLS_" + DateTime.Today.ToString().Substring(0,10).Replace(".", "") + ".csv";
                    using (var f = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                    {
                    

                        StreamWriter writer = new StreamWriter(f, Encoding.UTF8);
                        CsvWriter glsoutput = new CsvWriter(writer);
                        glsoutput.Configuration.RegisterClassMap<GLSMapperWrite>();
                        glsoutput.Configuration.HasHeaderRecord = false;

                        glsoutput.WriteRecords(glsList);
                        if (csv.Context.Row == glsoutput.Context.Row)
                        {
                            ViewData["SuccessMessage"] = "Erfolgreich " + (glsoutput.Context.Row - 1) + " Adressen verarbeitet";
                            glsoutput.Dispose();
                            return View(list);
                        }
                        else
                        {
                            ViewData["SuccessMessage"] = "Etwas hat nicht geklappt! " + (glsoutput.Context.Row - 1) + " Adressen verarbeitet";
                            glsoutput.Dispose();
                            return View(list);
                        }
                        

                    }
                }

            }





            return View();
        }

        private List<Artikel> printPickliste(IEnumerable<GLSFile> glsFile)
        {
            List<Artikel> list = new List<Artikel>();
            LagerContext con = HttpContext.RequestServices.GetService(typeof(LagerApp.Model.LagerContext)) as LagerContext;
            foreach (var item in glsFile)
            {
                Artikel artikel = con.GetLagerPlatzByArtikel(item.ArtikelNr);
                artikel.ArtikelId = item.ArtikelNr;
                artikel.ArtikelBezeichnung = item.ArtikelName;
                list.Add(artikel);
            }
            return list;
        }

        private IEnumerable<GLSFile> addArtikelNr(IEnumerable<GLSFile> glsFile)
        {
            foreach (var item in glsFile)
            {
                String artikelName = item.ArtikelName;
                
                item.ArtikelNr = extractArtikelNr(artikelName);
            }
            return glsFile;
        }

        private String extractArtikelNr(String ArtikelBezeichnung)
        {
            //filter A15000
            Regex regex=new Regex(Config.Nummern);
            Match match = regex.Match(ArtikelBezeichnung);

            if (match.Success)
            {
                Console.WriteLine("Found Match for {0}", match.Value);
                String ArtikelNr = match.Value;
                return ArtikelNr;
            }
            else
            {
                return null;
            }

        }


    }
}