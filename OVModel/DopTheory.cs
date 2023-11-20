using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OxyPlot;
using OxyPlot.Wpf;
using OxyPlot.Axes;


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
            return Math.Round(n + ((n * n * n) / 2) * (f + s + t), 9);
        }

        private static double n_y(double x, double n, double R, double b)
        {
            double mu_R_R = mu / (R * R);
            double f = mu_R_R * x * x * (p11 - (p12 / mu) + p12);
            double mu_R = mu / R;
            double s = mu_R * x * (2 * p11 + 2 * p12 - (2 * p12 / mu));
            double t = mu_R_R * b * b * ((p12 / mu) - p12 - p11);
            return Math.Round(n + ((n * n * n) / 4) * (f + s + t), 9);
        }

        private static double n_z(double x, double n, double R, double b)
        {
            double mu_R_R = mu / (R * R);
            double f = mu_R_R * x * x * (p11 - (p12 / mu) + p12);
            double mu_R = mu / R;
            double s = mu_R * x * (4 * p12 - ((2*p11)/mu));
            double t = mu_R_R * b * b * ((p12 / mu) - p12 - p11);
            return Math.Round(n + ((n * n * n) / 4) * (f + s + t), 9);
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
            // Список элементов x, n_x, n_y, n_z для таблицы
            List<List<double>> result = new List<List<double>>();

            // Количество x в таблице
            int count = 0;
            if (h != 0) count = System.Convert.ToInt32(Math.Abs(x_end - x_start) / h);

            PlotModel schedule = new PlotModel() {
                Title = title,
                Legends = { new OxyPlot.Legends.Legend() { LegendPosition = OxyPlot.Legends.LegendPosition.LeftBottom } },
                IsLegendVisible = true,
                Axes =
                {
                    new LinearAxis() {Title = titleAxisX, Position = AxisPosition.Bottom, IsPanEnabled = false, IsZoomEnabled = false },
                    new LinearAxis() {Title = titleAxisY, Position = AxisPosition.Left, IsPanEnabled = false, IsZoomEnabled = false },
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

            for (int i = 0; i < equalsElements.Count; i++)
            {
                EqualElements e = equalsElements[i];
                e.x = Math.Round(e.x, 6);
                e.cross = $"{e.first}/{e.second}";
                equalsElements[i] = e;
            }

            schedule.Series.Add(lineSeries_n);
            schedule.Series.Add(lineSeries_n_x);
            schedule.Series.Add(lineSeries_n_y);
            schedule.Series.Add(lineSeries_n_z);

            return new Data() { equalsElements = equalsElements, itemsSourceTable = result, scheduleModel = schedule};
        }
    }
}
