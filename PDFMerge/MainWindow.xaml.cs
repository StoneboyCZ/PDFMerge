using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
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

namespace PDFMerge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Btn_selectInputFiles_Click(object sender, RoutedEventArgs e)
        {
            /// Set up open file dialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                DefaultExt = "*.pdf",
                Filter = "Soubory PDF (.pdf)|*.pdf",
                Title = "Kliknutím vyberte soubory PDF, které chcete spojit (pro výběr více souborů držte CTRL)"
            };
            bool? result = dlg.ShowDialog();

            // If the files are picked
            if (result == true)
            {
                // smazat aktuálně vybrané soubory
                listBox.Items.Clear();

                foreach (string fn in dlg.FileNames)
                {
                    _ = listBox.Items.Add(fn);
                }
            }
        }

        private void Btn_mergePDFs_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.Items.Count == 0)
            {
                MessageBox.Show("Vyberte soubory, které se mají spojit", "Chybí soubory, které se mají spojit", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (listBox.Items.Count == 1)
            {
                MessageBox.Show("Vybráno příliš málo souborů.", "Vybráno příliš málo souborů.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                PdfDocument outputPDF = new PdfDocument();

                foreach (string fn in listBox.Items)
                {
                    PdfDocument inputFile = PdfReader.Open(fn, PdfDocumentOpenMode.Import);

                    for (int i = 0; i < inputFile.PageCount; i++)
                    {
                        PdfPage p = inputFile.Pages[i];
                        outputPDF.AddPage(p);
                    }

                    inputFile.Close();
                }

                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
                {
                    DefaultExt = "*.pdf",
                    Filter = "Soubory PDF (*.pdf)|*.pdf"
                };

                bool? result = dlg.ShowDialog();

                if (result == true)
                {
                    outputPDF.Save(dlg.FileName);
                }

                outputPDF.Close();
            }
        }
    }
}
