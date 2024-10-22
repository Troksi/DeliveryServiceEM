using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryServiceEM.Models
{
    public class Order
    {
        public int Id { get; set; }
        public float Weight { get; set; }
        public string District { get; set; }
        public DateTime DeliveryTime { get; set; }
    }
}
