﻿using System;
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
            // Проверяю все ли значения можно перевести в double
            if (isCanConvertToDouble(Input_2b.Text) &&
                isCanConvertToDouble(Input_h.Text) &&
                isCanConvertToDouble(Input_n.Text) &&
                isCanConvertToDouble(Input_n_ob.Text) &&
                isCanConvertToDouble(Input_R.Text) &&
                isCanConvertToDouble(Input_x_start.Text) &&
                isCanConvertToDouble(Input_x_end.Text))
            {
                // Получаю все значения
                double b = System.Convert.ToDouble(Input_2b.Text) / 2;
                double h = System.Convert.ToDouble(Input_h.Text);
                double n = System.Convert.ToDouble(Input_n.Text);
                double n_ob = System.Convert.ToDouble(Input_n_ob.Text);
                double R = System.Convert.ToDouble(Input_R.Text);
                double x_start = System.Convert.ToDouble(Input_x_start.Text);
                double x_end = System.Convert.ToDouble(Input_x_end.Text);

                // Список элементов x, n_x, n_y, n_z для таюлицы
                List<List<double>> result = new List<List<double>>();

                // Количество x в таблице
                int count = System.Convert.ToInt32(Math.Abs(x_end - x_start) / h);
                //Console.WriteLine();

                // Списки точек x и n для графика (x = n, Y = x)
                PointCollection points_n_x = new PointCollection();
                PointCollection points_n_y = new PointCollection();
                PointCollection points_n_z = new PointCollection();
                PointCollection points_n = new PointCollection();

                // Минимальное значение n, которое будет на графике "0"
                double n_min = double.MaxValue;
                double n_max = double.MinValue;

                double scheduleWidth = BorderForSchedule.ActualWidth;
                double scheduleHeigth = BorderForSchedule.ActualHeight;

                double coef = scheduleHeigth / count;

                // Точки для прямой n
                points_n.Add(new Point() { X = 0, Y = scheduleHeigth - n});
                points_n.Add(new Point() { X = scheduleWidth, Y = scheduleHeigth - n});

                for (int i = 0; i < count; i++)
                {
                    // Вычисление значений текущего x и значений n
                    double x_now = x_start + h * i;
                    double n_x = OVModel_DopTheory.DopTheory.n_x(x_now, n, R, b);
                    double n_y = OVModel_DopTheory.DopTheory.n_y(x_now, n, R, b);
                    double n_z = OVModel_DopTheory.DopTheory.n_z(x_now, n, R, b);

                    // Нахождение минимального и максимального n среди текущих значений
                    n_min = Math.Min(Math.Min(Math.Min(n_min, n_x), n_y), n_z);
                    n_max = Math.Max(Math.Max(Math.Max(n_max, n_x), n_y), n_z);

                    result.Add(new List<double> { x_now, n_x, n_y, n_y });

                    points_n_x.Add(new Point() { X = x_now, Y = scheduleHeigth - n_x });
                    points_n_y.Add(new Point() { X = x_now, Y = scheduleHeigth - n_y });
                    points_n_z.Add(new Point() { X = x_now, Y = scheduleHeigth - n_z });

                }
                // От значений n отнять height?

                //double mins = BorderForSchedule.ActualHeight;
                //double maxs = BorderForSchedule.ActualWidth;

                //points_n = SubtractMinN(points_n, n_min);
                //points_n_x = SubtractMinN(points_n_x, n_min);
                //points_n_y = SubtractMinN(points_n_y, n_min);
                //points_n_z = SubtractMinN(points_n_z, n_min);

                // Нахожу коэффициент умножения, чтобы увелечить значение n для точке
                // Т.к. сейчас их значения слишком малы, чтобы с ними можно было нормально работать на графике
                //int coef = 250;

                //for (int i = 0; i < points_n_x.Count; i++)
                //{
                //    Point point = points_n_x[i];
                //    point.Y *= coef;
                //    point.Y = mins - point.X;
                //    point.X *= coef;
                //    point.X = maxs - point.Y;
                //    points_n_x[i] = point;
                //}

                Console.WriteLine(BorderForSchedule.ActualHeight);
                Console.WriteLine(BorderForSchedule.ActualWidth);

                Console.WriteLine(n_min);
                Console.WriteLine(n_max);


                Table.ItemsSource = result;
                Schedule_n_x.Points = points_n_x;
                Schedule_n_y.Points = points_n_y;
                Schedule_n_z.Points = points_n_z;
                Schedule_n.Points = points_n;

                //DrawSchedule(points_n_x, points_n_y, points_n_z, points_n);

                Console.WriteLine("End Calculating.");
            }
            else
            {
                Console.WriteLine("ELSE!");
            }
            

        }

        private PointCollection SubtractMinN(PointCollection c, double n_min)
        {
            // Т.к. график начинается с 0, мы от чисел отнимает минимальную n(представляя что её значение будет 0, т.е начальным)
            // И тем самым получаем правильные координаты для построения графиков
            for (int i = 0; i < c.Count; i++)
            {
                Point point = c[i];
                point.X -= n_min;
                c[i] = point;
            }
            return c;
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
