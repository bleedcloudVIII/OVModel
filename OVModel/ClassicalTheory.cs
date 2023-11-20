using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OVModel_CommonClasses;


namespace OVModel_ClassicalTheory
{
    internal class ClassicalTheory
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
                double mu_R = mu / R;
                double s = mu_R * x * (p11 + p12 - (p12 / mu));
                double t = mu_R_R * b * b * ((p11 / (2 * mu)) - p12);
                return Math.Round(n + ((n * n * n) / 2) * (s + t), 9);
            }

            private static double n_y(double x, double n, double R, double b)
            {
                double mu_R_R = mu / (R * R);
                double mu_R = mu / R;
                double s = mu_R * x * (2 * p11 + 2 * p12 - (2 * p12 / mu));
                double t = mu_R_R * b * b * ((p12 / mu) - p12 - p11);
                return Math.Round(n + ((n * n * n) / 4) * (s + t), 9);
            }

            private static double n_z(double x, double n, double R, double b)
            {
                double mu_R_R = mu / (R * R);
                double mu_R = mu / R;
                double s = mu_R * x * (4 * p12 - ((2 * p11) / mu));
                double t = mu_R_R * b * b * ((p12 / mu) - p12 - p11);
                return Math.Round(n + ((n * n * n) / 4) * (s + t), 9);
            }

            private static List<EqualElements> getSetList(List<EqualElements> list)
            {
                List<EqualElements> result = new List<EqualElements>();

                for (int i = 0; i < list.Count; i++)
                {
                    if (!result.Contains(list[i])) result.Add(list[i]);
                }
                return result;
            }

            public static Data Calculating(double b, double h, double n, double n_ob, double R, double x_start, double x_end, string title, string titleAxisX, string titleAxisY)
            { 
                return new Data();
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
                resultDot.y = Math.Round(y3 + (y4 - y3) * n, 9);  // y3 +(-b(y))*n
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
                PlotModel model = OxyPlotSchedule.Model;
                OxyPlot.SkiaSharp.PngExporter.Export(model, dlg.FileName, 1000, 800);
            }

            public static void Export_Schedule_pdf(OxyPlot.Wpf.PlotView OxyPlotSchedule, Microsoft.Win32.SaveFileDialog dlg)
            {
                PlotModel model = OxyPlotSchedule.Model;
                OxyPlot.SkiaSharp.PdfExporter.Export(model, dlg.FileName, 1000, 800);
            }

            public static void Export_Schedule_jpg(OxyPlot.Wpf.PlotView OxyPlotSchedule, Microsoft.Win32.SaveFileDialog dlg)
            {
                PlotModel model = OxyPlotSchedule.Model;
                OxyPlot.SkiaSharp.JpegExporter.Export(model, dlg.FileName, 1000, 800, 100);
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
}
