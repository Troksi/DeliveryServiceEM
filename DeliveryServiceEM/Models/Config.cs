using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryServiceEM.Models
{
    public class Config
    {
        public string ConnectionString;
        public string LogPath;

        public bool RunWithoutWindow;
        public string CityDistrict;
        public DateTime FirstDeliveryDateTime;

        public string DeliveryLogPath;
        public string DeliveryOrderPath;
    }
}
