﻿using System;
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

        const string masodlagos_kulcs = "ALTER TABLE `kepessegek` ADD FOREIGN KEY (`harcos_id`) REFERENCES `harcosok`(`id`) ON DELETE RESTRICT ON UPDATE RESTRICT;";

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
            var frissites = conn.CreateCommand();
            frissites.CommandText = @"SELECT nev FROM harcosok;";
            var olvas = frissites.ExecuteReader();
            ComboBox_Hasznalo.Items.Clear();

            while (olvas.Read())
            {
                var nev_f = olvas.GetString("nev");
                ComboBox_Hasznalo.Items.Add(nev_f);
            }
            olvas.Close();
            Button_Letrehoz.Click += (Sender, e) => 
            {

                if (Harcos_Neve_Text_Box.Text!=null && !Harcos_Neve_Text_Box.Text.Trim(' ').Equals(""))
                {
                    var ellenorzes_insert = conn.CreateCommand();
                    string nev = Harcos_Neve_Text_Box.Text.Trim(' ');
                    ellenorzes_insert.CommandText= @"SELECT COUNT(*) FROM harcosok WHERE nev=@nev;";
                    ellenorzes_insert.Parameters.AddWithValue("@nev",nev);
                    var hossz = (long)ellenorzes_insert.ExecuteScalar();
                    MessageBox.Show(""+hossz);
                    DateTime letrehoz=DateTime.Now;
                    if (hossz==0)
                    {
                        
                        
                        var insert = conn.CreateCommand();
                        insert.CommandText = @"
                        INSERT INTO `harcosok`( `nev`, `letrehozas`) VALUES (@nev,@letrehoz);";
                        insert.Parameters.AddWithValue("@nev",nev);
                        insert.Parameters.AddWithValue("@letrehoz", letrehoz);
                        insert.ExecuteNonQuery();
                         frissites = conn.CreateCommand();
                        frissites.CommandText=@"SELECT nev FROM harcosok;";
                         olvas = frissites.ExecuteReader();
                        ComboBox_Hasznalo.Items.Clear();

                        while (olvas.Read())
                        {
                            var nev_f = olvas.GetString("nev");
                            ComboBox_Hasznalo.Items.Add(nev_f);
                        }
                        olvas.Close();
                    }

                }





            };


        }
    }
}
