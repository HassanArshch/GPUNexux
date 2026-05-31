# GPU Store - Cart and Order Logic Implementation Guide

## Overview
This document outlines the completed cart and order logic for the GPU Nexus application. The system provides comprehensive shopping cart management and order processing capabilities for users.

## Cart Logic

### Features Implemented
- **Add to Cart**: Users can add GPUs to their shopping cart with quantity validation (1-100 items)
- **Update Quantity**: Modify item quantities with validation
- **Remove from Cart**: Delete individual items from the cart
- **Clear Cart**: Empty the entire cart at once
- **Inventory Check**: Validates items are in stock before adding

### Key Components

#### CartController Actions
1. **Index (GET)** - Display shopping cart
   - Loads user's cart with all items
   - Calculates totals automatically
   - Shows empty cart message if no items

2. **AddToCart (POST)** - Add GPU to cart
   - Validates GPU exists and is in stock
   - Validates quantity (1-100 range)
   - Updates existing items or creates new cart items
   - Returns success/error messages via TempData

3. **UpdateQuantity (POST)** - Modify item quantity
   - Validates new quantity (1-100)
   - Verifies user authorization
   - Updates cart item and timestamp
   - Includes error handling

4. **RemoveFromCart (POST)** - Delete item from cart
   - Verifies user owns the cart item
   - Updates cart timestamp
   - Shows confirmation message

5. **ClearCart (POST)** - Empty entire cart
   - Removes all items
   - Shows appropriate feedback (cleared/already empty)

#### Data Models
- **Cart**: Represents a user's shopping session
  - `UserId`: Links to user
  - `Items`: Collection of CartItems
  - `TotalPrice`: Computed property for order total
  - `TotalQuantity`: Computed property for item count
  - `CreatedAt/UpdatedAt`: Timestamps

- **CartItem**: Individual products in cart
  - `CartId`: Foreign key to Cart
  - `GpuId`: Foreign key to GPU
  - `Quantity`: Number of units
  - `Price`: Stored at time of add
  - `AddedAt`: Timestamp

#### Views
- **Views/Cart/Index.cshtml**
  - Displays all items in a table format
  - Shows unit price, quantity, and total
  - Provides quantity selector (1-10 options)
  - Remove and clear buttons
  - Checkout link
  - Order summary with total

## Order Logic

### Features Implemented
- **Checkout**: Multi-step checkout process with validation
- **Order Creation**: Creates order from cart items with timestamps
- **Order History**: Users can view their past orders
- **Order Details**: View complete order information
- **Order Cancellation**: Cancel pending orders only
- **Admin Management**: Admins can update order status and view all orders
- **Status Tracking**: Pending → Confirmed → Shipped → Delivered/Cancelled

### Key Components

#### OrderController Actions

1. **Checkout (GET)** - Display checkout form
   - Loads user's cart
   - Pre-fills shipping address from user profile
   - Shows order summary
   - Returns error if cart is empty

2. **Checkout (POST)** - Process order
   - Validates checkout form (shipping address required)
   - Verifies all items still in stock
   - Creates Order with unique order number
   - Transfers CartItems to OrderItems
   - Clears cart after successful order
   - Includes exception handling for edge cases

3. **MyOrders (GET)** - Display user's orders
   - Lists all orders for current user
   - Sorted by date descending
   - Shows order status badges
   - Quick actions for viewing details or cancelling

4. **Details (GET)** - View order information
   - Shows complete order details
   - Timeline of order status changes
   - Lists all items with prices
   - Cancel option for pending orders

5. **CancelOrder (POST)** - Cancel pending order
   - Only allows cancelling "Pending" orders
   - Updates status to "Cancelled"
   - Validates user authorization
   - Shows appropriate error messages

6. **AllOrders (GET)** - Admin view of all orders
   - Lists all orders in the system
   - Shows customer information
   - Displays order totals and dates
   - Status dropdown for quick updates

7. **UpdateOrderStatus (POST)** - Admin status update
   - Updates order status with validation
   - Sets timestamp when shipped/delivered
   - Only accessible to Admin role
   - Validates status is from approved list

#### Data Models
- **Order**: Represents a customer purchase
  - `OrderNumber`: Unique identifier (ORD-TIMESTAMP-USERID)
  - `UserId`: Customer reference
  - `TotalAmount`: Order total
  - `Status`: Current state (Pending, Confirmed, Shipped, Delivered, Cancelled)
  - `ShippingAddress`: Delivery location
  - `Notes`: Optional customer instructions
  - `CreatedAt`: Order date
  - `ShippedAt/DeliveredAt`: Status timestamps

- **OrderItem**: Product details in order
  - `OrderId`: Foreign key to Order
  - `GpuId`: Reference to GPU
  - `GpuName`: Snapshot of GPU name
  - `Quantity`: Units ordered
  - `UnitPrice`: Price at time of order
  - `TotalPrice`: Line item total

#### Views
- **Views/Order/Checkout.cshtml**
  - Shipping address textarea (pre-populated)
  - Optional notes field
  - Order summary panel
  - Cart items preview
  - Complete Purchase button

- **Views/Order/MyOrders.cshtml**
  - List of all user orders
  - Status badges with color coding
  - Items preview (first 3, "+X more")
  - Order totals
  - View Details and Cancel buttons
  - Empty state message

- **Views/Order/Details.cshtml**
  - Complete order information
  - Customer shipping address
  - Timeline showing status changes
  - Items table with pricing
  - Order summary
  - Cancel button (if pending)

- **Views/Order/AllOrders.cshtml** (NEW)
  - Admin view of all orders
  - Table showing customer, date, items, total
  - Status dropdown for quick updates
  - View Details links
  - Responsive design for desktop admin panels

#### CheckoutViewModel
- `ShippingAddress`: Required, max 500 chars
- `Notes`: Optional, max 500 chars
- `TotalAmount`: Display-only order total
- `Cart`: Display-only cart reference

## Validation & Error Handling

### Cart Validation
- Quantity must be 1-100
- GPU must exist in database
- GPU must be in stock
- User must be authenticated
- Cart item must belong to user

### Order Validation
- Cart must not be empty
- Shipping address required
- All items must still be in stock at checkout time
- User must be authenticated
- Only pending orders can be cancelled
- Only admin can update order status

### Error Messages
- Success messages shown via green alerts
- Error messages shown via red alerts
- Warning messages shown via yellow alerts
- Info messages shown via blue alerts
- All alerts are dismissible

### Exception Handling
- Try-catch blocks in checkout and order operations
- Graceful error messages to user
- Transaction consistency maintained
- Detailed error logging capability

## Frontend Enhancements

### JavaScript (site.js)
- **Add to Cart Button Handler**: 
  - Intercepts button click
  - Creates hidden form with CSRF token
  - Extracts GPU ID from data attribute
  - Submits to CartController.AddToCart
  - Fallback for browsers with disabled JavaScript

### HTML Attributes
- `data-gpu-id` on GPU cards for JavaScript integration
- Proper form validation attributes
- Accessible button and link elements

## Security Features

### Authorization
- [Authorize] attribute on all cart/order actions
- [Authorize(Roles = "Admin")] on admin actions
- User ID verification on all operations
- CSRF token validation on all POST actions

### Input Validation
- Server-side validation for all inputs
- Model state validation before processing
- Numeric range checking for quantities
- String length validation for addresses

## Database Schema

### Relationships
- One User → Many Carts (1:N)
- One Cart → Many CartItems (1:N)
- One GPU → Many CartItems (1:N, Restrict on delete)
- One User → Many Orders (1:N)
- One Order → Many OrderItems (1:N)
- One GPU → Many OrderItems (1:N, Restrict on delete)

### Cascade Delete
- Cart deleted → CartItems deleted
- Order deleted → OrderItems deleted
- User deleted → Carts and Orders deleted

## Testing Checklist

- [ ] Add single item to cart
- [ ] Add multiple quantities
- [ ] Update item quantity
- [ ] Remove item from cart
- [ ] Clear entire cart
- [ ] Try adding out-of-stock item (should fail)
- [ ] Try adding with invalid quantity (should fail)
- [ ] Proceed to checkout
- [ ] Update shipping address
- [ ] Complete order
- [ ] View order in MyOrders
- [ ] View order details
- [ ] Cancel pending order
- [ ] Verify order cannot be re-cancelled
- [ ] Admin: View all orders
- [ ] Admin: Update order status
- [ ] Admin: Check timestamp updates

## Future Enhancements

Potential features for future implementation:
- Payment gateway integration
- Email notifications for order status changes
- Wishlist functionality
- Order notes/history
- Inventory management with low stock alerts
- Automatic order reminders
- Customer reviews and ratings
- Bulk order discounts
- Subscription/recurring orders
- Order refund system

## API Endpoints Summary

### Cart Endpoints
- `GET /Cart/Index` - View cart
- `POST /Cart/AddToCart` - Add item
- `POST /Cart/RemoveFromCart` - Delete item
- `POST /Cart/UpdateQuantity` - Change quantity
- `POST /Cart/ClearCart` - Empty cart

### Order Endpoints
- `GET /Order/MyOrders` - List user's orders
- `GET /Order/Details/{id}` - View order details
- `GET /Order/Checkout` - Show checkout form
- `POST /Order/Checkout` - Process checkout
- `POST /Order/CancelOrder/{id}` - Cancel pending order
- `GET /Order/AllOrders` - Admin: View all orders
- `POST /Order/UpdateOrderStatus` - Admin: Update status

## Troubleshooting

### Issue: Items not adding to cart
**Solution**: Verify the GPU ID is correctly passed and the GPU exists in database

### Issue: Checkout fails
**Solution**: Ensure cart is not empty and all items are still in stock

### Issue: Order not appearing
**Solution**: Verify user is logged in and order status is visible in MyOrders

### Issue: Cannot cancel order
**Solution**: Only "Pending" orders can be cancelled. Check order status in details page.

---

For questions or issues, please refer to the main application documentation or contact the development team.
