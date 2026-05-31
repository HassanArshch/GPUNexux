# GPU Nexus

A dark-themed GPU e-commerce platform built with ASP.NET Core 8 MVC. Browse, filter, and purchase graphics cards through a responsive storefront with full cart, order management, and admin panel support.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8.0 MVC |
| Templating | Razor Views |
| ORM | Entity Framework Core 8.0 |
| Database | SQLite |
| Auth | ASP.NET Identity + RBAC |
| Frontend | HTML5, CSS3, Vanilla JS |

## Architecture

```text
┌─────────────────────────────────────┐
│     Presentation Layer (Views)      │
│   Razor Views - HTML Templates      │
└────────────────┬────────────────────┘
                 │
┌────────────────▼────────────────────┐
│   Application Layer (Controllers)   │
│ Business Logic & Route Handling     │
└────────────────┬────────────────────┘
                 │
┌────────────────▼────────────────────┐
│         Domain Layer (Models)       │
│ Entities, ViewModels, DTOs          │
└────────────────┬────────────────────┘
                 │
┌────────────────▼────────────────────┐
│     Data Access Layer (DbContext)   │
│ Entity Framework Core - GpuContext  │
└────────────────┬────────────────────┘
                 │
┌────────────────▼────────────────────┐
│      Persistence Layer (SQLite)     │
│      Physical Data Storage          │
└─────────────────────────────────────┘
```
---

## Features

- **Storefront** — GPU listing grid with filtering, detail pages, and stock status
- **Shopping Cart** — Add, update, remove items; quantity validation (1–100); persistent per-user
- **Checkout & Orders** — Multi-step checkout, order history, status tracking (Pending → Confirmed → Shipped → Delivered), cancellation
- **Authentication** — Register/login via ASP.NET Identity; role-based access (User / Admin)
- **Admin Panel** — Manage all orders, update statuses, view customer info
- **Responsive Design** — Mobile-first layouts with a dark theme and orange accents

---

## Architecture

```
GPU Nexus
├── Controllers/        # MVC controllers (Home, Cart, Order, Account, Admin)
├── Models/             # EF Core entities (GPU, Cart, CartItem, Order, OrderItem)
├── Views/              # Razor views per controller
├── Data/               # DbContext + EF migrations
├── wwwroot/
│   ├── css/            # Site-wide + page-specific stylesheets
│   └── js/             # Vanilla JS (cart interactions, UI helpers)
└── Areas/Identity/     # ASP.NET Identity scaffolded pages
```

**Key relationships:**
- `User` → `Cart` → `CartItems` → `GPU`
- `User` → `Orders` → `OrderItems` → `GPU`

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022+ or VS Code

### Run locally

```bash
git clone https://github.com/your-username/gpu-nexus.git
cd gpu-nexus

dotnet restore
dotnet ef database update
dotnet run
```

Navigate to `https://localhost:{PORT}` in your browser.

### Seed data
The app seeds an initial Admin account and sample GPU listings on first run. Check `Data/DbInitializer.cs` for credentials.

---

## Key Endpoints

| Route | Description |
|---|---|
| `GET /` | GPU storefront |
| `GET /Home/Details/{id}` | GPU detail page |
| `GET /Cart` | View cart |
| `POST /Cart/AddToCart` | Add item to cart |
| `GET /Order/Checkout` | Checkout form |
| `GET /Order/MyOrders` | User order history |
| `GET /Order/AllOrders` | Admin: all orders |
| `POST /Order/UpdateOrderStatus` | Admin: update status |

---

## Security

- CSRF protection on all POST actions
- `[Authorize]` on all cart and order routes
- `[Authorize(Roles = "Admin")]` on admin routes
- Server-side input validation + EF parameterized queries (SQL injection prevention)


