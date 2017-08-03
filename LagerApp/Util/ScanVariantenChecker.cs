using LagerApp.Util.xlsx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LagerApp.Util
{
    public class ScanVariantenChecker
    {
        public ScanVarianten CheckLine(Scan scan)
        {
            // check: druck_pseudonym =A15000
            Regex regexNr = new Regex(Config.Nummern);
            Regex regexPlatz = new Regex(Config.lagerPlätze);
            Regex regexBox = new Regex(Config.Boxen);


            Match matchNrDruck = regexNr.Match(scan.druck_pseudonym);
            Match matchBoxDruck = regexBox.Match(scan.druck_pseudonym);
            Match matchPlatzPlatz = regexPlatz.Match(scan.lager_fach);
            Match matchBoxPlatz = regexBox.Match(scan.lager_fach);

            if (matchNrDruck.Success && matchPlatzPlatz.Success)
            {
                return ScanVarianten.ArtikelZuPlatz;
            }

            if (matchNrDruck.Success && matchBoxPlatz.Success)
            {
                return ScanVarianten.ArtikelZuBox;
            }

            if (matchBoxDruck.Success && matchPlatzPlatz.Success)
            {
                return ScanVarianten.BoxZuPlatz;
            }

            return ScanVarianten.unbekannt;

        }

    }



    public enum ScanVarianten { ArtikelZuBox, ArtikelZuPlatz, BoxZuPlatz, unbekannt };
}

