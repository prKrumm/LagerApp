using System;
using Xunit;
using LagerApp.Controllers;
using LagerApp.Util.xlsx;
using LagerApp.Util;

namespace LagerAppTest
{
    public class UnitTest1
    {
        private readonly XLSXImportExportController _xlsxcont;

            public UnitTest1()
        {
            _xlsxcont = new XLSXImportExportController();
        }

        [Fact]
        public void Test1()
        {
            var scan1 = new Scan();
            scan1.druck_pseudonym = "A15000";
            scan1.lager_fach = "A01-2-3";
            ScanVariantenChecker checker = new ScanVariantenChecker();
            Assert.True(checker.CheckLine(scan1).Equals(ScanVarianten.ArtikelZuPlatz));

            var scan2 = new Scan();
            scan2.druck_pseudonym = "AA1500";
            scan2.lager_fach = "A01-2-3";
            Assert.False(checker.CheckLine(scan2).Equals(ScanVarianten.ArtikelZuPlatz));

            var scan3 = new Scan();
            scan3.druck_pseudonym = "BOX2000";
            scan3.lager_fach = "A01-2-3";
            Assert.True(checker.CheckLine(scan3).Equals(ScanVarianten.BoxZuPlatz));

            var scan4 = new Scan();
            scan4.druck_pseudonym = "C13000";
            scan4.lager_fach = "A01-2-3";
            Assert.True(checker.CheckLine(scan4).Equals(ScanVarianten.ArtikelZuPlatz));

            var scan5 = new Scan();
            scan5.druck_pseudonym = "C13000";
            scan5.lager_fach = "BOX2000";
            Assert.True(checker.CheckLine(scan5).Equals(ScanVarianten.ArtikelZuBox));


        }
    }
}
