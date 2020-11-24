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
            Testbox.Text = File_Path();
        }

        private void Entpacken_Click(object sender, RoutedEventArgs e)
        {
            Testbox.Text = File_Path();
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
    }
}
