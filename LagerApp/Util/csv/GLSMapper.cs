using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagerApp.Util.csv
{
    public class GLSMapper : CsvClassMap<GLSFile>
    {
        public GLSMapper()
        {
            Map(m => m.DrAuftragsnr).Index(0);
            Map(m => m.KundenName).Index(1);
            Map(m => m.SonderFeld).Index(2);
            Map(m => m.Straße).Index(3);
            Map(m => m.SonderFeld2).Index(4);
            Map(m => m.PLZ).Index(5);
            Map(m => m.Stadt).Index(6);
            Map(m => m.SonderFeld2).Index(7);
            Map(m => m.SonderFeld3).Index(8);
            Map(m => m.Land).Index(9);
            Map(m => m.SonderFeld4).Index(10);
            Map(m => m.SonderFeld5).Index(11);
            Map(m => m.ArtikelName).Index(12);
            Map(m => m.SonderFeld6).Index(13);
            Map(m => m.SonderFeld7).Index(14);
            Map(m => m.SonderFeld8).Index(15);
            Map(m => m.Mail).Index(16);
            Map(m => m.SonderFeld9).Index(17);
           // Map(m => m.ArtikelNr).Index(18);
        }
    }
}
