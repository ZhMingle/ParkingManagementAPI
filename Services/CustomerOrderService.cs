using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParkingManagementAPI.Models;
using ParkingManagementAPI.Data;
using Microsoft.EntityFrameworkCore;


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
        public async Task<CustomerOrder?> GetOrderByPlateNumberAsync(string plateNumber)
        {

            return await _context.CustomerOrders
                .FirstOrDefaultAsync(order => order.PlateNumber == plateNumber);
        }
        // 更新订单
        public async Task UpdateOrderAsync(CustomerOrder order)
        {
            _context.CustomerOrders.Update(order);
            await _context.SaveChangesAsync();
        }


    }
}