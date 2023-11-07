﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OVModel_DopTheory
{
    public static class DopTheory 
    {
        // Global Variable
        private const double p11 = 0.121;
        private const double p12 = 0.27;
        private const double mu = 0.164;

        public static double n_x(double x, double n, double R, double b)
        {
            double mu_R_R = mu / (R * R);
            double f = mu_R_R * x * x * (p12 - (p11 / (2 * mu)));
            double mu_R = mu / R;
            double s = mu_R * x * (p11 + p12 - (p12 / mu));
            double t = mu_R_R * b * b * ((p11 / (2 * mu)) - p12);
            return n + ((n * n * n) / 2) * (f + s + t);
        }

        public static double n_y(double x, double n, double R, double b)
        {
            double mu_R_R = mu / (R * R);
            double f = mu_R_R * x * x * (p11 - (p12 / mu) + p12);
            double mu_R = mu / R;
            double s = mu_R * x * (2 * p11 + 2 * p12 - (2 * p12 / mu));
            double t = mu_R_R * b * b * ((p12 / mu) - p12 - p11);
            return n + ((n * n * n) / 4) * (f + s + t);
        }

        public static double n_z(double x, double n, double R, double b)
        {
            double mu_R_R = mu / (R * R);
            double f = mu_R_R * x * x * (p11 - (p12 / mu) + p12);
            double mu_R = mu / R;
            double s = mu_R * x * (4 * p12 - ((2*p11)/mu));
            double t = mu_R_R * b * b * ((p12 / mu) - p12 - p11);
            return n + ((n * n * n) / 4) * (f + s + t);
        }
    }


}
