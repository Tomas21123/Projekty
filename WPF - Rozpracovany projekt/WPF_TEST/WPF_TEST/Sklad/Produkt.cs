using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_TEST.Sklad
{
    public class Produkt
    {
        public int Id { get; set; }
        public string PoziceNaSklade { get; set; } = "";
        public string Nazev {  get; set; } = "";
        public int Mnozstvi { get; set; }
    }
}
