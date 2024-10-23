using iText.Kernel.Pdf;
using OxyPlot;
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

namespace OVModel.Lib.Export
{
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

        //private static void releaseObject(object obj)
        //{
        //    try
        //    {
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
        //        obj = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        obj = null;
        //        //MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
        //    }
        //    finally
        //    {
        //        GC.Collect();
        //    }
        //}

        // https://stackoverflow.com/questions/56351038/how-to-export-a-datagrid-to-excel-in-wpf
        // https://stackoverflow.com/questions/23928146/populate-excel-sheet-with-c-sharp
        public static void Export_Table_xlsx(System.Windows.Controls.DataGrid Table, Microsoft.Win32.SaveFileDialog dlg)
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook = xlApp.Workbooks.Add();
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            int index = 0;
            foreach (System.Windows.Controls.DataGridColumn c in Table.Columns)
            {
                xlWorkSheet.Range["A1"].Offset[0, index].Value = c.Header;
                index++;
            }

            List<List<double>> s = Table.Items.OfType<List<double>>().ToList().ToList();

            index = 0;
            foreach (List<double> r in s)
            {
                xlWorkSheet.Range["A2"].Offset[index].Resize[1, r.Count].Value = r.ToArray();
                index++;
            }

            xlWorkBook.SaveAs(Filename: dlg.FileName, FileFormat: Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
            xlWorkBook.Close(SaveChanges: true);
            xlApp.Quit();
        }

    }
}
