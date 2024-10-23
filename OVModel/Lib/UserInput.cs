using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace OVModel.Lib.UserInput
{
    public class UserInput
    {
        public UserInput() {}

        private bool isCanConvertToDouble(string str)
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

        public int setValues(MainWindow window)
        {
            if (isCanConvertToDouble(window.Input_2b.Text) &&
                isCanConvertToDouble(window.Input_h.Text) &&
                isCanConvertToDouble(window.Input_n.Text) &&
                isCanConvertToDouble(window.Input_R.Text) &&
                isCanConvertToDouble(window.Input_x_start.Text) &&
                isCanConvertToDouble(window.Input_x_end.Text))
            {
                b = double.Parse(window.Input_2b.Text) / 2;
                h = double.Parse(window.Input_h.Text);
                n = double.Parse(window.Input_n.Text);
                R = double.Parse(window.Input_R.Text);
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
        public double x_start = 0;
        public double x_end = 0;
    }
}
