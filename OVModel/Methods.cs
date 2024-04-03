using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OxyPlot;
using OxyPlot.Axes;
using OVModel_CommonClasses;


namespace OVModel_Methods
{
    
    public static class SLAY
    {
        private static int global_n;
        private static List<List<double>> global_C1;
        private static List<List<double>> global_C2;
        private static List<double> global_d;


        public static List<double> multiplyMatrixOnVector(List<List<double>> list, List<double> vector)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < list.Count; i++)
            {
                double sum = 0;
                for (int j = 0; j < list.Count; j++) sum += list[i][j] * vector[j];
                result.Add(sum);
            }

            return result;
        }

        public static double normaForVector(List<double> list)
        {
            return list.Max();
        }

        public static List<double> vectorMinusVector(List<double> list1, List<double> list2)
        {
            List<double> result = new List<double>();

            for (int i = 0; i < global_n; i++)
            {
                double sum = 0;
                sum = list1[i] - list2[i];
                result.Add(sum);
            }

            return result;
        }

        private static List<double> x_k_plus_1(List<double> x_k)
        {
            List<double> result = new List<double>(global_n);
            for (int i = 0; i < global_n; i++) result.Add(0);

            List<double> C1_x_k = new List<double>();
            C1_x_k = multiplyMatrixOnVector(global_C1, x_k);
            for (int i = 0; i < global_n; i++)
            {
                result[i] = C1_x_k[i] + multiplyMatrixOnVector(global_C2, result)[i] + global_d[i];
            }
            return result;
        }

        public static List<double> Zeidelya(List<List<long>> matrix, List<long> B)
        {
            int n = matrix.Count;
            // TODO: Проверка на преобладание главной диагонали

            List<List<double>> C = new List<List<double>>();
            List<List<double>> C1 = new List<List<double>>();
            List<List<double>> C2 = new List<List<double>>();

            List<double> D = new List<double>();

            for (int i = 0; i < n; i++)
            {
                List<double> list = new List<double>();
                for (int j = 0; j < n; j++)
                {
                    if (i != j) list.Add((-1) * ((double)matrix[i][j] / (double)matrix[i][i]));
                    else list.Add((double)0);
                }
                C.Add(list);
                D.Add((double)B[i] / (double)matrix[i][i]);
            }

            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < n; j++)
                {
                    if (i != j) sum += C[i][j];
                }

                if (sum > C[i][i]) return new List<double>();
            }

            for (int i = 0; i < n; i++)
            {
                List<double> tmp1 = new List<double>();
                List<double> tmp2 = new List<double>();

                for (int j = 0; j < n; j++)
                {
                    if (j < i) tmp1.Add(C[i][j]);
                    else tmp1.Add(0);

                    if (j > i) tmp2.Add(C[i][j]);
                    else tmp2.Add(0);
                }
                C1.Add(tmp1);
                C2.Add(tmp2);
            }

            global_C1 = C1;
            global_C2 = C2;
            global_n = n;
            global_d = D;

            // Метод Зейделя
            List<double> x_0 = new List<double>(n);
            for (int i = 0; i < n; i++) x_0.Add(0);
            double E = 0.000001;
            
            List<double> x_1 = x_k_plus_1(x_0);
            while(normaForVector(vectorMinusVector(x_1, x_0)) >= E)
            {
                x_0 = x_1;
                x_1 = x_k_plus_1(x_0);
            }

            return x_1; 
        }
    }
    public static class Approksimacia
    {
        public static List<List<double>> approksimacia_polinom_1(List<EqualElements> list)
        {
            List<EqualElements> points = new List<EqualElements>(list);

            if (points.Count == 0) return new List<List<double>>();

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

            List<List<long>> A = new List<List<long>>()
            {
                new List<long>() {points.Count, sumx},
                new List<long>() {sumx, sumx2}
            };

            List<long> B = new List<long>() { sumy, sumyx };

            List<double> X = SLAY.Zeidelya(A, B);
            for (int i = 0; i < X.Count; i++) X[i] = X[i] / 1000000;

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

            if (points.Count == 0) return new List<List<double>>();

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
          
            List<List<long>> A = new List<List<long>>()
            {
                new List<long>() {points.Count, sumx, sumx2},
                new List<long>() {sumx, sumx2, sumx3},
                new List<long>() {sumx2, sumx3, sumx4}
            };

            List<long> B = new List<long>() { sumy, sumyx, sumyx2 };

            List<double> X = SLAY.Zeidelya(A, B);
            for (int i = 0; i < X.Count; i++) X[i] = X[i] / 1000000;

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
        public static List<List<double>> interpolyacia_newtoon(List<EqualElements> list)
        {
            List<List<double>> points = new List<List<double>>();
            points.Add(new List<double>());
            points.Add(new List<double>());

            for (int i = 0; i < list.Count; i++)
            {
                //points[0].Add((long)(list[i].x * 1000000));
                //points[1].Add((long)(list[i].n_value * 1000000));
                points[0].Add((double)(list[i].x));
                points[1].Add((double)(list[i].n_value));
            }

            if (points.Count == 0) return new List<List<double>>();

            List<List<double>> koeff = new List<List<double>>();
            for (int i = 0; i < points[0].Count; i++)
            {
                List<double> tmp = new List<double>();
                for (int j = 0; j < points[0].Count; j++)
                    tmp.Add(0);
                koeff.Add(tmp);
            }

            for (int i = 0; i < points[0].Count; i++)
            {
                koeff[i][0] = 1;
                for (int j = 1; j < points[0].Count; j++)
                {
                    double tmp = 1;
                    for (int k = 0; k < j; k++)
                        tmp *= (points[0][i] - points[0][k]);
                    koeff[i][j] = tmp;
                }
            }


            List<double> X = new List<double>();
            X.Add(points[1][0]);
            for (int i = 1; i < points[0].Count; i++)
            {
                double sum = points[1][i];
                for (int j = 0; j < i; j++)
                {
                    sum -= ((double)koeff[i][j] * X[j]);
                }
                X.Add((sum / koeff[i][i]));
            }

            //for (int i = 0; i < points[0].Count; i++) X[i] = X[i] / 1000000;


            double start = points[0].Min();
            double end = points[0].Max();

            //start = start / 1000000;
            //end = end / 1000000;

            int length = (Int32)((end - start)/0.000001);
            List<List<double>> result = new List<List<double>>(2)
            {
                new List<double>(length),
                new List<double>(length)
            };

            for (int i = 0; i < length; i++)
            {
                double x = Math.Round(start + 0.000001 * i, 6);
                double y = X[0];
                for (int j = 1; j < points[0].Count; j++)
                {
                    double proiz = X[j];
                    for (int k = 0; k < j; k++)
                    {
                        proiz *= (x - points[0][k]);
                    }
                    y += proiz;
                }
                //double y = X[0] + X[1] * x + X[2] * x * x;
                result[0].Add(x);
                result[1].Add(y);
            }

            return result;
        }
    }
}
