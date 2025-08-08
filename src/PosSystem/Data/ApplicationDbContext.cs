using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PosSystem.Models;

namespace PosSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Store> Stores { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionItem> TransactionItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        
        // Inventory Management
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<StockReceiving> StockReceivings { get; set; }
        public DbSet<StockReceivingItem> StockReceivingItems { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseStock> WarehouseStocks { get; set; }
        public DbSet<StockTransfer> StockTransfers { get; set; }
        public DbSet<StockTransferItem> StockTransferItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure User entity
            builder.Entity<User>(entity =>
            {
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
                entity.Property(e => e.PIN).HasMaxLength(10);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Store)
                    .WithMany(s => s.Users)
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Store entity
            builder.Entity<Store>(entity =>
            {
                entity.Property(e => e.StoreName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.StoreCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.TaxRate).HasPrecision(5, 4).HasDefaultValue(0.11m);
                entity.Property(e => e.Currency).HasMaxLength(3).HasDefaultValue("IDR");
                entity.Property(e => e.Timezone).HasMaxLength(50).HasDefaultValue("Asia/Jakarta");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.StoreCode).IsUnique();
            });

            // Configure Category entity
            builder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.ParentCategory)
                    .WithMany(c => c.SubCategories)
                    .HasForeignKey(e => e.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Product entity
            builder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Barcode).HasMaxLength(100);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.UnitPrice).HasPrecision(15, 2);
                entity.Property(e => e.CostPrice).HasPrecision(15, 2);
                entity.Property(e => e.StockQuantity).HasDefaultValue(0);
                entity.Property(e => e.MinStockLevel).HasDefaultValue(0);
                entity.Property(e => e.UnitOfMeasure).HasMaxLength(20).HasDefaultValue("PCS");
                entity.Property(e => e.HasVariants).HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.HasExpiry).HasDefaultValue(false);
                entity.Property(e => e.RequiresWeighing).HasDefaultValue(false);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.ProductCode).IsUnique();
                entity.HasIndex(e => e.Barcode).IsUnique();
                entity.HasIndex(e => e.CategoryId);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure ProductVariant entity
            builder.Entity<ProductVariant>(entity =>
            {
                entity.Property(e => e.VariantName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.VariantValue).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UnitPrice).HasPrecision(15, 2);
                entity.Property(e => e.StockQuantity).HasDefaultValue(0);
                entity.Property(e => e.Barcode).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasIndex(e => e.Barcode).IsUnique();

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.ProductVariants)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Customer entity
            builder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerCode).HasMaxLength(20);
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.CustomerGroup).HasMaxLength(20).HasDefaultValue("Regular");
                entity.Property(e => e.LoyaltyPoints).HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.CustomerCode).IsUnique();
            });

            // Configure Transaction entity
            builder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.TransactionNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TransactionDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.SubTotal).HasPrecision(15, 2);
                entity.Property(e => e.DiscountAmount).HasPrecision(15, 2).HasDefaultValue(0);
                entity.Property(e => e.TaxAmount).HasPrecision(15, 2).HasDefaultValue(0);
                entity.Property(e => e.TotalAmount).HasPrecision(15, 2);
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Completed");
                entity.Property(e => e.PaymentStatus).HasMaxLength(20).HasDefaultValue("Paid");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.TransactionNumber).IsUnique();
                entity.HasIndex(e => e.TransactionDate);
                entity.HasIndex(e => e.StoreId);
                entity.HasIndex(e => e.UserId);

                entity.HasOne(e => e.Store)
                    .WithMany(s => s.Transactions)
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Transactions)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Transactions)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure TransactionItem entity
            builder.Entity<TransactionItem>(entity =>
            {
                entity.Property(e => e.Quantity).HasPrecision(10, 3);
                entity.Property(e => e.UnitPrice).HasPrecision(15, 2);
                entity.Property(e => e.DiscountAmount).HasPrecision(15, 2).HasDefaultValue(0);
                entity.Property(e => e.LineTotal).HasPrecision(15, 2);

                entity.HasOne(e => e.Transaction)
                    .WithMany(t => t.TransactionItems)
                    .HasForeignKey(e => e.TransactionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.TransactionItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany(pv => pv.TransactionItems)
                    .HasForeignKey(e => e.ProductVariantId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Payment entity
            builder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Amount).HasPrecision(15, 2);
                entity.Property(e => e.ReceivedAmount).HasPrecision(15, 2);
                entity.Property(e => e.ChangeAmount).HasPrecision(15, 2).HasDefaultValue(0);
                entity.Property(e => e.ReferenceNumber).HasMaxLength(100);
                entity.Property(e => e.PaymentDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Success");

                entity.HasOne(e => e.Transaction)
                    .WithMany(t => t.Payments)
                    .HasForeignKey(e => e.TransactionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Promotion entity
            builder.Entity<Promotion>(entity =>
            {
                entity.Property(e => e.PromotionName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PromotionCode).HasMaxLength(50);
                entity.Property(e => e.PromotionType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Value).HasPrecision(15, 2);
                entity.Property(e => e.MinimumPurchase).HasPrecision(15, 2);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.UsedCount).HasDefaultValue(0);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.PromotionCode).IsUnique();
            });

            // Configure StockMovement entity
            builder.Entity<StockMovement>(entity =>
            {
                entity.Property(e => e.MovementType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Quantity).HasPrecision(10, 3);
                entity.Property(e => e.UnitCost).HasPrecision(15, 2);
                entity.Property(e => e.ReferenceType).HasMaxLength(20);
                entity.Property(e => e.BatchNumber).HasMaxLength(50);
                entity.Property(e => e.MovementDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.ProductId);
                entity.HasIndex(e => e.MovementDate);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.StockMovements)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany(pv => pv.StockMovements)
                    .HasForeignKey(e => e.ProductVariantId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.StockMovements)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure AuditLog entity
            builder.Entity<AuditLog>(entity =>
            {
                entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
                entity.Property(e => e.TableName).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => new { e.UserId, e.CreatedAt });

                entity.HasOne(e => e.User)
                    .WithMany(u => u.AuditLogs)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure SystemSetting entity
            builder.Entity<SystemSetting>(entity =>
            {
                entity.Property(e => e.SettingKey).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SettingValue).IsRequired();
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.SettingKey).IsUnique();

                entity.HasOne(e => e.UpdatedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.UpdatedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Supplier entity
            builder.Entity<Supplier>(entity =>
            {
                entity.Property(e => e.SupplierCode).HasMaxLength(50);
                entity.Property(e => e.SupplierName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ContactPerson).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.City).HasMaxLength(50);
                entity.Property(e => e.PostalCode).HasMaxLength(20);
                entity.Property(e => e.Country).HasMaxLength(50);
                entity.Property(e => e.TaxId).HasMaxLength(50);
                entity.Property(e => e.CreditLimit).HasPrecision(15, 2).HasDefaultValue(0);
                entity.Property(e => e.PaymentTermDays).HasDefaultValue(30);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.SupplierCode).IsUnique();
            });

            // Configure PurchaseOrder entity
            builder.Entity<PurchaseOrder>(entity =>
            {
                entity.Property(e => e.PurchaseOrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("DRAFT");
                entity.Property(e => e.SubTotal).HasPrecision(15, 2).HasDefaultValue(0);
                entity.Property(e => e.TaxAmount).HasPrecision(15, 2).HasDefaultValue(0);
                entity.Property(e => e.DiscountAmount).HasPrecision(15, 2).HasDefaultValue(0);
                entity.Property(e => e.ShippingCost).HasPrecision(15, 2).HasDefaultValue(0);
                entity.Property(e => e.TotalAmount).HasPrecision(15, 2).HasDefaultValue(0);
                entity.Property(e => e.Terms).HasMaxLength(500);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.PurchaseOrderNumber).IsUnique();
                entity.HasIndex(e => e.SupplierId);
                entity.HasIndex(e => e.Status);

                entity.HasOne(e => e.Supplier)
                    .WithMany(s => s.PurchaseOrders)
                    .HasForeignKey(e => e.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Store)
                    .WithMany()
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ApprovedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ApprovedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure PurchaseOrderItem entity
            builder.Entity<PurchaseOrderItem>(entity =>
            {
                entity.Property(e => e.QuantityOrdered).HasPrecision(10, 3);
                entity.Property(e => e.QuantityReceived).HasPrecision(10, 3).HasDefaultValue(0);
                entity.Property(e => e.UnitCost).HasPrecision(15, 2);
                entity.Property(e => e.DiscountAmount).HasPrecision(15, 2).HasDefaultValue(0);
                entity.Property(e => e.BatchNumber).HasMaxLength(50);
                entity.Property(e => e.Notes).HasMaxLength(500);
                
                // Ignore calculated properties
                entity.Ignore(e => e.QuantityPending);
                entity.Ignore(e => e.TotalCost);

                entity.HasOne(e => e.PurchaseOrder)
                    .WithMany(po => po.PurchaseOrderItems)
                    .HasForeignKey(e => e.PurchaseOrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany()
                    .HasForeignKey(e => e.ProductVariantId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure StockReceiving entity
            builder.Entity<StockReceiving>(entity =>
            {
                entity.Property(e => e.ReceivingNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("PENDING");
                entity.Property(e => e.TotalQuantity).HasPrecision(10, 3).HasDefaultValue(0);
                entity.Property(e => e.TotalCost).HasPrecision(15, 2).HasDefaultValue(0);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.ReceivingNumber).IsUnique();
                entity.HasIndex(e => e.PurchaseOrderId);
                entity.HasIndex(e => e.SupplierId);
                entity.HasIndex(e => e.Status);

                entity.HasOne(e => e.PurchaseOrder)
                    .WithMany(po => po.StockReceivings)
                    .HasForeignKey(e => e.PurchaseOrderId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Supplier)
                    .WithMany(s => s.StockReceivings)
                    .HasForeignKey(e => e.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Store)
                    .WithMany()
                    .HasForeignKey(e => e.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ReceivedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ReceivedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PostedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.PostedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure StockReceivingItem entity
            builder.Entity<StockReceivingItem>(entity =>
            {
                entity.Property(e => e.QuantityOrdered).HasPrecision(10, 3);
                entity.Property(e => e.QuantityReceived).HasPrecision(10, 3).HasDefaultValue(0);
                entity.Property(e => e.QuantityAccepted).HasPrecision(10, 3).HasDefaultValue(0);
                entity.Property(e => e.QuantityRejected).HasPrecision(10, 3).HasDefaultValue(0);
                entity.Property(e => e.UnitCost).HasPrecision(15, 2);
                entity.Property(e => e.BatchNumber).HasMaxLength(50);
                entity.Property(e => e.QualityStatus).HasMaxLength(20).HasDefaultValue("PENDING");
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasOne(e => e.StockReceiving)
                    .WithMany(sr => sr.StockReceivingItems)
                    .HasForeignKey(e => e.StockReceivingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.PurchaseOrderItem)
                    .WithMany(poi => poi.StockReceivingItems)
                    .HasForeignKey(e => e.PurchaseOrderItemId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany()
                    .HasForeignKey(e => e.ProductVariantId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Warehouse entity
            builder.Entity<Warehouse>(entity =>
            {
                entity.Property(e => e.WarehouseName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.WarehouseCode).HasMaxLength(20);
                entity.Property(e => e.WarehouseType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.City).HasMaxLength(50);
                entity.Property(e => e.PostalCode).HasMaxLength(20);
                entity.Property(e => e.ContactPerson).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.WarehouseCode).IsUnique();
            });

            // Configure WarehouseStock entity
            builder.Entity<WarehouseStock>(entity =>
            {
                entity.Property(e => e.QuantityOnHand).HasPrecision(10, 3).HasDefaultValue(0);
                entity.Property(e => e.QuantityReserved).HasPrecision(10, 3).HasDefaultValue(0);
                entity.Property(e => e.MinStockLevel).HasPrecision(10, 3).HasDefaultValue(0);
                entity.Property(e => e.MaxStockLevel).HasPrecision(10, 3).HasDefaultValue(0);
                entity.Property(e => e.ReorderPoint).HasPrecision(10, 3).HasDefaultValue(0);
                entity.Property(e => e.Location).HasMaxLength(50);
                entity.Property(e => e.AverageCost).HasPrecision(15, 2).HasDefaultValue(0);
                entity.Property(e => e.LastCost).HasPrecision(15, 2).HasDefaultValue(0);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Ignore calculated properties
                entity.Ignore(e => e.QuantityAvailable);

                entity.HasIndex(e => new { e.WarehouseId, e.ProductId, e.ProductVariantId }).IsUnique();

                entity.HasOne(e => e.Warehouse)
                    .WithMany(w => w.WarehouseStocks)
                    .HasForeignKey(e => e.WarehouseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany()
                    .HasForeignKey(e => e.ProductVariantId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure StockTransfer entity
            builder.Entity<StockTransfer>(entity =>
            {
                entity.Property(e => e.TransferNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("DRAFT");
                entity.Property(e => e.TransferType).IsRequired().HasMaxLength(20).HasDefaultValue("REGULAR");
                entity.Property(e => e.TotalQuantity).HasPrecision(10, 3).HasDefaultValue(0);
                entity.Property(e => e.TotalValue).HasPrecision(15, 2).HasDefaultValue(0);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.ShippingNotes).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.TransferNumber).IsUnique();
                entity.HasIndex(e => e.FromWarehouseId);
                entity.HasIndex(e => e.ToWarehouseId);
                entity.HasIndex(e => e.Status);

                entity.HasOne(e => e.FromWarehouse)
                    .WithMany()
                    .HasForeignKey(e => e.FromWarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ToWarehouse)
                    .WithMany()
                    .HasForeignKey(e => e.ToWarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ShippedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ShippedBy)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.ReceivedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ReceivedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure StockTransferItem entity
            builder.Entity<StockTransferItem>(entity =>
            {
                entity.Property(e => e.QuantityRequested).HasPrecision(10, 3);
                entity.Property(e => e.QuantityShipped).HasPrecision(10, 3).HasDefaultValue(0);
                entity.Property(e => e.QuantityReceived).HasPrecision(10, 3).HasDefaultValue(0);
                entity.Property(e => e.UnitCost).HasPrecision(15, 2);
                entity.Property(e => e.BatchNumber).HasMaxLength(50);
                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.HasOne(e => e.StockTransfer)
                    .WithMany(st => st.StockTransferItems)
                    .HasForeignKey(e => e.StockTransferId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ProductVariant)
                    .WithMany()
                    .HasForeignKey(e => e.ProductVariantId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.FromWarehouseStock)
                    .WithMany()
                    .HasForeignKey(e => e.FromWarehouseStockId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.ToWarehouseStock)
                    .WithMany()
                    .HasForeignKey(e => e.ToWarehouseStockId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}