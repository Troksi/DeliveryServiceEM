using DeliveryServiceEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryServiceEM.Interfaces
{
    public interface IDatabaseService
    {
        List<Order> GetOrders();
        //void SaveOrder(Order order);
        //void SaveOrders(List<Order> orders);
    }
}
