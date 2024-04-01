using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OxyPlot;
using OxyPlot.Axes;

namespace OVModel_Methods
{
    public static class Approksimacia
    {
        public static List<List<double>> approksimacia_polinom_1(List<List<double>> points)
        {
            for (int i = 0; i < points.Count; i++)
                for (int j = 0; j < points[i].Count; j++)
                    points[i][j] = points[i][j] * 1000000;

            List<long> x_second = new List<long>(points[0].Count) { };
            for (int i = 0; i < x_second.Count; i++) x_second[i] = System.Convert.ToInt64(points[0][i] * points[0][i]);

            List<long> x_third = new List<long>(points[0].Count) { };
            for (int i = 0; i < x_third.Count; i++) x_third[i] = System.Convert.ToInt64(points[0][i] * x_second[i]);

            List<long> x_fourth = new List<long>(points[0].Count) { };
            for (int i = 0; i < x_fourth.Count; i++) x_fourth[i] = System.Convert.ToInt64(points[0][i] * x_third[i]);

            long sumx = points[0].Sum(x => System.Convert.ToInt64(x));
            long sumx2 = x_second.Sum();

            long sumy = points[1].Sum(x => System.Convert.ToInt64(x));

            long sumyx = 0;
            for (int i = 0; i < points.Count; i++) sumyx += System.Convert.ToInt64(points[0][i] * points[1][i]);

            /*
            List<List<long>> A = new List<List<long>>()
            {
                    new List<long> { points[0].Count, sumx,  sumx2 },
                    new List<long> { sumx,            sumx2, sumx3 },
                    new List<long> { sumx2,           sumx3, sumx4 }
            };
            */
            List<List<long>> A_transport = new List<List<long>>()
            {
                new List<long> {points[0].Count, sumx},
                new List<long> {sumx,            sumx2}
            };
            long opredelitel = points[0].Count * sumx2 - sumx * sumx;

            List<List<double>> A_inverse = new List<List<double>>(2) { new List<double>(2), new List<double>(2)};
            for (int i = 0; i < A_inverse.Count; i++)
                for (int j = 0; j < A_inverse[i].Count; j++)
                    A_inverse[i][j] = (double)A_transport[i][j] / (double)opredelitel;

            List<double> B = new List<double>()
            {
                sumy, sumyx
            };

            List<double> X = new List<double>(2);

            for (int i = 0; i < X.Count; i++)
            {
                double sum = 0;
                for (int j = 0; j < X.Count; j++)
                    sum += A_inverse[i][j] * B[j];
                X[i] = sum;
            }

            double start = points[0].Min();
            double end = points[0].Max();

            List<List<double>> result = new List<List<double>>();

            int n = 0;
            for (double i = start; start < end; i += 0.000001)
            {
                result[0][n] = i;
                result[1][n] = X[0] + X[1] * i;
                n++;
            }

            return result;
        }

        public static List<List<double>> approksimacia_polinom_2(List<List<double>> points)
        {
            for (int i = 0; i < points.Count; i++)
                for (int j = 0; j < points[i].Count; j++)
                    points[i][j] = points[i][j] * 1000000;

            List<long> x_second = new List<long>(points[0].Count) {};
            for (int i = 0; i < x_second.Count; i++) x_second[i] = System.Convert.ToInt64(points[0][i] * points[0][i]);

            List<long> x_third = new List<long>(points[0].Count) { };
            for (int i = 0; i < x_third.Count; i++) x_third[i] = System.Convert.ToInt64(points[0][i] * x_second[i]);

            List<long> x_fourth = new List<long>(points[0].Count) { };
            for (int i = 0; i < x_fourth.Count; i++) x_fourth[i] = System.Convert.ToInt64(points[0][i] * x_third[i]);

            long sumx = points[0].Sum(x => System.Convert.ToInt64(x));
            long sumx2 = x_second.Sum();
            long sumx3 = x_third.Sum();
            long sumx4 = x_fourth.Sum();

            long sumy = points[1].Sum(x => System.Convert.ToInt64(x));

            long sumyx = 0;
            for (int i = 0; i < points.Count; i++) sumyx += System.Convert.ToInt64(points[0][i] * points[1][i]);

            long sumyx2 = 0; ;
            for (int i = 0; i < points.Count; i++) sumyx += System.Convert.ToInt64(points[0][i] * points[0][i] * points[1][i]);

            /*
            List<List<long>> A = new List<List<long>>()
            {
                    new List<long> { points[0].Count, sumx,  sumx2 },
                    new List<long> { sumx,            sumx2, sumx3 },
                    new List<long> { sumx2,           sumx3, sumx4 }
            };
            */
            List<List<long>> A_transport = new List<List<long>>()
            {
                new List<long> {points[0].Count, sumx, sumx2},
                new List<long> {sumx, sumx2, sumx3},
                new List<long> {sumx2, sumx3, sumx4}
            };
            long opredelitel = points[0].Count*sumx2*sumx4 + sumx*sumx3*sumx2 + sumx*sumx3*sumx2 - sumx2*sumx2*sumx2 - sumx*sumx3*sumx2 - sumx*sumx3*sumx2;

            List<List<double>> A_inverse = new List<List<double>>(3) { new List<double>(3), new List<double>(3), new List<double>(3)};
            for (int i = 0; i < A_inverse.Count; i++)
                for (int j = 0; j < A_inverse[i].Count; j++)
                    A_inverse[i][j] = (double) A_transport[i][j] / (double) opredelitel;

            List<double> B = new List<double>()
            {
                sumy, sumyx, sumyx2
            };

            List<double> X = new List<double>(3);

            for (int i = 0; i < X.Count; i++)
            {
                double sum = 0;
                for (int j = 0; j < X.Count; j++)
                    sum += A_inverse[i][j] * B[j];
                X[i] = sum;
            }

            double start = points[0].Min();
            double end = points[0].Max();

            List<List<double>> result = new List<List<double>>();

            int n = 0;
            for (double i = start; start < end; i += 0.000001)
            {
                result[0][n] = i;
                result[1][n] = X[0] + X[1] * i + X[2] * i * i;
                n++;
            }

            return result;
        }
    }

    public static class Interpolyacia
    {

    }
}
