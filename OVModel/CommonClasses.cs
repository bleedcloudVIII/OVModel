using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OVModel_CommonClasses
{
    public struct EqualElements
    {
        public double x { get; set; }
        public double n_value { get; set; }
        public string first { get; set; }
        public string second { get; set; }

        public static bool operator ==(EqualElements f, EqualElements s)
        {
            return (f.x == s.x && f.n_value == s.n_value && f.first == s.first && f.second == s.second);
        }

        public static bool operator !=(EqualElements f, EqualElements s)
        {
            return !(f == s);
        }
    }

    public struct Data
    {
        public List<List<double>> itemsSourceTable { get; set; }
        public OxyPlot.PlotModel scheduleModel { get; set; }
        public List<EqualElements> equalsElements { get; set; }
    }
}
