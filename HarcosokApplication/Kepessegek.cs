using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarcosokApplication
{
    class Kepessegek
    {
        private int id;
        private string nev;


        public Kepessegek(int id, string nev)
        {
            this.id = id;
            this.nev = nev;

        }

        public int Id { get => id; set => id = value; }
        public string Nev { get => nev; set => nev = value; }
 
    }
}
