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


using OVModel_CommonClasses;
using OVModel_Methods;
using OVModel_Errors;
using OVModel_Spravka;
using OVModel.Lib.Export;
using OVModel.Lib.ClassicalTeory;
using OVModel.Lib.DopTheory;
using OVModel.Lib.Dot;
using OVModel.Lib.EqualElements;
using OVModel.Lib.CalculatingResult;
using OVModel.Lib.UserInput;

using OxyPlot;
using OxyPlot.Axes;
using System.Diagnostics.Eventing.Reader;



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

            for (int i = 0; i < CrossingCords.Count; i++)
                equals.Add(CrossingCords[i], new List<EqualElements>() { });

            //equals.Add("n_x/n", new List<EqualElements>() { });
            //equals.Add("n_y/n", new List<EqualElements>() { });
            //equals.Add("n_z/n", new List<EqualElements>() { });
            //equals.Add("n_x/n_y", new List<EqualElements>() { });
            //equals.Add("n_x/n_z", new List<EqualElements>() { });
            //equals.Add("n_y/n_z", new List<EqualElements>() { });


        }

        /*
         Модель
         1 - классическая
         2 - уточнённая
         Метод
         1 - аппроксимация полином 1 степени
         2 - аппроксимация полином 2 степени
         3 - интерполяция
         */
        static int model_number = 1;
        static int method_number = 2;

        List<EqualElements> equals_list = new List<EqualElements>();

        //List<EqualElements> equals_list_nx_n = new List<EqualElements>();
        //List<EqualElements> equals_list_ny_n = new List<EqualElements>();
        //List<EqualElements> equals_list_nz_n = new List<EqualElements>();
        //List<EqualElements> equals_list_nx_ny = new List<EqualElements>();
        //List<EqualElements> equals_list_nx_nz = new List<EqualElements>();
        //List<EqualElements> equals_list_ny_nz = new List<EqualElements>();

        public Dictionary<String, List<EqualElements>> equals = new Dictionary<string, List<EqualElements>>();
        public List<String> CrossingCords = new List<String>() { "n_x/n", "n_y/n", "n_z/n", "n_x/n_y", "n_x/n_z", "n_y/n_z"};

        List<EqualElements> cur_equals = new List<EqualElements>();
        
        private void DrawScheduleAndTable()
        {

            UserInput userInput = new UserInput();
            
            int result = userInput.setValues((MainWindow)Application.Current.MainWindow);

            if (result == -1)
            {
                Error_input err_window = new Error_input();
                err_window.Show();
                return;
            }

            string title = InputScheduleTitle.Text;
            string titleAxisX = InputScheduleAxisX.Text;
            string titleAxisY = InputScheduleAxisY.Text;

            if (titleAxisX == "") titleAxisX = DefaultTitleAxisX;
            if (titleAxisY == "") titleAxisY = DefaultTitleAxisY;

            PlotModel schedule = new PlotModel()
            {
                Title = title,
                Legends = { new OxyPlot.Legends.Legend() { LegendPosition = OxyPlot.Legends.LegendPosition.LeftBottom } },
                IsLegendVisible = true,
                Axes =
            {
                new LinearAxis() {Title = titleAxisX, Position = AxisPosition.Bottom, IsPanEnabled = false, IsZoomEnabled = false },
                new LinearAxis() {Title = titleAxisY, Position = AxisPosition.Left, IsPanEnabled = false, IsZoomEnabled = false },
            },
            };

            Cursor = Cursors.Wait;

            CalculatingResult data;
            data = model_number == 1 ?
                ClassicalTheory.Calculating(userInput, schedule) :
                DopTheory.Calculating(userInput, schedule);

            Cursor = Cursors.Arrow;
            OxyPlotSchedule.Model = data.scheduleModel;
            Table.ItemsSource = data.itemsSourceTable;
            cur_equals = data.equalsElements;
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

        private void Button_Click_Calculating(object sender, RoutedEventArgs e)
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

                Cursor = Cursors.Wait;

                foreach (var equalsElements in equals)
                {
                    if (equalsElements.Value.Count > 1)
                    {
                        List<List<double>> result = new List<List<double>>() { };

                        if (method_number == 1) result = Approksimacia.approksimacia_polinom_2(equalsElements.Value);
                        else if (method_number == 2) result = Approksimacia.approksimacia_line(equalsElements.Value);
                        else if (method_number == 3) result = Interpolyacia.interpolyacia_lagrange(equalsElements.Value);
                        else
                        {
                            // ERROR
                        }


                        if (result[0].Count != 0)
                        {
                            OxyPlot.Series.LineSeries lineSeries = new OxyPlot.Series.LineSeries() { Title = equalsElements.Key };
                            for (int i = 0; i < result[0].Count; i++) lineSeries.Points.Add(new OxyPlot.DataPoint(result[0][i], result[1][i]));
                            tmp_model.Series.Add(lineSeries);
                        }
                    }
                }


                Cursor = Cursors.Arrow;
                OxyPlotScheduleApproksimacia.Model = tmp_model;
            }
            else
            {
                // Произошёл сброс данных
                OxyPlot.PlotModel tmp_model1 = new OxyPlot.PlotModel() { };
                OxyPlot.PlotModel tmp_model2 = new OxyPlot.PlotModel() { };

                OxyPlotScheduleApproksimacia.Model = tmp_model1;
                OxyPlotSchedulePeresechenie.Model = tmp_model2;
                //OxyPlotSchedule2.Model = tmp_model;
            }
        }

        private void Button_Click_Add_Points(object sender, RoutedEventArgs e)
        {
            EqualsTable.ItemsSource = new List<EqualElements>() { };
            for (int i = 0; i  < cur_equals.Count; i++)
            {
                EqualElements tmp = new EqualElements(cur_equals[i]);
                tmp.n_value = Math.Round(cur_equals[i].n_value, 6);
                tmp.x = Math.Round(cur_equals[i].x, 6);

                if (tmp.cross == "n_x/n") equals["n_x/n"].Add(tmp);
                else if (tmp.cross == "n_y/n") equals["n_y/n"].Add(tmp);
                else if (tmp.cross == "n_z/n") equals["n_z/n"].Add(tmp);
                else if (tmp.cross == "n_x/n_y") equals["n_x/n_y"].Add(tmp);
                else if (tmp.cross == "n_x/n_z") equals["n_x/n_z"].Add(tmp);
                else if (tmp.cross == "n_y/n_z") equals["n_y/n_z"].Add(tmp);

                equals_list.Add(tmp);
            }
            equals_list.Sort();
            equals_list.Reverse();
            EqualsTable.ItemsSource = equals_list;

            OxyPlot.PlotModel tmp_model = new OxyPlot.PlotModel()
            {
                Legends = { new OxyPlot.Legends.Legend() { LegendPosition = OxyPlot.Legends.LegendPosition.LeftBottom } },
                IsLegendVisible = true,
                Axes =
                    {
                        new LinearAxis() { Title = "x", Position = AxisPosition.Bottom, IsZoomEnabled = false },
                        new LinearAxis() { Title = "n", Position = AxisPosition.Left,  IsZoomEnabled = false },
                    },
            };

            foreach (var equalsElements in equals.ToArray())
            {
                if (equalsElements.Value.Count > 1)
                {
                    OxyPlot.Series.LineSeries tmp = new OxyPlot.Series.LineSeries { Title = equalsElements.Key };
                    for (int j = 0; j < equalsElements.Value.Count; j++) tmp.Points.Add(new OxyPlot.DataPoint(equalsElements.Value[j].x, equalsElements.Value[j].n_value));
                    tmp_model.Series.Add(tmp);
                }
            }

            OxyPlotSchedulePeresechenie.Model = tmp_model;

            if (equals_list.Count != 0)
            {
                Input_x_end.IsReadOnly = true;
                Input_x_start.IsReadOnly = true;
                Input_h.IsReadOnly = true;
                SbrosButton.Visibility = Visibility.Visible;
                Draw_Schedule_For_Points();
            }
        }

        private void Button_Click_Create_Model(object sender, RoutedEventArgs e)
        {
            ModelOfOV modelOfOV_window = new ModelOfOV();
            modelOfOV_window.Show();
        }

        private void Button_Click_Sbros(object sender, RoutedEventArgs e)
        {
            Input_x_end.IsReadOnly = false;
            Input_x_start.IsReadOnly = false;
            Input_h.IsReadOnly = false;
            SbrosButton.Visibility = Visibility.Hidden;

            for (int i = 0; i < CrossingCords.Count; i++) equals[CrossingCords[i]] = new List<EqualElements>();

            cur_equals = new List<EqualElements>();
            equals_list = new List<EqualElements>();
            EqualsTable.ItemsSource = equals_list;
            Draw_Schedule_For_Points();
            DrawScheduleAndTable();
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

        private void MenuItem_Click_Save_Schedule_Peresech(object sender, RoutedEventArgs e)
        {
            if (OxyPlotSchedulePeresechenie.Model == null)
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
                        Export.Export_Schedule_pdf(OxyPlotSchedulePeresechenie, dlg);
                    }
                    else if (extension == "png")
                    {
                        Export.Export_Schedule_png(OxyPlotSchedulePeresechenie, dlg);
                    }
                    else if (extension == "jpg")
                    {
                        Export.Export_Schedule_jpg(OxyPlotSchedulePeresechenie, dlg);

                    }
                }
            }
        }

        private void MenuItem_Click_Save_Schedule_Tendencia(object sender, RoutedEventArgs e)
        {
            if (OxyPlotScheduleApproksimacia.Model == null)
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
                        Export.Export_Schedule_pdf(OxyPlotScheduleApproksimacia, dlg);
                    }
                    else if (extension == "png")
                    {
                        Export.Export_Schedule_png(OxyPlotScheduleApproksimacia, dlg);
                    }
                    else if (extension == "jpg")
                    {
                        Export.Export_Schedule_jpg(OxyPlotScheduleApproksimacia, dlg);

                    }
                }
            }
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

        private void MenuItem_Click_Method_3(object sender, RoutedEventArgs e)
        {
            method_number = 3;
            if (equals_list.Count != 0) Draw_Schedule_For_Points();
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            DrawScheduleAndTable();
        }
    }
}