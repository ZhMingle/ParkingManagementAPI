using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagementAPI.Models
{
    public class ParkingSpace
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public bool IsAvailable { get; set; }
    }
}