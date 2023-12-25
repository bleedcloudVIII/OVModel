using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OxyPlot;
using OxyPlot.Wpf;
using OxyPlot.Axes;

using iText.Kernel.Pdf;
using iText.Layout;

namespace OVModel_CommonClasses
{
    public struct EqualElements
    {
        public double x { get; set; }
        public double n_value { get; set; }
        public string first { get; set; }
        public string second { get; set; }
        public string cross { get; set; }

        public static bool operator ==(EqualElements f, EqualElements s)
        {
            return (f.x == s.x && f.n_value == s.n_value && f.first == s.first && f.second == s.second);
        }

        public static bool operator !=(EqualElements f, EqualElements s)
        {
            return !(f == s);
        }
    }

    public struct Data
    {
        public List<List<double>> itemsSourceTable { get; set; }
        public OxyPlot.PlotModel scheduleModel { get; set; }
        public List<EqualElements> equalsElements { get; set; }
    }

    public static class Export
    {
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

        public static void Export_Table_xlsx(System.Windows.Controls.DataGrid Table, Microsoft.Win32.SaveFileDialog dlg)
        {
            // https://stackoverflow.com/questions/56351038/how-to-export-a-datagrid-to-excel-in-wpf
            //Microsoft.Office.Interop.Excel.Application excel = null;
            //Microsoft.Office.Interop.Excel.Workbook wb = null;
            //object missing = Type.Missing;
            //Microsoft.Office.Interop.Excel.Range rng = null;

            // collection of DataGrid Items
            //var dtExcelDataTable = Microsoft.Office.Interop.Excel.Excel ExcelTimeReport(txtFrmDte.Text, txtToDte.Text, strCondition);

            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook wb = excel.Workbooks.Add();
            Microsoft.Office.Interop.Excel.Worksheet ws = null;
            
            ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.ActiveSheet;
            //ws.Columns.AutoFit();
            ws.Columns.EntireColumn.ColumnWidth = 25;

            int index = 0;
            foreach (System.Windows.Controls.DataGridColumn c in Table.Columns)
            {
                //table.AddCell(new iText.Layout.Element.Paragraph(c.Header.ToString()));
                ws.Range["A1"].Offset[0, index].Value = c.Header;
                index++;
            }

            List<List<double>> s = Table.Items.OfType<List<double>>().ToList().ToList();

            index = 0;
            foreach (List<double> r in s)
            {
                //for (int i = 0; i < r.Count; i++)
                //{
                    //table.AddCell(new iText.Layout.Element.Paragraph(r[i].ToString()));
                ws.Range["A2"].Offset[index].Resize[1, s.Count].Value = r.ToArray();
                index++;
                //}
            }

            //// Header row
            //for (int i = 0; i < dtExcelDataTable.Columns.Count; i++)
            //{
            //    ws.Range["A1"].Offset[0, Idx].Value = dtExcelDataTable.Columns[Idx].ColumnName;
            //}

            //// Data Rows
            //for (int Idx = 0; Idx < dtExcelDataTable.Rows.Count; Idx++)
            //{
            //    ws.Range["A2"].Offset[Idx].Resize[1, dtExcelDataTable.Columns.Count].Value = dtExcelDataTable.Rows[Idx].ItemArray;
            //}
            
            excel.DisplayAlerts = false;
            wb.SaveAs(Filename: dlg.SafeFileName, FileFormat: Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Local: dlg.FileName);
            wb.Close();
            //excel.Visible = true;
            //excel.GetSaveAsFilename();
            //wb.Activate();
            
        }
    }

    struct Dot
    {
        public double x { get; set; }
        public double y { get; set; }
        public static Dot CrossTwoLines(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
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

    public static class CommonMethods
    {
        public static List<EqualElements> getSetList(List<EqualElements> list)
        {
            List<EqualElements> result = new List<EqualElements>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!result.Contains(list[i])) result.Add(list[i]);
            }
            return result;
        }
    }

    public static class GlobalVariable
    {
        // Global Variable
        public const double p11 = 0.121;
        public const double p12 = 0.27;
        public const double mu = 0.164;
    }
}
