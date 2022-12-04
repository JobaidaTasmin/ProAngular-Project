using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Angular_Project_Foodhub_Work_02.Models;

namespace Angular_Project_Foodhub_Work_02.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersContextController : ControllerBase
    {
        private readonly FoodDbContext _context;

        public OrdersContextController(FoodDbContext context)
        {
            _context = context;
        }

        // GET: api/OrdersContext
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/OrdersContext/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/OrdersContext/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.OrderID)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

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
        [HttpPut("VM/{id}")]
        public async Task<IActionResult> PutOrderWithOrderItem(int id, Order order)
        {
            if (id != order.OrderID)
            {
                return BadRequest();
            }
            var existing = await _context.Orders.Include(x=> x.OrderDetails).FirstAsync(o=> o.OrderID == id);
            _context.OrderDetails.RemoveRange(existing.OrderDetails);
            foreach(var item in order.OrderDetails)
            {
                _context.OrderDetails.Add(new OrderDetail {  OrderID=order.OrderID, FoodID=item.FoodID, Quantity=item.Quantity});
            }
            _context.Entry(existing).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
               
                    throw  ex.InnerException;
               
            }

            return NoContent();
        }
        // POST: api/OrdersContext
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.OrderID }, order);
        }

        // DELETE: api/OrdersContext/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}
