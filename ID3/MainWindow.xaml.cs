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
        bool isLoad;
        public List<string> RuleID3 = new List<string>();
        List<string> ListRule = new List<string>();

      
        public MainWindow()
        {
            
            InitializeComponent();
            InitPredictItems();
        }
        public void InitPredictItems()
        {
            haircolor.Items.Add("Black");
            haircolor.Items.Add("Gray");
            haircolor.Items.Add("Silver");

            height.Items.Add("Short");
            height.Items.Add("Medium");
            height.Items.Add("Tall");

            weight.Items.Add("Light");
            weight.Items.Add("Medium");
            weight.Items.Add("Heavy");

            cream.Items.Add("No");
            cream.Items.Add("Yes");
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx|All files (*.*)|*.*";
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
                    isLoad = true;
                    
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }



        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Phần mềm mô phỏng cây định danh ID3\nSinh viên: \nNguyễn Trần Minh Tân - 13520747 \nPhạm Hồ Lê Nguyễn - 13520566", "About", MessageBoxButton.OK);
        }
        void DumpVisualTree(TreeViewItem parentNode, TreeNode root)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = ": " + root.attribute.ToString();
            parentNode.Items.Add(item);

            int count = root.totalChilds;
            if (root.attribute.values != null)
            {
                for (int i = 0; i < count; i++)
                {
                    item = new TreeViewItem();
                    item.Header = ": " + root.attribute.values[i].ToString();
                    parentNode.Items.Add(item);
                    TreeNode child = root.getChild(i);
                    DumpVisualTree(item, child);
                }
            }
        }
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            if (isLoad == true)
            {
                Attribute hair = new Attribute("HairColor", new string[] { "Black", "Gray", "Silver" });
                Attribute height = new Attribute("Height", new string[] { "Short", "Medium", "High" });
                Attribute weight = new Attribute("Weight", new string[] { "Light", "Medium", "Heavy" });
                Attribute cream = new Attribute("Cream", new string[] { "Yes", "No" });

                Attribute[] attributes = new Attribute[] { hair, height, weight, cream };

                DataTable samples = datatable;

                DecisionTree id3 = new DecisionTree();
                TreeNode root = id3.mountTree(samples, "Result", attributes);
                TreeNode root1 = root;
                var decisiontree = new DecisionTree();
                decisiontree.SearchRule(root);
                RuleID3 = decisiontree.RuleID3;  
                int i = 1;
                foreach (var rule in RuleID3)
                {
                    ListRule.Add("Rule [" +i+ "]: IF {" + rule);
                    i++;
                }
                lvRule.ItemsSource = ListRule;
                DecisionTree.printNode(root, "     ");

                tvDecisionTree.Items.Clear();
                TreeViewItem item = new TreeViewItem();
                item.Header = "Logical Tree";

                DumpVisualTree(item, root1);

                tvDecisionTree.Items.Add(item);
                item.ExpandSubtree();

                txtTree.Text = DecisionTree.TreeList;
            }
            else
                MessageBox.Show("Data must load before run", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnPredict_Click(object sender, RoutedEventArgs e)
        {
            bool[] attr = new bool[4] {false, false,false,false };
            if (isLoad)
            {
                string _haircolor = haircolor.SelectedValue.ToString();
                string _height = height.SelectedValue.ToString();
                string _weight = weight.SelectedValue.ToString();
                string _cream = cream.SelectedValue.ToString();

                foreach (var _rule in RuleID3)
                {
                    if (_rule.Contains("HairColor"))
                    {
                        if (_rule.Contains("HairColor =  "+_haircolor))
                            attr[0] = true;
                        else
                            attr[0] = false;
                    }
                    else
                        attr[0] = true;
                    if (_rule.Contains("Height"))
                    {
                        if (_rule.Contains("Height =  " + _height))
                            attr[1] = true;
                        else
                            attr[1] = false;
                    }
                    else
                        attr[1] = true;
                    if (_rule.Contains("Weight"))
                    {
                        if (_rule.Contains("Weight =  "+ _weight))
                            attr[2] = true;
                        else
                            attr[2] = false;
                    }
                    else
                        attr[2] = true;
                    if (_rule.Contains("Cream"))
                    {
                        if (_rule.Contains("Cream =  " + _cream))
                            attr[3] = true;
                        else
                            attr[3] = false;
                    }
                    else
                        attr[3] = true;
                    if (attr[0] == true && attr[1]==true && attr[2]==true && attr[3]==true)
                    {
                        if (_rule.Contains("True"))
                            txtResult.Text = "True";
                        else if (_rule.Contains("False"))
                            txtResult.Text = "False";
                        return;
                    }
                }              
            }
            else
                MessageBox.Show("Data must load before run", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
