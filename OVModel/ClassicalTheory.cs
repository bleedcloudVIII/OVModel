using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OVModel_CommonClasses;

using OxyPlot;
using OxyPlot.Axes;

namespace OVModel_ClassicalTheory
{
    public static class ClassicalTheory
    {
        private static double n_x(double x, double n, double R, double b)
        {
            double mu_R_R = GlobalVariable.mu / (R * R);
            double mu_R = GlobalVariable.mu / R;
            double s = mu_R * x * (GlobalVariable.p11 + GlobalVariable.p12 - (GlobalVariable.p12 / GlobalVariable.mu));
            double t = mu_R_R * b * b * ((GlobalVariable.p11 / (2 * GlobalVariable.mu)) - GlobalVariable.p12);
            return Math.Round(n + ((n * n * n) / 2) * (s + t), 9);
        }
        private static double n_y(double x, double n, double R, double b)
        {
            double mu_R_R = GlobalVariable.mu / (R * R);
            double mu_R = GlobalVariable.mu / R;
            double s = mu_R * x * (2 * GlobalVariable.p11 + 2 * GlobalVariable.p12 - (2 * GlobalVariable.p12 / GlobalVariable.mu));
            double t = mu_R_R * b * b * ((GlobalVariable.p12 / GlobalVariable.mu) - GlobalVariable.p12 - GlobalVariable.p11);
            return Math.Round(n + ((n * n * n) / 4) * (s + t), 9);
        }
        private static double n_z(double x, double n, double R, double b)
        {
            double mu_R_R = GlobalVariable.mu / (R * R);
            double mu_R = GlobalVariable.mu / R;
            double s = mu_R * x * (4 * GlobalVariable.p12 - ((2 * GlobalVariable.p11) / GlobalVariable.mu));
            double t = mu_R_R * b * b * ((GlobalVariable.p12 / GlobalVariable.mu) - GlobalVariable.p12 - GlobalVariable.p11);
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

        public static Data Calculating(double b, double h, double n, double R, double x_start, 
            double x_end, string title, string titleAxisX, string titleAxisY)
        {
            // Список элементов x, n_x, n_y, n_z для таблицы
            List<List<double>> result = new List<List<double>>();

            // Количество x в таблице
            int count = 0;
            if (h != 0) count = (Int32)(Math.Abs(x_end - x_start) / h);

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

            OxyPlot.Series.LineSeries lineSeries_n = new OxyPlot.Series.LineSeries() { 
                Points = { new OxyPlot.DataPoint(x_start, n), new OxyPlot.DataPoint(x_end, n) }, 
                Title = "n" };

            OxyPlot.Series.LineSeries lineSeries_n_x = new OxyPlot.Series.LineSeries();
            lineSeries_n_x.Title = "n_x";

            OxyPlot.Series.LineSeries lineSeries_n_y = new OxyPlot.Series.LineSeries();
            lineSeries_n_y.Title = "n_y";

            OxyPlot.Series.LineSeries lineSeries_n_z = new OxyPlot.Series.LineSeries();
            lineSeries_n_z.Title = "n_z";

            

            for (int i = 0; i <= count; i++)
            {
                double x_now = x_start + h * i;
                double n_x = ClassicalTheory.n_x(x_now, n, R, b);
                double n_y = ClassicalTheory.n_y(x_now, n, R, b);
                double n_z = ClassicalTheory.n_z(x_now, n, R, b);

                result.Add(new List<double> { x_now, n_x, n_y, n_z });
            }

            double h_schedule = 0.00001;
            for (int i = 0; i < (Int32)(Math.Abs(x_end - x_start) / h_schedule); i++)
            {
                double x_now = x_start + h_schedule * i;
                double n_x = ClassicalTheory.n_x(x_now, n, R, b);
                double n_y = ClassicalTheory.n_y(x_now, n, R, b);
                double n_z = ClassicalTheory.n_z(x_now, n, R, b);

                lineSeries_n_x.Points.Add(new OxyPlot.DataPoint(x_now, n_x));
                lineSeries_n_y.Points.Add(new OxyPlot.DataPoint(x_now, n_y));
                lineSeries_n_z.Points.Add(new OxyPlot.DataPoint(x_now, n_z));
            }

            List<EqualElements> equalsElements = Dot.CrossPoints(result, n);
            equalsElements = CommonMethods.getSetList(equalsElements);

            schedule.Series.Add(lineSeries_n);
            schedule.Series.Add(lineSeries_n_x);
            schedule.Series.Add(lineSeries_n_y);
            schedule.Series.Add(lineSeries_n_z);

            return new Data() { equalsElements = equalsElements,
                itemsSourceTable = result,
                scheduleModel = schedule 
            };
        }
    }
}
