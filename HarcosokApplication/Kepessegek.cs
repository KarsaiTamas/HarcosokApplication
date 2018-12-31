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
        private string leiras;
        private int harcos_id;

        public Kepessegek(int id, string nev, string leiras, int harcos_id)
        {
            this.id = id;
            this.nev = nev;
            this.leiras = leiras;
            this.harcos_id = harcos_id;
        }

        public int Id { get => id; set => id = value; }
        public string Nev { get => nev; set => nev = value; }
        public string Leiras { get => leiras; set => leiras = value; }
        public int Harcos_id { get => harcos_id; set => harcos_id = value; }
    }
}
