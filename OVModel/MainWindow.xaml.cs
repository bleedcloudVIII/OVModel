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
using OVModel_CommonClasses;


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
        public static string TitleAxisX = "x, мм";

        public static string TitleAxisY = "Значение показателя преломления n";

        public MainWindow()
        {
            InitializeComponent();

            CreateColumns();
        }

        private void DrawScheduleAndTable()
        {
            //Проверяю все ли значения можно перевести в double
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

                Data data = OVModel_DopTheory.DopTheory.Calculating(b, h, n, n_ob, R, x_start, x_end);

                OxyPlotSchedule.Model = data.scheduleModel;
                Table.ItemsSource = data.itemsSourceTable;

                PointsLabel.Content = $"Пересечения:\n";
                
                for (int i = 0; i < data.equalsElements.Count; i++)
                {
                    PointsLabel.Content += $"{data.equalsElements[i].first} и {data.equalsElements[i].second}:\n x = {data.equalsElements[i].x}\n n = {data.equalsElements[i].n_value}\n";
                }
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
            column1.MaxWidth = 50;
            column1.IsReadOnly = true;
            Table.Columns.Add(column1);


            DataGridTextColumn column2 = new DataGridTextColumn();
            column2.Header = "n_x";
            column2.Binding = new Binding($"[1]");
            column2.MaxWidth = 75;
            column2.IsReadOnly = true;
            Table.Columns.Add(column2);

            DataGridTextColumn column3 = new DataGridTextColumn();
            column3.Header = "n_y";
            column3.Binding = new Binding($"[2]");
            column3.MaxWidth = 75;
            column3.IsReadOnly = true;
            Table.Columns.Add(column3);

            DataGridTextColumn column4 = new DataGridTextColumn();
            column4.Header = "n_z";
            column4.Binding = new Binding($"[3]");
            column4.MaxWidth = 75;
            column4.IsReadOnly = true;
            Table.Columns.Add(column4);

        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            DrawScheduleAndTable();
        }

        private void MenuItem_Click_ChangeTitleAxisX(object sender, RoutedEventArgs e)
        {
            //ChangeNameAxisWindow window = new ChangeNameAxisWindow();
            //window.Show();  
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
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "schedule"; // Default file name
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "Text documents (.jpg)|*.jpg|Text documents (.pdf)|*.pdf"; // Filter files by extension
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string extension = GetExtension(dlg.SafeFileName);
                
                if (extension == "pdf" && OxyPlotSchedule.Model != null)
                {
                    OVModel_DopTheory.Export.Export_Schedule_pdf(OxyPlotSchedule, dlg);
                }
                else if(extension == "jpg" && OxyPlotSchedule.Model != null)
                {
                    OVModel_DopTheory.Export.Export_Schedule_png(OxyPlotSchedule, dlg);
                }
                else { }
            }
        }

        private void MenuItem_Click_Save_Table(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "schedule"; // Default file name
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "Text documents (.pdf)|*.pdf"; // Filter files by extension

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                OVModel_DopTheory.Export.Export_Table_pdf(Table, dlg);
            }
        }
    }
}
