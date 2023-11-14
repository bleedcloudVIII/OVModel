﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OxyPlot;
using OxyPlot.Wpf;
using OxyPlot.Axes;

using iText.Kernel.Pdf;
using iText.Layout;

using OVModel_CommonClasses;

namespace OVModel_DopTheory
{
    public static class DopTheory 
    {
        // Global Variable
        private const double p11 = 0.121;
        private const double p12 = 0.27;
        private const double mu = 0.164;

        private static double n_x(double x, double n, double R, double b)
        {
            double mu_R_R = mu / (R * R);
            double f = mu_R_R * x * x * (p12 - (p11 / (2 * mu)));
            double mu_R = mu / R;
            double s = mu_R * x * (p11 + p12 - (p12 / mu));
            double t = mu_R_R * b * b * ((p11 / (2 * mu)) - p12);
            return n + ((n * n * n) / 2) * (f + s + t);
        }

        private static double n_y(double x, double n, double R, double b)
        {
            double mu_R_R = mu / (R * R);
            double f = mu_R_R * x * x * (p11 - (p12 / mu) + p12);
            double mu_R = mu / R;
            double s = mu_R * x * (2 * p11 + 2 * p12 - (2 * p12 / mu));
            double t = mu_R_R * b * b * ((p12 / mu) - p12 - p11);
            return n + ((n * n * n) / 4) * (f + s + t);
        }

        private static double n_z(double x, double n, double R, double b)
        {
            double mu_R_R = mu / (R * R);
            double f = mu_R_R * x * x * (p11 - (p12 / mu) + p12);
            double mu_R = mu / R;
            double s = mu_R * x * (4 * p12 - ((2*p11)/mu));
            double t = mu_R_R * b * b * ((p12 / mu) - p12 - p11);
            return n + ((n * n * n) / 4) * (f + s + t);
        }

        private const string DefaultTitleAxisX = "x, мм";
        private const string DefaultTitleAxisY = "Значение показателя преломления n";

        private static List<EqualElements> getSetList(List<EqualElements> list)
        {
            List<EqualElements> result = new List<EqualElements>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!result.Contains(list[i])) result.Add(list[i]);
            }
            return result;
        }


        public static Data Calculating(double b, double h, double n, double n_ob, double R, double x_start, double x_end)
        {
            // Список элементов x, n_x, n_y, n_z для таблицы
            List<List<double>> result = new List<List<double>>();

            // Количество x в таблице
            int count = 0;
            if (h != 0) count = System.Convert.ToInt32(Math.Abs(x_end - x_start) / h);

            PlotModel schedule = new PlotModel() {
                Legends = { new OxyPlot.Legends.Legend() { LegendPosition = OxyPlot.Legends.LegendPosition.LeftBottom } },
                IsLegendVisible = true,
                Axes =
                {
                    new LinearAxis() {Title = DefaultTitleAxisX, Position = AxisPosition.Bottom, IsPanEnabled = false, IsZoomEnabled = false },
                    new LinearAxis() {Title = DefaultTitleAxisY, Position = AxisPosition.Left, IsPanEnabled = false, IsZoomEnabled = false },
                },
            };

            OxyPlot.Series.LineSeries lineSeries_n = new OxyPlot.Series.LineSeries() { Points = { new OxyPlot.DataPoint(x_start, n), new OxyPlot.DataPoint(x_end, n) }, Title = "n" };

            OxyPlot.Series.LineSeries lineSeries_n_x = new OxyPlot.Series.LineSeries();
            lineSeries_n_x.Title = "n_x";

            OxyPlot.Series.LineSeries lineSeries_n_y = new OxyPlot.Series.LineSeries();
            lineSeries_n_y.Title = "n_y";

            OxyPlot.Series.LineSeries lineSeries_n_z = new OxyPlot.Series.LineSeries();
            lineSeries_n_z.Title = "n_z";

            List<EqualElements> equalsElements = new List<EqualElements>();

            double n_x_prev = 0, n_y_prev = 0, n_z_prev = 0, x_prev = 0;

            for (int i = 0; i <= count; i++)
            {
                // Вычисление значений текущего x и значений n
                double x_now = x_start + h * i;
                double n_x = OVModel_DopTheory.DopTheory.n_x(x_now, n, R, b);
                double n_y = OVModel_DopTheory.DopTheory.n_y(x_now, n, R, b);
                double n_z = OVModel_DopTheory.DopTheory.n_z(x_now, n, R, b);

                // Если значения n равны, то помещаем в список элементов точки, пересечения
                if (n_x == n_y) equalsElements.Add(new EqualElements() { x = x_now, first = "n_x", second = "n_y", n_value = n_x });
                else if (n_x == n_z) equalsElements.Add(new EqualElements() { x = x_now, first = "n_x", second = "n_z", n_value = n_x });
                else if (n_y == n_z) equalsElements.Add(new EqualElements() { x = x_now, first = "n_y", second = "n_z", n_value = n_y });
                // Т.к. иногда может быть пересечения графиков, не в точках x, а между ними
                // Поэтому мы берём 4 точки (2 предыдущих для n и две текущих) и находим их точки пересечения
                // x пред     x текущее
                // (x3,y3)
                //      \    (x2,y2)
                //       \   /
                //        \ / 
                //         X
                //        / \
                //  (x1,y1)  \
                //          (x4,y4)
                // В кратце, если точка 3 находится выше(или =) точки 1, а точка 4 ниже 2
                // Значит у двух векторов есть точка пересечения, которую мы и расчитываем
                // Функция для нахождения точки пересечения взята из интернета https://habr.com/ru/articles/523440/
                else if ((n_y_prev >= n_x) && (n_x >= n_y))
                {
                    Dot dot = Dot.CrossTwoLines(x_prev, n_x_prev, x_now, n_x, x_prev, n_y_prev, x_now, n_y);
                    equalsElements.Add(new EqualElements() { x = dot.x, first = "n_y", second = "n_x", n_value = dot.y });
                }
                else if ((n_y_prev >= n_z) && (n_z >= n_y))
                {
                    Dot dot = Dot.CrossTwoLines(x_prev, n_z_prev, x_now, n_z, x_prev, n_y_prev, x_now, n_y);
                    equalsElements.Add(new EqualElements() { x = dot.x, first = "n_y", second = "n_z", n_value = dot.y });
                }
                else if ((n_x_prev >= n_z) && (n_z >= n_x))
                {
                    Dot dot = Dot.CrossTwoLines(x_prev, n_z_prev, x_now, n_z, x_prev, n_x_prev, x_now, n_x);
                    equalsElements.Add(new EqualElements() { x = dot.x, first = "n_x", second = "n_z", n_value = dot.y });
                }

                lineSeries_n_x.Points.Add(new OxyPlot.DataPoint(x_now, n_x));
                lineSeries_n_y.Points.Add(new OxyPlot.DataPoint(x_now, n_y));
                lineSeries_n_z.Points.Add(new OxyPlot.DataPoint(x_now, n_z));

                result.Add(new List<double> { x_now, n_x, n_y, n_z });

                n_x_prev = n_x;
                n_y_prev = n_y;
                n_z_prev = n_z;
                x_prev = x_now;
            }

            // Нахождение уникальных точек пересечения, т.к. точки могут пересекаться
            // Например, значения n равны в точки x
            // И значения n равны в точке пересечения графиков, при этом n отличаются на 0.0...01
            // То есть по сути являясь одной точкой
            equalsElements = getSetList(equalsElements);

            schedule.Series.Add(lineSeries_n);
            schedule.Series.Add(lineSeries_n_x);
            schedule.Series.Add(lineSeries_n_y);
            schedule.Series.Add(lineSeries_n_z);

            return new Data() { equalsElements = equalsElements, itemsSourceTable = result, scheduleModel = schedule};
        }
    }

    struct Dot
    {
        public double x { get; set; }
        public double y { get; set; }
        public static Dot CrossTwoLines(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            // https://habr.com/ru/articles/523440/
            double n;
            Dot resultDot = new Dot();
            if (y2 - y1 != 0)
            {  // a(y)
                double q = (x2 - x1) / (y1 - y2);
                double sn = (x3 - x4) + (y3 - y4) * q;
                if (sn == 0)
                {
                    resultDot.x = -1;
                    resultDot.y = -1;
                    return resultDot;
                }// c(x) + c(y)*q
                double fn = (x3 - x1) + (y3 - y1) * q;   // b(x) + b(y)*q
                n = fn / sn;
            }
            else
            {
                if (y3 - y4 == 0)
                {
                    resultDot.x = -1;  // b(y)
                    resultDot.y = -1;
                    return resultDot;
                }
                n = (y3 - y1) / (y3 - y4);   // c(y)/b(y)
            }
            resultDot.x = x3 + (x4 - x3) * n;  // x3 + (-b(x))*n
            resultDot.y = y3 + (y4 - y3) * n;  // y3 +(-b(y))*n
            return resultDot;
        }
    }

    public static class Export
    {
        private static string GetExtension(string fileName)
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

        public static void Export_Schedule_png(OxyPlot.Wpf.PlotView OxyPlotSchedule, Microsoft.Win32.SaveFileDialog dlg)
        {
            var pngExporter = new PngExporter { Width = 1000, Height = 800 };
            pngExporter.ExportToFile(OxyPlotSchedule.Model, dlg.FileName);
        }

        public static void Export_Schedule_pdf(OxyPlot.Wpf.PlotView OxyPlotSchedule, Microsoft.Win32.SaveFileDialog dlg)
        {
            var pdfExporter = new PdfExporter { Width = 1000, Height = 800 };

            PlotModel model = OxyPlotSchedule.Model;
            model.Axes[0].Title = "x, mm";
            model.Axes[1].Title = "Refractive index value n";

            pdfExporter.ExportToFile(model, dlg.FileName);
        }

        public static void Export_Table_pdf(System.Windows.Controls.DataGrid Table, Microsoft.Win32.SaveFileDialog dlg)
        {
            PdfWriter writer = new PdfWriter(dlg.FileName);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            iText.Layout.Element.Table table = new iText.Layout.Element.Table(Table.Columns.Count);
            
            foreach (System.Windows.Controls.DataGridColumn c in Table.Columns)
            {
                table.AddCell(new iText.Layout.Element.Paragraph(c.Header.ToString()));

            }

            List<List<double>> s = Table.Items.OfType<List<double>>().ToList().ToList();

            foreach (List<double> r in s)
            {
                for (int i = 0; i < r.Count; i++)
                {
                    table.AddCell(new iText.Layout.Element.Paragraph(r[i].ToString()));

                }
            }

            document.Add(table);
            document.Close();
        }
    }
}
