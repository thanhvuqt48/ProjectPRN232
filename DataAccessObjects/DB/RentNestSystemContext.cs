﻿using System;
using System.Collections.Generic;
using BusinessObjects.Domains;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DB;

public partial class RentNestSystemContext : DbContext
{
    public RentNestSystemContext()
    {
    }

    public RentNestSystemContext(DbContextOptions<RentNestSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Accommodation> Accommodations { get; set; }

    public virtual DbSet<AccommodationAmenity> AccommodationAmenities { get; set; }

    public virtual DbSet<AccommodationDetail> AccommodationDetails { get; set; }

    public virtual DbSet<AccommodationImage> AccommodationImages { get; set; }

    public virtual DbSet<AccommodationType> AccommodationTypes { get; set; }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Amenity> Amenities { get; set; }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<FavoritePost> FavoritePosts { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<PackagePricing> PackagePricings { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostApproval> PostApprovals { get; set; }

    public virtual DbSet<PostPackageDetail> PostPackageDetails { get; set; }

    public virtual DbSet<PostPackageType> PostPackageTypes { get; set; }

    public virtual DbSet<PromoCode> PromoCodes { get; set; }

    public virtual DbSet<PromoUsage> PromoUsages { get; set; }

    public virtual DbSet<QuickReplyTemplate> QuickReplyTemplates { get; set; }

    public virtual DbSet<TimeUnitPackage> TimeUnitPackages { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=THANHVU;Database=RentNestSystem;User Id=sa; Password=123;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Accommodation>(entity =>
        {
            entity.HasKey(e => e.AccommodationId).HasName("PK__Accommod__004EC3259E51BC31");

            entity.ToTable("Accommodation");

            entity.Property(e => e.AccommodationId).HasColumnName("accommodation_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Area).HasColumnName("area");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DistrictName)
                .HasMaxLength(255)
                .HasColumnName("district_name");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProvinceName)
                .HasMaxLength(255)
                .HasColumnName("province_name");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength()
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .HasColumnName("title");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("video_url");
            entity.Property(e => e.WardName)
                .HasMaxLength(255)
                .HasColumnName("ward_name");

            entity.HasOne(d => d.Owner).WithMany(p => p.Accommodations)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK_Accommodation_Owner");

            entity.HasOne(d => d.Type).WithMany(p => p.Accommodations)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Accommodation_Type");
        });

        modelBuilder.Entity<AccommodationAmenity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Accommod__3213E83F0744701B");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccommodationId).HasColumnName("accommodation_id");
            entity.Property(e => e.AmenityId).HasColumnName("amenity_id");

            entity.HasOne(d => d.Accommodation).WithMany(p => p.AccommodationAmenities)
                .HasForeignKey(d => d.AccommodationId)
                .HasConstraintName("FK_Amenities_Accommodation");

            entity.HasOne(d => d.Amenity).WithMany(p => p.AccommodationAmenities)
                .HasForeignKey(d => d.AmenityId)
                .HasConstraintName("FK_Amenities");
        });

        modelBuilder.Entity<AccommodationDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__Accommod__38E9A224AFB7438A");

            entity.HasIndex(e => e.AccommodationId, "UQ__Accommod__004EC324CC818CC5").IsUnique();

            entity.Property(e => e.DetailId).HasColumnName("detail_id");
            entity.Property(e => e.AccommodationId).HasColumnName("accommodation_id");
            entity.Property(e => e.BathroomCount)
                .HasDefaultValue(0)
                .HasColumnName("bathroom_count");
            entity.Property(e => e.BedroomCount)
                .HasDefaultValue(0)
                .HasColumnName("bedroom_count");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FurnitureStatus)
                .HasMaxLength(100)
                .HasColumnName("furniture_status");
            entity.Property(e => e.HasAirConditioner)
                .HasDefaultValue(false)
                .HasColumnName("has_air_conditioner");
            entity.Property(e => e.HasKitchenCabinet)
                .HasDefaultValue(false)
                .HasColumnName("has_kitchen_cabinet");
            entity.Property(e => e.HasLoft)
                .HasDefaultValue(false)
                .HasColumnName("has_loft");
            entity.Property(e => e.HasRefrigerator)
                .HasDefaultValue(false)
                .HasColumnName("has_refrigerator");
            entity.Property(e => e.HasWashingMachine)
                .HasDefaultValue(false)
                .HasColumnName("has_washing_machine");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Accommodation).WithOne(p => p.AccommodationDetail)
                .HasForeignKey<AccommodationDetail>(d => d.AccommodationId)
                .HasConstraintName("FK_Details_Accommodation");
        });

        modelBuilder.Entity<AccommodationImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Accommod__DC9AC9554778C5EA");

            entity.ToTable("AccommodationImage");

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.AccommodationId).HasColumnName("accommodation_id");
            entity.Property(e => e.Caption)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("caption");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("image_url");

            entity.HasOne(d => d.Accommodation).WithMany(p => p.AccommodationImages)
                .HasForeignKey(d => d.AccommodationId)
                .HasConstraintName("FK_AccommodationImage_Accommodation");
        });

        modelBuilder.Entity<AccommodationType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__Accommod__2C000598C28DAF21");

            entity.ToTable("AccommodationType");

            entity.HasIndex(e => e.TypeName, "UQ__Accommod__543C4FD940FAABC3").IsUnique();

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.TypeName)
                .HasMaxLength(100)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__46A222CD4B1F6CBE");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Email, "UQ__Account__AB6E616446C684F9").IsUnique();

            entity.HasIndex(e => e.AuthProviderId, "UX_Account_AuthProviderId")
                .IsUnique()
                .HasFilter("([auth_provider_id] IS NOT NULL)");

            entity.HasIndex(e => e.Username, "UX_Account_Username")
                .IsUnique()
                .HasFilter("([Username] IS NOT NULL)");

            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.AuthProvider)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("auth_provider");
            entity.Property(e => e.AuthProviderId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("auth_provider_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .IsFixedLength()
                .HasColumnName("is_active");
            entity.Property(e => e.IsOnline)
                .HasDefaultValue(false)
                .HasColumnName("is_online");
            entity.Property(e => e.LastActiveAt)
                .HasColumnType("datetime")
                .HasColumnName("last_active_at");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.AmenityId).HasName("PK__Amenitie__E908452DBAA392C5");

            entity.Property(e => e.AmenityId).HasColumnName("amenity_id");
            entity.Property(e => e.AmenityName)
                .HasMaxLength(100)
                .HasColumnName("amenity_name");
            entity.Property(e => e.IconSvg)
                .IsUnicode(false)
                .HasColumnName("iconSvg");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.ConversationId).HasName("PK__Conversa__311E7E9ABEE6F24B");

            entity.ToTable("Conversation");

            entity.HasIndex(e => new { e.SenderId, e.ReceiverId }, "IX_Conversation_Users");

            entity.HasIndex(e => new { e.SenderId, e.ReceiverId, e.PostId }, "UX_Conversation_Uniqueness").IsUnique();

            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.StartedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("started_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Post).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_Conversation_Post");

            entity.HasOne(d => d.Receiver).WithMany(p => p.ConversationReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conversation_Receiver");

            entity.HasOne(d => d.Sender).WithMany(p => p.ConversationSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conversation_Sender");
        });

        modelBuilder.Entity<FavoritePost>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PK__Favorite__46ACF4CB2D7510EF");

            entity.ToTable("FavoritePost");

            entity.HasIndex(e => new { e.AccountId, e.PostId }, "UQ_FavoritePost_Account_Post").IsUnique();

            entity.Property(e => e.FavoriteId).HasColumnName("favorite_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PostId).HasColumnName("post_id");

            entity.HasOne(d => d.Account).WithMany(p => p.FavoritePosts)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_FavoritePost_Account");

            entity.HasOne(d => d.Post).WithMany(p => p.FavoritePosts)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FavoritePost_Post");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Message__0BBF6EE6785CC9EA");

            entity.ToTable("Message");

            entity.HasIndex(e => e.ConversationId, "IX_Message_ConversationId");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("image_url");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("sent_at");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ConversationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_Conversation");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Message_Sender");
        });

        modelBuilder.Entity<PackagePricing>(entity =>
        {
            entity.HasKey(e => e.PricingId).HasName("PK__PackageP__A25A9FB76EE70D20");

            entity.ToTable("PackagePricing");

            entity.HasIndex(e => new { e.TimeUnitId, e.PackageTypeId, e.DurationValue }, "UQ_PackagePricing_Combination").IsUnique();

            entity.Property(e => e.PricingId).HasColumnName("pricing_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DurationValue).HasColumnName("duration_value");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PackageTypeId).HasColumnName("package_type_id");
            entity.Property(e => e.TimeUnitId).HasColumnName("time_unit_id");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("unit_price");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.PackageType).WithMany(p => p.PackagePricings)
                .HasForeignKey(d => d.PackageTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PackagePricing_Type");

            entity.HasOne(d => d.TimeUnit).WithMany(p => p.PackagePricings)
                .HasForeignKey(d => d.TimeUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PackagePricing_TimeUnit");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__ED1FC9EABF5D04C7");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.MethodId).HasColumnName("method_id");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("payment_date");
            entity.Property(e => e.PostPackageDetailsId).HasColumnName("post_package_details_id");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Account).WithMany(p => p.Payments)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Payment_Account");

            entity.HasOne(d => d.Method).WithMany(p => p.Payments)
                .HasForeignKey(d => d.MethodId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Payment_Method");

            entity.HasOne(d => d.PostPackageDetails).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PostPackageDetailsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_PostPackageDetails");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.MethodId).HasName("PK__PaymentM__747727B6EAF0B898");

            entity.ToTable("PaymentMethod");

            entity.Property(e => e.MethodId).HasColumnName("method_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IconUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("icon_url");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MethodName)
                .HasMaxLength(100)
                .HasColumnName("method_name");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Post__3ED787660714A123");

            entity.ToTable("Post");

            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.AccommodationId).HasColumnName("accommodation_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrentStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("P")
                .IsFixedLength()
                .HasColumnName("current_status");
            entity.Property(e => e.PublishedAt)
                .HasColumnType("datetime")
                .HasColumnName("published_at");
            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Accommodation).WithMany(p => p.Posts)
                .HasForeignKey(d => d.AccommodationId)
                .HasConstraintName("FK_Post_Accommodation");

            entity.HasOne(d => d.Account).WithMany(p => p.Posts)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Post_Account");
        });

        modelBuilder.Entity<PostApproval>(entity =>
        {
            entity.HasKey(e => e.ApprovalId).HasName("PK__PostAppr__C94AE61A0DBEB322");

            entity.Property(e => e.ApprovalId).HasColumnName("approval_id");
            entity.Property(e => e.ApprovedByAccountId).HasColumnName("approved_by_account_id");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.ProcessedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("processed_at");
            entity.Property(e => e.RejectionReason)
                .HasMaxLength(255)
                .HasColumnName("rejection_reason");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("P")
                .IsFixedLength()
                .HasColumnName("status");

            entity.HasOne(d => d.ApprovedByAccount).WithMany(p => p.PostApprovals)
                .HasForeignKey(d => d.ApprovedByAccountId)
                .HasConstraintName("FK_PostApprovals_Approver");

            entity.HasOne(d => d.Post).WithMany(p => p.PostApprovals)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_PostApprovals_Post");
        });

        modelBuilder.Entity<PostPackageDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PostPack__3213E83F266DC02E");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("P")
                .IsFixedLength()
                .HasColumnName("payment_status");
            entity.Property(e => e.PaymentTransactionId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("payment_transaction_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.PricingId).HasColumnName("pricing_id");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_price");

            entity.HasOne(d => d.Post).WithMany(p => p.PostPackageDetails)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_PostPackageDetails_Post");

            entity.HasOne(d => d.Pricing).WithMany(p => p.PostPackageDetails)
                .HasForeignKey(d => d.PricingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostPackageDetails_Pricing");
        });

        modelBuilder.Entity<PostPackageType>(entity =>
        {
            entity.HasKey(e => e.PackageTypeId).HasName("PK__PostPack__DFBEE40A4F2272E2");

            entity.ToTable("PostPackageType");

            entity.Property(e => e.PackageTypeId).HasColumnName("package_type_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.PackageTypeName)
                .HasMaxLength(100)
                .HasColumnName("package_type_name");
            entity.Property(e => e.Priority).HasColumnName("priority");
        });

        modelBuilder.Entity<PromoCode>(entity =>
        {
            entity.HasKey(e => e.PromoId).HasName("PK__PromoCod__84EB4CA56F43B46A");

            entity.ToTable("PromoCode");

            entity.HasIndex(e => e.Code, "UQ__PromoCod__357D4CF945D04915").IsUnique();

            entity.Property(e => e.PromoId).HasColumnName("promo_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.DiscountAmount)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount_amount");
            entity.Property(e => e.DiscountPercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount_percent");
            entity.Property(e => e.DurationDays).HasColumnName("duration_days");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.IsNewUserOnly)
                .HasDefaultValue(false)
                .HasColumnName("is_new_user_only");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<PromoUsage>(entity =>
        {
            entity.HasKey(e => e.PromoUsageId).HasName("PK__PromoUsa__60E8C63857746FDD");

            entity.ToTable("PromoUsage");

            entity.Property(e => e.PromoUsageId).HasColumnName("promo_usage_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.PromoId).HasColumnName("promo_id");
            entity.Property(e => e.UsedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("used_at");

            entity.HasOne(d => d.Account).WithMany(p => p.PromoUsages)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_PromoUsage_Account");

            entity.HasOne(d => d.Post).WithMany(p => p.PromoUsages)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PromoUsage_Post");

            entity.HasOne(d => d.Promo).WithMany(p => p.PromoUsages)
                .HasForeignKey(d => d.PromoId)
                .HasConstraintName("FK_PromoUsage_Promo");
        });

        modelBuilder.Entity<QuickReplyTemplate>(entity =>
        {
            entity.HasKey(e => e.TemplateId).HasName("PK__QuickRep__BE44E07986EE2A57");

            entity.ToTable("QuickReplyTemplate");

            entity.Property(e => e.TemplateId).HasColumnName("template_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDefault)
                .HasDefaultValue(true)
                .HasColumnName("is_default");
            entity.Property(e => e.Message)
                .HasMaxLength(255)
                .HasColumnName("message");
            entity.Property(e => e.TargetRole)
                .HasMaxLength(20)
                .HasColumnName("target_role");

            entity.HasOne(d => d.Account).WithMany(p => p.QuickReplyTemplates)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_QuickReplyTemplate_Account");
        });

        modelBuilder.Entity<TimeUnitPackage>(entity =>
        {
            entity.HasKey(e => e.TimeUnitId).HasName("PK__TimeUnit__8304AF47E0B87EBF");

            entity.ToTable("TimeUnitPackage");

            entity.Property(e => e.TimeUnitId).HasColumnName("time_unit_id");
            entity.Property(e => e.Data)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("data");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.TimeUnitName)
                .HasMaxLength(20)
                .HasColumnName("time_unit_name");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__UserProf__AEBB701FD9F52B13");

            entity.ToTable("UserProfile");

            entity.HasIndex(e => e.AccountId, "UQ__UserProf__46A222CC2FFB7F7D").IsUnique();

            entity.Property(e => e.ProfileId).HasColumnName("profile_id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Occupation)
                .HasMaxLength(100)
                .HasColumnName("occupation");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Account).WithOne(p => p.UserProfile)
                .HasForeignKey<UserProfile>(d => d.AccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserProfile_Account");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
