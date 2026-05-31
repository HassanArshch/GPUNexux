# GPU Nexus Store 🖥️

A full-featured GPU e-commerce website built with **ASP.NET Core 8 MVC**, inspired by TechPowerUp's hardware enthusiast aesthetic. Dark tech design, dense spec tables, and full 
panel.

---

## Features

- **Hero Deals Section** — Prominently displays all GPUs marked on sale with discount percentage, savings amount, and key specs
- **Brand Filter Pills** — Top-of-page NVIDIA / AMD / Intel checkboxes, synced with sidebar
- **Left Panel Filters** — Filter by VRAM, price range, max TDP, ray tracing support, on-sale status
- **Sort Options** — Price, VRAM, boost clock, efficiency, name
- **GPU Cards** — Full spec tables (VRAM, boost clock, bus width, TDP, shader count, PCIe), feature badges (DLSS 3 / FSR 3 / XeSS / Ray Tracing), stock status
- **Admin Panel** — `/Admin` — Add, edit, remove GPUs; toggle sale status with sale price; toggle stock
- **12 Seeded GPUs** — RTX 4090, 4080 Super, 4070 Ti Super, 4070 Super, 4060 Ti, 4060 · RX 7900 XTX/XT, 7800 XT, 7600 XT · Arc A770, A750
- **SQLite database** — Zero-config, file-based, auto-migrated on startup
- **Railway-ready** — Dockerfile + railway.toml included

---

## Quick Start (Local)

```bash
# 1. Clone / extract the project
cd GpuStore

# 2. Restore & run
dotnet restore
dotnet run

# 3. Open http://localhost:5000
```

---

## Railway Deployment

### Option A — Via GitHub (recommended)

1. Push the project to a GitHub repo
2. Go to [railway.app](https://railway.app) → **New Project → Deploy from GitHub**
3. Select your repo — Railway auto-detects the `Dockerfile`
4. The app will be live at your `.up.railway.app` URL

### Option B — Railway CLI

```bash
npm install -g @railway/cli
railway login
railway init
railway up
```

### SQLite Persistence on Railway

For Railway, add a **Volume** to persist the database between deploys:

1. In your Railway project → **Volumes** → Add Volume
2. Mount path: `/data`
3. The app uses `/data/gpustore.db` when `DB_PATH` env var is set:
   - Set `DB_PATH=Data Source=/data/gpustore.db` in Railway environment variables

---

## Project Structure

```
GpuStore/
├── Controllers/
│   ├── HomeController.cs       # Store + filtering logic
│   └── AdminController.cs      # CRUD admin operations
├── Models/
│   ├── GPU.cs                  # Entity model (all GPU specs)
│   └── ViewModels.cs           # Filter + admin view models
├── Data/
│   └── GpuContext.cs           # EF Core context + seed data
├── Views/
│   ├── Home/Index.cshtml       # Store page (hero + grid + filters)
│   ├── Admin/Index.cshtml      # Admin inventory table
│   ├── Admin/Create.cshtml     # Add GPU form
│   ├── Admin/Edit.cshtml       # Edit GPU form
│   └── Shared/_Layout.cshtml  # Site layout (header/footer/nav)
├── wwwroot/
│   ├── css/site.css            # Full TechPowerUp-inspired dark theme
│   └── js/site.js              # Filter interactions + animations
├── Program.cs                  # App startup + DI
├── Dockerfile                  # Multi-stage Docker build
└── railway.toml                # Railway deploy config
```

---

## Admin Panel

Navigate to `/Admin`:

| Action | Description |
|--------|-------------|
| **Add GPU** | Full form with all specs, sale pricing, stock status |
| **Edit GPU** | Modify any spec, toggle on-sale with sale price |
| **Remove GPU** | Delete from inventory (with confirmation) |
| **Toggle Stock** | Flip in/out of stock without deleting |

---

## GPU Data Model

Each GPU stores:

| Field | Type | Description |
|-------|------|-------------|
| Brand | string | NVIDIA / AMD / Intel |
| Architecture | string | Ada Lovelace / RDNA 3 / etc |
| VramGB | int | 8, 12, 16, 20, 24... |
| VramType | string | GDDR6X, GDDR7, HBM3... |
| MemoryBusWidth | int | 128, 256, 384-bit |
| CoreCount | int | Shader/CU count |
| BoostClock | int | MHz |
| TDP | int | Watts |
| Price / SalePrice | decimal | MSRP and sale price |
| IsOnSale | bool | Shows in hero + discount badge |
| RayTracingSupport | bool | Feature badge |
| DLSSSupport | bool | DLSS / FSR / XeSS badge |

---

## Design

**Aesthetic**: TechPowerUp dark hardware enthusiast theme

- **Fonts**: Rajdhani (display titles) + Exo 2 (body) + Share Tech Mono (specs/values)
- **Colors**: `#0e0e10` base · `#ff6b00` orange accent · `#76b900` NVIDIA green · `#ed1c24` AMD red · `#0071c5` Intel blue
- **Accents**: Per-brand colored top bars on cards, brand pills, badge system
- **Animations**: Staggered card fade-in on load, hover lift effects
