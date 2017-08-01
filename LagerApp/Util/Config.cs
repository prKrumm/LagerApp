using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagerApp.Util
{
    public class Config
    {
        //Nummernkreise
        public const String Nummern= "[a-cA-CsS][0-9]{5,5}"; //A15000, S15000
        public const String ebayNummern = "[a-cA-C][0-9]{5,5}"; //A15000
        public const String shopNummern = "[sS][0-9]{5,5}";	//S15000	
        public const String lagerPlätze = "[kKsSiIaA]\\d\\d-\\d-\\d";
        
    }
}
