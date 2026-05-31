using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GpuStore.Models;

namespace GpuStore.Data
{
    public class GpuContext : IdentityDbContext<ApplicationUser>
    {
        public GpuContext(DbContextOptions<GpuContext> options) : base(options) { }

        public DbSet<GPU> GPUs { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Cart and User relationship
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure CartItem relationships
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.GPU)
                .WithMany()
                .HasForeignKey(ci => ci.GpuId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Order and User relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure OrderItem relationships
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            modelBuilder.Entity<GPU>().HasData(
                new GPU { Id = 1, Name = "GeForce RTX 4090", Brand = "NVIDIA", Architecture = "Ada Lovelace", ChipName = "AD102", VramGB = 24, VramType = "GDDR6X", MemoryBusWidth = 384, CoreCount = 16384, BaseClock = 2230, BoostClock = 2520, TDP = 450, PCIeGeneration = 4, PCIeLanes = 16, DisplayOutputs = "3x DP 1.4a, 1x HDMI 2.1", RayTracingSupport = true, DLSSSupport = true, Price = 1599.00f, IsOnSale = false, InStock = true, ImageUrl = "https://static.gigabyte.com/StaticFile/Image/Global/09d32ada8b1f37d37a708ba4581238f9/ProductRemoveBg/32013", Description = "The fastest consumer GPU ever made. Titan-class performance for 4K gaming and creative workloads.", CreatedAt = new DateTime(2024, 1, 1) },
                new GPU { Id = 2, Name = "GeForce RTX 4080 Super", Brand = "NVIDIA", Architecture = "Ada Lovelace", ChipName = "AD103", VramGB = 16, VramType = "GDDR6X", MemoryBusWidth = 256, CoreCount = 10240, BaseClock = 2295, BoostClock = 2550, TDP = 320, PCIeGeneration = 4, PCIeLanes = 16, DisplayOutputs = "3x DP 1.4a, 1x HDMI 2.1", RayTracingSupport = true, DLSSSupport = true, Price = 999.00f, SalePrice = 849.00f, IsOnSale = true, InStock = true, ImageUrl = "https://storage-asset.msi.com/global/picture/product/product_170470049111744051fdaa00ae211c3172060a5866.webp", Description = "Top-tier performance with 16GB GDDR6X. Perfect for enthusiast 4K gaming.", CreatedAt = new DateTime(2024, 1, 1) },
                new GPU { Id = 3, Name = "GeForce RTX 4070 Ti Super", Brand = "NVIDIA", Architecture = "Ada Lovelace", ChipName = "AD103", VramGB = 16, VramType = "GDDR6X", MemoryBusWidth = 256, CoreCount = 8448, BaseClock = 2340, BoostClock = 2610, TDP = 285, PCIeGeneration = 4, PCIeLanes = 16, DisplayOutputs = "3x DP 1.4a, 1x HDMI 2.1", RayTracingSupport = true, DLSSSupport = true, Price = 799.00f, SalePrice = 699.00f, IsOnSale = true, InStock = true, ImageUrl = "https://static.gigabyte.com/StaticFile/Image/Global/4ff5ef905e0d95bb802303397cccb4b2/ProductRemoveBg/39097", Description = "Supercharged for 1440p. Excellent ray tracing and DLSS 3 support.", CreatedAt = new DateTime(2024, 1, 1) },
                new GPU { Id = 4, Name = "GeForce RTX 4070 Super", Brand = "NVIDIA", Architecture = "Ada Lovelace", ChipName = "AD104", VramGB = 12, VramType = "GDDR6X", MemoryBusWidth = 192, CoreCount = 7168, BaseClock = 1980, BoostClock = 2475, TDP = 220, PCIeGeneration = 4, PCIeLanes = 16, DisplayOutputs = "3x DP 1.4a, 1x HDMI 2.1", RayTracingSupport = true, DLSSSupport = true, Price = 599.00f, IsOnSale = false, InStock = true, ImageUrl = "https://hcomdistributors.pk/wp-content/uploads/2024/10/asus-rtx-4070-super-evo-oc-gpu-price-in-pakistan.jpg.webp", Description = "The sweet spot for 1440p gaming with DLSS 3 Frame Generation.", CreatedAt = new DateTime(2024, 1, 1) },
                new GPU { Id = 5, Name = "GeForce RTX 4060 Ti", Brand = "NVIDIA", Architecture = "Ada Lovelace", ChipName = "AD106", VramGB = 8, VramType = "GDDR6", MemoryBusWidth = 128, CoreCount = 4352, BaseClock = 2310, BoostClock = 2535, TDP = 160, PCIeGeneration = 4, PCIeLanes = 8, DisplayOutputs = "3x DP 1.4a, 1x HDMI 2.1", RayTracingSupport = true, DLSSSupport = true, Price = 399.00f, SalePrice = 349.00f, IsOnSale = true, InStock = true, ImageUrl = "https://static.gigabyte.com/StaticFile/Image/Global/296b2ab8d0ab5576fd74b73dfc806f8f/ProductRemoveBg/36480", Description = "Efficient 1080p/1440p card with DLSS 3 and low power consumption.", CreatedAt = new DateTime(2024, 1, 1) },
                new GPU { Id = 6, Name = "Radeon RX 7900 XTX", Brand = "AMD", Architecture = "RDNA 3", ChipName = "Navi 31", VramGB = 24, VramType = "GDDR6", MemoryBusWidth = 384, CoreCount = 12288, BaseClock = 1855, BoostClock = 2500, TDP = 355, PCIeGeneration = 4, PCIeLanes = 16, DisplayOutputs = "2x DP 2.1, 1x HDMI 2.1, 1x USB-C", RayTracingSupport = true, DLSSSupport = false, Price = 999.00f, IsOnSale = false, InStock = true, ImageUrl = "https://techarc.pk/wp-content/uploads/2026/01/sapphire-pulse-rx-7900-xtx-gaming-24gb-graphics-card-1.webp", Description = "AMD's flagship with 24GB GDDR6 and DisplayPort 2.1 for 8K gaming.", CreatedAt = new DateTime(2024, 1, 1) },
                new GPU { Id = 7, Name = "Radeon RX 7900 XT", Brand = "AMD", Architecture = "RDNA 3", ChipName = "Navi 31", VramGB = 20, VramType = "GDDR6", MemoryBusWidth = 320, CoreCount = 10752, BaseClock = 1500, BoostClock = 2400, TDP = 315, PCIeGeneration = 4, PCIeLanes = 16, DisplayOutputs = "2x DP 2.1, 1x HDMI 2.1, 1x USB-C", RayTracingSupport = true, DLSSSupport = false, Price = 799.00f, SalePrice = 699.00f, IsOnSale = true, InStock = true, ImageUrl = "https://www.pakbyte.pk/cdn/shop/files/Asus-Tuf-Gaming-Radeon-RX-7900-XTX-OC-Edition-24GB-GDDR6-Graphics-Card-PakByte-Computers-25199385444419.jpg?v=1753676793", Description = "20GB of GDDR6 memory makes this a content creator's dream card.", CreatedAt = new DateTime(2024, 1, 1) },
                new GPU { Id = 8, Name = "Radeon RX 7800 XT", Brand = "AMD", Architecture = "RDNA 3", ChipName = "Navi 32", VramGB = 16, VramType = "GDDR6", MemoryBusWidth = 256, CoreCount = 3840, BaseClock = 1295, BoostClock = 2430, TDP = 263, PCIeGeneration = 4, PCIeLanes = 8, DisplayOutputs = "1x DP 2.1, 2x DP 1.4, 1x HDMI 2.1", RayTracingSupport = true, DLSSSupport = false, Price = 499.00f, IsOnSale = false, InStock = true, ImageUrl = "https://techarc.pk/wp-content/uploads/2026/01/sapphire-pulse-rx-7800-xt-16gb-gaming-graphics-card-1.webp", Description = "Excellent 1440p performance with 16GB VRAM at a competitive price.", CreatedAt = new DateTime(2024, 1, 1) },
                new GPU { Id = 9, Name = "Radeon RX 7600 XT", Brand = "AMD", Architecture = "RDNA 3", ChipName = "Navi 33", VramGB = 16, VramType = "GDDR6", MemoryBusWidth = 128, CoreCount = 2048, BaseClock = 1720, BoostClock = 2755, TDP = 165, PCIeGeneration = 4, PCIeLanes = 8, DisplayOutputs = "1x DP 2.1, 2x DP 1.4, 1x HDMI 2.1", RayTracingSupport = true, DLSSSupport = false, Price = 329.00f, SalePrice = 279.00f, IsOnSale = true, InStock = true, ImageUrl = "https://static.webx.pk/files/19643/Images/arktek-rx-7600-xt-8gb-price-in-pakistan-junaidtech-1-19643-2528224-210126070808960.webp", Description = "16GB VRAM on a budget card. Ideal for 1080p and light 1440p gaming.", CreatedAt = new DateTime(2024, 1, 1) },
                new GPU { Id = 10, Name = "Arc A770 16GB", Brand = "Intel", Architecture = "Alchemist", ChipName = "ACM-G10", VramGB = 16, VramType = "GDDR6", MemoryBusWidth = 256, CoreCount = 4096, BaseClock = 2100, BoostClock = 2400, TDP = 225, PCIeGeneration = 4, PCIeLanes = 16, DisplayOutputs = "3x DP 2.0, 1x HDMI 2.0b", RayTracingSupport = true, DLSSSupport = false, Price = 349.00f, SalePrice = 299.00f, IsOnSale = true, InStock = true, ImageUrl = "https://m.media-amazon.com/images/I/71X6uBcNk3L.jpg", Description = "Intel's flagship Arc card with XeSS upscaling and AV1 encode/decode.", CreatedAt = new DateTime(2024, 1, 1) },
                new GPU { Id = 11, Name = "Arc A750", Brand = "Intel", Architecture = "Alchemist", ChipName = "ACM-G10", VramGB = 8, VramType = "GDDR6", MemoryBusWidth = 256, CoreCount = 3584, BaseClock = 2050, BoostClock = 2400, TDP = 225, PCIeGeneration = 4, PCIeLanes = 16, DisplayOutputs = "3x DP 2.0, 1x HDMI 2.0b", RayTracingSupport = true, DLSSSupport = false, Price = 249.00f, IsOnSale = false, InStock = true, ImageUrl = "https://www.dateks.lv/images/pic/2400/2400/891/1889.jpg", Description = "Value-oriented Arc card with solid rasterization and hardware ray tracing.", CreatedAt = new DateTime(2024, 1, 1) },
                new GPU { Id = 12, Name = "GeForce RTX 4060", Brand = "NVIDIA", Architecture = "Ada Lovelace", ChipName = "AD107", VramGB = 8, VramType = "GDDR6", MemoryBusWidth = 128, CoreCount = 3072, BaseClock = 1830, BoostClock = 2460, TDP = 115, PCIeGeneration = 4, PCIeLanes = 8, DisplayOutputs = "3x DP 1.4a, 1x HDMI 2.1", RayTracingSupport = true, DLSSSupport = true, Price = 299.00f, IsOnSale = false, InStock = true, ImageUrl = "https://static.webx.pk/files/83855/Images/asus-dual-geforce-rtx-4060-oc-edition-8gb-gddr6-pcie-4.0,-8g-83855-2371091-190525101152393.jpg", Description = "Ultra-efficient 1080p card with the lowest power draw in its class.", CreatedAt = new DateTime(2024, 1, 1) }
            );
        }
    }
}
