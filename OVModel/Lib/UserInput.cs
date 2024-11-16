using iText.Kernel.Numbering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using OVModel.Lib.CommonClasses;

namespace OVModel.Lib.UserInput
{
    public class UserInput
    {
        public UserInput() {}



        public int setValues(MainWindow window)
        {
            if (CommonMethods.isCanConvertToDouble(window.Input_2b.Text) &&
                CommonMethods.isCanConvertToDouble(window.Input_h.Text) &&
                CommonMethods.isCanConvertToDouble(window.Input_n.Text) &&
                CommonMethods.isCanConvertToDouble(window.Input_R.Text) &&
                CommonMethods.isCanConvertToDouble(window.Input_x_start.Text) &&
                CommonMethods.isCanConvertToDouble(window.Input_x_end.Text) &&
                CommonMethods.isCanConvertToDouble(window.Input_Alpha.Text))
            {
                b = double.Parse(window.Input_2b.Text) / 2;
                h = double.Parse(window.Input_h.Text);
                n = double.Parse(window.Input_n.Text);
                R = double.Parse(window.Input_R.Text);
                Alpha = double.Parse(window.Input_Alpha.Text);
                x_start = double.Parse(window.Input_x_start.Text);
                x_end = double.Parse(window.Input_x_end.Text);
                
                return 0;
            }
            return -1;
        }

        public double b = 0;
        public double h = 0;
        public double n = 0;
        public double R = 0;
        public double Alpha = 0;
        public double x_start = 0;
        public double x_end = 0;
    }
}
