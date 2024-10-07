using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ParkingManagementAPI.Models;

namespace ParkingManagementAPI.Data
{
    public class SmartParkingContext:DbContext
    {
        public SmartParkingContext(DbContextOptions<SmartParkingContext> options) : base(options)
        {
        }

        public DbSet<SystemUser> SystemUsers { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<ParkingSpace> ParkingSpaces { get; set; }
    }
}