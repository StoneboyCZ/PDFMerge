using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Specialized;
using System.Windows;

namespace PDFMerge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string selectionDialogTitle = "Vyberte soubory PDF, které chcete spojit (pro výběr více souborů držte CTRL)";
        private const string errorNoFilesToMergeTitle = "Chybí soubory, které se mají spojit";
        private const string errorNoFilesToMergeText = "Vyberte soubory, které se mají spojit.";
        private const string pdfFilter = "Soubory PDF (*.pdf)|*.pdf";
        private const string errorTooFewFilesText = "Vybráno příliš málo souborů. Pro spojení je třeba vybrat minimálně 2.";
        private const string errorTooFewFilesTitle = "Vybráno příliš málo souborů.";

        public MainWindow()
        {
            InitializeComponent();

            ((INotifyCollectionChanged)listBox.Items).CollectionChanged += ListBox_CollectionChanged;
        }

        private void ListBox_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (listBox.Items.Count > 0)
            {
                btn_moveUp.IsEnabled = true;
                btn_moveDown.IsEnabled = true;
                btn_delete.IsEnabled = true;
                btn_mergePDFs.IsEnabled = true;
            }
            else
            {
                btn_moveUp.IsEnabled = false;
                btn_moveDown.IsEnabled = false;
                btn_delete.IsEnabled = false;
                btn_mergePDFs.IsEnabled = false;
            }
        }

        private void Btn_selectInputFiles_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                DefaultExt = "*.pdf",
                Filter = pdfFilter,
                Title = selectionDialogTitle
            };
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                foreach (string fn in dlg.FileNames)
                {
                    _ = listBox.Items.Add(fn);
                }
            }
        }

        private void Btn_mergePDFs_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.Items.Count == 1)
            {
                _ = MessageBox.Show(messageBoxText: errorTooFewFilesText,
                                    caption: errorTooFewFilesTitle,
                                    button: MessageBoxButton.OK,
                                    icon: MessageBoxImage.Error);
            }
            else
            {
                PdfDocument outputFile = new PdfDocument();

                foreach (string fn in listBox.Items)
                {
                    PdfDocument inputFile = PdfReader.Open(fn, PdfDocumentOpenMode.Import);

                    for (int i = 0; i < inputFile.PageCount; i++)
                    {
                        PdfPage p = inputFile.Pages[i];
                        outputFile.AddPage(p);
                    }

                    inputFile.Close();
                }

                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
                {
                    DefaultExt = "*.pdf",
                    Filter = pdfFilter
                };

                bool? result = dlg.ShowDialog();

                if (result == true)
                {
                    outputFile.Save(dlg.FileName);

                    _ = MessageBox.Show(messageBoxText: String.Format("Soubor úspěšně uložen do\n{0}", dlg.FileName),
                                    caption: "Soubor PDF úspěšně uložen",
                                    button: MessageBoxButton.OK,
                                    icon: MessageBoxImage.Information);
                }

                outputFile.Close();
            }
        }

        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            // remove currently selected object
            listBox.Items.Remove(listBox.SelectedItem);
        }

        /// <summary>
        /// Click method for move up button. Attribution: https://stackoverflow.com/questions/4796109/how-to-move-item-in-listbox-up-and-down 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_moveUp_Click(object sender, RoutedEventArgs e) => MoveItem(-1);

        /// <summary>
        /// Attribution: https://stackoverflow.com/questions/4796109/how-to-move-item-in-listbox-up-and-down 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_moveDown_Click(object sender, RoutedEventArgs e) => MoveItem(1);

        /// <summary>
        /// Attribution: https://stackoverflow.com/questions/4796109/how-to-move-item-in-listbox-up-and-down
        /// </summary>
        /// <param name="direction"></param>
        private void MoveItem(int direction)
        {
            // Checking selected item
            if (listBox.SelectedItem == null || listBox.SelectedIndex < 0)
                return; // No selected item - nothing to do

            // Calculate new index using move direction
            int newIndex = listBox.SelectedIndex + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= listBox.Items.Count)
                return; // Index out of range - nothing to do

            object selected = listBox.SelectedItem;

            // Removing removable element
            listBox.Items.Remove(selected);
            // Insert it in new position
            listBox.Items.Insert(newIndex, selected);
            // Restore selection
            listBox.SelectedItem = selected;
        }

        /// <summary>
        /// Event handler for dropping files into the listBox. Attribution: https://stackoverflow.com/questions/5662509/drag-and-drop-files-into-wpf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string fn in files)
                {
                    _ = listBox.Items.Add(fn);
                }
            }
        }
    }
}
