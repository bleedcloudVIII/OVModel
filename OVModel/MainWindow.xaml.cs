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

        }

        //private void DrawSchedule(PointCollection points_n_x, PointCollection points_n_y, PointCollection points_n_z, PointCollection point_n )
        //{
        //    //List<Point> values = new List<Point>();
        //    PointCollection v = new PointCollection();
        //    v.Add(new Point() { X = 1, Y = 2});
        //    v.Add(new Point() { X = 3, Y = 4 });
        //    v.Add(new Point() { X = 5, Y = 6 });
        //    v.Add(new Point() { X = 7, Y = 8 });
        //    v.Add(new Point() { X = 9, Y = 10 });
        //    Schedule.Points = v;
        //}

        private void CreateColumns()
        {
            DataGridTextColumn column1 = new DataGridTextColumn();
            column1.Header = "x";
            column1.Binding = new Binding($"[0]");
            column1.MaxWidth = 50;
            Table.Columns.Add(column1);


            DataGridTextColumn column2 = new DataGridTextColumn();
            column2.Header = "n_x";
            column2.Binding = new Binding($"[1]");
            column2.MaxWidth = 75;
            Table.Columns.Add(column2);

            DataGridTextColumn column3 = new DataGridTextColumn();
            column3.Header = "n_y";
            column3.Binding = new Binding($"[2]");
            column3.MaxWidth = 75;
            Table.Columns.Add(column3);

            DataGridTextColumn column4 = new DataGridTextColumn();
            column4.Header = "n_z";
            column4.Binding = new Binding($"[3]");
            column4.MaxWidth = 75;
            Table.Columns.Add(column4);
            
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
                PointCollection points_n_x = new PointCollection();
                PointCollection points_n_y = new PointCollection();
                PointCollection points_n_z = new PointCollection();
                PointCollection points_n = new PointCollection();

                double scheduleMarginLeft = ScheduleGrid.Margin.Left;
                double scheduleMarginRight = ScheduleGrid.Margin.Right;
                double scheduleMarginTop = ScheduleGrid.Margin.Top;
                double scheduleMarginBottom = ScheduleGrid.Margin.Bottom;

                double scheduleWidth = scheduleMarginLeft - scheduleMarginRight;
                double scheduleHeight = scheduleMarginTop - scheduleMarginBottom;

                double coefficientMultiplication = scheduleWidth / count;

                Console.WriteLine(scheduleMarginLeft);
                Console.WriteLine(scheduleMarginRight);
                Console.WriteLine(scheduleWidth);
                Console.WriteLine(scheduleHeight);

                points_n.Add(new Point() { X = n * 100, Y = x * 100});
                points_n.Add(new Point() { X = n * 100, Y = (x + h * count) * 100});

                for (int i = 0; i < count; i++)
                {
                    double x_now = x + h * i;
                    double n_x = OVModel_DopTheory.DopTheory.n_x(x_now, n, R, b);
                    double n_y = OVModel_DopTheory.DopTheory.n_y(x_now, n, R, b);
                    double n_z = OVModel_DopTheory.DopTheory.n_z(x_now, n, R, b);
                    result.Add(new List<double> { x_now, n_x, n_y, n_y});
                    points_n_x.Add(new Point() { X = n_x * 100, Y = x_now * 100 });
                    points_n_y.Add(new Point() { X = n_y * 100, Y = x_now * 100});
                    points_n_z.Add(new Point() { X = n_z * 100, Y = x_now * 100});

                }

                Table.ItemsSource = result;
                Schedule_n_x.Points = points_n_x;
                Schedule_n_y.Points = points_n_y;
                PointCollection s = new PointCollection();
                s.Add(new Point() { X = 0, Y = 0 });
                s.Add(new Point() { X = 100, Y = 200 });
                //Schedule_n_z.Points = points_n_z;
                Schedule_n_z.Points = s;

                Schedule_n.Points = points_n;

                //DrawSchedule(points_n_x, points_n_y, points_n_z, points_n);

                Console.WriteLine("End Calculating.");
            }
            else
            {
                Console.WriteLine("ELSE!");
            }
            

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
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartCalculation();
            //drawSchedule();
        }


    }
}
