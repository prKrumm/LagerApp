﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagerApp.Model
{
    public class Artikel
    {
        public string ArtikelId { get; set; }
        public string DRAuftragsnr { get; set; }
        public string LagerPlatz { get; set; }
        public string LagerPlatzOhneBox { get; set; }
        public string LagerBox { get; set; }
        public string ArtikelBezeichnung { get; set; }
        public string created_at { get; set; }
        public Boolean foundArticleNr{ get; set; }
    }
}
