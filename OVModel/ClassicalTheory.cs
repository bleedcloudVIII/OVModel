using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OVModel_CommonClasses;

namespace OVModel_ClassicalTheory
{
    internal class ClassicalTheory
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
                double mu_R = mu / R;
                double s = mu_R * x * (p11 + p12 - (p12 / mu));
                double t = mu_R_R * b * b * ((p11 / (2 * mu)) - p12);
                return Math.Round(n + ((n * n * n) / 2) * (s + t), 9);
            }

            private static double n_y(double x, double n, double R, double b)
            {
                double mu_R_R = mu / (R * R);
                double mu_R = mu / R;
                double s = mu_R * x * (2 * p11 + 2 * p12 - (2 * p12 / mu));
                double t = mu_R_R * b * b * ((p12 / mu) - p12 - p11);
                return Math.Round(n + ((n * n * n) / 4) * (s + t), 9);
            }

            private static double n_z(double x, double n, double R, double b)
            {
                double mu_R_R = mu / (R * R);
                double mu_R = mu / R;
                double s = mu_R * x * (4 * p12 - ((2 * p11) / mu));
                double t = mu_R_R * b * b * ((p12 / mu) - p12 - p11);
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

            public static Data Calculating(double b, double h, double n, double n_ob, double R, double x_start, double x_end, string title, string titleAxisX, string titleAxisY)
            { 
                return new Data();
            }
        }
    }
}
