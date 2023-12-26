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



//using System.IO;
//using System.Drawing;

//using iText.Kernel.Pdf;
//using iText.Layout;
//using iText.Layout.Element;
//using iText.Layout.Properties;
//using iText.IO.Font;
//using iText.Kernel.Pdf.Canvas.Draw;
//using iText.Kernel.Colors;
//using iText.Kernel.Font;
//using iText.IO.Font.Constants;

//using OxyPlot;
//using OxyPlot.Wpf;
//using OxyPlot.Axes;
//using OxyPlot.Series;

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
         1 - классическая
         2 - уточнённая
         */
        static int model_number = 1;

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
                double b = System.Convert.ToDouble(Input_2b.Text) / 2;
                double h = System.Convert.ToDouble(Input_h.Text);
                double n = System.Convert.ToDouble(Input_n.Text);
                double R = System.Convert.ToDouble(Input_R.Text);
                double x_start = System.Convert.ToDouble(Input_x_start.Text);
                double x_end = System.Convert.ToDouble(Input_x_end.Text);

                string title = InputScheduleTitle.Text;
                string titleAxisX = InputScheduleAxisX.Text;
                string titleAxisY = InputScheduleAxisY.Text;

                if (titleAxisX == "") titleAxisX = DefaultTitleAxisX;
                if (titleAxisY == "") titleAxisY = DefaultTitleAxisY;

                Data data;
                data = model_number == 1 ? 
                    ClassicalTheory.Calculating(b, h, n, R, x_start, x_end, title, titleAxisX, titleAxisY) :
                    DopTheory.Calculating(b, h, n, R, x_start, x_end, title, titleAxisX, titleAxisY);

                OxyPlotSchedule.Model = data.scheduleModel;
                Table.ItemsSource = data.itemsSourceTable;
                EqualsTable.ItemsSource = data.equalsElements;
            }
            else if(
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
                System.Convert.ToDouble(str);
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
                dlg.Filter = "jpeg image (.jpg)|*.jpg|pdf docuemnt (.pdf)|*.pdf|png image (.png)|*.png"; // Filter files by extension
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
                    else { }
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
                dlg.Filter = "Excel document (.xlsx)|*.xlsx|Pdf documents (.pdf)|*.pdf"; // Filter files by extension

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
    }
}
