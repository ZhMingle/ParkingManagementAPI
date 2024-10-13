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

        public string camera_id {get; set; } // 1 - 入口  2 - 出口 用于区分是哪个摄像头. 这要方便操作对应的道闸.
        public string PlateImage { get; set; } // 车牌照片的URL
        public int Status { get; set; } // 两个状态: 进入停车场: active complete 
        public DateTime StartTime { get; set; } // 进入停车场的时间
        public DateTime? EndTime { get; set; } = null; // 出停车场的时间
        public decimal Price { get; set; }

    }
}