using DeliveryServiceEM.Interfaces;
using DeliveryServiceEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryServiceEM.Services
{
    public class OrderFilterService
    {
        private readonly IDatabaseService _databaseService;

        public OrderFilterService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public List<Order> FilterOrders(string district, DateTime startDateTime, DateTime endDateTime)
        {
            var allOrders = _databaseService.GetOrders();

            var filteredOrders = allOrders
                .Where(order => order.District == district
                                && order.DeliveryTime >= startDateTime
                                && order.DeliveryTime <= endDateTime)
                .ToList();

            var firstOrder = filteredOrders.OrderBy(order => order.DeliveryTime).FirstOrDefault();
            if (firstOrder == null)
            {
                return new List<Order>();
            }

            DateTime thresholdTime = firstOrder.DeliveryTime.AddMinutes(30);
            return filteredOrders.Where(order => order.DeliveryTime <= thresholdTime).ToList();
        }
    }

}
