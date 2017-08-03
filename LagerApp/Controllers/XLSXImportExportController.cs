using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.IO;
using System.Text;
using LagerApp.Util.xlsx;
using System.Text.RegularExpressions;
using LagerApp.Util;
using LagerApp.Model;
using LagerApp.DTOs;

namespace LagerApp.Controllers
{

    public class XLSXImportExportController : Controller
    {
        [Route("scan")]
        [HttpPost]
        public IActionResult Scan(IFormFile file)
        {
            //FileInfo file = new FileInfo();
            StreamReader streamReader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            List<Scan> scanList = new List<Scan>();
            List<Scan> successList=new List<Scan>();
            List<Scan> failList = new List<Scan>();
            using (ExcelPackage package = new ExcelPackage(streamReader.BaseStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = 2;               
                for (int row = 2; row <= rowCount; row++)
                {
                    Scan scan = new Scan();
                    for (int col = 1; col <= ColCount; col++)
                    {

                        if (col == 1)
                        {
                            scan.lager_fach = worksheet.Cells[row, col].Value.ToString();
                        }
                        else
                        {
                            scan.druck_pseudonym = worksheet.Cells[row, col].Value.ToString();
                        }

                    }
                    scanList.Add(scan);
                }

            }
            LagerContext con = HttpContext.RequestServices.GetService(typeof(LagerApp.Model.LagerContext)) as LagerContext;
            ScanVariantenChecker checker = new ScanVariantenChecker();
            //Über alle Einträge in Liste iterieren
            foreach (var item in scanList)
            {
                switch (checker.CheckLine(item))
                {
                    case ScanVarianten.ArtikelZuBox:
                        LagerBoxDTO dto=new LagerBoxDTO();
                        dto.ArtikelId = item.druck_pseudonym;
                        dto.LagerBox = item.lager_fach;
                        if (con.saveOrUpdateToBox(dto) > 0)
                        {
                            successList.Add(item);
                        }
                        else
                        {
                            failList.Add(item);
                        }                      
                        break;
                    case ScanVarianten.ArtikelZuPlatz:
                        ArtikelLagerPlatzDTO dto2 = new ArtikelLagerPlatzDTO();
                        dto2.ArtikelId = item.druck_pseudonym;
                        dto2.LagerPlatz = item.lager_fach;                       
                        if (con.saveOrUpdateArtikelToPlatz(dto2) > 0)
                        {
                            successList.Add(item);
                        }
                        else
                        {
                            failList.Add(item);
                        }
                        break;
                    case ScanVarianten.BoxZuPlatz:
                        LagerPlatzDTO dto3 = new LagerPlatzDTO();
                        dto3.LagerBox = item.druck_pseudonym;
                        dto3.LagerPlatz = item.lager_fach;                        
                        if (con.saveOrUpdateBoxToPlatz(dto3) > 0)
                        {
                            successList.Add(item);
                        }
                        else
                        {
                            failList.Add(item);
                        }
                        break;
                    default:
                        failList.Add(item);
                        break;                       
                }


            }
            LoggerXLSX log = new LoggerXLSX();
            log.fail = failList;
            log.success = successList;
            return View(log);
        }
    } 
}