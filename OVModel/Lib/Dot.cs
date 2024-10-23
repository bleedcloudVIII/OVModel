using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OVModel.Lib.EqualElements;

namespace OVModel.Lib.Dot
{
    public struct Dot
    {
        public double x { get; set; }
        public double y { get; set; }
        public static Dot CrossTwoLines(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            double n;
            Dot resultDot = new Dot();
            if (y2 - y1 != 0)
            {  // a(y)
                double q = (x2 - x1) / (y1 - y2);
                double sn = (x3 - x4) + (y3 - y4) * q;
                if (sn == 0)
                {
                    resultDot.x = -1;
                    resultDot.y = -1;
                    return resultDot;
                }// c(x) + c(y)*q
                double fn = (x3 - x1) + (y3 - y1) * q;   // b(x) + b(y)*q
                n = fn / sn;
            }
            else
            {
                if (y3 - y4 == 0)
                {
                    resultDot.x = -1;  // b(y)
                    resultDot.y = -1;
                    return resultDot;
                }
                n = (y3 - y1) / (y3 - y4);   // c(y)/b(y)
            }
            resultDot.x = x3 + (x4 - x3) * n;  // x3 + (-b(x))*n
            resultDot.y = Math.Round(y3 + (y4 - y3) * n, 9);  // y3 +(-b(y))*n
            return resultDot;
        }

        //public static Dot CrossPoint(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        public static List<EqualElements.EqualElements> CrossPoints(List<List<double>> arr, double n)
        {
            // https://algolist.ru/maths/geom/intersect/lineline2d.php
            //      0   1   2   3
            // arr: x, nx, ny, nz
            double x_start = arr[0][0];
            double x_end = arr[arr.Count - 1][0];

            double n_x_start = arr[0][1];
            double n_x_end = arr[arr.Count - 1][1];

            double n_y_start = arr[0][2];
            double n_y_end = arr[arr.Count - 1][2];

            double n_z_start = arr[0][3];
            double n_z_end = arr[arr.Count - 1][3];
            List<EqualElements.EqualElements> list = new List<EqualElements.EqualElements>();

            //        x1  y1    x2   y2   x3  y3   x4  y4
            // nx/x (x_s, nx), (x_e, nx), (x_s, n), (x_e, n)
            // U_a = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

            double U_a = ((x_end - x_start) * (n_x_start- n) - (n - n)*(x_start - x_start)) / ((n - n) * (x_end - x_start) - (x_end - x_start) * (n_x_end - n_x_start));
            if (U_a >= 0 && U_a <= 1)
            {
                double x = x_start + U_a * (x_end - x_start);
                double y = n_x_start + U_a * (n_x_end - n_x_start);
                list.Add(new EqualElements.EqualElements() { x = x, n_value = y, first = "n_x", second = "n", cross = "n_x/n" });
            }

            U_a = ((x_end - x_start) * (n_y_start - n) - (n - n) * (x_start - x_start)) / ((n - n) * (x_end - x_start) - (x_end - x_start) * (n_y_end - n_y_start));
            if (U_a >= 0 && U_a <= 1)
            {
                double x = x_start + U_a * (x_end - x_start);
                double y = n_y_start + U_a * (n_y_end - n_y_start);
                list.Add(new EqualElements.EqualElements() { x = x, n_value = y, first = "n_y", second = "n", cross = "n_y/n" });
            }

            U_a = ((x_end - x_start) * (n_z_start - n) - (n - n) * (x_start - x_start)) / ((n - n) * (x_end - x_start) - (x_end - x_start) * (n_z_end - n_z_start));
            if (U_a >= 0 && U_a <= 1)
            {
                double x = x_start + U_a * (x_end - x_start);
                double y = n_z_start + U_a * (n_z_end - n_z_start);
                list.Add(new EqualElements.EqualElements() { x = x, n_value = y, first = "n_z", second = "n", cross = "n_z/n" });
            }

            //        x1   y1      x2   y2    x3   y3      x4   y4
            // nx/ny (x_s, nxs), (x_e, nxe), (x_s, nys), (x_e, nye)
            // U_a = ((x4 - x3)      * (y1        - y3)        - (y4       -  y3)        * (x1      - x3))      / ((y4      - y3)        * (x2    - x1)      - (x4    - x3)      * (y2      - y1));
            U_a = ((x_end - x_start) * (n_x_start - n_y_start) - (n_y_end  -  n_y_start) * (x_start - x_start)) / ((n_y_end - n_y_start) * (x_end - x_start) - (x_end - x_start) * (n_x_end - n_x_start));
            if (U_a >= 0 && U_a <= 1)
            {
                double x = x_start + U_a * (x_end - x_start);
                double y = n_x_start + U_a * (n_x_end - n_x_start);
                list.Add(new EqualElements.EqualElements() { x = x, n_value = y, first = "n_x", second = "n_y", cross = "n_x/n_y" });
            }

            U_a = ((x_end - x_start) * (n_x_start - n_z_start) - (n_z_end - n_z_start) * (x_start - x_start)) / ((n_z_end - n_z_start) * (x_end - x_start) - (x_end - x_start) * (n_x_end - n_x_start));
            if (U_a >= 0 && U_a <= 1)
            {
                double x = x_start + U_a * (x_end - x_start);
                double y = n_x_start + U_a * (n_x_end - n_x_start);
                list.Add(new EqualElements.EqualElements() { x = x, n_value = y, first = "n_x", second = "n_z", cross = "n_x/n_z" });
            }

            U_a = ((x_end - x_start) * (n_y_start - n_z_start) - (n_z_end - n_z_start) * (x_start - x_start)) / ((n_z_end - n_z_start) * (x_end - x_start) - (x_end - x_start) * (n_y_end - n_y_start));
            if (U_a >= 0 && U_a <= 1)
            {
                double x = x_start + U_a * (x_end - x_start);
                double y = n_y_start + U_a * (n_y_end - n_y_start);
                list.Add(new EqualElements.EqualElements() { x = x, n_value = y, first = "n_y", second = "n_z", cross = "n_y/n_z" });
            }

            return list;
        }
    }

}
