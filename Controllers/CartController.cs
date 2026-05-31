using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GpuStore.Data;
using GpuStore.Models;

namespace GpuStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly GpuContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(GpuContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.GPU)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                cart = new Cart { UserId = user.Id };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return View(cart);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
         /*   var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {  
                return Json(new { success = false, redirectUrl = Url.Action("Login", "Account", new { returnUrl = Request.Path }) });
            }*/

            var user = await _userManager.GetUserAsync(User);
            int gpuId = request.GpuId;
            int quantity = request.Quantity > 0 ? request.Quantity : 1;
           
          

            // Validate quantity
            if (quantity <= 0 || quantity > 100)
            {
                return Json(new { success = false, message = "Invalid quantity. Please enter a value between 1 and 100." });
            }

            var gpu = await _context.GPUs.FindAsync(gpuId);
            if (gpu == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            if (!gpu.InStock)
            {
                return Json(new { success = false, message = $"{gpu.Name} is out of stock." });
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                cart = new Cart { UserId = user.Id };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingItem = cart.Items.FirstOrDefault(ci => ci.GpuId == gpuId);
            bool isMaxed = false;

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                if (existingItem.Quantity > 100)
                {
                    existingItem.Quantity = 100;
                    isMaxed = true;
                }
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    GpuId = gpuId,
                    Quantity = quantity,
                    Price = gpu.DisplayPrice
                };
                _context.CartItems.Add(cartItem);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var totalItems = cart.Items.Sum(ci => ci.Quantity);
            var message = isMaxed 
                ? $"{gpu.Name} added to cart! (Max quantity reached)"
                : $"{gpu.Name} added to cart!";
            var gpuhere = await _context.GPUs.FindAsync(gpuId);
            if (gpuhere == null)
            {
                // Temporarily log what IDs exist to diagnose
                var allIds = await _context.GPUs.Select(g => g.Id).ToListAsync();
                return Json(new
                {
                    success = false,
                    message = $"Product not found. Searched for ID {gpuId}. Available IDs: {string.Join(",", allIds)}"
                });
            }
            return Json(new { success = true, message = message, cartCount = totalItems });
        }
        public class AddToCartRequest
        {
            public int GpuId { get; set; }
            public int Quantity { get; set; }
        }
        // POST: Cart/RemoveFromCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);

            if (cartItem == null)
            {
                TempData["Error"] = "Item not found in cart.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (cartItem.Cart.UserId != user!.Id)
            {
                TempData["Error"] = "Unauthorized access.";
                return Unauthorized();
            }

            _context.CartItems.Remove(cartItem);
            cartItem.Cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Item removed from cart.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            if (quantity <= 0 || quantity > 100)
            {
                TempData["Error"] = "Invalid quantity. Please enter a value between 1 and 100.";
                return RedirectToAction(nameof(Index));
            }

            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                TempData["Error"] = "Item not found in cart.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.Id == cartItem.CartId && c.UserId == user!.Id);

            if (cart == null)
            {
                TempData["Error"] = "Unauthorized access.";
                return Unauthorized();
            }

            cartItem.Quantity = quantity;
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Quantity updated.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/ClearCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return Unauthorized();
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart != null && cart.Items.Any())
            {
                _context.CartItems.RemoveRange(cart.Items);
                cart.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cart cleared successfully.";
            }
            else
            {
                TempData["Info"] = "Cart is already empty.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
