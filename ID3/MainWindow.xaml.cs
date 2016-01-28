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
using System.Data;
using System.IO;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;


namespace ID3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable datatable;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog(this) == true)
            {
                string filename = openFileDialog.FileName;
                string extension = System.IO.Path.GetExtension(filename);
                if (extension == ".xls" || extension == ".xlsx")
                {
                    Excel.Application excelApp = new Excel.Application();
                    Excel.Workbook workbook;
                    Excel.Worksheet worksheet;
                    Excel.Range range;
                    workbook = excelApp.Workbooks.Open(filename);
                    worksheet = (Excel.Worksheet)workbook.Sheets["Data"];

                    int column = 0;
                    int row = 0;

                    range = worksheet.UsedRange;
                    DataTable dt = new DataTable();
                    for (column = 1; column <= range.Columns.Count; column++)
                    {
                        dt.Columns.Add((range.Cells[1, column] as Excel.Range).Value2.ToString());
                    }
                    for (row = 2; row <= range.Rows.Count; row++)
                    {
                        DataRow dr = dt.NewRow();
                        for (column = 1; column <= range.Columns.Count; column++)
                        {
                            dr[column - 1] = (range.Cells[row, column] as Excel.Range).Value2.ToString();
                        }
                        dt.Rows.Add(dr);
                        dt.AcceptChanges();
                    }
                    workbook.Close(true, Missing.Value, Missing.Value);
                    excelApp.Quit();
                    Input.DataContext = dt.DefaultView;
                    datatable = dt;
                    
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }



        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Phần mềm mô phỏng cây định danh ID3\nSinh viên: \nNguyễn Trần Minh Tân - 13520747 \nPhạm Hồ Lê Nguyễn-13520566", "About", MessageBoxButton.OK);
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            Attribute hair = new Attribute("HairColor", new string[] { "Black", "Gray", "Silver" });
            Attribute height = new Attribute("Height", new string[] { "Short", "Medium", "High" });
            Attribute weight = new Attribute("Weight", new string[] { "Light", "Medium", "Heavy" });
            Attribute cream = new Attribute("Cream", new string[] { "Yes", "No" });

            Attribute[] attributes = new Attribute[] { hair, height, weight, cream };

            DataTable samples = datatable;

            DecisionTree id3 = new DecisionTree();
            TreeNode root = id3.mountTree(samples, "Result", attributes);

            DecisionTree.printNode(root, "");
        }
    }
}
