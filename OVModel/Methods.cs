using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OVModel_Methods
{
    public static class Approksimacia
    {
        public static List<double> approksimacia_polinom_1(List<List<double>> points)
        {
            List<double> x_second = new List<double>(points[0].Count) {};
            for (int i = 0; i < x_second.Count; i++) x_second[i] = points[0][i] * points[0][i];

            List<double> x_third = new List<double>(points[0].Count) { };
            for (int i = 0; i < x_third.Count; i++) x_third[i] = points[0][i] * points[0][i];

            return new List<double>() { };
        }

        public static List<double> approksimacia_polinom_2(List<List<double>> points)
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

            List<List<long>> matrix = new List<List<long>>() {
                    new List<long> { points[0].Count, sumx, sumx2 },
                    new List<long> { sumx, sumx2, sumx3 },
                    new List<long> { sumx2, sumx3, sumx4 }
            };

            return new List<double>() { };
        }
    }

    public static class Interpolyacia
    {

    }
}
