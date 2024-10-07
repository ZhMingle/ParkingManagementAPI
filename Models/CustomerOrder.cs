using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagementAPI.Models
{
    public class CustomerOrder
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; }
        public string PlateImage { get; set; }
        public int ParkingSpotId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }

        public ParkingSpace ParkingSpace { get; set; }
    }
}