using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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

namespace PostOfficeApp.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //comboBoxColumn.DataContext = new List<string> { "Hello", "World" };
            
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "Excel Files | *.xls;*.xlsx;*.xlsm;";
            ofd.Filter = "Excel Workbook | *.xlsx;*.xlsm;";
            ofd.Multiselect = false;
            ofd.Title = "Import Excel Workbook (.xlsx, .xlsm)";

            if (ofd.ShowDialog() == true)
            {
                DataTable dt = new DataTable();
                using (XLWorkbook workbook = new XLWorkbook(ofd.FileName))
                {
                    bool isFirstRow = true;
                    var rows = workbook.Worksheet(1).RowsUsed();
                    foreach (var row in rows)
                    {
                        if (isFirstRow)
                        {
                            foreach (IXLCell cell in row.Cells())
                            {
                                try
                                {
                                    dt.Columns.Add(cell.Value.ToString());
                                }
                                catch (DuplicateNameException except)
                                {
                                    MessageBox.Show(except.Message, "Message");
                                    // Exit Open File Dialog and return to MainWindow.
                                    return;
                                }
                            }
                            isFirstRow = false;
                        }
                        else
                        {
                            dt.Rows.Add();
                            int i = 0;
                            foreach (IXLCell cell in row.Cells())
                                dt.Rows[dt.Rows.Count - 1][i++] = cell.Value.ToString();
                        }
                    }
                    dataGrid.ItemsSource = dt.DefaultView;
                    // Set the data source for the comboBox items.
                    comboBoxColumn.ItemsSource = dt.Columns;
                    label1.Content = $"Total records: {dataGrid.Items.Count}";
                }
            }
        }


        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataView dv = dataGrid.ItemsSource as DataView;
                if (dv != null)
                {
                    string columnName = dv.Table.Columns[comboBoxColumn.SelectedIndex].ColumnName;
                    dv.RowFilter = $"{columnName} LIKE '%{textBoxKeyword.Text}%'";
                    label1.Content = $"Total records: {dataGrid.Items.Count}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message");
            }
        }

        private void TextBoxKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter && textBoxKeyword.IsKeyboardFocusWithin)
                {
                    DataView dv = dataGrid.ItemsSource as DataView;
                    if (dv != null)
                    {
                        string columnName = dv.Table.Columns[comboBoxColumn.SelectedIndex].ColumnName;
                        dv.RowFilter = $"{columnName} LIKE '%{textBoxKeyword.Text}%'";
                        label1.Content = $"Total records: {dataGrid.Items.Count}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message");
            }

        }

        private void TextBoxKeyword_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
