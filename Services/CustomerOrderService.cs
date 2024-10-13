using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParkingManagementAPI.Models;
using ParkingManagementAPI.Data;

namespace ParkingManagementAPI.Services
{
    public class CustomerOrderService
    {
        private readonly SmartParkingContext _context;

        public CustomerOrderService(SmartParkingContext context)
        {
            _context = context;
        }

        public async Task<CustomerOrder> CreateOrderAsync(CustomerOrder customerOrder)
        {
            _context.CustomerOrders.Add(customerOrder);
            await _context.SaveChangesAsync();
            return customerOrder;
        }

    }
}