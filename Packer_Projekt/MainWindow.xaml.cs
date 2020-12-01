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
            openFileDialog1.Filter = "Bilddateien (*.bmp, *.jpg)|*.bmp;*.jpg";
            //|*.txt|All files (*.*)|*.*
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

        static void Entpacken_Method(string Filename)
        {
            FileStream file_r = new FileStream(Filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(file_r);
            string newFilename = Filename + "1";
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
            FileStream o_fw = new FileStream(s_DateiPath, FileMode.Create, FileAccess.Write);
            BinaryWriter o_bw = new BinaryWriter(o_fw);
            char c_Zeichen = ' ';
            char c_Marker = '{';
            int i_Zaehler = 1;
            //Schleife mit welcher die Files verkürzt werden
            while (o_fr.Position < o_fr.Length - 4)
            {
                c_Zeichen = o_br.ReadChar();
                while (c_Zeichen == o_br.ReadChar())
                {
                    i_Zaehler++;
                }
                if (i_Zaehler >= 3)
                {
                    o_bw.Write(c_Marker);
                    o_bw.Write(i_Zaehler);
                    o_bw.Write(c_Zeichen);
                    i_Zaehler = 1;
                }
                else
                {
                    for (int i = 0; i < i_Zaehler; i++)
                    {
                        o_bw.Write(c_Zeichen);
                    }
                    i_Zaehler = 1;
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
