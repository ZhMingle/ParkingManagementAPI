using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagementAPI.Models
{

    public class SystemUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }  // 存储加密的密码
        public DateTime CreateAt { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }
    }
}