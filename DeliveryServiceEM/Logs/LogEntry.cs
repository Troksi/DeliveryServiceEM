﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryServiceEM.Logs
{
    public class LogEntry
    {
        public int Id { get; set; } 
        public string IpAddress { get; set; }
        public DateTime AccessTime { get; set; }
    }
}
