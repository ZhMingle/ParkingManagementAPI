using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParkingManagementAPI.Data;
using ParkingManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using ParkingManagementAPI.Models.DTO;
using ParkingManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace ParkingManagementAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomerOrderController : ControllerBase
    {
        private readonly SmartParkingContext _context;
        private readonly CustomerOrderService _customerOrderService;
        public CustomerOrderController(SmartParkingContext context, CustomerOrderService customerOrderService)
        {
            _context = context;
            _customerOrderService = customerOrderService;
        }
        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerOrder>>> GetOrders([FromQuery] PagingParametersDTO pagingParameters)
        {
            var total = await _context.CustomerOrders.CountAsync();
            var orders = await _context.CustomerOrders
                .OrderBy(o => o.StartTime)
                .Skip((pagingParameters.PageNumber - 1) * pagingParameters.PageSizeLimit)
                .Take(pagingParameters.PageSizeLimit)
                .ToListAsync();
            // 返回分页数据和总条数
            var pagedResponse = new PagedResponse<CustomerOrder>(orders, total);
            return Ok(pagedResponse);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerOrder>> GetOrder(int id)
        {
            var order = await _context.CustomerOrders
            .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return order;
        }

        // POST: api/order
        [HttpPost]
        public async Task<ActionResult<CustomerOrder>> CreateOrder(CustomerOrder order)
        {
            var createdOrder = await _customerOrderService.CreateOrderAsync(order);
            return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
        }

        // PUT: api/order/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, CustomerOrder updatedOrder)
        {
            if (id != updatedOrder.Id)
            {
                return BadRequest("CustomerOrder ID mismatch");
            }

            _context.Entry(updatedOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/order/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.CustomerOrders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.CustomerOrders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 检查订单是否存在
        private bool OrderExists(int id)
        {
            return _context.CustomerOrders.Any(e => e.Id == id);
        }
    }
}