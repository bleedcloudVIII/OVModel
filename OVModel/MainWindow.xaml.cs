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
using OVModel_DopTheory;

namespace OVModel
{
    public partial class MainWindow : Window
    {
        DataGrid dataGrid = new DataGrid { AutoGenerateColumns = false };

        public MainWindow()
        {
            InitializeComponent();

            CreateColumns();

            //StartCalculation();
            //double b_2 = 0.125;
            //double b = b_2 / 2;
            //double n_ob = 1.4627;
            //double n = 1.4738;
            //double x = -0.004;
            //double R = 2;
            //Console.WriteLine(OVModel_DopTheory.DopTheory.n_x(x, n, R, b));
            //Console.WriteLine(OVModel_DopTheory.DopTheory.n_y(x, n, R, b));
            //Console.WriteLine(OVModel_DopTheory.DopTheory.n_z(x, n, R, b));
        }

        private void CreateColumns()
        {
            DataGridTextColumn column1 = new DataGridTextColumn();
            column1.Header = "x";
            column1.Binding = new Binding($"[0]");
            column1.MaxWidth = 50;
            Tablica.Columns.Add(column1);


            DataGridTextColumn column2 = new DataGridTextColumn();
            column2.Header = "n_x";
            column2.Binding = new Binding($"[1]");
            column2.MaxWidth = 75;
            Tablica.Columns.Add(column2);

            DataGridTextColumn column3 = new DataGridTextColumn();
            column3.Header = "n_y";
            column3.Binding = new Binding($"[2]");
            column3.MaxWidth = 75;
            Tablica.Columns.Add(column3);

            DataGridTextColumn column4 = new DataGridTextColumn();
            column4.Header = "n_z";
            column4.Binding = new Binding($"[3]");
            column4.MaxWidth = 75;
            Tablica.Columns.Add(column4);
            
        }

        private void StartCalculation()
        {
            if (isCanConvertToDouble(Input_2b.Text) &&
                isCanConvertToDouble(Input_h.Text) &&
                isCanConvertToDouble(Input_n.Text) &&
                isCanConvertToDouble(Input_n_ob.Text) &&
                isCanConvertToDouble(Input_R.Text) &&
                isCanConvertToDouble(Input_x.Text))
            {
                Console.WriteLine("Starting Calculating...");
                double b = System.Convert.ToDouble(Input_2b.Text) / 2;
                double h = System.Convert.ToDouble(Input_h.Text);
                double n = System.Convert.ToDouble(Input_n.Text);
                double n_ob = System.Convert.ToDouble(Input_n_ob.Text);
                double R = System.Convert.ToDouble(Input_R.Text);
                double x = System.Convert.ToDouble(Input_x.Text);

                List<List<double>> result = new List<List<double>>();

                int count = 30;

                for (int i = 0; i < count; i++)
                {
                    result.Add(new List<double> { x + h * i, OVModel_DopTheory.DopTheory.n_x(x + h * i, n, R, b), OVModel_DopTheory.DopTheory.n_y(x + h * i, n, R, b), OVModel_DopTheory.DopTheory.n_z(x + h * i, n, R, b) });
                }
                Tablica.ItemsSource = result;
                Console.WriteLine("End Calculating.");
                //FullTable(result);
            }
            else
            {
                Console.WriteLine("ELSE!");
            }
            

        }

        private void FullTable(List<List<double>> list)
        {

        }

        private bool isCanConvertToDouble(string str)
        {
            try
            {
                System.Convert.ToDouble(str);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            //return true;
        }

        private void Input_n_ob_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Input_n_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Input_2b_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Input_R_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Input_h_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Input_x_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            Console.WriteLine("TextChanged");
            StartCalculation();
        }

        //private void SetRows()
        //{
        //    //int countYear = System.Convert.ToInt32(CountYear.Text);
        //    int count = 30;
        //    List<element> list = new List<element>();
        //    for (int i = 0; i <= count; i++)
        //    {
        //        element row = new element { list = new List<int> { x, 0, 0, 0 } };
        //        list.Add(row);
        //    }
        //    Tablica.ItemsSource = list;
        //}
        //private struct element
        //{ 
        //    public list<double> list { get; set; }
        //}



    }
}
