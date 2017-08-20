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

    public class CSVImportExportController : Controller
    {
        private readonly IHostingEnvironment _environment;
        private readonly IFileProvider _fileProvider;

        public CSVImportExportController(IHostingEnvironment hostingEnvironment, IFileProvider fileProvider)
        {
            _environment = hostingEnvironment;
            _fileProvider = fileProvider;
        }


        [HttpGet("/lager/{id}", Name = "Lager_List")]
        public IActionResult Lager(String id)
        {
            string artikelNr = id.Trim();
            LagerContext con = HttpContext.RequestServices.GetService(typeof(LagerApp.Model.LagerContext)) as LagerContext;
            
            Artikel artikel=con.GetLagerPlatzByArtikel(artikelNr);
            return Json(artikel);
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

                        //glsoutput.WriteRecords(glsList);
                        if (csv.Context.Row == glsoutput.Context.Row)
                        {
                            ViewData["SuccessMessage"] = "Erfolgreich " + (glsoutput.Context.Row - 1) + " Bestellungen verarbeitet";
                            glsoutput.Dispose();
                            List<Artikel> SortedList = list.OrderByDescending(o => o.foundArticleNr).ThenBy(o => o.LagerPlatz).ThenBy(o => o.LagerBox).ToList();
                            return View(SortedList);
                        }
                        else
                        {
                            ViewData["SuccessMessage"] = "Etwas hat nicht geklappt!!!!!!!!!!!! " + (glsoutput.Context.Row - 1) + " Adressen verarbeitet";
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
                //Mehrere Artikel pro Bestellung
                if (item.ArtikelNr!=null&&item.ArtikelNr.Contains(","))
                {
                    String[] artikelNrn = item.ArtikelNr.Split(',');
                    String[] artikelBzn = item.ArtikelName.Split('/');
                    for (int i = 0; i < artikelBzn.Length - 1; i++)
                    {
                        if (i < artikelBzn.Length - 1 && i < artikelNrn.Length)
                        {
                            Artikel artikel = con.GetLagerPlatzByArtikel(artikelNrn[i]);
                            if (!String.IsNullOrEmpty(artikel.LagerPlatz))
                            {
                                artikel.foundArticleNr = true;
                            }
                            
                            artikel.DRAuftragsnr = item.DrAuftragsnr;
                            artikel.ArtikelId = artikelNrn[i];
                            artikel.ArtikelBezeichnung = artikelBzn[i];
                            list.Add(artikel);
                        }
                    }
                }
                else
                {

                    Artikel artikel = con.GetLagerPlatzByArtikel(item.ArtikelNr);
                    if (!String.IsNullOrEmpty(artikel.LagerPlatz))
                    {
                        artikel.foundArticleNr = true;
                    }
                    artikel.DRAuftragsnr = item.DrAuftragsnr;
                    artikel.ArtikelId = item.ArtikelNr;
                    artikel.ArtikelBezeichnung = item.ArtikelName;
                    list.Add(artikel);

                }
                
               
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
            Regex regex = new Regex(Config.Nummern);
            MatchCollection matches = regex.Matches(ArtikelBezeichnung);
            if (matches.Count > 0) { 
            String ArtikelNr = "";
                var zaehler = 0;
            foreach (var item in matches)
            {
                    if (zaehler == 0)
                    {
                        ArtikelNr = item.ToString();
                    }
                    else
                    {
                        ArtikelNr = ArtikelNr+","+item.ToString();       
                    }
                    zaehler++;
            }
                return ArtikelNr;
            }
            else
            {
                return null;
            }

        }


    }
}