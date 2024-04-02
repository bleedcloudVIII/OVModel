﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OxyPlot;
using OxyPlot.Axes;
using OVModel_CommonClasses;


namespace OVModel_Methods
{
    public static class Approksimacia
    {
        public static List<List<double>> approksimacia_polinom_1(List<EqualElements> list)
        {
            List<EqualElements> points = new List<EqualElements>(list);

            for (int i = 0; i < points.Count; i++)
            {
                EqualElements eq = points[i];
                eq.n_value = points[i].n_value * 1000000;
                eq.x = points[i].x * 1000000;
                points[i] = eq;
            }

            List<long> x_second = new List<long>(points.Count);
            for (int i = 0; i < points.Count; i++) x_second.Add((Int64)(points[i].x * points[i].x));

            long sumx = points.Sum(elem => (Int64)elem.x);
            long sumx2 = x_second.Sum();

            long sumy = points.Sum(elem => (Int64)elem.n_value);

            long sumyx = 0;
            for (int i = 0; i < points.Count; i++) sumyx += (Int64)(points[i].n_value * points[i].x);

            List<List<long>> C_transport = new List<List<long>>()
            {
                new List<long> {sumx2,     (-1)*sumx},
                new List<long> {(-1)*sumx, points.Count}
            };

            long opredelitel = points.Count*sumx2 - sumx*sumx;
            List<List<double>> A_inverse = new List<List<double>>(3) { new List<double>(2) { 0, 0 }, new List<double>(2) { 0, 0 } };
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    A_inverse[i][j] = (double)C_transport[i][j] / (double)opredelitel;

                }
            }

            List<double> B = new List<double>()
            {
                (double)sumy, (double)sumyx
            };

            List<double> X = new List<double>(3)
            {
                (A_inverse[0][0]*B[0] + A_inverse[0][1]*B[1])/1000000,
                (A_inverse[1][0]*B[0] + A_inverse[1][1]*B[1])/1000000
            };

            double start = points.Min(elem => elem.x);
            double end = points.Max(elem => elem.x);

            start = start / 1000000;
            end = end / 1000000;

            int length = (Int32)((end - start) / 0.000001);
            List<List<double>> result = new List<List<double>>(2)
            {
                new List<double>(length),
                new List<double>(length)
            };

            for (int i = 0; i < length; i++)
            {
                double x = Math.Round(start + 0.000001 * i, 6);
                double y = X[0] + X[1] * x;
                result[0].Add(x);
                result[1].Add(y);
            }

            return result;
        }

        public static List<List<double>> approksimacia_polinom_2(List<EqualElements> list)
        {
            List<EqualElements> points = new List<EqualElements>(list);

            for (int i = 0; i < points.Count; i++)
            {
                EqualElements eq = points[i];
                eq.n_value = points[i].n_value * 1000000;
                eq.x = points[i].x * 1000000;
                points[i] = eq;
            }

            List<long> x_second = new List<long>(points.Count);
            for (int i = 0; i < points.Count; i++) x_second.Add((Int64)(points[i].x * points[i].x));

            List<long> x_third = new List<long>(points.Count);
            for (int i = 0; i < points.Count; i++) x_third.Add((Int64)(points[i].x * x_second[i]));

            List<long> x_fourth = new List<long>(points.Count);
            for (int i = 0; i < points.Count; i++) x_fourth.Add((Int64)(points[i].x * x_third[i]));

            long sumx = points.Sum(elem => (Int64)elem.x);
            long sumx2 = x_second.Sum();
            long sumx3 = x_third.Sum();
            long sumx4 = x_fourth.Sum();

            long sumy = points.Sum(elem => (Int64)elem.n_value);

            long sumyx = 0;
            for (int i = 0; i < points.Count; i++) sumyx += (Int64)(points[i].n_value * points[i].x);

            long sumyx2 = 0; ;
            for (int i = 0; i < points.Count; i++) sumyx2 += (Int64)(points[i].n_value * points[i].x * points[i].x);

            List<List<long>> C_transport = new List<List<long>>()
            {
                new List<long> {sumx2 * sumx4 - sumx3 * sumx3,          (-1) * (sumx * sumx4 - sumx2 * sumx3),         sumx * sumx3 - sumx2 * sumx2},
                new List<long> {(-1) * (sumx * sumx4 - sumx2 * sumx3),  points.Count * sumx4 - sumx2 * sumx2,           (-1) * (points.Count * sumx3 - sumx * sumx2)},
                new List<long> {sumx * sumx3 - sumx2 * sumx2,         (-1) * (points.Count * sumx3 - sumx * sumx2),    points.Count * sumx2 - sumx * sumx }
            };

            long opredelitel = points.Count*sumx2*sumx4 + sumx*sumx3*sumx2 + sumx*sumx3*sumx2 - sumx2*sumx2*sumx2 - sumx*sumx*sumx4 - sumx3*sumx3*points.Count;
            List<List<double>> A_inverse = new List<List<double>>(3) { new List<double>(3) { 0, 0, 0}, new List<double>(3) { 0, 0, 0}, new List<double>(3) { 0, 0, 0 } };
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    A_inverse[i][j] = (double)C_transport[i][j] / (double)opredelitel;

                }
            }

            List<double> B = new List<double>()
            {
                (double)sumy, (double)sumyx, (double)sumyx2
            };

            List<double> X = new List<double>(3) 
            {
                (A_inverse[0][0]*B[0] + A_inverse[0][1]*B[1] + A_inverse[0][2]*B[2])/1000000,
                (A_inverse[1][0]*B[0] + A_inverse[1][1]*B[1] + A_inverse[1][2]*B[2])/1000000,
                (A_inverse[2][0]*B[0]+ A_inverse[2][1]*B[1]+ A_inverse[2][2]*B[2])/1000000,
            };

            double start = points.Min(elem => elem.x);
            double end = points.Max(elem => elem.x);

            start = start / 1000000;
            end = end / 1000000;

            int length = (Int32)((end - start) / 0.000001);
            List<List<double>> result = new List<List<double>>(2)
            {
                new List<double>(length),
                new List<double>(length)
            };

            for (int i = 0; i < length; i++)
            {
                double x = Math.Round(start + 0.000001 * i, 6);
                double y = X[0] + X[1] * x + X[2] * x * x;
                result[0].Add(x);
                result[1].Add(y);
            }

            return result;
        }
    }

    public static class Interpolyacia
    {

    }
}