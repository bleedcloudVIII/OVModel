using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Wpf;
using OxyPlot.Axes;
using iText.Kernel.Pdf;
using iText.Layout;
using OVModel.Lib.EqualElements;

namespace OVModel.Lib.CommonClasses
{
    public static class CommonMethods
    {
        public static bool isCanConvertToDouble(string str)
        {
            try
            {
                double.Parse(str);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static List<EqualElements.EqualElements> getSetList(List<EqualElements.EqualElements> list)
        {
            List<EqualElements.EqualElements> result = new List<EqualElements.EqualElements>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!result.Contains(list[i])) result.Add(list[i]);
            }
            return result;
        }
    }

    public static class GlobalVariable
    {
        // Global Variable
        public const double p11 = 0.121;
        public const double p12 = 0.27;
        public const double mu = 0.164;
    }
}