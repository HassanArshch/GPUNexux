using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GpuStore.Data;
using GpuStore.Models;
using System.ComponentModel.DataAnnotations;

namespace GpuStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly GpuContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(GpuContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Order/MyOrders
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        // GET: Order/Details
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // GET: Order/Checkout
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.GPU)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Your cart is empty";
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutViewModel
            {
                ShippingAddress = $"{user.Address}, {user.City}, {user.State} {user.PostalCode}, {user.Country}",
                Cart = cart,
                TotalAmount = cart.TotalPrice
            };

            return View(model);
        }

        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(ci => ci.GPU)
                    .FirstOrDefaultAsync(c => c.UserId == user.Id);

                model.Cart = cart;
                model.TotalAmount = cart?.TotalPrice ?? 0;
                return View(model);
            }

            var cartToCheckout = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.GPU)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cartToCheckout == null || !cartToCheckout.Items.Any())
            {
                TempData["Error"] = "Your cart is empty. Cannot proceed with checkout.";
                return RedirectToAction("Index", "Cart");
            }

            try
            {
                // Validate all items are still in stock
                foreach (var item in cartToCheckout.Items)
                {
                    if (!item.GPU.InStock)
                    {
                        TempData["Error"] = $"{item.GPU.Name} is no longer in stock. Please update your cart.";
                        return RedirectToAction("Index", "Cart");
                    }
                }

                // Create order
                var order = new Order
                {
                    UserId = user.Id,
                    OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{user.Id.Substring(0, Math.Min(6, user.Id.Length))}",
                    ShippingAddress = model.ShippingAddress,
                    Notes = model.Notes,
                    Status = "Pending",
                    TotalAmount = cartToCheckout.TotalPrice,
                    CreatedAt = DateTime.UtcNow
                };

                // Add cart items to order
                foreach (var item in cartToCheckout.Items)
                {
                    var orderItem = new OrderItem
                    {
                        GpuId = item.GpuId,
                        GpuName = item.GPU.Name,
                        Quantity = item.Quantity,
                        UnitPrice = (decimal)item.Price,
                        TotalPrice = item.Quantity * (decimal)item.Price
                    };
                    order.Items.Add(orderItem);
                }

                _context.Orders.Add(order);

                // Clear cart
                _context.CartItems.RemoveRange(cartToCheckout.Items);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Order placed successfully! Order Number: {order.OrderNumber}";
                return RedirectToAction(nameof(Details), new { id = order.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while processing your order: {ex.Message}";
                model.Cart = cartToCheckout;
                model.TotalAmount = cartToCheckout.TotalPrice;
                return View(model);
            }
        }

        // POST: Order/CancelOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return Unauthorized();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return NotFound();
            }

            if (order.Status != "Pending")
            {
                TempData["Error"] = $"Cannot cancel {order.Status.ToLower()} orders. Only pending orders can be cancelled.";
                return RedirectToAction(nameof(Details), new { id = order.Id });
            }

            try
            {
                order.Status = "Cancelled";
                await _context.SaveChangesAsync();

                TempData["Success"] = "Order cancelled successfully.";
                return RedirectToAction(nameof(Details), new { id = order.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while cancelling the order: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = order.Id });
            }
        }

        // Admin: GET: Order/AllOrders
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        // Admin: POST: Order/UpdateOrderStatus
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return NotFound();
            }

            var validStatuses = new[] { "Pending", "Confirmed", "Shipped", "Delivered", "Cancelled" };
            if (string.IsNullOrEmpty(status) || !validStatuses.Contains(status))
            {
                TempData["Error"] = "Invalid status. Valid statuses are: Pending, Confirmed, Shipped, Delivered, Cancelled.";
                return RedirectToAction("Orders", "Admin");
            }

            try
            {
                order.Status = status;

                if (status == "Shipped")
                {
                    order.ShippedAt = DateTime.UtcNow;
                }
                else if (status == "Delivered")
                {
                    order.DeliveredAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = $"Order status updated to {status} successfully.";
                return RedirectToAction("Orders", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while updating the order status: {ex.Message}";
                return RedirectToAction("Orders", "Admin");
            }
        }
    }

    public class CheckoutViewModel
    {
        [Required]
        [StringLength(500, ErrorMessage = "Shipping address cannot exceed 500 characters")]
        public string ShippingAddress { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }

        public decimal TotalAmount { get; set; }

        public Cart? Cart { get; set; }
    }
}
