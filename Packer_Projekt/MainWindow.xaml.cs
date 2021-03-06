﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;

namespace Packer_Projekt
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public string File_Path(int verpacken)
        {
            // Zugriff auf Explorer
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.InitialDirectory = "Desktop"; // Festlegen des Verzeichnisses, welches als erstes ausgewählt wird

            //IF Abfrage für den Filter
            if (verpacken == 1) //Dateiauswahl zum Verpacken
            {
                openFileDialog1.Filter = "Bilddateien (*.bmp, *.jpg)|*.bmp;*.jpg | All files (*.*)|*.*";
                // Filter, welche Dateien angezeigt werden
                openFileDialog1.FilterIndex = 2; // Festlegen welcher Filter beim öffnen ausgewählt ist

            }
            else if (verpacken == 2)//Dateiauswahl zum Entpacken
            {
                openFileDialog1.Filter = "Selfmade-Dateien (*.smd)|*.smd"; // Begrenzung des Entpackens auf eigene Dateien
                openFileDialog1.FilterIndex = 1;
            }
            else if (verpacken == 3) //Dateiauswahl über Button (unbekannt ob Verpacken oder Entpacken)
            {
                openFileDialog1.Filter = "Bilddateien (*.bmp, *.jpg)|*.bmp;*.jpg| Selfmade-Dateien (*.smd)|*.smd| All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 3;
            }
            // IF-Abfrage ob eine Datei ausgewählt worden ist
            if (openFileDialog1.ShowDialog() == true)
            {
                string fileSelected = openFileDialog1.FileName;
                return fileSelected; // Rückgabe der ausgewählten Datei
            }
            else return "Fehler";
        }
        private string ziel_Ordner()
        {
            string folder = "";
            FolderBrowserDialog diag = new FolderBrowserDialog();
            if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folder = diag.SelectedPath;  //selected folder path
            }
            return folder;
        }
        private void Dateiauswahl_Click(object sender, RoutedEventArgs e)
        {
            string s_Dateipfad = File_Path(3);
            tb_Datei.Text = s_Dateipfad;
        }
        private void Zielort_Click(object sender, RoutedEventArgs e)
        {
            string s_Zielort = ziel_Ordner();
            tb_Ziel.Text = s_Zielort;
        }
        private void Verpacken_Click(object sender, RoutedEventArgs e)
        {
            string s_DateiPath = tb_Datei.Text;
            string s_ZielPath = tb_Ziel.Text;
            if (!File.Exists(s_DateiPath))
            {
                tb_Status.Text = "Bitte wählen Sie eine Datei aus.";
                s_DateiPath = File_Path(1);
                tb_Datei.Text = s_DateiPath;
            }
            if (!Directory.Exists(s_ZielPath))
            {
                tb_Status.Text = "Bitte wählen Sie einen Zielort aus.";
                s_ZielPath = ziel_Ordner();
                tb_Ziel.Text = s_ZielPath;
            }
            if (File.Exists(s_DateiPath))
            {
                tb_Status.Text = "Datei wird verpackt. Bitte warten.";
                VerpackenMethode(s_DateiPath);
            }
        }
        private void Entpacken_Click(object sender, RoutedEventArgs e)
        {
            string s_DateiPath = tb_Datei.Text;
            string s_ZielPath = tb_Ziel.Text;
            if (!File.Exists(s_DateiPath))
            {
                tb_Status.Text = "Bitte wählen Sie eine Datei aus.";
                s_DateiPath = File_Path(1);
                tb_Datei.Text = s_DateiPath;
            }
            if (!Directory.Exists(tb_Ziel.Text))
            {
                tb_Status.Text = "Bitte wählen Sie einen Zielort aus.";
                s_ZielPath = ziel_Ordner();
                tb_Ziel.Text = s_ZielPath;
            }
            if (File.Exists(s_DateiPath))
            {
                tb_Status.Text = "Datei wird Entpackt. Bitte warten.";
                EntpackenMethod(s_DateiPath);
            }
            // OB DATEI UNSERE (.smd) MUSS GETESTET WERDEN!!! wegen button und manueller Eingabe
        }
        private void Header(string s_newFilePath, string s_oldFilePath, char c_Marker)
        {
            string s_Filename = System.IO.Path.GetFileNameWithoutExtension(s_oldFilePath);
            //Rausziehen des Namens ohne Endung (aufgrund der Namenslängenbegrenzung)
            //ausm Pfad und speicherung in der Variable
            string s_endung = System.IO.Path.GetExtension(s_oldFilePath);
            // Speicherung der Endung in einer Variable
            FileStream o_fw = new FileStream(s_newFilePath, FileMode.Create, FileAccess.Write);
            BinaryWriter o_bw = new BinaryWriter(o_fw);
            // Öffnen des Streams zum Schreiben und erstellen der Zieldatei
            o_bw.Write((byte)'s'); //Byteweises Schreiben unserer Endung im Header
            o_bw.Write((byte)'m'); //Zur identifikation, dass es unsere Dateien sind
            o_bw.Write((byte)'d');
            o_bw.Write((byte)c_Marker); // Setzen des seltensten Zeichens als Marker 
            // Um herauszufinden was das Trennzeichen in der Datei ist
            //IF Abfrage der Länge des Namens + Anpassungen je nach Länge
            if (s_Filename.Length > 8)
            { //Name verkürzen, wenn länger als 8 Zeichen
                for (int i = 0; i < 7; i++)
                {
                    o_bw.Write((byte)s_Filename[i]);
                }
                o_bw.Write((byte)'~');
            }
            else if (s_Filename.Length < 8)
            { //Name auffüllen auf 8 Zeichen, wenn kleiner
                int i_Zaehler = 0;
                for (int i = 0; i < s_Filename.Length; i++)
                {
                    o_bw.Write((byte)s_Filename[i]);
                    i_Zaehler++;
                }
                while (i_Zaehler != 8)
                {
                    i_Zaehler++;
                    o_bw.Write((byte)'_');
                }
            }
            else
            { //Bei genau 8 Zeichen Namen schreiben
                for (int i = 0; i < s_Filename.Length; i++)
                {
                    o_bw.Write((byte)s_Filename[i]);
                }
            }
            for (int i = 0; i < s_endung.Length; i++)// Hinzufügen der Ursprünglichen Endung
            {
                o_bw.Write((byte)s_endung[i]);
            }
            o_bw.Write((byte)'\r');
            o_bw.Write((byte)'\n'); //Endmarkierung des Headers
            o_fw.Flush();
            o_bw.Close(); //Streams Schließen
            o_fw.Close();
        }
        static char Marker_Suche(string Filename)
        {
            int[] a_charsuche = new int[256]; //Array erstellt um gezählte Zeichen zu speichern
            FileStream o_fsr = new FileStream(Filename, FileMode.Open, FileAccess.Read);
            BinaryReader o_br = new BinaryReader(o_fsr);  //Streams zum Dateilesen geöffnet.
            while (o_fsr.Position <= o_fsr.Length - 1) // Schleife um die Datei Zeichen für Zeichen durchzugehen
            {
                int stelle = o_fsr.ReadByte();
                int inhalt = a_charsuche[stelle]; // Zwischenspeichern der Anzahl des Zeichens
                //o_fsr.Position -= 1; // Position einen Schritt zuruück gehen um kein Zeichen zu überspringen
                a_charsuche[stelle] = inhalt + 1; // Anzahl des Zeichens um eins erhöhen 
                inhalt = 0; // Zwischenspeicher Löschen zur Vermeidung von Falschzählungen
            }
            o_fsr.Flush();
            o_br.Close();
            o_fsr.Close(); // Streams leeren und Schließen
            //Nehmen der kleinsten Anzahl, davon den Index in Char umwandeln (Index == ASCII Tabelle) und in Variable speichern
            char marker = (char)Array.IndexOf(a_charsuche, a_charsuche.Min());
            return marker; //Rückgabe des Zeichens, welches am seltensten vorkommt
        }
        private bool Dateinutzbar(string s_DateiPath)
        {
            FileStream o_fsr = new FileStream(s_DateiPath, FileMode.Open, FileAccess.Read);
            BinaryReader o_br = new BinaryReader(o_fsr);
            if (o_br.ReadByte() == 's')
            {
                if (o_br.ReadByte() == 'm')
                {
                    if (o_br.ReadByte() == 'd')
                        return true;
                    else return false;
                }
                else return false;
            }
            else
                return false;

        }
        private void EntpackenMethod(string s_DateiPath)
        {
            tb_Status.Text = "Datei wird Entpackt. Bitte warten.";
            string newFilename = s_DateiPath;
            byte c_Marker = 0;
            byte i_anzahl;
            FileStream fr = new FileStream(s_DateiPath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fr);
            for (int i = 3; i < 16; i++)
            {
                fr.Position = i;
                if (i == 3)
                    c_Marker = br.ReadByte();
                else
                    newFilename += (char)br.ReadByte();
            }
            fr.Position = 18;
            FileStream fw = new FileStream(newFilename, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fw);

            while (fr.Position < fr.Length)
            {
                byte c_zeichen = br.ReadByte();
                if (c_zeichen == c_Marker)
                {
                    i_anzahl = br.ReadByte();
                    byte c_buchstabe = br.ReadByte();
                    for (int i = 0; i < i_anzahl; i++)
                    {
                        bw.Write((char)c_buchstabe);
                    }
                }
                else
                {
                    bw.Write((char)c_zeichen);
                }
            }
            tb_Status.Text = "Datei wurde Entpackt.";
        }
    
        private void VerpackenMethode(string s_DateiPath)
        {
            //Öffnen der Files
            FileStream o_fr = new FileStream(s_DateiPath, FileMode.Open, FileAccess.Read);
            BinaryReader o_br = new BinaryReader(o_fr);

            char c_Marker = Marker_Suche(s_DateiPath);
            string newFilepath = tb_Ziel.Text;
            string s_Dateiname = "";
            if(tb_Name.Text == "Neuer Dateiname (optional)")
                s_Dateiname = System.IO.Path.GetFileNameWithoutExtension(s_DateiPath);
            else
             s_Dateiname = tb_Name.Text;
            string newFilename = newFilepath + "\\" +  s_Dateiname + ".smd";

            Header(newFilename, s_DateiPath, c_Marker);
            FileStream o_fw = new FileStream(newFilename, FileMode.Open, FileAccess.Write);
            BinaryWriter o_bw = new BinaryWriter(o_fw);
            o_fw.Position = o_fw.Length;
            byte b_Zeichen;
            bool b_ZaehlerG255 = false;
            int i_Zaehler = 0;
            //Schleife mit welcher die Files verkürzt werden
            while (o_fr.Position < o_fr.Length)
            {
                b_ZaehlerG255 = false;
                b_Zeichen = o_br.ReadByte();
                o_fr.Position -= 1;
                while ((b_Zeichen == o_br.ReadByte()) && !b_ZaehlerG255)
                {
                    i_Zaehler++;
                    if (o_fr.Position < o_fr.Length)
                    {
                        b_ZaehlerG255 = false;
                    }
                    if (i_Zaehler == 255)
                    {
                        b_ZaehlerG255 = true;
                    }
                    if (o_fr.Position == o_fr.Length)
                    { break; }
                }
                if (o_fr.Position == o_fr.Length) { }
                else
                    o_fr.Position -= 1;
                if (i_Zaehler > 3 || b_Zeichen == c_Marker)
                {
                    o_bw.Write((byte)c_Marker);
                    o_bw.Write((byte)i_Zaehler);
                    o_bw.Write(b_Zeichen);
                    i_Zaehler = 0;
                }
                else
                {
                    for (int i = 0; i < i_Zaehler; i++)
                    {
                        o_bw.Write(b_Zeichen);
                    }
                    i_Zaehler = 0;
                }
            }
            o_fr.Flush();
            o_br.Close();
            o_fr.Close();
            o_fw.Flush();
            o_bw.Close();
            o_fw.Close();
            tb_Status.Text = "Datei fertig gepackt.";
        }

    }
}