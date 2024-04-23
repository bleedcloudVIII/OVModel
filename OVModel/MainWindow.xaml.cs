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
using OVModel_CommonClasses;
using OVModel_ClassicalTheory;
using OVModel_Methods;

using OxyPlot;
using OxyPlot.Axes;


namespace OVModel
{
    public partial class MainWindow : Window
    {
        private const string DefaultTitleAxisX = "x, мм";
        private const string DefaultTitleAxisY = "Значение показателя преломления n";

        public MainWindow()
        {
            InitializeComponent();

            CreateColumns();
        }

        /*
         Модель
         1 - классическая
         2 - уточнённая
         Метод
         1 - аппроксимация полином 1 степени
         2 - аппроксимация полином 2 степени
         */
        static int model_number = 1;
        static int method_number = 2;

        List<EqualElements> equals_list = new List<EqualElements>();

        List<EqualElements> equals_list_nx_n = new List<EqualElements>();
        List<EqualElements> equals_list_ny_n = new List<EqualElements>();
        List<EqualElements> equals_list_nz_n = new List<EqualElements>();
        List<EqualElements> equals_list_nx_ny = new List<EqualElements>();
        List<EqualElements> equals_list_nx_nz = new List<EqualElements>();
        List<EqualElements> equals_list_ny_nz = new List<EqualElements>();

        List<EqualElements> tmp_equals = new List<EqualElements>();
        
        private void DrawScheduleAndTable()
        {
            if (isCanConvertToDouble(Input_2b.Text) &&
                isCanConvertToDouble(Input_h.Text) &&
                isCanConvertToDouble(Input_n.Text) &&
                isCanConvertToDouble(Input_R.Text) &&
                isCanConvertToDouble(Input_x_start.Text) &&
                isCanConvertToDouble(Input_x_end.Text))
            {
                // Получаю все значения
                double b = double.Parse(Input_2b.Text) / 2;
                double h = double.Parse(Input_h.Text);
                double n = double.Parse(Input_n.Text);
                double R = double.Parse(Input_R.Text);
                double x_start = double.Parse(Input_x_start.Text);
                double x_end = double.Parse(Input_x_end.Text);

                string title = InputScheduleTitle.Text;
                string titleAxisX = InputScheduleAxisX.Text;
                string titleAxisY = InputScheduleAxisY.Text;

                if (titleAxisX == "") titleAxisX = DefaultTitleAxisX;
                if (titleAxisY == "") titleAxisY = DefaultTitleAxisY;

                Cursor = Cursors.Wait;

                Data data;
                data = model_number == 1 ?
                    ClassicalTheory.Calculating(b, h, n, R, x_start, x_end, title, titleAxisX, titleAxisY) :
                    DopTheory.Calculating(b, h, n, R, x_start, x_end, title, titleAxisX, titleAxisY);

                Cursor = Cursors.Arrow;
                OxyPlotSchedule.Model = data.scheduleModel;
                Table.ItemsSource = data.itemsSourceTable;
                tmp_equals = data.equalsElements;
            }
            else if (
                Input_2b.Text != "" &&
                Input_h.Text != "" &&
                Input_n.Text != "" &&
                Input_R.Text != "" &&
                Input_x_start.Text != "" &&
                Input_x_end.Text != ""
                )
            {
                Error_input err_window = new Error_input();
                err_window.Show();
            }
        }

        private bool isCanConvertToDouble(string str)
        {
            try
            {
                double.Parse(str);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void CreateColumns()
        {
            DataGridTextColumn column1 = new DataGridTextColumn();
            column1.Header = "x";
            column1.Binding = new Binding($"[0]");
            column1.MaxWidth = 100;
            column1.MinWidth = 75;
            column1.IsReadOnly = true;
            Table.Columns.Add(column1);

            DataGridTextColumn column2 = new DataGridTextColumn();
            column2.Header = "n_x";
            column2.Binding = new Binding($"[1]");
            column2.MaxWidth = 125;
            column2.MinWidth = 100;
            column2.IsReadOnly = true;
            Table.Columns.Add(column2);

            DataGridTextColumn column3 = new DataGridTextColumn();
            column3.Header = "n_y";
            column3.Binding = new Binding($"[2]");
            column3.MaxWidth = 125;
            column3.MinWidth = 100;
            column3.IsReadOnly = true;
            Table.Columns.Add(column3);

            DataGridTextColumn column4 = new DataGridTextColumn();
            column4.Header = "n_z";
            column4.Binding = new Binding($"[3]");
            column4.MaxWidth = 125;
            column4.MinWidth = 100;
            column4.IsReadOnly = true;
            Table.Columns.Add(column4);

            DataGridTextColumn c1 = new DataGridTextColumn();
            c1.Header = "Пересечение";
            c1.Binding = new Binding($".cross");
            c1.MaxWidth = 115;
            c1.MinWidth = 90;
            c1.IsReadOnly = true;
            EqualsTable.Columns.Add(c1);

            DataGridTextColumn c2 = new DataGridTextColumn();
            c2.Header = "x";
            c2.Binding = new Binding($".x");
            c2.MaxWidth = 85;
            c2.MinWidth = 60;
            c2.IsReadOnly = true;
            EqualsTable.Columns.Add(c2);

            DataGridTextColumn c3 = new DataGridTextColumn();
            c3.Header = "n";
            c3.Binding = new Binding($".n_value");
            c3.MaxWidth = 100;
            c3.MinWidth = 75;
            c3.IsReadOnly = true;
            EqualsTable.Columns.Add(c3);
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            DrawScheduleAndTable();
        }

        private void Draw_Schedule_For_Points()
        {
            if (equals_list.Count != 0)
            {
                // Создание модели для графика
                OxyPlot.PlotModel tmp_model = new OxyPlot.PlotModel()
                {
                    //Title = title,
                    Legends = { new OxyPlot.Legends.Legend() { LegendPosition = OxyPlot.Legends.LegendPosition.LeftBottom } },
                    IsLegendVisible = true,
                    Axes =
                    {
                        new LinearAxis() { Title = "x", Position = AxisPosition.Bottom, IsZoomEnabled = false },
                        new LinearAxis() { Title = "n", Position = AxisPosition.Left,  IsZoomEnabled = false },
                    },
                };


                OxyPlot.Series.LineSeries lineSeries = new OxyPlot.Series.LineSeries
                {
                    Title = "Точки пересечения"
                };

                Cursor = Cursors.Wait;

                // Берутся точки, которые выбрал пользователь
                // Затем добавляются в модель
                for (int i = 0; i < equals_list.Count; i++)
                    lineSeries.Points.Add(new OxyPlot.DataPoint(equals_list[i].x, equals_list[i].n_value));

                if (method_number == 1)
                {
                    // Рассчёт по методу
                    //List<List<double>> result = Approksimacia.approksimacia_polinom_1(equals_list);

                    OxyPlot.Series.LineSeries lineSeries2 = new OxyPlot.Series.LineSeries
                    {
                        Title = "Аппроксимация(Полином 1 степени)"
                    };

                    //for (int i = 0; i < result[0].Count; i++) lineSeries2.Points.Add(new OxyPlot.DataPoint(result[0][i], result[1][i]));


                    tmp_model.Series.Add(lineSeries2);
                }
                else if (method_number == 2)
                {
                    // Рассчёт по методу
                    List<List<double>> result_nx_n = Approksimacia.approksimacia_polinom_2(equals_list_nx_n);
                    List<List<double>> result_ny_n = Approksimacia.approksimacia_polinom_2(equals_list_ny_n);
                    List<List<double>> result_nz_n = Approksimacia.approksimacia_polinom_2(equals_list_nz_n);
                    List<List<double>> result_nx_ny = Approksimacia.approksimacia_polinom_2(equals_list_nx_ny);
                    List<List<double>> result_nx_nz = Approksimacia.approksimacia_polinom_2(equals_list_nx_nz);
                    List<List<double>> result_ny_nz = Approksimacia.approksimacia_polinom_2(equals_list_ny_nz);

                    OxyPlot.Series.LineSeries lineSeries_nx_n = new OxyPlot.Series.LineSeries { Title = "n_x/n" };
                    OxyPlot.Series.LineSeries lineSeries_ny_n = new OxyPlot.Series.LineSeries { Title = "n_y/n" };
                    OxyPlot.Series.LineSeries lineSeries_nz_n = new OxyPlot.Series.LineSeries { Title = "n_z/n" };
                    OxyPlot.Series.LineSeries lineSeries_nx_ny = new OxyPlot.Series.LineSeries { Title = "n_x/n_y" };
                    OxyPlot.Series.LineSeries lineSeries_nx_nz = new OxyPlot.Series.LineSeries { Title = "n_x/n_z" };
                    OxyPlot.Series.LineSeries lineSeries_ny_nz = new OxyPlot.Series.LineSeries { Title = "n_x/n" };

                    // NOTE
                    // Добавление серии сделать в if блоке
                    if (result_nx_n.Count != 0) for (int i = 0; i < result_nx_n[0].Count; i++) lineSeries_nx_n.Points.Add(new OxyPlot.DataPoint(result_nx_n[0][i], result_nx_n[1][i]));
                    if (result_ny_n.Count != 0) for (int i = 0; i < result_ny_n[0].Count; i++) lineSeries_ny_n.Points.Add(new OxyPlot.DataPoint(result_ny_n[0][i], result_ny_n[1][i]));
                    if (result_nz_n.Count != 0) for (int i = 0; i < result_nz_n[0].Count; i++) lineSeries_nz_n.Points.Add(new OxyPlot.DataPoint(result_nz_n[0][i], result_nz_n[1][i]));
                    if (result_nx_ny.Count != 0) for (int i = 0; i < result_nx_ny[0].Count; i++) lineSeries_nx_ny.Points.Add(new OxyPlot.DataPoint(result_nx_ny[0][i], result_nx_ny[1][i]));
                    if (result_nx_nz.Count != 0) for (int i = 0; i < result_nx_nz[0].Count; i++) lineSeries_nx_nz.Points.Add(new OxyPlot.DataPoint(result_nx_nz[0][i], result_nx_nz[1][i]));
                    if (result_ny_nz.Count != 0) for (int i = 0; i < result_ny_nz[0].Count; i++) lineSeries_ny_nz.Points.Add(new OxyPlot.DataPoint(result_ny_nz[0][i], result_ny_nz[1][i]));

                    tmp_model.Series.Add(lineSeries_nx_n);
                    tmp_model.Series.Add(lineSeries_ny_n);
                    tmp_model.Series.Add(lineSeries_nz_n);
                    tmp_model.Series.Add(lineSeries_nx_ny);
                    tmp_model.Series.Add(lineSeries_nx_nz);
                    tmp_model.Series.Add(lineSeries_ny_nz);

                    OxyPlotScheduleApproksimacia.Model = tmp_model;
                    //if (result.Count != 0)
                    //{
                    //OxyPlot.Series.LineSeries lineSeries2 = new OxyPlot.Series.LineSeries
                    //{
                    //Title = "Аппроксимация(Полином 2 степени)"
                    //};

                    //for (int i = 0; i < result[0].Count; i++)
                    //lineSeries2.Points.Add(new OxyPlot.DataPoint(result[0][i], result[1][i]));

                    //tmp_model.Series.Add(lineSeries2);
                    //}
                }
                else
                {
                    // ERROR
                }

                Cursor = Cursors.Arrow;

                tmp_model.Series.Add(lineSeries);
                OxyPlotScheduleApproksimacia.Model = tmp_model;
            }
            else
            {
                OxyPlot.PlotModel tmp_model = new OxyPlot.PlotModel();
                //OxyPlotSchedule2.Model = tmp_model;
            }
        }

        private void Button_Click_Add_Points(object sender, RoutedEventArgs e)
        {
            /*
            EqualsTable.ItemsSource = new List<EqualElements>() { };
            for (int i = 0; i < tmp_equals.Count; i++)
            {
                EqualElements tmp = new EqualElements(tmp_equals[i]);
                tmp.n_value = Math.Round(tmp_equals[i].n_value, 6);
                tmp.x = Math.Round(tmp_equals[i].x, 6);

                equals_list.Add(tmp);
            }
            equals_list.Sort();
            equals_list.Reverse();
            */
            for (int i = 0; i  < tmp_equals.Count; i++)
            {
                EqualElements tmp = new EqualElements(tmp_equals[i]);
                tmp.n_value = Math.Round(tmp_equals[i].n_value, 6);
                tmp.x = Math.Round(tmp_equals[i].x, 6);

                Console.Write(tmp.cross);

                if (tmp.cross == "n_x/n") equals_list_nx_n.Add(tmp);
                else if (tmp.cross == "n_y/n") equals_list_ny_n.Add(tmp);
                else if (tmp.cross == "n_z/n") equals_list_nz_n.Add(tmp);
                else if (tmp.cross == "n_x/n_y") equals_list_nx_ny.Add(tmp);
                else if (tmp.cross == "n_x/n_z") equals_list_nx_nz.Add(tmp);
                else if (tmp.cross == "n_y/n_z") equals_list_ny_nz.Add(tmp);

                equals_list.Add(tmp);
            }

            EqualsTable.ItemsSource = equals_list;



            if (equals_list.Count != 0)
            {
                Input_x_end.IsReadOnly = true;
                Input_x_start.IsReadOnly = true;
                Input_h.IsReadOnly = true;
                SbrosButton.Visibility = Visibility.Visible;
                Draw_Schedule_For_Points();
            }
        }

        private void Button_Click_Sbros(object sender, RoutedEventArgs e)
        {
            Input_x_end.IsReadOnly = false;
            Input_x_start.IsReadOnly = false;
            Input_h.IsReadOnly = false;
            SbrosButton.Visibility = Visibility.Hidden;

            equals_list = new List<EqualElements>();
            EqualsTable.ItemsSource = equals_list;
            Draw_Schedule_For_Points();
        }

        private string GetExtension(string fileName)
        {
            int i = fileName.Length - 1;
            string result = "";
            while (fileName[i] != '.')
            {
                result += fileName[i];
                i--;
            }
            char[] charArray = result.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private void MenuItem_Click_Save_Schedule(object sender, RoutedEventArgs e)
        {
            if (OxyPlotSchedule.Model == null)
            {
                Error_save_without_schedule err_window = new Error_save_without_schedule();
                err_window.Show();
            }
            else
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "schedule"; // Default file name
                dlg.DefaultExt = ".png"; // Default file extension
                dlg.Filter = "jpeg image (.jpg)|*.jpg|pdf docuemnt (.pdf)|*.pdf|png image (.png)|*.png";
                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    string extension = GetExtension(dlg.SafeFileName);

                    if (extension == "pdf")
                    {
                        Export.Export_Schedule_pdf(OxyPlotSchedule, dlg);
                    }
                    else if (extension == "png")
                    {
                        Export.Export_Schedule_png(OxyPlotSchedule, dlg);
                    }
                    else if (extension == "jpg")
                    {
                        Export.Export_Schedule_jpg(OxyPlotSchedule, dlg);

                    }
                }
            }
        }

        private void MenuItem_Click_Save_Schedule_2(object sender, RoutedEventArgs e)
        {
            //if (OxyPlotSchedule2.Model == null)
            //{
            //    Error_save_without_schedule err_window = new Error_save_without_schedule();
            //    err_window.Show();
            //}
            //else
            //{
            //    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            //    dlg.FileName = "schedule"; // Default file name
            //    dlg.DefaultExt = ".png"; // Default file extension
            //    dlg.Filter = "jpeg image (.jpg)|*.jpg|pdf docuemnt (.pdf)|*.pdf|png image (.png)|*.png";
            //    Nullable<bool> result = dlg.ShowDialog();

            //    if (result == true)
            //    {
            //        string extension = GetExtension(dlg.SafeFileName);

            //        if (extension == "pdf")
            //        {
            //            Export.Export_Schedule_pdf(OxyPlotSchedule2, dlg);
            //        }
            //        else if (extension == "png")
            //        {
            //            Export.Export_Schedule_png(OxyPlotSchedule2, dlg);
            //        }
            //        else if (extension == "jpg")
            //        {
            //            Export.Export_Schedule_jpg(OxyPlotSchedule2, dlg);

            //        }
            //    }
            //}
        }

        private void MenuItem_Click_Save_Table(object sender, RoutedEventArgs e)
        {
            if (Table.Items.OfType<List<double>>().ToList().ToList().Count == 0)
            {
                Error_save_without_table err_window = new Error_save_without_table();
                err_window.Show();
            }
            else
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "table"; // Default file name
                dlg.DefaultExt = ".png"; // Default file extension
                dlg.Filter = "Excel document (.xlsx)|*.xlsx|Pdf documents (.pdf)|*.pdf";

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    string extension = GetExtension(dlg.SafeFileName);
                    Console.WriteLine(extension);
                    if (extension == "pdf")
                    {
                        Export.Export_Table_pdf(Table, dlg);
                    }
                    else if (extension == "xlsx")
                    {
                        Export.Export_Table_xlsx(Table, dlg);
                    }
                }
            }
        }

        private void MenuItem_Click_Spravka_About(object sender, RoutedEventArgs e)
        {
            Spravka_about spravka_window = new Spravka_about();
            spravka_window.Show();
        }

        private void MenuItem_Click_Spravka(object sender, RoutedEventArgs e)
        {
            Spravka_show_spravka spravka_window = new Spravka_show_spravka();
            spravka_window.Show();
        }

        private void MenuItem_Click_Model_1(object sender, RoutedEventArgs e)
        {
            model_number = 1;
            DrawScheduleAndTable();
        }

        private void MenuItem_Click_Model_2(object sender, RoutedEventArgs e)
        {
            model_number = 2;
            DrawScheduleAndTable();
        }

        private void MenuItem_Click_Method_1(object sender, RoutedEventArgs e)
        {
            method_number = 1;
            if (equals_list.Count != 0) Draw_Schedule_For_Points();
        }

        private void MenuItem_Click_Method_2(object sender, RoutedEventArgs e)
        {
            method_number = 2;
            if (equals_list.Count != 0) Draw_Schedule_For_Points();
        }
    }
}