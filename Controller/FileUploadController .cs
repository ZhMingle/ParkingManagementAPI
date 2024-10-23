using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using ParkingManagementAPI.Models;
using ParkingManagementAPI.Controller;
using ParkingManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using ParkingManagementAPI.Enum;

namespace ParkingManagementAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly CustomerOrderService _customerOrderService;
        public FileUploadController(IWebHostEnvironment env, IHttpClientFactory httpClientFactory, CustomerOrderService customerOrderService)
        {
            _env = env;
            _httpClientFactory = httpClientFactory;
            _customerOrderService = customerOrderService;

        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file, string cameraId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // 设置保存路径，可以将图片存到服务器本地的 "Uploads" 目录下
            var uploadPath = Path.Combine(_env.WebRootPath, "Uploads");



            // 如果文件夹不存在，创建它
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            // 获取文件的扩展名
            var fileExtension = Path.GetExtension(file.FileName);

            // 使用时间戳生成唯一文件名
            var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff"); // 年月日时分秒毫秒
            var newFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{timeStamp}{fileExtension}";

            var filePath = Path.Combine(uploadPath, newFileName);

            // 将文件保存到指定路径
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            // 调用第三方 API 进行图片识别
            var recognitionResult = await RecognizeImageAsync(filePath, file.ContentType);

            // 假设识别结果中有车牌号，解析返回的 JSON
            var plateNumber = ParsePlateNumberFromResult(recognitionResult); // 解析车牌号

            if (string.IsNullOrEmpty(plateNumber))
            {
                return BadRequest("Failed to recognize plate number.");
            }
            var PlateImage = $"http://localhost:5240/Uploads/{newFileName}";

            // 创建订单
            if (cameraId == "1")
            {
                int randomNumber = new Random().Next(1, 69);
                var newOrder = new CustomerOrder
                {
                    PlateNumber = plateNumber,
                    PlateImage = PlateImage, // 假设你已经存储了图片并获取其 URL
                    Status = OrderStatus.Active, // 进入停车场，状态设置为 active
                    camera_id = cameraId,
                    StartTime = DateTime.Now, // 设置进入停车场的时间
                    EndTime = null,
                    Price = 0, // 初始价格为 0，可以在离场时计算价格
                    SpotNumber = randomNumber // 1-68的随机数
                };
                await _customerOrderService.CreateOrderAsync(newOrder);
            }
            else if (cameraId == "2")
            {
                // 当 cameraId 为 2 时，更新订单状态为已完成，并计算费用
                var existingOrder = await _customerOrderService.GetOrderByPlateNumberAsync(plateNumber);
                if (existingOrder == null)
                {
                    return NotFound("Order not found for the given plate number.");
                }

                // 更新订单状态为已完成
                existingOrder.Status = OrderStatus.Completed; // 使用枚举表示完成状态
                existingOrder.EndTime = DateTime.Now; // 设置当前时间为离场时间

                // 计算停车时长并计算费用
                if (existingOrder.StartTime.HasValue)
                {
                    var duration = existingOrder.EndTime.Value - existingOrder.StartTime.Value;
                    double totalMinutes = duration.TotalMinutes;

                    // 每半小时 2 美元
                    double halfHourUnits = Math.Ceiling(totalMinutes / 30); // 向上取整，至少收 0.5 小时费用
                    existingOrder.Price = (decimal)halfHourUnits * 2; // 每 0.5 小时 2 美元
                }

                // 保存更新后的订单
                await _customerOrderService.UpdateOrderAsync(existingOrder);
            }



            return Ok(new { FilePath = filePath, recognitionResult });
        }
        private async Task<string> RecognizeImageAsync(string filePath, string ContentType)
        {
            var client = _httpClientFactory.CreateClient();

            // 创建一个 MultipartFormDataContent 对象用于上传图片
            var content = new MultipartFormDataContent();
            var fileStream = System.IO.File.OpenRead(filePath);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(ContentType); // 根据实际的图片类型设置
            content.Add(fileContent, "upload", Path.GetFileName(filePath));

            // 添加 Authorization Token 到请求头
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", "3700483d10b1ee7098ae84b4c07c018badd73315");

            var response = await client.PostAsync("https://api.platerecognizer.com/v1/plate-reader/", content);

            // 检查 API 响应
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result; // 假设识别结果是 JSON 或者其他文本格式
            }
            else
            {
                return "Failed to recognize the image";
            }
        }
        private string ParsePlateNumberFromResult(string recognitionResult)
        {
            // 假设 recognitionResult 是一个 JSON 格式的字符串
            // 使用 Newtonsoft.Json 或 System.Text.Json 进行解析
            var json = JsonConvert.DeserializeObject<dynamic>(recognitionResult);
            string plateNumber = json.results[0].plate; // 根据实际 JSON 结构提取车牌号
            return plateNumber;
        }


    }

}