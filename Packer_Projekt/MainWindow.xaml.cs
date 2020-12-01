using Microsoft.Win32;
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

        public string File_Path()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Bilddateien (*.bmp, *.jpg)|*.bmp;*.jpg |Textdateien (*.txt)|*.txt | All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == true)
            {
                string fileSelected = openFileDialog1.FileName;
                return fileSelected;
            }

            else return "Fehler";
        }
        private void Verpacken_Click(object sender, RoutedEventArgs e)
        {
            string s_DateiPath = File_Path();
            Testbox.Text =s_DateiPath;
            VerpackenMethode(s_DateiPath);
            
        }

        private void Entpacken_Click(object sender, RoutedEventArgs e)
        {
            string s_DateiPath = File_Path();
            Testbox.Text = s_DateiPath;
        }
        private void Header(string s_newFilePath, string s_oldFilePath, char c_Marker)
        {

            FileStream o_fw = new FileStream(s_newFilePath, FileMode.Create, FileAccess.Write);
            BinaryWriter o_bw = new BinaryWriter(o_fw);
            c_Marker = '{';
            o_bw.Write((byte)'s');
            o_bw.Write((byte)'m');
            o_bw.Write((byte)'d');
            o_bw.Write((byte)c_Marker);
            if(s_oldFilePath.Length > 8)
            {
                for(int i = 0; i < 7; i++)
                {
                    o_bw.Write((byte)s_oldFilePath[i]);
                }
                o_bw.Write((byte)'~');
            }
            else if(s_oldFilePath.Length < 8)
            {
                int i_Zaehler = 0;
                for (int i = 0; i < s_oldFilePath.Length; i++)
                {
                    o_bw.Write((byte)s_oldFilePath[i]);
                    i_Zaehler++;
                }
                while(i_Zaehler != 8)
                {
                    i_Zaehler++;
                    o_bw.Write((byte)'_');
                }
            }
            else
            {
                for (int i = 0; i < s_oldFilePath.Length; i++)
                {
                    o_bw.Write((byte)s_oldFilePath[i]);
                }
            }
            o_bw.Write((byte)'\r');
            o_bw.Write((byte)'\n');
            o_bw.Close();
            o_fw.Close();
        }

        static void Entpacken_Method(string Filename)
        {
            FileStream file_r = new FileStream(Filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(file_r);
            string newFilename = Filename + "";
            FileStream file_w = new FileStream(newFilename, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(file_w);

            while(file_r.Position < file_r.Length)
            {
                int i_int = 4;
                while (file_r.Position > 5)
                {
                    file_r.Position = file_r.Position - i_int;
                    i_int = 8;
                    bw.Write(br.ReadChar());
                }

            }
        }

        private void VerpackenMethode(string s_DateiPath)
        {

            //Öffnen der Files
            FileStream o_fr = new FileStream(s_DateiPath, FileMode.Open, FileAccess.Read);
            BinaryReader o_br = new BinaryReader(o_fr);
            string newFilename = s_DateiPath + ".smd";
            Header(newFilename, s_DateiPath, '{');
            FileStream o_fw = new FileStream(newFilename, FileMode.Create, FileAccess.Write);
            BinaryWriter o_bw = new BinaryWriter(o_fw);

            byte b_Zeichen;
            bool b_ZaehlerG255 = false;
            char c_Marker = '{';
            int i_Zaehler = 0;
            //Schleife mit welcher die Files verkürzt werden
            while (o_fr.Position < o_fr.Length -8)
            {
                b_Zeichen = o_br.ReadByte();
                while (b_Zeichen == o_br.ReadByte() && !b_ZaehlerG255)
                {
                    i_Zaehler++;
                    if(i_Zaehler==255)
                    {
                        b_ZaehlerG255 = true;
                    }
                }
                if (i_Zaehler >= 3)
                {
                    o_bw.Write((byte)c_Marker);
                    o_bw.Write((byte)i_Zaehler);
                    o_bw.Write((byte)b_Zeichen);
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
            o_br.Close();
            o_fr.Close();
            o_bw.Flush();
            o_fw.Close();
            o_bw.Close();
            Close();
        }

        
    }
}
