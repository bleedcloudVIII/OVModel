﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OVModel.Lib.EqualElements;

namespace OVModel.Lib.Data
{
    public struct Data
    {
        public List<List<double>> itemsSourceTable { get; set; }
        public OxyPlot.PlotModel scheduleModel { get; set; }
        public List<EqualElements.EqualElements> equalsElements { get; set; }
    }

}
