using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace HarcosokApplication
{
    public partial class Form1 : Form
    {


        MySqlConnection conn;

        const string harcosok_tabla = @"
            CREATE TABLE IF NOT EXISTS harcosok(
                id INTEGER AUTO_INCREMENT PRIMARY KEY,
                nev VARCHAR(80) NOT NULL UNIQUE,
                letrehozas DATE NOT NULL
            )

        ";
        const string kepessegek_tabla = @"
            CREATE TABLE IF NOT EXISTS kepessegek(
                id INTEGER AUTO_INCREMENT PRIMARY KEY,
                nev VARCHAR(80) NOT NULL,
                leiras TEXT NOT NULL,
                harcos_id INTEGER NOT NULL
            )

        ";



        const string adatbazis = "CREATE DATABASE IF NOT EXISTS `cs_harcosok`;";
        static private List<Kepessegek>kepessegek;
        const string masodlagos_kulcs = "ALTER TABLE `kepessegek` ADD FOREIGN KEY (`harcos_id`) REFERENCES `harcosok`(`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;";
        public int Id_Kereses()
            {
            var frissites = conn.CreateCommand();
            var nev = ComboBox_Hasznalo.SelectedItem;
            frissites.CommandText = @"SELECT id FROM harcosok WHERE nev=@nev;";
            frissites.Parameters.AddWithValue("@nev",nev);
            var olvas = frissites.ExecuteReader();

            int id = 1;
            while (olvas.Read())
            {
                id = olvas.GetInt32("id");
            }
            olvas.Close();
            return id;

        }
        public int Id_Kereses_ListBox()
        {
            var frissites = conn.CreateCommand();
            var nev = ListBox_Harcosok.SelectedItem;
            frissites.CommandText = @"SELECT id FROM harcosok WHERE nev=@nev;";
            frissites.Parameters.AddWithValue("@nev", nev);
            var olvas = frissites.ExecuteReader();

            int id = 1;
            while (olvas.Read())
            {
                id = olvas.GetInt32("id");
            }
            olvas.Close();
            return id;

        }
        public void Harcosok_Frissites(){
            var frissites = conn.CreateCommand();
            frissites.CommandText = @"SELECT nev FROM harcosok;";
            var olvas = frissites.ExecuteReader();
            ComboBox_Hasznalo.Items.Clear();
            ListBox_Harcosok.Items.Clear();
            while (olvas.Read())
            {
                var nev_f = olvas.GetString("nev");
                ComboBox_Hasznalo.Items.Add(nev_f);
                ListBox_Harcosok.Items.Add(nev_f);
            }
            olvas.Close();

        }
        public void Kepesseg_Frissites(int i)
        {
            
            var frissites = conn.CreateCommand();
            frissites.CommandText = @"SELECT id,nev,leiras,harcos_id FROM kepessegek WHERE harcos_id=@i;";
            frissites.Parameters.AddWithValue("@i", i);
            var olvas = frissites.ExecuteReader();
            ListBox_Kepessegek.Items.Clear();
            kepessegek.Clear();
            
            while (olvas.Read())
            {
                ListBox_Kepessegek.Items.Add(olvas.GetString("nev"));
                kepessegek.Add(new Kepessegek(olvas.GetInt32("id"), olvas.GetString("nev")));
            }
            olvas.Close();

        }
        public bool Tartalmaz_Kepessegek(int i)
        {
            var frissites = conn.CreateCommand();
            frissites.CommandText = @"SELECT count(*) FROM kepessegek WHERE harcos_id=@i;";
            frissites.Parameters.AddWithValue("@i", i);
            var vissza=(long) frissites.ExecuteNonQuery();
            return (vissza == 0 ? true : false);
            
        }

        public Form1()
        {
            InitializeComponent();
            try
            {
                  conn = new MySqlConnection("Server=localhost;Uid=root;Password=;");

              //  conn = new MySqlConnection("hiba kilép");
                   conn.Open();
                
                var database = new MySqlCommand(adatbazis, conn);
                database.ExecuteNonQuery();
                var belep = conn.CreateCommand();
                belep.CommandText = "USE cs_harcosok;";
                belep.ExecuteNonQuery();
                var comm = conn.CreateCommand();
                comm.CommandText = harcosok_tabla;
                comm.ExecuteNonQuery();
                var comm2 = conn.CreateCommand();

                comm2.CommandText = kepessegek_tabla;
                comm2.ExecuteNonQuery();
                var kulso_kulcs = conn.CreateCommand();
                kulso_kulcs.CommandText = masodlagos_kulcs;
                kulso_kulcs.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show("Hiba: "+e);
                throw;
                conn.Close();
            }
            FormClosed += (Sender, e) => conn.Close();
            kepessegek = new List<Kepessegek>();
            Harcosok_Frissites();
            int id = Id_Kereses();
            Kepesseg_Frissites(id);
            TextBox_Kepesseg_Leirasa.Text = "Nincs kiválasztott képesség";
            Button_Letrehoz.Click += (Sender, e) => 
            {

                if (Harcos_Neve_Text_Box.Text!=null && !Harcos_Neve_Text_Box.Text.Trim(' ').Equals(""))
                {
                    var ellenorzes_insert = conn.CreateCommand();
                    string nev = Harcos_Neve_Text_Box.Text.Trim(' ');
                    ellenorzes_insert.CommandText= @"SELECT COUNT(*) FROM harcosok WHERE nev=@nev;";
                    ellenorzes_insert.Parameters.AddWithValue("@nev",nev);
                    var hossz = (long)ellenorzes_insert.ExecuteScalar();
                     
                    DateTime letrehoz=DateTime.Now;
                    if (hossz==0)
                    {
                        
                        
                        var insert = conn.CreateCommand();
                        insert.CommandText = @"
                        INSERT INTO `harcosok`( `nev`, `letrehozas`) VALUES (@nev,@letrehoz);";
                        insert.Parameters.AddWithValue("@nev",nev);
                        insert.Parameters.AddWithValue("@letrehoz", letrehoz);
                        insert.ExecuteNonQuery();
                        Harcosok_Frissites();
                        
                    }
                    else
                    {
                        MessageBox.Show("Ilyen harcos már létezik!");
                    }

                }
            };
            Button_Hozzaad.Click += (Sender, e) => 
            {
                if (ComboBox_Hasznalo.SelectedItem!=null && ComboBox_Hasznalo.Items.Contains(ComboBox_Hasznalo.SelectedItem) &&
                TextBox_Kepesseg_Neve.Text!=null && !TextBox_Kepesseg_Neve.Text.Trim(' ').Equals("") &&
                TextBox_Leiras.Text!=null && !TextBox_Leiras.Text.Trim(' ').Equals("")
                )
                {
                    int kivalasztott = ComboBox_Hasznalo.SelectedIndex;
                    id =Id_Kereses();
                    string kepesseg_nev=TextBox_Kepesseg_Neve.Text.Trim(' ');
                    string leiras = TextBox_Leiras.Text;
                    var insert = conn.CreateCommand();
                    
                    
                    
                    insert.CommandText = @"
                        INSERT INTO `kepessegek`( `nev`, `leiras`,harcos_id) VALUES (@nev,@leiras,@id);";
                    insert.Parameters.AddWithValue("@nev", kepesseg_nev);
                    insert.Parameters.AddWithValue("@leiras", leiras);
                    insert.Parameters.AddWithValue("@id", id);
                    insert.ExecuteNonQuery();
                    Harcosok_Frissites();
                    Kepesseg_Frissites(id);

                    ComboBox_Hasznalo.SelectedIndex = kivalasztott;
                    ListBox_Harcosok.SelectedIndex = kivalasztott;
                    }
                
 
            };
            ComboBox_Hasznalo.SelectedIndexChanged += (sender, e) => 
            {
                Kepesseg_Frissites(Id_Kereses());
                TextBox_Kepesseg_Leirasa.Text = "Nincs kiválasztott képesség";
            };
             ListBox_Harcosok.SelectedIndexChanged += (sender, e) => 
            {
                Kepesseg_Frissites(Id_Kereses_ListBox());
                TextBox_Kepesseg_Leirasa.Text = "Nincs kiválasztott képesség";
            };
            ListBox_Kepessegek.SelectedIndexChanged += (sender, e) =>
            {
                if (ListBox_Kepessegek.SelectedIndex>-1)
                {

                
                var kepesseg_leiras = conn.CreateCommand();
                kepesseg_leiras.CommandText = @"SELECT leiras FROM kepessegek where id=@id;";
                MessageBox.Show(ListBox_Kepessegek.SelectedIndex+"");
                kepesseg_leiras.Parameters.AddWithValue("@id", kepessegek[ListBox_Kepessegek.SelectedIndex].Id);
                
                var olvas = kepesseg_leiras.ExecuteReader();

                string nev="Nincs kiválasztott képesség";
                while (olvas.Read())
                {
                    nev = olvas.GetString("leiras");
                }
                TextBox_Kepesseg_Leirasa.Text = nev;
                olvas.Close();
                }
            };
            Button_Modosit.Click += (sender, e) =>
            {
                if (ListBox_Kepessegek.SelectedItem != null)
                {


                    id = kepessegek[ListBox_Kepessegek.SelectedIndex].Id;
                    string leiras = TextBox_Kepesseg_Leirasa.Text;
                    var kepesseg_leiras = conn.CreateCommand();
                    kepesseg_leiras.CommandText = "UPDATE kepessegek SET leiras=@leiras WHERE id=@id";
                    kepesseg_leiras.Parameters.AddWithValue("@id", id);
                    kepesseg_leiras.Parameters.AddWithValue("@leiras", leiras);
                    kepesseg_leiras.ExecuteNonQuery();
                     
                }
                else { MessageBox.Show("Kérem módosítás előtt válasszon ki egy képességet!"); }
            };


            Button_Torles.Click += (sender, e) => 
            {

                var kepesseg_torles = conn.CreateCommand();
                kepesseg_torles.CommandText = @"DELETE FROM kepessegek WHERE id=@id;"; ;
            };


        }
    }
}
