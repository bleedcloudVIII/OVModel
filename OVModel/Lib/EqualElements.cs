using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OVModel.Lib.EqualElements
{
    public struct EqualElements : IComparable
    {
        public double x { get; set; }
        public double n_value { get; set; }
        public string first { get; set; }
        public string second { get; set; }
        public string cross { get; set; }

        public static bool operator ==(EqualElements f, EqualElements s)
        {
            return (f.x == s.x && f.n_value == s.n_value && f.first == s.first && f.second == s.second);
        }

        public static bool operator !=(EqualElements f, EqualElements s)
        {
            return !(f == s);
        }

        public EqualElements(EqualElements elem)
        {
            x = elem.x;
            n_value = elem.n_value;
            first = elem.first;
            second = elem.second;
            cross = elem.cross;
        }

        public int CompareTo(object o)
        {
            if (o is EqualElements eq) return eq.x.CompareTo(x);
            else throw new ArgumentException("Error on EqualElements struct");
        }
    }

}
