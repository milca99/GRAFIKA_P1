﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class NetworkModel
    {
        public List<SubstationEntity> Substations { get; set; }
        public List<NodeEntity> Nodes { get; set; }
        public List<SwitchEntity> Switches { get; set; }
        public List<LineEntity> Lines { get; set; }
    }
}
