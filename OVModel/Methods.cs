using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OxyPlot;
using OxyPlot.Axes;
using OVModel_CommonClasses;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Interpolation;

using OVModel.Lib.EqualElements;

namespace OVModel_Methods
{
    
    public static class SLY
    {
        private static int global_n;
        private static List<List<double>> global_C1;
        private static List<List<double>> global_C2;
        private static List<double> global_d;

        private static List<List<decimal>> global_C1_dec;
        private static List<List<decimal>> global_C2_dec;
        private static List<decimal> global_d_dec;

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

            List<List<double>> C = new List<List<double>>();
            List<List<double>> C1 = new List<List<double>>();
            List<List<double>> C2 = new List<List<double>>();

            List<double> D = new List<double>();

            // Расчёт C и d
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

            // Проверка на диагональное преобладание
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < n; j++)
                {
                    if (i != j) sum += C[i][j];
                }

                if (sum > C[i][i]) return new List<double>();
            }

            // Расчёт C1 и C2
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

        public static List<decimal> multiplyMatrixOnVector(List<List<decimal>> list, List<decimal> vector)
        {
            List<decimal> result = new List<decimal>();

            for (int i = 0; i < list.Count; i++)
            {
                decimal sum = 0;
                for (int j = 0; j < list.Count; j++) sum += list[i][j] * vector[j];
                result.Add(sum);
            }

            return result;
        }

        public static decimal normaForVector(List<decimal> list)
        {
            return list.Max();
        }

        private static List<decimal> x_k_plus_1(List<decimal> x_k)
        {
            List<decimal> result = new List<decimal>(global_n);
            for (int i = 0; i < global_n; i++) result.Add(0);

            List<decimal> C1_x_k = new List<decimal>();
            C1_x_k = multiplyMatrixOnVector(global_C1_dec, x_k);
            for (int i = 0; i < global_n; i++)
            {
                result[i] = C1_x_k[i] + multiplyMatrixOnVector(global_C2_dec, result)[i] + global_d_dec[i];
            }
            return result;
        }

        public static List<decimal> vectorMinusVector(List<decimal> list1, List<decimal> list2)
        {
            List<decimal> result = new List<decimal>();

            for (int i = 0; i < global_n; i++)
            {
                decimal sum = 0;
                sum = list1[i] - list2[i];
                result.Add(sum);
            }

            return result;
        }

        public static List<decimal> Zeidelya(List<List<decimal>> matrix, List<decimal> B)
        {
            int n = matrix.Count;

            List<List<decimal>> C = new List<List<decimal>>();
            List<List<decimal>> C1 = new List<List<decimal>>();
            List<List<decimal>> C2 = new List<List<decimal>>();

            List<decimal> D = new List<decimal>();

            // Расчёт C и d
            for (int i = 0; i < n; i++)
            {
                List<decimal> list = new List<decimal>();
                for (int j = 0; j < n; j++)
                {
                    if (i != j) list.Add((-1) * (matrix[i][j] / matrix[i][i]));
                    else list.Add((decimal)0);
                }
                C.Add(list);
                D.Add(B[i] / matrix[i][i]);
            }

            // Проверка на диагональное преобладание
            for (int i = 0; i < n; i++)
            {
                decimal sum = 0;
                for (int j = 0; j < n; j++)
                {
                    if (i != j) sum += C[i][j];
                }

                if (sum > C[i][i]) return new List<decimal>();
            }

            // Расчёт C1 и C2
            for (int i = 0; i < n; i++)
            {
                List<decimal> tmp1 = new List<decimal>();
                List<decimal> tmp2 = new List<decimal>();

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

            global_C1_dec = C1;
            global_C2_dec = C2;
            global_n = n;
            global_d_dec = D;

            // Метод Зейделя
            List<decimal> x_0 = new List<decimal>(n);
            for (int i = 0; i < n; i++) x_0.Add(0);
            decimal E = 0.000001M;

            List<decimal> x_1 = x_k_plus_1(x_0);
            while (normaForVector(vectorMinusVector(x_1, x_0)) >= E)
            {
                x_0 = x_1;
                x_1 = x_k_plus_1(x_0);
            }

            return x_1;
        }



        public static List<decimal> Kramer(List<EqualElements> list)
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
            for (int i = 0; i < points.Count; i++) x_second.Add((long)points[i].x * (long)points[i].x);

            List<long> x_third = new List<long>(points.Count);
            for (int i = 0; i < points.Count; i++) x_third.Add((long)points[i].x * x_second[i]);

            List<long> x_fourth = new List<long>(points.Count);
            for (int i = 0; i < points.Count; i++) x_fourth.Add((long)points[i].x * x_third[i]);

            long sumx = points.Sum(elem => (long)elem.x);
            long sumx2 = x_second.Sum();
            long sumx3 = x_third.Sum();
            long sumx4 = x_fourth.Sum();

            long sumy = points.Sum(elem => (long)elem.n_value);

            long sumyx = 0;
            for (int i = 0; i < points.Count; i++) sumyx += ((long)points[i].n_value * (long)points[i].x);

            long sumyx2 = 0; ;
            for (int i = 0; i < points.Count; i++) sumyx2 += ((long)points[i].n_value * (long)points[i].x * (long)points[i].x);

            System.Numerics.BigInteger opredelitel = 0, x1_opred, x2_opred, x3_opred, x1b, x2b, x3b;

            opredelitel = points.Count * sumx2 * sumx4;
            opredelitel += sumx * sumx2 * sumx3;
            opredelitel += sumx * sumx2 * sumx3;
            opredelitel -= sumx2 * sumx2 * sumx2;
            opredelitel -= sumx * sumx * sumx4;
            opredelitel -= points.Count * sumx3 * sumx3;


            x1_opred = sumy * sumx2 * sumx4 + sumx2 * sumx3 * sumyx + sumx * sumx3 * sumyx2 - sumx * sumyx * sumx4 - sumx3 * sumx3 * sumy;
            x2_opred = points.Count * sumyx * sumx4 + 2 * sumx * sumx3 * sumx2 - sumx2 * sumx2 * sumyx - sumy * sumx * sumx4 - sumx3 * sumyx2 * points.Count;
            x3_opred = points.Count * sumx2 * sumyx2 + sumx * sumx3 * sumy + sumx * sumyx * sumy - sumx2 * sumx2 * sumy - sumx * sumx * sumyx2 - sumyx * sumx3 * points.Count;

            //decimal x1 = (decimal)(x1_opred / opredelitel);
            //decimal x2 = (decimal)(x2_opred / opredelitel);
            //decimal x3 = (decimal)(x3_opred / opredelitel);

            //x1b = x1_opred / opredelitel;
            //x2b = x2_opred / opredelitel;
            //x3b = x3_opred / opredelitel;

            return new List<decimal>() {};
        }

        private static List<decimal> vectorDevidedNumber(List<decimal> a, decimal number)
        {
            List<decimal> result = new List<decimal>();

            for (int i = 0; i < a.Count; i++) result.Add(a[i] * (1/number));

            return result;
        }

        private static List<decimal> vectorMinusNumber(List<decimal> a, decimal number)
        {
            List<decimal> result = new List<decimal>();

            for (int i = 0; i < a.Count; i++) result.Add(a[i] - number);

            return result;
        }
        
        private static List<decimal> vectorMultiplyNumber(List<decimal> a, decimal number)
        {
            List<decimal> result = new List<decimal>();
            for (int i = 0; i < a.Count; i++) result.Add(a[i] * number);

            return result;
        }

        private static decimal SquareVector(List<decimal> a)
        {
            decimal result = 0;
            for (int i = 0; i < a.Count; i++) result += a[i] * a[i];

            return result;
        }

        private static List<decimal> VectorPlusVector(List<decimal> a, List<decimal> b)
        {
            List<decimal> result = new List<decimal>();
            for (int i = 0; i < a.Count; i++) result.Add(a[i] + b[i]);

            return result;
        }

        private static List<decimal> vectorMinusVectorOrtodoks(List<decimal> a, List<decimal> b)
        {
            List<decimal> result = new List<decimal>();
            for (int i = 0; i < a.Count; i++) result.Add(a[i] - b[i]);

            return result;
        }

        /*
         matrix(polinom 2) = 
                  |n    x    x^2  y   |
                  |x    x^2  x^3  xy  |
                  |x^2  x^3  x^4  x^2y|
         */
        // https://core.ac.uk/download/pdf/77241064.pdf
        public static List<decimal> ortodoks(List<List<decimal>> matrix)
        {
            int count = matrix[0].Count - 1;
            decimal E = 0.000001M;

            List<List<decimal>> B = new List<List<decimal>>();
            List<decimal> U = new List<decimal>();
            //List<decimal> A = new List<decimal>();

            List<List<decimal>> allU = new List<List<decimal>>();
            U = matrix[0];
            //decimal sum = U.Sum();
            B.Add(vectorDevidedNumber(U, U.Sum()));
            allU.Add(U);
            for (int i = 1; i < matrix[0].Count; i++)
            {
                if (i != matrix[0].Count - 1) U = matrix[i];
                else
                {
                    U = new List<decimal>();
                    for (int y = 0; y < count; y++) U.Add(0);
                    U.Add(1);
                }

                //Для U
                // Несколько раз, чтобы повысить точность
                List<decimal> sum = new List<decimal>();
                for (int k = 0; k < matrix[0].Count; k++) sum.Add(0);

                for (int k = 0; k < 1; k++)
                {
                    for (int j = 0; j < i; j++) sum = VectorPlusVector(sum, vectorMultiplyNumber(allU[j], SquareVector(B[j])));
                    U = vectorMinusVectorOrtodoks(U, sum);
                }

                allU.Add(U);
                B.Add(vectorDevidedNumber(U, U.Sum()));
            }

            List<decimal> result = new List<decimal>()
            {
                B[count][0] / B[count][count],
                B[count][1] / B[count][count],
                B[count][2] / B[count][count]
            };

            return result;
        }
    }
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
        
        public static List<List<double>> interpolyacia_newtoon(List<EqualElements> list)
        {
            List<List<double>> points = new List<List<double>>();
            points.Add(new List<double>());
            points.Add(new List<double>());

            if (points.Count == 0) return new List<List<double>>();


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
                for (int j = 1; j < i + 1; j++)
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

            double start = points[0].Min();
            double end = points[0].Max();

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
                result[0].Add(x);
                result[1].Add(y);
            }

            return result;
        }
    }
}
