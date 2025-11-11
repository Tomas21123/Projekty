using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Knihovna.Models
{
    public class Kniha
    {
        public int? Id { get; set; }
        public string? Nazev { get; set; }
        public string? Autor { get; set; }
        public int? RokVydani { get; set; }
        public string? Zanr { get; set; }
        public string? Stav { get; set; }
        public int? Hodnoceni { get; set; }


        public Kniha() { }

        public Kniha(int id, string nazev, string autor, int rok, string zanr, string stav, int hodnoceni)
        {
            Id = id;
            Nazev = nazev;
            Autor = autor;
            RokVydani = rok;
            Zanr = zanr;
            Stav = stav;
            Hodnoceni = hodnoceni;
        }
    }

}
