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

using System.IO;
using System.Drawing;

using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

using OxyPlot;
using OxyPlot.Wpf;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace OVModel
{
    //public class Schedule
    //{
    //    public Schedule()
    //    {
    //        this.shedule = new PlotModel { Title = " "};
    //        this.shedule.Legends.Add(new OxyPlot.Legends.Legend() { LegendPosition = OxyPlot.Legends.LegendPosition.LeftBottom });
    //        this.shedule.IsLegendVisible = true;
    //    }

    //    public PlotModel shedule { get; private set; }
    //}

    public partial class MainWindow : Window
    {
        public static string TitleAxisX = "x, мм";

        public static string TitleAxisY = "Значение показателя преломления n";

        public MainWindow()
        {
            InitializeComponent();

            CreateColumns();
        }

        /*
            Сделать:
            - для лэйбла перемотку, если очень много значений
            -
        */

        private void StartCalculation()
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

                // Список элементов x, n_x, n_y, n_z для таблицы
                List<List<double>> result = new List<List<double>>();

                // Количество x в таблице
                int count = 0;
                if (h != 0) count = System.Convert.ToInt32(Math.Abs(x_end - x_start) / h);
                //Console.WriteLine();

                Schedule schedule = new Schedule() { };

                schedule.shedule.Axes.Add(new LinearAxis() { Title = TitleAxisX, Position = AxisPosition.Bottom, IsPanEnabled = false, IsZoomEnabled = false });
                schedule.shedule.Axes.Add(new LinearAxis() { Title = TitleAxisY, Position = AxisPosition.Left, IsPanEnabled = false, IsZoomEnabled = false });

                OxyPlot.Series.LineSeries lineSeries_n = new OxyPlot.Series.LineSeries() { Points = { new OxyPlot.DataPoint(x_start, n), new OxyPlot.DataPoint(x_end, n) }, Title = "n" };

                OxyPlot.Series.LineSeries lineSeries_n_x = new OxyPlot.Series.LineSeries();
                lineSeries_n_x.Title = "n_x";

                OxyPlot.Series.LineSeries lineSeries_n_y = new OxyPlot.Series.LineSeries();
                lineSeries_n_y.Title = "n_y";

                OxyPlot.Series.LineSeries lineSeries_n_z = new OxyPlot.Series.LineSeries();
                lineSeries_n_z.Title = "n_z";

                List<equals> equalsElements = new List<equals>();

                double n_x_prev = 0, n_y_prev = 0, n_z_prev = 0, x_prev = 0;

                for (int i = 0; i <= count; i++)
                {
                    // Вычисление значений текущего x и значений n
                    double x_now = x_start + h * i;
                    double n_x = OVModel_DopTheory.DopTheory.n_x(x_now, n, R, b);
                    double n_y = OVModel_DopTheory.DopTheory.n_y(x_now, n, R, b);
                    double n_z = OVModel_DopTheory.DopTheory.n_z(x_now, n, R, b);

                    // Если значения n равны, то помещаем в список элементов точки, пересечения
                    if (n_x == n_y) equalsElements.Add(new equals() { x = x_now, first = "n_x", second = "n_y", n_value = n_x });
                    else if (n_x == n_z) equalsElements.Add(new equals() { x = x_now, first = "n_x", second = "n_z", n_value = n_x });
                    else if (n_y == n_z) equalsElements.Add(new equals() { x = x_now, first = "n_y", second = "n_z", n_value = n_y });
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
                        dot dot = CrossTwoLines(x_prev, n_x_prev, x_now, n_x, x_prev, n_y_prev, x_now, n_y);
                        equalsElements.Add(new equals() { x = dot.x, first = "n_y", second = "n_x", n_value = dot.y });
                    }
                    else if ((n_y_prev >= n_z) && (n_z >= n_y))
                    {
                        dot dot = CrossTwoLines(x_prev, n_z_prev, x_now, n_z, x_prev, n_y_prev, x_now, n_y);
                        equalsElements.Add(new equals() { x = dot.x, first = "n_y", second = "n_z", n_value = dot.y });
                    }
                    else if ((n_x_prev >= n_z) && (n_z >= n_x))
                    {
                        dot dot = CrossTwoLines(x_prev, n_z_prev, x_now, n_z, x_prev, n_x_prev, x_now, n_x);
                        equalsElements.Add(new equals() { x = dot.x, first = "n_x", second = "n_z", n_value = dot.y });
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

                schedule.shedule.Series.Add(lineSeries_n);
                schedule.shedule.Series.Add(lineSeries_n_x);
                schedule.shedule.Series.Add(lineSeries_n_y);
                schedule.shedule.Series.Add(lineSeries_n_z);

                OxyPlotSchedule.Model = schedule.shedule;
                Table.ItemsSource = result;

                PointsLabel.Content = $"Пересечения:\n";

                for (int i = 0; i < equalsElements.Count; i++)
                {
                    //schedule.shedule..Add(new LineSeries() { Points = { new OxyPlot.DataPoint(equalsElements[i].x, equalsElements[i].n_value)}, Title = "POINT" });
                    PointsLabel.Content += $"{equalsElements[i].first} и {equalsElements[i].second}:\n x = {equalsElements[i].x}\n n = {equalsElements[i].n_value}\n";
                }
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

        //private struct equals
        //{
        //    public double x { get; set; }
        //    public double n_value { get; set; }
        //    public string first { get; set; }
        //    public string second { get; set; }

        //    public static bool operator ==(equals f, equals s)
        //    {
        //        return (f.x == s.x && f.n_value == s.n_value && f.first == s.first && f.second == s.second);
        //    }

        //    public static bool operator !=(equals f, equals s)
        //    {
        //        return !(f == s);
        //    }

        //}

        //private struct dot
        //{
        //    public double x { get; set; }
        //    public double y { get; set; }
        //}

        //dot CrossTwoLines(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        //{
        //    // https://habr.com/ru/articles/523440/
        //    double n;
        //    dot resultDot = new dot();
        //    if (y2 - y1 != 0)
        //    {  // a(y)
        //        double q = (x2 - x1) / (y1 - y2);
        //        double sn = (x3 - x4) + (y3 - y4) * q;
        //        if (sn == 0)
        //        {
        //            resultDot.x = -1;
        //            resultDot.y = -1;
        //            return resultDot;
        //        }// c(x) + c(y)*q
        //        double fn = (x3 - x1) + (y3 - y1) * q;   // b(x) + b(y)*q
        //        n = fn / sn;
        //    }
        //    else
        //    {
        //        if (y3 - y4 == 0)
        //        {
        //            resultDot.x = -1;  // b(y)
        //            resultDot.y = -1;
        //            return resultDot;
        //        }
        //        n = (y3 - y1) / (y3 - y4);   // c(y)/b(y)
        //    }
        //    resultDot.x = x3 + (x4 - x3) * n;  // x3 + (-b(x))*n
        //    resultDot.y = y3 + (y4 - y3) * n;  // y3 +(-b(y))*n
        //    return resultDot;
        //}

        //private List<equals> getSetList(List<equals> list)
        //{
        //    List<equals> result = new List<equals>();

        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        if (!result.Contains(list[i])) result.Add(list[i]);
        //    }
        //    return result;
        //}

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartCalculation();
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
                    //var pdfExporter = new PdfExporter { Width = 1000, Height = 800 };

                    //PlotModel model = OxyPlotSchedule.Model;
                    //model.Axes[0].Title = "x, mm";
                    //model.Axes[1].Title = "Refractive index value n";

                    //pdfExporter.ExportToFile(model, dlg.FileName);
                }
                else if(extension == "jpg" && OxyPlotSchedule.Model != null)
                {
                    //var pngExporter = new PngExporter { Width = 1000, Height = 800 };
                    //pngExporter.ExportToFile(OxyPlotSchedule.Model, dlg.FileName);
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
                //PdfWriter writer = new PdfWriter(dlg.FileName);
                //PdfDocument pdf = new PdfDocument(writer);
                //Document document = new Document(pdf);
                
                //iText.Layout.Element.Table table = new iText.Layout.Element.Table(Table.Columns.Count);

                //foreach (DataGridColumn c in Table.Columns)
                //{
                //    table.AddCell(new iText.Layout.Element.Paragraph(c.Header.ToString()));

                //}

                //List<List<double>> s = Table.Items.OfType<List<double>>().ToList().ToList();

                //foreach(List<double> r in s)
                //{
                //    for(int i = 0; i < r.Count; i++)
                //    {
                //        table.AddCell(new iText.Layout.Element.Paragraph(r[i].ToString()));

                //    }
                //}

                //document.Add(table);
                //document.Close();

            }
        }
    }
}
