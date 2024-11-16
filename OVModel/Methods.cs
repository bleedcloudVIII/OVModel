using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OxyPlot;
using OxyPlot.Axes;
using OVModel.Lib.CommonClasses;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Interpolation;

using OVModel.Lib.EqualElements;

namespace OVModel_Methods
{
    public static class Approksimacia
    {
        public static List<List<double>> approksimacia_polinom_1(List<EqualElements> list)
        {
            List<EqualElements> points = new List<EqualElements>(list);

            if (points.Count == 0) return new List<List<double>>();

            if (isEquals(points))
            {
                List<List<double>> r = new List<List<double>>(2)
                {
                    new List<double>(points.Count),
                    new List<double>(points.Count)
                };

                for (int i = 0; i < points.Count; i++)
                {
                    r[0].Add(points[i].x);
                    r[1].Add(points[i].n_value);
                }

                return r;
            }

            List<decimal> x_second = new List<decimal>(points.Count);
            for (int i = 0; i < points.Count; i++) x_second.Add((decimal)points[i].x * (decimal)points[i].x);

            decimal sumx = points.Sum(elem => (decimal)elem.x);
            decimal sumx2 = x_second.Sum();

            decimal sumy = points.Sum(elem => (decimal)elem.n_value);

            decimal sumyx = 0;
            for (int i = 0; i < points.Count; i++) sumyx += ((decimal)points[i].n_value * (decimal)points[i].x);

            decimal sumyx2 = 0; ;
            for (int i = 0; i < points.Count; i++) sumyx2 += ((decimal)points[i].n_value * (decimal)points[i].x * (decimal)points[i].x);

      

            decimal n = points.Count;
            decimal x1 = (sumyx - sumx*sumx/n) / (sumx2 - sumx*sumx/n);
            decimal x0 = (sumy*sumx/n - (sumx*sumx/n)*x1) / sumx;


            double start = points.Min(elem => elem.x);
            double end = points.Max(elem => elem.x);

            int length = (Int32)((end - start) / 0.000001);

            List<List<double>> result = new List<List<double>>(2)
            {
                new List<double>(length),
                new List<double>(length)
            };

            for (int i = 0; i < length; i++)
            {
                double x = Math.Round(start + 0.000001 * i, 6);
                double y = (double)x0 + (double)x1 * x;
                result[0].Add(x);
                result[1].Add(y);
            }

            return result;
        }

        private static bool isEquals(List<EqualElements> points)
        {
            List<double> x = new List<double>();
            for (int i = 0; i < points.Count; i++) if (!x.Contains(points[i].x)) x.Add(points[i].x);
            return x.Count == 1 ? true : false;
        }

        public static List<List<double>> approksimacia_line(List<EqualElements> list)
        {
            List<EqualElements> points = new List<EqualElements>(list);

            //if (points.Count == 0) return new List<List<double>>();

            if (isEquals(points))
            {
                List<List<double>> r = new List<List<double>>(2)
                {
                    new List<double>(points.Count),
                    new List<double>(points.Count)
                };

                for (int i = 0; i < points.Count; i++)
                {
                    r[0].Add(points[i].x);
                    r[1].Add(points[i].n_value);
                }

                return r;
            }

            List<decimal> x_second = new List<decimal>(points.Count);
            for (int i = 0; i < points.Count; i++) x_second.Add((decimal)points[i].x * (decimal)points[i].x);

            decimal sumx = points.Sum(elem => (decimal)elem.x);
            decimal sumx2 = x_second.Sum();

            decimal sumy = points.Sum(elem => (decimal)elem.n_value);

            decimal sumyx = 0;
            for (int i = 0; i < points.Count; i++) sumyx += ((decimal)points[i].n_value * (decimal)points[i].x);

            decimal n = points.Count;

            decimal a = (n * sumyx - sumx * sumy) / (n * sumx2 - sumx * sumx);
            decimal b = (sumy - a * sumx) / n;

            double start = points.Min(elem => elem.x);
            double end = points.Max(elem => elem.x);

            int length = (Int32)((end - start) / 0.000001);

            List<List<double>> result = new List<List<double>>(2)
            {
                new List<double>(length),
                new List<double>(length)
            };

            for (int i = 0; i < length; i++)
            {
                double x = Math.Round(start + 0.000001 * i, 6);
                double y = (double)a*x + (double)b;
                result[0].Add(x);
                result[1].Add(y);
            }

            return result;
        }
        public static List<List<double>> approksimacia_polinom_2(List<EqualElements> list)
        {
            List<EqualElements> points = new List<EqualElements>(list);

            //if (points.Count == 0) return new List<List<double>>();

            if (isEquals(points))
            {
                List<List<double>> r = new List<List<double>>(2)
                {
                    new List<double>(points.Count),
                    new List<double>(points.Count)
                };

                for (int i = 0; i < points.Count; i++)
                {
                    r[0].Add(points[i].x);
                    r[1].Add(points[i].n_value);
                }

                return r;
            }

            List<decimal> x_second = new List<decimal>(points.Count);
            for (int i = 0; i < points.Count; i++) x_second.Add((decimal)points[i].x * (decimal)points[i].x);

            List<decimal> x_third = new List<decimal>(points.Count);
            for (int i = 0; i < points.Count; i++) x_third.Add((decimal)points[i].x * x_second[i]);

            List<decimal> x_fourth = new List<decimal>(points.Count);
            for (int i = 0; i < points.Count; i++) x_fourth.Add((decimal)points[i].x * x_third[i]);

            decimal sumx = points.Sum(elem => (decimal)elem.x);
            decimal sumx2 = x_second.Sum();
            decimal sumx3 = x_third.Sum();
            decimal sumx4 = x_fourth.Sum();

            decimal sumy = points.Sum(elem => (decimal)elem.n_value);

            decimal sumyx = 0;
            for (int i = 0; i < points.Count; i++) sumyx += ((decimal)points[i].n_value * (decimal)points[i].x);

            decimal sumyx2 = 0; ;
            for (int i = 0; i < points.Count; i++) sumyx2 += ((decimal)points[i].n_value * (decimal)points[i].x * (decimal)points[i].x);

            decimal n = points.Count;
            decimal x2 = (sumyx - (sumy*sumx/n) - ((sumyx2 - sumy*sumx2/n)*(sumx2 - sumx*sumx/n)/
                (sumx3-sumx*sumx2/n))) / (sumx3 - (sumx2*sumx2/n) - ((sumx4-sumx2*sumx2/n)*(sumx2-sumx*sumx/n)/(sumx3-sumx*sumx2/n)));
            decimal x1 = (((sumyx2 - sumy*sumx2/n)*(sumx2-sumx*sumx/n)/(sumx3-sumx*sumx2/n)) 
                - ((sumx4-sumx2*sumx2/n)*(sumx2-sumx*sumx/n)/(sumx3-sumx*sumx2/n))*x2) / (sumx2 - (sumx*sumx/n));
            decimal x0 = (sumy/n) - (sumx2/n)*x2 - (sumx/n)*x1;


            double start = points.Min(elem => elem.x);
            double end = points.Max(elem => elem.x);

            int length = (Int32)((end - start) / 0.000001);

            List<List<double>> result = new List<List<double>>(2)
            {
                new List<double>(length),
                new List<double>(length)
            };

            for (int i = 0; i < length; i++)
            {
                double x = Math.Round(start + 0.000001 * i, 6);
                double y = (double)x0 + (double)x1 * x + (double)x2 * x * x;
                result[0].Add(x);
                result[1].Add(y);
            }

            return result;
        }
    }

    public static class Interpolyacia
    {

        private static bool isEquals(List<EqualElements> points)
        {
            List<double> x = new List<double>();
            for (int i = 0; i < points.Count; i++) if (!x.Contains(points[i].x)) x.Add(points[i].x);
            return x.Count == 1 ? true : false;
        }
        public static List<List<double>> interpolyacia_lagrange(List<EqualElements> list)
        {
            List<List<double>> points = new List<List<double>>();
            points.Add(new List<double>());
            points.Add(new List<double>());

            if (isEquals(list))
            {
                List<List<double>> r = new List<List<double>>(2)
                {
                    new List<double>(list.Count),
                    new List<double>(list.Count)
                };

                for (int i = 0; i < list.Count; i++)
                {
                    r[0].Add(list[i].x);
                    r[1].Add(list[i].n_value);
                }

                return r;
            }

            for (int i = 0; i < list.Count; i++)
            {
                points[0].Add((double)(list[i].x));
                points[1].Add((double)(list[i].n_value));
            }

            double start = points[0].Min();
            double end = points[0].Max();

            int length = (Int32)((end - start) / 0.000001);
            List<List<double>> result = new List<List<double>>(2)
            {
                new List<double>(length),
                new List<double>(length)
            };


            for (int k = 0; k < length; k++)
            {
                double x = Math.Round(start + 0.000001 * k, 6);
                double y = 0;

                for (int i = 0; i < points[0].Count; i++)
                {
                    double alpha = 1;
                    double chisl = 1;
                    double znam = 1;
                    for (int j = 0; j < points[0].Count; j++)
                    {
                        if (j != i)
                        {
                            chisl *= (x - points[0][j]);
                            znam *= (points[0][i] - points[0][j]);
                        }
                    }
                    alpha = chisl / znam;
                    y += alpha * points[1][i];
                }

                result[0].Add(x);
                result[1].Add(y);
            }

            return result;
        }
    }
}
